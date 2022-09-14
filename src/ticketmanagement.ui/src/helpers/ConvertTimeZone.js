import authService from "../services/AuthService";
import moment from "moment-timezone";
import 'moment-timezone';
import { findIana } from "windows-iana";

export function getUserTime() {
    const tzString = authService.isAuthenticated()
        ? authService.getCurrentUser().timezoneId
        : Intl.DateTimeFormat().resolvedOptions().timeZone;

    return new Date(Date().toLocaleString({ timeZone: tzString }));
}

export function localeDateToUtc(date) {
    const timeZone = authService.getCurrentUser().timezoneId;
    var ianaTimeZone = findIana(timeZone)[0];

    var utc = moment
        .tz(date, Intl.DateTimeFormat().resolvedOptions().timeZone)
        .tz(ianaTimeZone, true)
        .tz("UTC").format();

    return utc;
}

export function utcToLocaleDate(date) {
    const timeZone = authService.isAuthenticated()
        ? authService.getCurrentUser().timezoneId
        : Intl.DateTimeFormat().resolvedOptions().timeZone;

    var ianaTimeZone = findIana(timeZone)[0];
    
    var utc = moment.tz(date, "UTC").tz(ianaTimeZone).format();

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
    const timeZone = authService.isAuthenticated()
        ? authService.getCurrentUser().timezoneId
        : Intl.DateTimeFormat().resolvedOptions().timeZone;

    var ianaTimeZone = findIana(timeZone)[0];

    const culture = authService.isAuthenticated()
        ? authService.getCurrentUser().culture
        : "en-US";

    var utc = moment.tz(date, "UTC").tz(ianaTimeZone).format();
    
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