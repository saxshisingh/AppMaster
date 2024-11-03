$(document).ready(function () {
    $('#empLoader').hide();
    UnitBookingDetails();
});
Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 5000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
})
function customerVerification() {
    var emailID = document.getElementById("email").value;
    var password = document.getElementById("password").value;
    var payLoad = {
        email: emailID,
        password: password,
    }
    $('#empLoader').show();
    $.ajax({
        url: '/api/AuthController/customerAuth',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success || data.isSuccess) {
                SuccessMessage(data.message);
                window.location.replace("../Customer/UnitVerification?ubmID=" + data.data[0].ubmID)
                $('#empLoader').hide();
            }
            else {
                ErrorMessage(data.message);
                $('#empLoader').hide();
            }
        }
    })
}
