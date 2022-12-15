$(document).ready(function () {

    $("#Link_Select").val("");

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
            if (div.find('.Telegram_Link').length === 0) {
                div.append(`<tr class="Telegram_Link d-flex flex-row m-2"><td style="width: 30%" class="text-start">Telegram:</td><td style="width: 70%"><input class="text-center w-100 p-1" placeholder="Введите имя пользователя" /><img class="Cross ml-2 position-absolute" style="transform: translateY(40%);" src="src/Cross.svg" /></td></tr>`);
            }
            $(this).val("");
        } else if ($(this).val() == "WhatsApp") {
            if (div.find('.WhatsApp_Link').length === 0) {
                div.append(`<tr class="WhatsApp_Link d-flex flex-row m-2"><td style="width: 30%" class="text-start">WhatsApp:</td><td style="width: 70%"><input class="text-center w-100 p-1" placeholder="Введите номер" /><img class="Cross ml-2 position-absolute" style="transform: translateY(40%);" src="src/Cross.svg" /></td></tr>`);
            }
            $(this).val("");
        } else if ($(this).val() == "Email/Gmail") {
            if (div.find('.Email_Link').length === 0) {
                div.append(`<tr class="Email_Link d-flex flex-row m-2"><td style="width: 30%" class="text-start">Email/Gmail:</td><td style="width: 70%"><input class="text-center w-100 p-1" placeholder="Введите Email/Gmail" /><img class="Cross ml-2 position-absolute" style="transform: translateY(40%);" src="src/Cross.svg" /></td></tr>`);
            }
            $(this).val("");
        } else {
            if (div.find('.Another').find('input').val() != '')
                div.append(`<tr class="Another d-flex flex-row m-2"><td style="width: 100%"><input class="text-center w-100 p-1" placeholder="Введите ссылку" /><img class="Cross ml-2 position-absolute" style="transform: translateY(40%);" src="src/Cross.svg" /></td></tr>`);
            $(this).val("");
        }
    });

    $("#Link_Select > .Cross").click(function () {
        $(this).parent().parent().remove();
    });
})