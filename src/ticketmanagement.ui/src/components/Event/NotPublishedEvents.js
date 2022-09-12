import { useState, React, useEffect } from "react";
import { Link } from "react-router-dom";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';

export default function NotPublishedEvents() {
    const { t } = useTranslation();

    const [loading, setLoading] = useState(true);
    const [events, setEvents] = useState([]);

    useEffect(() => {
        async function fetchData() {
            await EventService.getNotPublished().then(events => {
                setEvents(events);
                setLoading(false);
            });
        }

        fetchData();
    }, []);

    function renderNotPublishedEventsTable(events) {
        return (
            <div>
                <Link className="btn btn-success" to="/Event/CreateEvent">{t("Add event")}</Link>
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>{t("Name")}</th>
                            <th>{t("Description")}</th>
                            <th>{t("Date of start")}</th>
                            <th>{t("Date of completion")}</th>
                            <th>{t("Action")}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            events.map(event => (
                            <tr key={event.id}>
                                <td>{event.name}</td>
                                <td>{event.description}</td>
                                <td>{new Date(event.startDate).toLocaleDateString()}</td>
                                <td>{new Date(event.endDate).toLocaleDateString()}</td>
                                <td>
                                    <Link className="btn btn-success" to={`/Purchase/PurchaseSeats?id=${event.id}`}>{t("Details")}</Link>
                                    <Link className="btn btn-primary" to={`/Event/EditEvent?id=${event.id}`}>{t("Edit")}</Link>
                                    <Link className="btn btn-danger" to={`/Event/DeleteEvent?id=${event.id}`}>{t("Delete")}</Link>
                                </td>
                            </tr>
                            ))
                        }
                    </tbody>
                </table>
            </div>
            
        )
    }

    let content = loading ? (
        <p>
            <em>{t("Loading...")}</em>
        </p>
    ) : (
        renderNotPublishedEventsTable(events)
    )
    
    return (
        <div>
            <h1>{t("Not published events")}</h1>
            <p>{t("Here you can see events waiting for publicaiton.")}</p>
            {content}
        </div>
    )
}