import { useState, React, useEffect } from "react";
import UserService from "../../services/UserService";
import { useTranslation } from 'react-i18next';
import { useSearchParams } from "react-router-dom";
import { useAlert } from "react-alert";

export default function AddFunds() {
    const { t } = useTranslation();
    const alert = useAlert();
    const [params] = useSearchParams();
    const [user, setUser] = useState({});

    useEffect(() => {
        async function fetchData() {
            const id = params.get('id');
            const user = await UserService.getById(id);

            setUser(user);
        }

        fetchData();
    }, [params]);

    async function handleSubmit(e) {
        e.preventDefault();

        const updatedUser = {
            id: user.id,
            firstName: user.firstName,
            lastName: user.lastName,
            cultureName: user.cultureName,
            timeZoneId: user.timeZoneId,
            email: user.email,
            balance: user.balance + 100,
        };

        await UserService.update(updatedUser).then(() => {
            setUser(updatedUser);
        }).catch(error => {
            alert.error(error.response.data.error);
        });
    }

    const formatter = new Intl.NumberFormat(user.culture, {
        style: 'currency',
        currency: 'USD',
    });

    return (
        <div>
            <h3>{t("Add funds to account")}</h3>
            <div>{t(`Current balance: ${formatter.format(user.balance)}`)}</div>
            <form onSubmit={handleSubmit}>
                <div className="form-group mt-2">
                    <input type="submit" value={t(`Add ${formatter.format(100)}`)} className="btn btn-primary" />
                </div>
            </form>
        </div>
    )
}