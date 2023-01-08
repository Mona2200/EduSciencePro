$(document).ready(function () {

    $(".Search_Education").width($(".EducationInput").parent().width() - 2 + "px");

    $(".Search_PlaceWork").width($(".PlaceWorkInput").parent().width() - 2 + "px");

    $(".Search_Organization").width($(".OrganizationInput").parent().width() - 2 + "px");

    $(".Search_Tags").width($(".TagsInput").parent().width() - 2 + "px");

    $("#Link_Select").val("");

    //var tags = $("#tags_input").val().split(/\s+/);
    //if (tags != "") {
    //    for (var int = 0; i < tags.length; i++) {
    //        $("#div_tags").append('<span class="bg_blue p-2 m-2">' + tags[i] + '</span>');
    //    }
    //}



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

    $("#tag_click").click(function () {
        var str = $(this).prev().val();
        if (str.replace(/\s/g, "") != "") {
            $("#div_tags").append('<span class="bg_blue p-2 m-2"><span>' + str + '</span><span style="padding-left: 5px;cursor:pointer;" onclick="remove_tag(this)">&#10008</span></span>');
            $("#tags_input").val($("#tags_input").val() + " " + str);
            $(this).prev().val('');
        }
    });

    $('.publ_inp[type="radio"]').click(function () {
        if ($(this).is(':checked')) {
            console.log($(this).val());
            if ($(this).val() == 'true') {
                console.log("новость");
                if ($(".div_cover").hasClass("d-none")) {
                    $(".div_cover").removeClass("d-none");
                }
            }
            else {
                console.log("обсуждение");
                if (!$(".div_cover").hasClass("d-none")) {
                    $(".div_cover").addClass("d-none");
                }
            }
        }
    });

    $(".ability_post").click(function () {
        if ($(this).next().hasClass('d-none')) {
            $(this).next().removeClass('d-none');
        } else {
            $(this).next().addClass('d-none');
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

function Search_Education_Click(e) {
    $(".EducationInput").val($(e).text());
    $(".Search_Education p").each(function () {
        $(this).remove();
    });
};

function Search_PlaceWork_Click(e) {
    $(".PlaceWorkInput").val($(e).text());
    $(".Search_PlaceWork p").each(function () {
        $(this).remove();
    });
};

function Search_Organization_Click(e) {
    $(".OrganizationInput").val($(e).text());
    $(".Search_Organization p").each(function () {
        $(this).remove();
    });
};

function Search_Tags_Click(e) {
    $(".TagsInput").val($(e).text());
    $(".Search_Tags p").each(function () {
        $(this).remove();
    });
};

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

function remove_tag(e) {
    var tag = $(e).prev().text();
    var inputTag = $("#tags_input").val();
    inputTag = inputTag.replace(tag, '')
    $("#tags_input").val(inputTag);
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

function Button_Add_Post(e) {
    var html = $("#editor").find(".ProseMirror.toastui-editor-contents").html();
    var encoded = new TextEncoder().encode(html);
    var cover = $(".input_cover_post")[0].files[0];
    console.log(encoded);
    $(e).Title = $(".input_title_post").val();
    $(e).Content = encoded;
    $(e).Tags = $("#tags_input").val();
    $(e).IsNew = $(".publ_inp").val();
    $(e).Cover = cover;

    $.ajax({
        url: '@Url.Action("AddPost", "Post")',
        type: 'POST',
        mimeType: 'multipart/form-data',
        data: e,
        success: function (data) {
            console.log(true);
        },
        error: function () {

        }
    });
};


//$("#button_addpost").click(function () {
//    var html = $("#editor").find(".ProseMirror.toastui-editor-contents").html();
//    var encoded = new TextEncoder().encode(html);
//    var cover = $(".input_cover_post")[0].files[0];
//    console.log(encoded);
//    var model = new FormData();
//    model.append('Title', $(".input_title_post").val());
//    model.append('Content', encoded);
//    model.append('Tags', $("#tags_input").val());
//    model.append('IsNew', $(".publ_inp").val());
//    model.append('Cover', cover);
//    //{
//    //    Title: $(".input_title_post").val(),
//    //    Content : encoded,
//    //    Tags: $("#tags_input").val(),
//    //    IsNew: $(".publ_inp").val(),
//    //    Cover : cover
//    //};
//    console.log(model);
//    $.ajax({
//        url: '@Url.Action("AddPost", "Post")',
//        type: 'POST',
//        data: model,
//        success: function (data) {
//            console.log(true);
//        },
//        error: function () {

//        }
//    });
//});