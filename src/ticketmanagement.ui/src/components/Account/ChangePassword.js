import { useState, React } from "react";
import UserService from "../../services/UserService";
import { useTranslation } from 'react-i18next';
import { useSearchParams } from "react-router-dom";
import { useAlert } from "react-alert";
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';

export default function ChangePassword() {
    const { t } = useTranslation();

    const formSchema = Yup.object().shape({
        oldPassword: Yup.string()
            .required(t('Old password is required'))
            .min(5, t('Password must be at least 5 charaters long')),
        newPassword: Yup.string()
            .required(t('New password is required'))
            .min(5, t('Password must be at least 5 charaters long')),
        confirmNewPassword: Yup.string()
            .required(t('New password confirmation is required'))
            .oneOf([Yup.ref('newPassword')], t('Passwords does not match')),
    })

    const formOptions = { resolver: yupResolver(formSchema) }
    const { register, handleSubmit, formState } = useForm(formOptions)
    const { errors } = formState

    const alert = useAlert();
    const [params] = useSearchParams();
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmNewPassword, setConfirmNewPassword] = useState('');

    async function onSubmit() {
        const id = params.get('id');

        await UserService.changePassword(id, oldPassword, newPassword).then(() => {
            alert.success(t("Password was changed!"));
        }).catch(error => {
            alert.error(error.response.data.error);
        });
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <h3>{t("Change password")}</h3>
            <div className="form-group">
                <label>{t("Old password")}:</label>
                <input
                    type="password"
                    name="oldPassword"
                    {...register('oldPassword')}
                    className={`form-control ${errors.oldPassword ? 'is-invalid' : ''}`}
                    value={oldPassword}
                    onChange={e => setOldPassword(e.target.value)}
                />
                <div className="invalid-feedback">{errors.oldPassword?.message}</div>
            </div>
            <div className="form-group">
                <label>{t("New password")}:</label>
                <input
                    type="password"
                    name="newPassword"
                    {...register('newPassword')}
                    className={`form-control ${errors.newPassword ? 'is-invalid' : ''}`}
                    value={newPassword}
                    onChange={e => setNewPassword(e.target.value)}
                />
                <div className="invalid-feedback">{errors.newPassword?.message}</div>
            </div>
            <div className="form-group">
                <label>{t("Confirm new password")}:</label>
                <input
                    type="password"
                    name="confirmNewPassword"
                    {...register('confirmNewPassword')}
                    className={`form-control ${errors.confirmNewPassword ? 'is-invalid' : ''}`}
                    value={confirmNewPassword}
                    onChange={e => setConfirmNewPassword(e.target.value)}
                />
                <div className="invalid-feedback">{errors.confirmNewPassword?.message}</div>
            </div>
            <div className="form-group mt-3">
                <input type="submit" value={t("Submit")} className="btn btn-primary" />
            </div>
        </form>
    )
}