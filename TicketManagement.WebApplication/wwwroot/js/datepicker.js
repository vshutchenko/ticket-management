var culture = getCookie(".AspNetCore.Culture").slice(-5)

new tempusDominus.TempusDominus(document.getElementById('datetimepicker1'), {
    localization: {
        locale: culture,
    },
    display: { components: { seconds: false } },
    restrictions:
    {
        minDate: Date()
    }, 
})

new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
    localization: {
        locale: culture,
    },
    display: { components: { seconds: false } },
    restrictions:
    {
        minDate: Date()
    },
})

function getCookie(name) {
    let cookie = {};
    document.cookie.split(';').forEach(function (el) {
        let [k, v] = el.split('=');
        cookie[k.trim()] = v;
    })
    return cookie[name];
}