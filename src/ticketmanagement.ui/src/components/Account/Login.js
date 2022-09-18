import { useState, React } from "react";
import { useNavigate } from "react-router";
import AuthService from "../../services/AuthService";
import { useTranslation } from 'react-i18next';

export default function Login() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [failed, setFailed] = useState(false);

    async function handleSubmit(e) {
        e.preventDefault();

        await AuthService.login(email, password).then(() =>{
            navigate('/');
        }).catch(error => {
            setFailed(true);
            setError(error.response.data.error);
        });

       
    }

    return (
        <div className="event-form">
            {failed && (<div className="alert alert-danger">{t(error, { ns: 'validation' })}</div>)} 
            <h3>{t("Login")}</h3>
            <form onSubmit={e => handleSubmit(e)}>
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
                    <input type="submit" value={t("Login")} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}