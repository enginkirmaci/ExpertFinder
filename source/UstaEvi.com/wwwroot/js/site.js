function FillDistrict(input) {
    var districtId = $(input).val();
    $.ajax({
        url: '/api/JSON/FillDistrict/' + districtId,
        type: "GET",
        dataType: "JSON",
        success: function (districts) {
            $("#districtSelect").html(""); // clear before appending new list
            $.each(districts, function (i, district) {
                $("#districtSelect").append(
                    $('<option></option>').val(district.Value).html(district.Text));
            });

            $("#districtSelect").trigger("chosen:updated");
        }
    });
}

$(function () {
    if ($('.input-validation-error').length > 0) {
        $('html, body').animate({ scrollTop: $('.input-validation-error:first').offset().top }, 400);
    }
    if ($('.validation-summary-errors').length > 0) {
        $('html, body').animate({ scrollTop: $('.validation-summary-errors').offset().top }, 400);
    }

    $('#buttonTeklifVer').on('click', function (e) {
        $('#bannerHizmetVer').slideUp(500);
        $('#bannerTeklifVer').delay(500).slideDown(500);

        return false;
    });
    $('#buttonHizmetVer').on('click', function (e) {
        $('#bannerTeklifVer').slideUp(500);
        $('#bannerHizmetVer').delay(500).slideDown(500);

        return false;
    });

    $('#GiveOffer').on('click', function (e) {
        $('.user-categories select').val([]);
        $('.user-categories select').trigger("chosen:updated");
        $('.user-categories').toggle();
    });

    $('[name*="Phone"').on('focus', function (e) {
        var element = $(this);
        if (element.val().trim() == '')
            element.val('0');
    });
    $('[name*="Phone"').on('blur', function (e) {
        var element = $(this);
        if (element.val().trim() == '0')
            element.val('');
    });

    if ($('[name*="UnknownPrice"').prop('checked'))
        $('.j-price').hide();
    else
        $('.j-price').show();

    $('[name*="UnknownPrice"').on('change', function (e) {
        if ($(this).prop('checked'))
            $('.j-price').hide();
        else
            $('.j-price').show();
    });

    $('.chosen-select').on('change', function (e) {
        $(this).trigger("chosen:updated");
        $(this).parent().find('.field-validation-error').hide();
    });

    $('.starrr').starrr({
        change: function (e, value) {
            $(".js-rate-value").val(value);
        }
    });

    $('.js-submit').on('change', function () {
        if (this.form)
            this.form.submit();
    });

    if (typeof (page_pos) != "undefined") {
        $("[data-page='" + page_pos + "']").attr('id', 'current');
    }

    if (typeof (returnUrl) != "undefined") {
        $.magnificPopup.open({
            type: 'inline',
            items: {
                src: $('#small-dialog')
            },

            fixedContentPos: false,
            fixedBgPos: true,

            overflowY: 'auto',

            closeOnBgClick: false,
            enableEscapeKey: false,
            showCloseBtn: false,

            closeBtnInside: true,
            preloader: false,

            midClick: true,
            removalDelay: 300,
            mainClass: 'my-mfp-zoom-in',

            callbacks: {
                open: function () {
                    $('#modalOK').on('click', function () {
                        $.magnificPopup.close();
                    });
                },
                close: function () {
                    if (returnUrl != '') {
                        window.location.href = returnUrl;
                    }
                }
            }
        });
    }

    $('.gallery, .gallery-edit').each(function () { // the containers for all your galleries
        $(this).magnificPopup({
            delegate: '.img-container', // the selector for gallery item
            type: 'image',
            gallery: {
                enabled: true,
                tCounter: ''
            },
            zoom: {
                enabled: true, // By default it's false, so don't forget to enable it

                duration: 300, // duration of the effect, in milliseconds
                easing: 'ease-in-out', // CSS transition easing function

                // The "opener" function should return the element from which popup will be zoomed in
                // and to which popup will be scaled down
                // By defailt it looks for an image tag:
                opener: function (openerElement) {
                    // openerElement is the element on which popup was initialized, in this case its <a> tag
                    // you don't need to add "opener" option if this code matches your needs, it's defailt one.
                    return openerElement.is('img') ? openerElement : openerElement.find('img');
                }
            }
        });
    });
});