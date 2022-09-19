import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import { useSearchParams } from "react-router-dom";
import CurrencyInput from 'react-currency-input-field';
import { toLocaleDate, utcToLocaleDate, localeDateToUtc } from "../../helpers/ConvertTimeZone";
import { useAlert } from "react-alert";
import isAbsoluteUrl from 'is-absolute-url';

const mvcAppUrl = process.env.REACT_APP_MVC_APP;

export default function EditNotPublishedEvent() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const alert = useAlert();

    const [params] = useSearchParams();
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [layoutId, setLayoutId] = useState(0);
    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());
    const [imageUrl, setImageUrl] = useState('');

    const [areas, setAreas] = useState([]);

    useEffect(() => {
        async function fetchData() {
            const eventId = params.get('id');

            await EventService.getAreasById(eventId).then(areas => {
                setAreas(areas);
            });

            await EventService.getById(eventId).then(event => {
                setName(event.name);
                setDescription(event.description);
                setStartDate(utcToLocaleDate(event.startDate));
                setEndDate(utcToLocaleDate(event.endDate));
                setImageUrl(event.imageUrl);
                setLayoutId(event.layoutId);
            });
        }

        fetchData();
    }, [params]);

    async function handleSubmit(e) {
        e.preventDefault();

        const id = params.get('id');

        let event = {
            id: id,
            name: name,
            description: description,
            startDate: localeDateToUtc(startDate),
            endDate: localeDateToUtc(endDate),
            layoutId: layoutId,
            published: true,
            imageUrl: imageUrl
        }

        await EventService.update(event).then(() => {
            navigate(`/`);
            alert.success(t("Event was updated!"));
        }).catch(error => {
            alert.error(t(error.response.data.error, { ns: 'validation' }));
        });
    }

    function handleChangePrice(value, index) {
        let updatedAreas = [...areas];
        updatedAreas[index].price = value;

        setAreas(updatedAreas);
    }

    return (
        <div>
            <h3>{t('Edit event')}</h3>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    {isAbsoluteUrl(imageUrl)
                        ? <img src={imageUrl} width={300} height={300} style={{ objectFit: 'contain', objectPosition: 'left' }} alt='' />
                        : <img src={`${mvcAppUrl}/${imageUrl}`} width={300} height={300} style={{ objectFit: 'contain', objectPosition: 'left' }} alt='' />
                    }
                    <h5>{t("Name")}:</h5>
                    <div>{name}</div>
                    <h5>{t("Description")}:</h5>
                    <div>{description}</div>
                    <h5>{t("Date of start")}:</h5>
                    <div>{toLocaleDate(startDate)}</div>
                    <h5>{t("Date of completion")}:</h5>
                    <div>{toLocaleDate(endDate)}</div>
                </div>
                {areas.map((area, index) => (
                    <div key={area.id}>
                        <h5>{t('Seat price', { area: area.description })}</h5>
                        <CurrencyInput
                            name="price"
                            prefix="$"
                            defaultValue={0}
                            decimalsLimit={2}
                            maxLength={7}
                            onValueChange={(value) => handleChangePrice(value, index)}
                            className="form-control"
                        />
                    </div>
                ))}

                <div className="form-group mt-2">
                    <input type="submit" value={t("Submit")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}