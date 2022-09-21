import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import AuthService from "../../services/AuthService";
import { useTranslation } from 'react-i18next';
import Select from "react-select";
import { useAlert } from "react-alert";
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';
import timeZonesData from '../../data/timeZones.json';

export default function Register() {
    const navigate = useNavigate();
    const alert = useAlert();
    const { t } = useTranslation();

    const formSchema = Yup.object().shape({
        firstName: Yup.string()
            .required(t('First name is required')),
        lastName: Yup.string()
            .required(t('Last name is required')),
        email: Yup.string()
            .required(t('Email is required'))
            .email(t('Enter valid email')),
        password: Yup.string()
            .required(t('Password is required'))
            .min(5, t('Password must be at least 5 charaters long')),
        confirmPassword: Yup.string()
            .required(t('Password confirmation is required'))
            .oneOf([Yup.ref('password')], t('Passwords does not match')),
    })

    const formOptions = { resolver: yupResolver(formSchema) }
    const { register, handleSubmit, formState, getValues } = useForm(formOptions)
    const { errors } = formState

    const cultures = [
        { value: 'en-US', label: 'English' },
        { value: 'ru-RU', label: 'Русский' },
        { value: 'be-BY', label: 'Беларуская' }
    ];

    const [culture, setCulture] = useState(cultures[0]);
    const [timeZone, setTimeZone] = useState({});
    const [timeZones, setTimeZones] = useState([]);

    useEffect(() => {
        let timeZonesList = timeZonesData.map(tz => {
            return { value: tz.id, label: tz.name };
        })

        setTimeZones(timeZonesList);
        setTimeZone(timeZonesList[0]);
    }, []);

    async function onSubmit() {
        const user = {
            "firstName": getValues('firstName'),
            "lastName": getValues('lastName'),
            "cultureName": culture.value,
            "timeZoneId": timeZone.value,
            "email": getValues('email'),
            "password": getValues('password')
        };

        await AuthService.register(user).then(() => {
            navigate('/');
        }).catch(error => {
            alert.error(t(error.response.data.error, { ns: 'validation'}));
        });
    }

    return (
        <div className="container mt-5">
            <div className="row">
                <div className="col-md-6 offset-md-3">
                    <form className="shadow rounded-5 px-5 py-3" onSubmit={handleSubmit(onSubmit)}>
                        <div className="mb-4">
                            <h2 className="text-center">{t("Register")}</h2>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("First name")}:</label>
                            <input
                                type="text"
                                {...register('firstName')}
                                className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.firstName?.message}</div>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Last name")}:</label>
                            <input
                                type="text"
                                {...register('lastName')}
                                className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.lastName?.message}</div>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Email")}:</label>
                            <input
                                type="text"
                                {...register('email')}
                                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.email?.message}</div>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Password")}:</label>
                            <input
                                type="password"
                                {...register('password')}
                                className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.password?.message}</div>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Confirm password")}:</label>
                            <input
                                type="password"
                                {...register('confirmPassword')}
                                className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.confirmPassword?.message}</div>
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Time zone")}:</label>
                            <Select
                                onChange={setTimeZone}
                                options={timeZones}
                                value={timeZone}
                            />
                        </div>

                        <div className="mb-2">
                            <label className="form-label">{t("Language")}:</label>
                            <Select
                                onChange={setCulture}
                                options={cultures}
                                value={culture}
                            />
                        </div>

                        <div className="mb-3 text-center">
                            <input type="submit" value={t("Submit")} className="btn btn-lg btn-primary" />
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}