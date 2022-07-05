var culture = getCookie(".AspNetCore.Culture").slice(-5)

new tempusDominus.TempusDominus(document.getElementById('datetimepicker1'), {
    localization: {
        locale: culture,
    },
    restrictions:
    {
        minDate: Date()
    },
    display: { components: { seconds: false } },
})

new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
    localization: {
        locale: culture,
    },
    restrictions:
    {
        minDate: Date()
    },
    display: { components: { seconds: false } }
})

function getCookie(name) {
    let cookie = {};
    document.cookie.split(';').forEach(function (el) {
        let [k, v] = el.split('=');
        cookie[k.trim()] = v;
    })
    return cookie[name];
}