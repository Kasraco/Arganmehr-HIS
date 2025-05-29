var AdminAction = new Object();
AdminAction.ActivUser = function (data) {
    if (data.result == "success") {
        generate("information", "کاربر فعال شد");
        $("#idGetNotActiveUsers").html(data.PartialViewHtml)
    }
    else if (data.result == "Faild") {
        generate("error", "کاربر غیرفعال شد");
    }
}

AdminAction.DeBanedUser = function (data) {
    if (data.result == "success") {
        generate("information", "حساب کاربری فعال شد");
        
    }
    else if (data.result == "Faild") {
        generate("error", "حساب کاربر غیرفعال شد");
    }
}

function generate(type, text) {

    var n = noty({
        text: text,
        type: type,
        dismissQueue: true,
        layout: 'topLeft',
        closeWith: ['click'],
        theme: 'relax',
        "timeout": 2500,
        maxVisible: 10,
        animation: {
            open: 'animated bounceInLeft',
            close: 'animated bounceOutLeft',
            easing: 'swing',
            speed: 500
        }
    });
    console.log('html: ' + n.options.id);
}

var LogOnForm = new Object();

LogOnForm.onSuccess = function () {
    $('#logOnModal').modal('show');
};

function loadAjaxComponents() {


    checkboxChecked();
    ajaxBind();
    slideButton();
    autoComplete();
    radioButtonSearch();
    $('select').chosen();
    $('.date-picker').datepicker({ changeMonth: true, changeYear: true });
}
function showLoading() {

    var top = ($(window).height() - $('div#loading').height()) / 2;

    $('div#loading').css({ 'z-index': '2000', 'top': top + $(document).scrollTop() }).fadeIn('slow');
}
function hideLoading() {
    $('div#loading').fadeOut('slow').css('z-index', '10');
}



function checkboxChecked() {
    $('#user-table input[type=checkbox]').on('mousedown', function () {
        //$this = $(this);
        if (!$(this).is(':checked')) {
            $(this).parents('tr').addClass('selected-row');
        } else {
            $(this).parents('tr').removeClass('selected-row');
        }

    });
}
function ajaxBind() {
    $('[data-i-ajax=true]').each(function () {
        //$this = $(this);
        $(this).off('click');
        $(this).on('click', function () {
            doAjax($(this), $(this).attr('data-i-src'), $(this).attr('data-i-ajax-method'), $(this).attr('data-i-data'), $(this).attr('data-i-target-id'));
        });
    });

}

function doAjax(caller, src, method, data, targetId) {
    showLoading();
    $.ajax({
        type: method,
        url: src,
        data: data,
        dataType: "json",
        complete: function (response, status) {

            hideLoading();
            $(targetId).html(response.responseText);
            if (caller.attr('data-i-show-dialog') == "true") {
                showDialog(targetId, caller);
                loadAjaxComponents();
                run(function () {
                    $(targetId).removeData("validator");
                    $(targetId).removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(targetId); $('#date-picker').datepicker({ changeMonth: true, changeYear: true });
                });
            }

        }

    });
}

function slideButton() {
    $('[data-slide-toggle-id]').each(function () {
        var $this = $(this);
        $this.off('click');
        $this.on('click', function (e) {
            e.preventDefault();
            $this.text(($this.text() === "پیشرفته") ? "معمولی" : "پیشرفته");
            $($this.attr('data-slide-toggle-id')).stop(true, false).slideToggle('blind');
        });
    });

}

function autoComplete() {
    $('input[data-autocomplete]').each(function () {
        $(this).autocomplete({ source: $(this).attr('data-autocomplete') + '?searchBy=' + $(this).attr('data-filter'), minLength: $(this).attr('data-min-length') });
    });
}
function radioButtonSearch() {
    $('div#search-types input[type=radio]').on('click', function () {
        $('input[data-autocomplete]').attr('data-filter', $(this).attr('value'));
        autoComplete();
    });
}