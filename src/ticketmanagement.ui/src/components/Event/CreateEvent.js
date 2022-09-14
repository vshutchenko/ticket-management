import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import Select from "react-select";
import VenueService from "../../services/VenueService";
import AuthService from "../../services/AuthService";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import "flatpickr/dist/themes/material_green.css";
import { Russian } from "flatpickr/dist/l10n/ru.js"
import { Belarusian } from "flatpickr/dist/l10n/be.js"
import { english } from "flatpickr/dist/l10n/default"
import Flatpickr from "react-flatpickr";
import { localeDateToUtc } from "../../helpers/ConvertTimeZone";

export default function CreateEvent() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [currentLayout, setCurrentLayout] = useState(null);
    const [currentVenue, setCurrentVenue] = useState(null);
    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());
    const [imageUrl, setImageUrl] = useState('');
    const [layouts, setLayouts] = useState([]);
    const [venues, setVenues] = useState([]);
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);

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

    function getPickerLocale() {
        const user = AuthService.getCurrentUser();
        const culture = user ? user.culture : 'en-US';

        if (culture === 'ru-RU')
            return Russian;

        if (culture === 'be-BY')
            return Belarusian;

        if (culture === 'en-US')
            return english;
    }

    async function handleSubmit(e) {
        e.preventDefault();
        let event = {
            id: 0,
            name: name,
            description: description,
            startDate: localeDateToUtc(startDate),
            endDate: localeDateToUtc(endDate),
            layoutId: currentLayout.id,
            published: false,
            imageUrl: imageUrl
        }

        await EventService.create(event).then(() => {
            navigate('/Event/NotPublishedEvents');
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });
    }

    return (
        <div className="event-form">
            {failed && (<div className="alert alert-danger">{t(error, { ns: 'validation' })}</div>)}
            <h3>{t('Add event')}</h3>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>{t("Name")}:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={name}
                        onChange={e => setName(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Description")}:</label>
                    <textarea
                        type="text"
                        className="form-control"
                        value={description}
                        onChange={e => setDescription(e.target.value)}
                    />
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
                            onChange={d => setStartDate(d.pop())}
                            options={{ minDate: new Date().setHours(0, 0, 0, 0), locale: getPickerLocale() }}
                        />
                    </div>
                    <div className="col">
                        <label>{t("Date of completion")}:</label>
                        <Flatpickr
                            className="form-control"
                            data-enable-time
                            value={endDate}
                            onChange={d => setEndDate(d.pop())}
                            options={{ minDate: new Date().setHours(0, 0, 0, 0), locale: getPickerLocale() }}
                        />
                    </div>
                </div>
                <div className="form-group">
                    <label>{t("Image URL")}:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={imageUrl}
                        onChange={e => setImageUrl(e.target.value)}
                    />
                </div>

                <div className="form-group mt-2">
                    <input type="submit" value={t("Add event")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}