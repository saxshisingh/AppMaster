

function validEmail(email) {
    const regex = /^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$/;
    return regex.test(email);
}
 Toast = Swal.mixin({
    toast: true,
     position: 'top-end',
    timer: 5000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
})



function logOut() {

    $.ajax({
        url: '/api/AuthController/logOut',
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                window.location.replace("../Index")
            }
            else {
                ErrorMessage(data.message);
            }
        }


    })
}
   


function SuccessMessage(text) {
    Toast.fire({
        icon: 'success',
      //  title: 'Hurray!',
        text: text,
        showCloseButton: true
    })
}

function ErrorMessage( text) {
    Toast.fire({
        icon: 'error',
       // title: 'oops!',
        text: text,
        showCloseButton: true
    })
}

//EMAIL VALIDATION COMMON FUNCTION 
function emailValidation(emailID) {
    filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (filter.test(emailID)) {
        return true;
    }
    else {
        return false;
    }
}
//PHONE NUMBER VALIDATION FOR NUMBER VALIDATION
function phoneNumberValidation(phoneNo) {
        var regx = /^[6-9]\d{9}$/;
    if (regx.test(phoneNo)) {
        return true;
    }
    else {
        return false;
    }
}
//PAN VALIDATION FOR PAN VALIDATION
function panValidation(panNo) {
       
        var regex = /([A-Z]){5}([0-9]){4}([A-Z]){1}$/;
    if (regex.test(panNo.toUpperCase())) {
        return true;
    } else {
        return false;
    }
}
//AADHAR VALIDATION FOR AADHAR VALIDATION
function aadharNoValidation(adharNo) {
      var expr = /^([0-9]{4}[0-9]{4}[0-9]{4}$)|([0-9]{4}\s[0-9]{4}\s[0-9]{4}$)|([0-9]{4}-[0-9]{4}-[0-9]{4}$)/;
    if (expr.test(adharNo)) {
        return true;
    } else {
        return false;
    }
}
//PIN NO VALIDATION FOR PIN VALIDATION
function pinValidation(pinNo) {
    if ((pinNo.length === 5 || pinNo.length === 7) && Number.isInteger(+pin)) {
        return true;
    }
    else {
        return false;
    }
   
}


