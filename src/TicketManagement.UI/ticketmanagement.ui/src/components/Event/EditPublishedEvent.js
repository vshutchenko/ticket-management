import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import Select from "react-select";
import VenueService from "../../services/VenueService";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import LayoutService from "../../services/LayoutService";
import { useSearchParams } from "react-router-dom";
import "flatpickr/dist/themes/material_green.css";
import Flatpickr from "react-flatpickr";
import { getUserTime, utcToLocaleDate, localeDateToUtc } from "../../helpers/ConvertTimeZone";
import { getPickerLocale } from "../../helpers/DatePicker";
import { useAlert } from "react-alert";
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';

export default function EditPublishedEvent() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const alert = useAlert();

    const formSchema = Yup.object().shape({
        name: Yup.string()
            .required(t('Name is required')),
        description: Yup.string()
            .required(t('Description is required')),
        imageUrl: Yup.string()
            .required(t('Image URL is required'))
    })

    const formOptions = { mode: "onChange", resolver: yupResolver(formSchema) }
    const { register, handleSubmit, getValues, formState, setValue } = useForm(formOptions)
    const { errors } = formState

    const [params] = useSearchParams();
    const [id, setId] = useState(0);
    const [currentLayout, setCurrentLayout] = useState({});
    const [currentVenue, setCurrentVenue] = useState({});
    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());
    const [layouts, setLayouts] = useState([]);
    const [venues, setVenues] = useState([]);

    useEffect(() => {
        async function fetchData() {
            const eventId = params.get('id');
            const event = await EventService.getById(eventId);
            const currentLayout = await LayoutService.getById(event.layoutId);
            const currentVenue = await VenueService.getById(currentLayout.venueId);
            const venues = await VenueService.getAll();
            const layouts = await VenueService.getLayoutsByVenueId(currentVenue.id);

            setVenues(venues);
            setLayouts(layouts);
            setCurrentLayout(currentLayout);
            setCurrentVenue(currentVenue);
            setId(event.id);
            setValue('name', event.name);
            setValue('description', event.description);
            setValue('imageUrl', event.imageUrl);
            setStartDate(utcToLocaleDate(event.startDate));
            setEndDate(utcToLocaleDate(event.endDate));
        }

        fetchData();
    }, [params, setValue]);

    async function handleVenueChange(venue) {
        const layouts = await VenueService.getLayoutsByVenueId(venue.id);
        setLayouts(layouts);
        setCurrentLayout(layouts[0]);
        setCurrentVenue(venue);
    }

    function handleLayoutChange(layout) {
        setCurrentLayout(layout);
    }

    async function onSubmit() {
        let event = {
            id: id,
            name: getValues('name'),
            description: getValues('description'),
            startDate: localeDateToUtc(startDate),
            endDate: localeDateToUtc(endDate),
            layoutId: currentLayout.id,
            published: false,
            imageUrl: getValues('imageUrl')
        }

        await EventService.update(event).then(() => {
            navigate(`/Event/EditNotPublishedEvent?id=${id}`);
            alert.success('Event was updated!');
        }).catch(error => {
            alert.error(t(error.response.data.error, { ns: 'validation'}));
        });
    }

    return (
        <div className="event-form">
            <form onSubmit={handleSubmit(onSubmit)}>
                <div>
                    <h2 className="text-center">{t('Edit event')}</h2>
                </div>

                <div>
                    <label className="form-label">{t("Name")}:</label>
                    <input
                        type="text"
                        {...register('name')}
                        className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                    />
                    <div className="invalid-feedback">{errors.name?.message}</div>
                </div>

                <div>
                    <label className="form-label">{t("Description")}:</label>
                    <textarea
                        type="text"
                        {...register('description')}
                        className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                    />
                    <div className="invalid-feedback">{errors.description?.message}</div>
                </div>

                <div className="form-group">
                    <label>{t("Venue")}:</label>
                    <Select
                        options={venues}
                        value={currentVenue ? currentVenue : null}
                        getOptionLabel={x => x.description}
                        getOptionValue={x => x.id}
                        onChange={handleVenueChange}
                        placeholder={t("Choose venue")}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Layout")}:</label>
                    <Select
                        options={layouts}
                        value={currentLayout ? currentLayout : null}
                        getOptionLabel={x => x.description}
                        getOptionValue={x => x.id}
                        onChange={handleLayoutChange}
                        placeholder={t("Choose layout")}
                    />
                </div>

                <div className="row">
                    <div className="col"><label>{t("Date of start")}:</label>
                        <Flatpickr
                            className="form-control"
                            data-enable-time
                            value={startDate}
                            onChange={d => setStartDate(d[0])}
                            options={{ minDate: getUserTime(), locale: getPickerLocale() }}
                        />
                    </div>
                    <div className="col">
                        <label>{t("Date of completion")}:</label>
                        <Flatpickr
                            className="form-control"
                            data-enable-time
                            value={endDate}
                            onChange={d => setEndDate(d[0])}
                            options={{ minDate: getUserTime(), locale: getPickerLocale() }}
                        />
                    </div>
                </div>

                <div>
                    <label className="form-label">{t("Image URL")}:</label>
                    <input
                        type="text"
                        {...register('imageUrl')}
                        className={`form-control ${errors.imageUrl ? 'is-invalid' : ''}`}
                    />
                    <div className="invalid-feedback">{errors.imageUrl?.message}</div>
                </div>

                <div className="form-group mt-2 text-center">
                    <input type="submit" value={t("Submit")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}