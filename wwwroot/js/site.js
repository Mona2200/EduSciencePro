$(document).ready(function () {



    $("#Link_Select").val("");

    $("#Links_Div input").each(function () {
        if ($(this).val() != '') {
            $(this).parent().parent().removeClass('d-none');
        };
    });

    $("#Links_Div").find('.field-validation-error').each(function () {
        if ($(this).parent().parent().prev().hasClass('d-none')) {
            $(this).parent().parent().prev().removeClass('d-none');
        }
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

    $("#Add_Skills").click(function () {
        var inp = $(".Skills_Input");
        if (inp.val() != "") {
            var skill = inp.val().replace(/\s/g,"");
            if (skill != "") {
                $("#Skills_List").append('<p class="text-start"><span class="p-1" style="background-color: #e9ecef;">' + skill + '</span><img onclick="Cross_Click_Skills(this)" class="Cross ml-2 position-absolute" style="transform: translateY(40%);" src="src/Cross.svg" /></p>');
                $("#Skills_Value").val($("#Skills_Value").val() + skill + " ");
                inp.val("");
            }
        }
    });

    //$(".Education").keyup(function (I) {
    //    switch (I.keyCode) {

    //        case 13:
    //        case 27:
    //        case 38:
    //        case 40:
    //            break;

    //        default:

    //            if ($(this).val().length > 2) {
    //                input_initial_value = $(this).val();
    //            };
    //    }
    //});

})

function Cross_Click(e) {
    $(e).parent().parent().addClass('d-none');
    $(e).parent().parent().next().addClass('d-none');
    $(e).prev().val("");
    if ($("#Link_Select").hasClass('d-none')) {
        $("#Link_Select").removeClass('d-none');
    }
};

function Cross_Click_Skills(e) {
    var val = $("#Skills_Value").val();
    val = val.replace($(e).prev().text(), "")
    $("#Skills_Value").val(val);
    $(e).parent().remove();
};

function Edu_KeyUp(NameEducations, inp) {
    Console.log(NameEducations);
    //if ($(inp).val().length > 2) {
    //    input_initial_value = $(inp).val();
    //    var n = 5;
    //    var k = 0;
    //    for (var i = 0; i < NameEducations.length && k < n; i++) {
    //        //if (NameEducations[i].substring(0, input_initial_value.length) == input_initial_value) {
    //        //    $(".Search_Education").append('<span>' + NameEducations[i] + '</span>');
    //        //    k++;
    //        //}
    //    }
    //};
};