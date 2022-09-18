import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import UserService from "../../services/UserService";
import { useTranslation } from 'react-i18next';
import Select from "react-select";
import { useSearchParams } from "react-router-dom";
import { findWindows, WINDOWS_TO_IANA_MAP } from "windows-iana";
import timeZonesData from '../../data/timeZones.json';

export default function EditUser() {
    const navigate = useNavigate();
    const { t } = useTranslation();

    const [cultures, setCultures] = useState([]);
    const [params] = useSearchParams();
    const [id, setId] = useState(0);
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [culture, setCulture] = useState(cultures[0]);
    const [timeZone, setTimeZone] = useState('');
    const [email, setEmail] = useState('');
    const [balance, setBalance] = useState(0);
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);
    const [timeZones, setTimeZones] = useState([]);

    useEffect(() => {
        async function fetchData() {
            const cultures = [
                { value: 'en-US', label: 'English' },
                { value: 'ru-RU', label: 'Русский' },
                { value: 'be-BY', label: 'Беларуская' }
            ];

            const id = params.get('id');
            const user = await UserService.getById(id);

            let timeZones2 = timeZonesData.map(tz => {
                return { value: tz.id, label: tz.name };
            })

            let userTimeZone = timeZones2.find(tz => {
                return tz.value === user.timeZoneId;
            })

            setId(id);
            setFirstName(user.firstName);
            setLastName(user.lastName);
            setEmail(user.email);
            setBalance(user.balance);
            setTimeZones(timeZones2);
            setTimeZone(userTimeZone);
            setCulture(cultures.find(c => {
                return c.value === user.cultureName;
            }));
            setCultures(cultures);
        }

        fetchData();
    }, [params]);

    params.get('id');

    async function handleSubmit(e) {
        e.preventDefault();

        const user = {
            id: id,
            firstName: firstName,
            lastName: lastName,
            cultureName: culture.value,
            timeZoneId: timeZone.value,
            email: email,
            balance: balance,
        };

        await UserService.update(user).then(() => {
            navigate(`/Account/EditUser?id=${id}`);
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });
    }

    return (
        <div>
            {failed && (<div className="alert alert-danger">{t(error)}</div>)}
            <h3>{t("Edit user")}</h3>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>{t("First name")}:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={firstName}
                        onChange={e => setFirstName(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Last name")}:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={lastName}
                        onChange={e => setLastName(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Email")}:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Time zone")}:</label>
                    <Select
                        onChange={setTimeZone}
                        options={timeZones}
                        value={timeZone}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Language")}:</label>
                    <Select
                        onChange={setCulture}
                        options={cultures}
                        value={culture}
                    />
                </div>
                <div className="form-group">
                    <input type="submit" value={t("Submit")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}