import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import Select from "react-select";
import VenueService from "../../services/VenueService";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import "flatpickr/dist/themes/material_green.css";
import Flatpickr from "react-flatpickr";
import { localeDateToUtc } from "../../helpers/ConvertTimeZone";
import { useAlert } from "react-alert";
import { getPickerLocale } from "../../helpers/DatePicker";
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';

export default function CreateEvent() {
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
    const { register, handleSubmit, getValues, formState } = useForm(formOptions)
    const { errors } = formState

    const [currentLayout, setCurrentLayout] = useState({});
    const [currentVenue, setCurrentVenue] = useState({});
    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());
    const [layouts, setLayouts] = useState([]);
    const [venues, setVenues] = useState([]);

    useEffect(() => {
        async function fetchData() {
            await VenueService.getAll().then(venues => {
                VenueService.getLayoutsByVenueId(venues[0].id).then(layouts => {
                    setVenues(venues);
                    setLayouts(layouts);
                    setCurrentVenue(venues[0]);
                    setCurrentLayout(layouts[0]);
                });
            });
        }

        fetchData();
    }, []);

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
            id: 0,
            name: getValues('name'),
            description: getValues('description'),
            startDate: localeDateToUtc(startDate),
            endDate: localeDateToUtc(endDate),
            layoutId: currentLayout.id,
            published: false,
            imageUrl: getValues('imageUrl')
        }

        await EventService.create(event).then(() => {
            navigate('/Event/NotPublishedEvents');
            alert.success('Event was created!');
        }).catch(error => {
            alert.error(t(error.response.data.error, { ns: 'validation'}));
        });
    }

    return (
        <div className="event-form">
            <form onSubmit={handleSubmit(onSubmit)}>
                <div>
                    <h2 className="text-center">{t('Add event')}</h2>
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

                <div>
                    <label className="form-label">{t("Venue")}:</label>
                    <Select
                        options={venues}
                        value={currentVenue ? currentVenue : null}
                        getOptionLabel={x => x.description}
                        getOptionValue={x => x.id}
                        onChange={handleVenueChange}
                        placeholder={t("Choose venue")}
                    />
                </div>

                <div>
                    <label className="form-label">{t("Layout")}:</label>
                    <Select
                        options={layouts}
                        value={currentLayout ? currentLayout : null}
                        getOptionLabel={x => x.description}
                        getOptionValue={x => x.id}
                        onChange={handleLayoutChange}
                        placeholder={t("Choose layout")}
                    />
                </div>

                <div className="row mt-1">
                    <div className="col">
                        <label className="form-label">{t("Date of start")}:</label>
                        <Flatpickr
                            className="form-control"
                            data-enable-time
                            value={startDate}
                            onChange={d => setStartDate(d.pop())}
                            options={{ minDate: new Date().setHours(0, 0, 0, 0), locale: getPickerLocale() }}
                        />
                    </div>
                    <div className="col">
                        <label className="form-label">{t("Date of completion")}:</label>
                        <Flatpickr
                            className="form-control"
                            data-enable-time
                            value={endDate}
                            onChange={d => setEndDate(d.pop())}
                            options={{ minDate: new Date().setHours(0, 0, 0, 0), locale: getPickerLocale() }}
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
                    <input type="submit" value={t("Add event")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}