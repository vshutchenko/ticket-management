import { useState, React } from "react";
import { useNavigate } from "react-router";
import AuthService from "../../services/AuthService";
import { useTranslation } from 'react-i18next';
import TimezoneSelect from 'react-timezone-select'
import Select from "react-select";

export default function Register() {
    const navigate = useNavigate();
    const { t } = useTranslation();

    const cultures = [
        { value: 'en-US', label: 'English' },
        { value: 'ru-RU', label: 'Русский' },
        { value: 'be-BY', label: 'Беларуская' }
    ];

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [culture, setCulture] = useState(cultures[0]);
    const [timeZone, setTimeZone] = useState(Intl.DateTimeFormat().resolvedOptions().timeZone);
    const [password, setPassword] = useState('');
    const [email, setEmail] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);

    async function handleSubmit(e) {
        e.preventDefault();

        const user = {
            "firstName": firstName,
            "lastName": lastName,
            "cultureName": culture.value,
            "timeZoneId": timeZone.altName,
            "email": email,
            "password": password
        };

        await AuthService.register(user).then(() => {
            navigate('/');
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });
    }

    return (
        <div className="event-form">
            {failed && (<div className="alert alert-danger">{t(error)}</div>)}
            <h3>{t("Register")}</h3>
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
                    <label>{t("Password")}:</label>
                    <input
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Confirm password")}:</label>
                    <input
                        type="password"
                        className="form-control"
                        value={confirmPassword}
                        onChange={e => setConfirmPassword(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label>{t("Time zone")}:</label>
                    <TimezoneSelect
                        value={timeZone}
                        onChange={setTimeZone}
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