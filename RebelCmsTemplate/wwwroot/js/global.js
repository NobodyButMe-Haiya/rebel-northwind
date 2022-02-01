const accessDeniedCode = 500;
function getLocaleDate(dateString){
    let dateStringOutput = new Date(dateString);
    return dateStringOutput.toLocaleDateString(); 
}
function callBackError() {
    Swal.fire("System", "System Error", "error");
}
function accessDenied() {
    let timerInterval = 0;
    Swal.fire({
        title: 'Auto close alert!',
        html: 'Session Out .Pease Re-login.I will close in <b></b> milliseconds.',
        timer: 2000,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading()
            const b = Swal.getHtmlContainer().querySelector('b')
            timerInterval = setInterval(() => {
                b.textContent = Swal.getTimerLeft()
            }, 100)
        },
        willClose: () => {
            clearInterval(timerInterval)
        }
    }).then((result) => {

        if (result.dismiss === Swal.DismissReason.timer) {
            console.log('session out .. ');
            location.href = "/";
        }
    });
}
function showPreview(event, imageTarget) {
    if (event.target.files.length > 0) {
        var src = URL.createObjectURL(event.target.files[0]);
        var preview = document.getElementById(imageTarget);
        console.log(event)
        if (event.target.files[0].size > 10485760) {
            alert("Too big dear file size.10 MB max size");
            event.target.files[0] = "";
        } else {
            preview.src = src;
            preview.style.display = "block";
        }
    }
}

function toggleChecked(status) {
    $(":checkbox").each(function () {
        if (status === true) {
            $(this).val(1);
            $(this).checked = true;
            $(this).attr("checked", true);
            $(this).prop("checked", true);
        } else {
            $(this).val(0);
            $(this).checked = false;
            $(this).attr("checked", false);
            $(this).prop("checked", false);
            $(this).removeAttr("checked");
        }


    });
}

function toggleRangeChecked(status, name) {
    //alert("status"+status);
    console.log("status" + status); // don't delete it..
    $('input:checkbox[name="' + name + '"]').each(function () {
        if (status === true) {
            $(this).val(1);
            $(this).checked = true;
            $(this).attr("checked", true);
            $(this).prop("checked", true);
        } else {
            $(this).val(0);
            $(this).checked = false;
            $(this).attr("checked", false);
            $(this).prop("checked", false);
            $(this).removeAttr("checked");
        }


    });
}

function validateMePassword(id) {
    validateMeColor(id, 0, '', '');
    $('#' + id).keyup(function () {
        var password = $('#' + id).val();
        if (password.length === 0) {
            validateMeColor(id, 1, 'error', '<img src=\'./images/icons/smiley-sad-blue.png\'> Please fill password field');
        } else if (password.length < 6) {
            validateMeColor(id, 1, 'warning', '<img src=\'./images/icons/smiley-sad-blue.png\'> Weak Password');
        } else {
            let regex_simple = /^[a-z]$/;
            let regex_capital = /^[A-Z]$/;
            let regex_numbers = /^[0-9]$/;

            let simple_status = '0';
            let capital_status = '0';
            let number_status = '0';
            let status_count = '0';
            for (i = 0; i < password.length; i++) {
                let check_character = password.charAt(i);
                if (regex_simple.test(check_character) && simple_status === '0') {
                    simple_status = '1';
                    status_count++;
                }
                if (regex_capital.test(check_character) && capital_status === '0') {
                    capital_status = '1';
                    status_count++;
                }
                if (regex_numbers.test(check_character) && number_status === '0') {
                    number_status = '1';
                    status_count++;
                }
            }
            switch (status_count) {
                case 0:
                    validateMeColor(id, 1, 'has-warning', '<img src=\'./images/icons/smiley-sad-blue.png\'> Weak Password');
                    break;
                case 1:
                    validateMeColor(id, 1, 'has-success', '<img src=\'./images/icons/smiley-neutral.png\'> Good Password');
                    break;
                case 2:
                    validateMeColor(id, 1, 'has-success', '<img src=\'./images/icons/smiley-wink.png\'> Strong Password');
                    break;
                case 3:
                    validateMeColor(id, 1, 'has-success', '<img src=\'./images/icons/smiley-wink.png\'> Superb Password');
                    break;
            }
        }
    });
}

function validateMeEmail(id) {
    // reset first any old validation color
    validateMeColor(id, 0, '', '');
    // start validate
    $("#" + id).blur(function () {
        var email = $('#' + id).val();
        var reEmail = /^[A-Za-z0-9][a-zA-Z0-9._-][A-Za-z0-9]+@([a-zA-Z0-9.-]+\.)+[a-zA-Z0-9.-]{2,4}$/;
        if (email === '') {
            validateMeColor(id, 1, 'has-warning', 'Field cannot be empty');
        } else if (email.length > 60) {
            validateMeColor(id, 1, 'has-warning', 'Email cannot exceed 60 characters');
        } else if (!reEmail.test(email)) {
            validateMeColor(id, 1, 'has-error', 'Invalid Email');
        } else {
            validateMeColor(id, 0, '', '');
        }
    });
}

function validateMeFilename(id) {
    $("#" + id).keyup(function () {
        $(this).val($(this).val().replace(/(?:^|\/|\\)((?:[a-z0-9])*\.(?:php))$/i, "").replace(/^\./, ""));
    }).blur(function () {
        $(this).val($(this).val().replace(/(?:^|\/|\\)((?:[a-z0-9])*\.(?:php))$/i, "").replace(/^/, ""));
    });
}

function validateMeAlphaNumeric(id) {
    $("#" + id).keyup(function () {
        $(this).val($(this).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^\./, ""));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^/, ""));
    });
}

function validateMeAlphaNumericRange(name) {
    // give coma dot space and email
    $("input:text[name='" + name + "[]']").keyup(function () {
        $(this).val($(this).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^\./, ""));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^/, ""));
    });
}

function validateMeAlphaNumericKeyUp(id) {
    $("#" + id).val($("#" + id).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^\./, ""));
}

function validateMeAlphaNumericBlur(id) {
    $("#" + id).val($("#" + id).val().replace(/[^0-9a-zA-Z\,\.\s\x20]/g, "").replace(/^\./, ""));
}

function validateMeNumeric(id) {
    $("#" + id).keyup(function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    });
}

function validateMeNumericRange(name) {
    $("input:text[name='" + name + "[]']").keyup(function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    });
}

function validateMeNumericKeyUp(id) {
    $("#" + id).val($("#" + id).val().replace(/[^0-9]/g, ''));
}

function validateMeNumericBlur(id) {
    $("#" + id).val($("#" + id).val().replace(/[^0-9]/g, ''));
}

function validateMeCurrency(id) {
    $("#" + id).on('keydown', function () {
        $(this).val($(this).val().replace(/[^,\-0-9.]/g, "").replace(/^\.,/, ""));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^,\-0-9.]/g, "").replace(/^\.,/, ""));
    });
}

function validateMeCurrencyRange(name) {
    $("input:text[name='" + name + "']").on('keydown', function () {
        $(this).val($(this).val().replace(/[^,\-0-9.]/g, "").replace(/^\.,/, ""));
    }).blur(function () {
        $(this).val($(this).val().replace(/[^,\-0-9.]/g, "").replace(/^\.,/, ""));
    });
}

function validateMeCurrencyBlur(id) {
    $("#" + id).val($("#" + id).val().replace(/[^\,\-0-9\.\-\,]/g, "").replace(/^\.\,/, ""));
}

function validateMeCurrencyKeyUp(id) {
    $("#" + id).val($("#" + id).val().replace(/[^\,\-0-9\.\-\,]/g, "").replace(/^\.\,/, ""));
}


