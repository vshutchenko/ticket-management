import AuthService from "../services/AuthService";
import { Russian } from "flatpickr/dist/l10n/ru.js"
import { Belarusian } from "flatpickr/dist/l10n/be.js"
import { english } from "flatpickr/dist/l10n/default"

export function getPickerLocale() {
    const user = AuthService.getCurrentUser();
    const culture = user ? user.culture : 'en-US';

    if (culture === 'ru-RU')
        return Russian;

    if (culture === 'be-BY')
        return Belarusian;

    if (culture === 'en-US')
        return english;
}