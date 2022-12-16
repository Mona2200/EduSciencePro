$(document).ready(function () {

    $("#Link_Select").val("");

    $("#Links_Div input").each(function () {
        if ($(this).val() != '') {
            $(this).parent().parent().removeClass('d-none');
        };
    });

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

    $("#Menu_Link").click(function () {
        $("#desktop_menu").toggleClass('d-none');
    });

    $("#Link_Select").change(function () {
        var div = $("#Links_Div");

        if ($(this).val() == "Telegram") {
            if (div.find('.Telegram_Link').hasClass('d-none')) {
                div.find('.Telegram_Link').toggleClass('d-none');
            }
            $(this).val("");
        } else if ($(this).val() == "WhatsApp") {
            if (div.find('.WhatsApp_Link').hasClass('d-none')) {
                div.find('.WhatsApp_Link').toggleClass('d-none');
            }
            $(this).val("");
        } else if ($(this).val() == "Почта") {
            if (div.find('.Email_Link').hasClass('d-none')) {
                div.find('.Email_Link').toggleClass('d-none');
            }
            $(this).val("");
        } else {
            if (div.find('.Another').hasClass('d-none')) {
                div.find('.Another').toggleClass('d-none');
            }
            $(this).val("");
        }

        if (!div.find('.Telegram_Link').hasClass('d-none') && !div.find('.WhatsApp_Link').hasClass('d-none') && !div.find('.Email_Link').hasClass('d-none') && !div.find('.Another').hasClass('d-none')) {
            $(this).addClass('d-none');
        }

    });
})

function Cross_Click(e) {
    $(e).parent().parent().addClass('d-none');
    $(e).prev().val("");
    if ($("#Link_Select").hasClass('d-none')) {
        $("#Link_Select").removeClass('d-none');
    }
};