$(function () {
    $('#venue').change(function () {
        var id = $(this).val();
        $.ajax({
            type: 'GET',
            url: '@Url.Action("PartialLayoutList")?venueId=' + id,
            success: function (data) {
                $('#layout').replaceWith(data);
            }
        });
    });
})