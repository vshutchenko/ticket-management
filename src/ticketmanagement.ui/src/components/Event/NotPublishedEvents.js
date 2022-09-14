import { useState, React, useEffect } from "react";
import { Link } from "react-router-dom";
import EventService from "../../services/EventService";
import { useTranslation } from 'react-i18next';
import { Button, Modal, } from "react-bootstrap";

export default function NotPublishedEvents() {
    const { t } = useTranslation();

    const [loading, setLoading] = useState(true);
    const [events, setEvents] = useState([]);
    const [selectedEvent, setSelectedEvent] = useState({});
    const [show, setShow] = useState(false);
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);

    useEffect(() => {
        async function fetchData() {
            await EventService.getNotPublished().then(events => {
                setEvents(events);
                setLoading(false);
            });
        }

        fetchData();
    }, []);

    async function handleDelete() {
        await EventService.delete(selectedEvent.id).then(() => {
            setEvents(events.filter(e => e.id !== selectedEvent.id));
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });

        setShow(false);
    }

    async function handleShow(event) {
        setSelectedEvent(event);
        setShow(true);
    }

    function renderNotPublishedEventsTable(events) {
        return (
            <div>
                {failed && (<div className="alert alert-danger">{t(error, { ns: 'validation' })}</div>)}
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
                                        <Link className="btn btn-success m-1" to={`/Purchase/PurchaseSeats?id=${event.id}`}>{t("Details")}</Link>
                                        <Link className="btn btn-primary m-1" to={`/Event/EditNotPublishedEvent?id=${event.id}`}>{t("Edit")}</Link>
                                        <Button className="m-1" variant="danger" onClick={() => handleShow(event)}>{t("Delete")}</Button>
                                    </td>
                                </tr>
                            ))
                        }
                    </tbody>
                </table>
                <Modal show={show} onHide={() => setShow(false)}>
                    <Modal.Header closeButton>
                        <Modal.Title>{t("Delete confiramtion")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>{t("Are you sure you want to delete this event?")}</Modal.Body>
                    <Modal.Footer>
                        <Button variant="danger" onClick={() => handleDelete()}>
                            {t("Confirm")}
                        </Button>
                        <Button variant="secondary" onClick={() => setShow(false)}>
                            {t("Cancel")}
                        </Button>
                    </Modal.Footer>
                </Modal>
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