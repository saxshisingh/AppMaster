$(document).ready(function () {
    $('#empLoader').hide();
    getRememberMeData();
});
var counter = 0;
var selectedUserIds = [];
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

function getRememberMeData() {
    var cookies = getCookies();
    if (cookies['rememberMe'] == "True") {
        $('#userEmail').val(cookies['email']);
        $('#userPassword').val(cookies['password']);
        document.getElementById("remembermeCheckBox").checked = true;
    } else {
        $('#userEmail').val('');
        $('#userPassword').val('');
        document.getElementById("remembermeCheckBox").checked = false;
      
    }
}




function userLogin() {
    
    $('#empLoader').show();
    var email = document.getElementById("userEmail").value;
    var password = document.getElementById("userPassword").value;
    var rememberMe = document.getElementById("remembermeCheckBox").checked;

   
        var dataToSend = {
            email: email,
            password: password,
            rememberMe: rememberMe
        };
        $.ajax({
            url: '/api/AuthController/UserLogin',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),

            success: function (res) {
                $('#empLoader').hide();
                if (res.success) {
                    if (res.data.length == 0) {
                        ErrorMessage('User not found!');
                        return;
                    }
                    SuccessMessage(res.message);
                    window.location.replace("../Dashboard/Dashboard")
                }
                else {
                    $('#empLoader').hide();
                    ErrorMessage(res.message);
                   
                }
            }


        })
    

}

function userList() {
    counter = 0;
    dataTable = $("#userList").DataTable({

        ajax: {
            'type': 'GET',
            'url': '/api/AuthController/userList',
            'contentType': 'application/json',
            data: {
                type:'RemsUser'
            }
        },
        headers: {
            Authorization: 'Bearer ' + getCookie("token"),
        },

        columns: [
            {
                'data': 'userId',
                'render': function (data, type, row, meta) {
                    return `<input type='checkbox' class='userCheckbox' data-userid='${row.userId}' />`;
                },
                'width': '5%'
            },
            {
                'data': 'userId', 'render': function (data, type, row) {
                    counter++; // Increment counter
                    return `<span>${counter}</span>`;
                },
                'width': '1%', 'font-size': '6px'
            },
            {
                'data': 'username', 
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'email', 
                'width': '5%', 'font-size': '6px'
            },
            {
                'data': 'roleName', 
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'ubRole', 
                'width': '10%', 'font-size': '6px'
            },
            
            {
                'data': 'isActive',
                'render': function (data, type, row) {
                    if (row.isActive == 1) {
                        return `<button onclick=changeStatus("${row.userId}") type="button" class="btn btn-success">Active</button>`
                    }
                    else {
                        return `<button onclick=changeStatus("${row.userId}") type="button" class="btn btn-danger">InActive</button>`
                    }
                },
                'width': '15%',
                'font-size': '6px'
            },

        ],

        "font- size": '1em',
        dom: 'lBfrtip',
        "bDestroy": true,
        "paging": true,
        "searching": false,
        "ordering": true,
        "scrollX": true,
        "info": false,

        language: {
            searchPlaceholder: "Search records",
            emptyTable: "No data found",
            width: '100%',
        },

    }
    );
 
    $('#userList').on('change', '.userCheckbox', function () {
        var userId = $(this).data('userid');
        if ($(this).is(':checked')) {
            if (!selectedUserIds.includes(userId)) {
                selectedUserIds.push(userId); 
            }
        } else {
            var index = selectedUserIds.indexOf(userId);
            if (index !== -1) {
                selectedUserIds.splice(index, 1); 
            }
        }
    });
}

function SaveUser() {
   
    console.log(selectedUserIds);
   
    var dataToSend = {
        useridList: selectedUserIds,
        roleName: document.getElementById("roleId").value,
    };
    if (selectedUserIds.length == 0 ) {
        ErrorMessage("Kindly Select Users in CheckBoxes");
        return;
    }
    else if (document.getElementById("roleId").value =="Select") {
        ErrorMessage("Kindly Select Role");
        return;
    }
    $('#empLoader').show();
    $.ajax({
        url: '/api/AuthController/addUser',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                counter = 0;
                $('#userList').DataTable().ajax.reload();
                while (selectedUserIds.length > 0) {
                    selectedUserIds.pop();
                }
                $('#roleId').val('Select');

            }
            else {
                ErrorMessage(data.message)
            }
            $('#empLoader').hide();
        }
    })

}
function getCookie(cName) {
    const name = cName + "=";
    const cDecoded = decodeURIComponent(document.cookie); //to be careful
    const cArr = cDecoded.split('; ');
    let res;
    cArr.forEach(val => {
        if (val.indexOf(name) === 0) res = val.substring(name.length);
    })
    return res
}


function changeStatus(userID) {
  
    Swal.fire({
        title: 'Are you sure you want to make this user Active/inActive?',
        text: "",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Change it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#empLoader').show();
            $.ajax({
                type: 'GET',
                url: '/api/AuthController/changeStatus',
                data: {
                    userID: userID
                },
                success: function (data) {
                    if (data.success == true) {

                        SuccessMessage(data.message);
                        counter = 0;
                        $('#userList').DataTable().ajax.reload();
                        $('#empLoader').hide();
                    }
                    else {

                        ErrorMessage(data.message);
                        $('#empLoader').hide();
                    }
                }
            });
        };
    })
}


//GET VALUE FROM SESSION


function getCookies() {
    var cookies = document.cookie;
    var cookieArray = cookies.split(';');
    var cookieData = {};
    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i].trim();
        var separatorIndex = cookie.indexOf('=');
        var cookieName = cookie.substring(0, separatorIndex);
        var cookieValue = decodeURIComponent(cookie.substring(separatorIndex + 1));
        cookieData[cookieName] = cookieValue;
    }
    return cookieData;

}
