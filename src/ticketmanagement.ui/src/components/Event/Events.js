import { useState, React, useEffect } from "react";
import { Link } from "react-router-dom";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import AuthService from "../../services/AuthService";

export default function Events() {
    const { t } = useTranslation();

    const [loading, setLoading] = useState(true);
    const [events, setEvents] = useState([]);

    useEffect(() => {
        async function fetchData() {
            await EventService.getPublished().then(events => {
                setEvents(events);
                setLoading(false);
            });
        }

        fetchData();
    }, []);

    function renderAllEventsTable(events) {
        return (
            <div>
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>{t("Name")}</th>
                            <th>{t("Description")}</th>
                            <th>{t("Date of start")}</th>
                            <th>{t("Date of completion")}</th>
                            <th></th>
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
                                        <Link className="btn btn-success" to="/Purchase/PurchaseSeats" query={{ the: `?id=${event.id}` }}>{t("Details")}</Link>
                                    {AuthService.isEventManager() && (
                                        <Link className="btn btn-success" to="/Event/EditEvent" query={{ the: `?id=${event.id}` }}>{t("Edit")}</Link>
                                    )}
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
        renderAllEventsTable(events)
    )

    return (
        <div>
            <h1>{t("All events")}</h1>
            <p>{t("Here you can see all events.")}</p>
            {content}
        </div>
    )
}