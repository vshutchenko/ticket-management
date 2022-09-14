import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import AuthService from "../../services/AuthService";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import { useSearchParams } from "react-router-dom";
import CurrencyInput from 'react-currency-input-field';

export default function EditNotPublishedEvent() {
    const navigate = useNavigate();
    const { t } = useTranslation();

    const [params] = useSearchParams();
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [layoutId, setLayoutId] = useState(0);
    const [startDate, setStartDate] = useState(Date());
    const [endDate, setEndDate] = useState(Date());
    const [imageUrl, setImageUrl] = useState('');
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);

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
                setStartDate(event.startDate);
                setEndDate(event.endDate);
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
            startDate: startDate,
            endDate: endDate,
            layoutId: layoutId,
            published: true,
            imageUrl: imageUrl
        }

        await EventService.update(event).then(() => {
            navigate(`/`);
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });
    }

    function handleChangePrice(value, index) {
        let updatedAreas = [...areas];
        updatedAreas[index].price = value;

        setAreas(updatedAreas);
    }

    return (
        <div>
            {failed && (<div className="alert alert-danger">{t(error, { ns: 'validation' })}</div>)}
            <h3>{t('Edit event')}</h3>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <img src={imageUrl} width={300} height={300} style={{objectFit: 'contain', objectPosition: 'left'}} alt=''/>
                    <h5>{t("Name")}:</h5>
                    <div>{name}</div>
                    <h5>{t("Description")}:</h5>
                    <div>{description}</div>
                    <h5>{t("Date of start")}:</h5>
                    <div>{new Date(startDate).toLocaleString(AuthService.getCurrentUser().culture)}</div>
                    <h5>{t("Date of completion")}:</h5>
                    <div>{new Date(endDate).toLocaleString(AuthService.getCurrentUser().culture)}</div>
                </div>
                {areas.map((area, index) => (
                    <div key={area.id}>
                        <h5>{t('Seat price', { area: area.description})}</h5>
                        <CurrencyInput
                            name="price"
                            prefix="$"
                            defaultValue={0}
                            decimalsLimit={2}
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