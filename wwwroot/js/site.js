$(document).ready(function () {

    $("input[type = checkbox]").change(function () {
        $(this).parent().find('.before_checkbox').toggleClass('checked');

        if ($(this).parent().hasClass('type_user')) {
            if ($(this).is(':checked')) {
                var t = $("#types_user").val();
                $("#types_user").val(t + $(this).val() + ' ');
            } else {
                var t = $("#types_user").val();
                var newdata = t.replace($(this).val(), '');
                $("#types_user").val(newdata);
            }
        };
    });
})