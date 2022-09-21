import authService from "../services/AuthService";
import moment from "moment-timezone";
import 'moment-timezone';
import { findIana } from "windows-iana";

export function getUserTime() {
    let ianaTimeZone = getIanaTimeZone();

    return new Date(Date().toLocaleString({ timeZone: ianaTimeZone }));
}

export function localeDateToUtc(date) {
    let ianaTimeZone = getIanaTimeZone();

    let utc = moment
        .tz(date, Intl.DateTimeFormat().resolvedOptions().timeZone)
        .tz(ianaTimeZone, true)
        .tz("UTC").format();

    return utc;
}

export function utcToLocaleDate(date) {
    let ianaTimeZone = getIanaTimeZone();
    
    let utc = moment.tz(date, "UTC").tz(ianaTimeZone).format();

    const options = {
        timeZone: ianaTimeZone,
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    };

    const culture = "en-US";

    return new Date(new Date(utc).toLocaleString(culture, options));
}

export function utcToLocaleString(date) {
    let ianaTimeZone = getIanaTimeZone();

    const culture = authService.isAuthenticated()
        ? authService.getCurrentUser().culture
        : "en-US";

    let utc = moment.tz(date, "UTC").tz(ianaTimeZone).format();
    
    const options = {
        timeZone: ianaTimeZone,
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    };

    return new Date(utc).toLocaleString(culture, options);
}

export function toLocaleDate(date) {
    const options = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    };

    const culture = authService.isAuthenticated()
        ? authService.getCurrentUser().culture
        : "en-US";

    return new Date(date).toLocaleString(culture, options);
}

function getIanaTimeZone()
{
    let ianaTimeZone;

    if (authService.isAuthenticated()) {
        let timeZone = authService.getCurrentUser().timezoneId;
        ianaTimeZone = findIana(timeZone)[0];
    }
    else {
        ianaTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    }

    return ianaTimeZone;
}