import { useState, React, useEffect } from "react";
import { useNavigate } from "react-router";
import UserService from "../../services/UserService";
import { useTranslation } from 'react-i18next';
import Select from "react-select";
import { useSearchParams } from "react-router-dom";
import { useAlert } from "react-alert";
import timeZonesData from '../../data/timeZones.json';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';

export default function EditUser() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const alert = useAlert();

    const formSchema = Yup.object().shape({
        firstName: Yup.string()
            .required(t('First name is required')),
        lastName: Yup.string()
            .required(t('Last name is required')),
        email: Yup.string()
            .required(t('Email is required'))
            .email(t('Enter valid email')),
    })

    const formOptions = { resolver: yupResolver(formSchema) }
    const { register, handleSubmit, formState, setValue, getValues } = useForm(formOptions)
    const { errors } = formState
    const [params] = useSearchParams();

    const [culture, setCulture] = useState({});
    const [cultures, setCultures] = useState({});
    const [timeZone, setTimeZone] = useState({});
    const [timeZones, setTimeZones] = useState([]);
    const [balance, setBalance] = useState(0);

    useEffect(() => {
        async function fetchData() {
            const id = params.get('id');
            const user = await UserService.getById(id);

            const cultures = [
                { value: 'en-US', label: 'English' },
                { value: 'ru-RU', label: 'Русский' },
                { value: 'be-BY', label: 'Беларуская' }
            ];

            let userCulture = cultures.find(c => {
                return c.value === user.cultureName;
            });

            let timeZonesList = timeZonesData.map(tz => {
                return { value: tz.id, label: tz.name };
            })

            let userTimeZone = timeZonesList.find(tz => {
                return tz.value === user.timeZoneId;
            })

            setValue('firstName', user.firstName);
            setValue('lastName', user.lastName);
            setValue('email', user.email);
            setBalance(user.balance);
            setCulture(userCulture);
            setTimeZone(userTimeZone);
            setCultures(cultures);
            setTimeZones(timeZonesList);
        }

        fetchData();
    }, [params, setValue]);

    async function onSubmit() {
        const user = {
            id: params.get('id'),
            firstName: getValues('firstName'),
            lastName: getValues('lastName'),
            email: getValues('email'),
            cultureName: culture.value,
            timeZoneId: timeZone.value,
            balance: balance,
        };

        await UserService.update(user).then(() => {
            navigate(`/Account/EditUser?id=${user.id}`);
            alert.success('Profile was updated!');
        }).catch(error => {
            alert.error(t(error.response.data.error, { ns: 'validation'}));
        });
    }

    return (
        <div className="container mt-3">
            <div className="row">
                <div className="col-md-6">
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className="mb-4">
                            <h2>{t("Edit user")}</h2>
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
                            <label className="form-label">{t("Time zone")}:</label>
                            <Select
                                value={timeZone}
                                options={timeZones}
                                onChange={setTimeZone}
                            />
                        </div>
                        <div className="mb-2">
                            <label className="form-label">{t("Language")}:</label>
                            <Select
                                value={culture}
                                options={cultures}
                                onChange={setCulture}
                            />
                        </div>
                        <div className="mb-3">
                            <input type="submit" value={t("Submit")} className="btn btn-primary" />
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}