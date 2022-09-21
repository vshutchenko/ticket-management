import { React } from "react";
import { useNavigate } from "react-router";
import AuthService from "../../services/AuthService";
import { useTranslation } from 'react-i18next';
import { useAlert } from "react-alert";
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';

export default function Login() {
    const navigate = useNavigate();
    const alert = useAlert();
    const { t } = useTranslation();

    const formSchema = Yup.object().shape({
        email: Yup.string()
            .required(t('Email is required'))
            .email(t('Enter valid email')),
        password: Yup.string()
            .required(t('Password is required'))
            .min(5, t('Password must be at least 5 charaters long')),
    })

    const formOptions = { mode: "onChange", resolver: yupResolver(formSchema) }
    const { register, handleSubmit, getValues, formState } = useForm(formOptions)
    const { errors } = formState

    async function onSubmit() {
        const email = getValues('email');
        const password = getValues('password');
        
        await AuthService.login(email, password).then(() => {
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
                            <h2 className="text-center">{t("Login")}</h2>
                        </div>

                        <div className="mb-4">
                            <input
                                type="text"
                                name="email"
                                placeholder={t("Email")}
                                {...register('email')}
                                className={`form-control form-control-lg ${errors.email ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.email?.message}</div>
                        </div>

                        <div className="mb-4">
                            <input
                                type="password"
                                name="password"
                                placeholder={t("Password")}
                                {...register('password')}
                                className={`form-control form-control-lg ${errors.password ? 'is-invalid' : ''}`}
                            />
                            <div className="invalid-feedback">{errors.password?.message}</div>
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