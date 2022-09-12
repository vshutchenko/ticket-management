import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import Select from "react-select";
import VenueService from "../../services/VenueService";
import AuthService from "../../services/AuthService";
import EventService from "../../services/EventService";
import { TempusDominus, Namespace } from '@eonasdan/tempus-dominus'
import { useTranslation } from 'react-i18next';

export default function CreateEvent() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [currentLayout, setCurrentLayout] = useState(null);
    const [currentVenue, setCurrentVenue] = useState(null);
    const [startDate, setStartDate] = useState(Date());
    const [endDate, setEndDate] = useState(Date());
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

        const user = AuthService.getCurrentUser();
        const culture = user ? user.culture : 'en-US';

        let pickerOptions = {
            defaultDate: Date(),
            localization: { locale: culture },
            display: { components: { seconds: false } },
            restrictions: { minDate: Date() }
        }

        let startDatePicker = new TempusDominus(document.getElementById('datetimepicker1'), pickerOptions);
        let endDatePicker = new TempusDominus(document.getElementById('datetimepicker2'), pickerOptions);

        startDatePicker.subscribe(Namespace.events.change, (e) => {
            setStartDate(e.date);
        });

        endDatePicker.subscribe(Namespace.events.change, (e) => {
            setEndDate(e.date);
        });
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

    async function handleSubmit(e) {
        e.preventDefault();

        let event = {
            id: 0,
            name: name,
            description: description,
            startDate: startDate,
            endDate: endDate,
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
                    <div className="col-sm-12" id="htmlTarget">
                        <label htmlFor="datetimepicker1Input" className="form-label">{t("Date of start")}:</label>
                        <div
                            className="input-group log-event"
                            id="datetimepicker1"
                            data-td-target-input="nearest"
                            data-td-target-toggle="nearest"
                        >
                            <input
                                id="datetimepicker1Input"
                                type="text"
                                className="form-control"
                                data-td-target="#datetimepicker1"
                            />
                            <span
                                className="input-group-text"
                                data-td-target="#datetimepicker1"
                                data-td-toggle="datetimepicker"
                            >
                                <i className="fas fa-calendar"></i>
                            </span>
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-12" id="htmlTarget">
                        <label htmlFor="datetimepicker2Input" className="form-label">{t("Date of completion")}:</label>
                        <div
                            className="input-group log-event"
                            id="datetimepicker2"
                            data-td-target-input="nearest"
                            data-td-target-toggle="nearest"
                        >
                            <input
                                id="datetimepicker2Input"
                                type="text"
                                className="form-control"
                                data-td-target="#datetimepicker2"
                            />
                            <span
                                className="input-group-text"
                                data-td-target="#datetimepicker2"
                                data-td-toggle="datetimepicker"
                            >
                                <i className="fas fa-calendar"></i>
                            </span>
                        </div>
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

                <div className="form-group">
                    <input type="submit" value={t("Add event")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}