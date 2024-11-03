
$(document).ready(function () {
    $('#Loader').hide();
    $('#ddlRole').select2();
    $('#ddlUser').select2();
    $('#ddlUser').append('<option value="0">Select</option>');
    $('#ddlNewUser').select2();
    $('#ddlNewUser').append('<option value="0">Select</option>');
});
var users = [];
var selectedBookingIds = [];
function GetUserByRoleName(rolename) {
    $('#ddlUser').empty();
    $('#Loader').show();
    $.ajax({
        url: '/api/ReallocationController/GetUserByRoleName?RoleName=' + rolename,
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {
            $('#Loader').hide();

            if (res.success) {
                users = res.data;
                $('#ddlUser').append('<option>Select</option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlUser').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                }
                $('#ddlUser').select2();
            }
            else {
                ErrorMessage(res.data)

            }
        }
    })
}
function BindNewUse(selectedUserId) {

    $('#ddlNewUser').empty();
    $('#ddlNewUser').append('<option value="0">Select</option>');
    for (i = 0; i < users.length; i++) {
        if (selectedUserId != users[i].key)
            $('#ddlNewUser').append('<option value=' + users[i].key + '> ' + users[i].value + ' </option>')
    }
    $('#ddlNewUser').select2();


}

function SearchUnit() {
    var rolename = $('#ddlRole').val();
    if (rolename == '0') {
        ErrorMessage('Select role.');
        return
    }

    var userId = $('#ddlUser').val();
    if (userId == 0) {
        ErrorMessage('Select user.');
        return
    }
    var unitNo = $('#txtUnitNo').val();
    $('#Loader').show();
    $.ajax({

        'type': 'GET',
        'url': '/api/ReallocationController/GetReallocationUnit?RoleName=' + rolename + '&UserID=' + userId + '&UnitNo=' + unitNo,
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {
            $('#Loader').hide();
            if (response.success == false) {
                ErrorMessage(response.data);
            }

            else {
                var html = "<table id='UnitReallocationList' class='table table-bordered table-hover' style='width:100%'>";
                html += "<thead>";
                html += "<tr>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'><input type='checkbox' style='margin-left:-6px' id='chkheader' onclick='checkAll(this)'></th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Project</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Tower</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Floor</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Unit No</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Status</th>";
                html += "</tr>";
                html += "</thead>";
                var i = 1;
                if (response.data.length > 0) {
                    debugger
                    response.data.forEach(function (entry) {
                        html += "<tr>";
                        html += "<td><input type='checkbox' onclick='check(this)' data-bookingId=" + entry.bookingID + " /></td>";
                        html += "<td>" + entry.projectName + "</td>";
                        html += "<td>" + entry.towerName + "</td>";
                        html += "<td>" + entry.floorName + "</td>";
                        html += "<td>" + entry.unitNo + "</td>";
                        html += "<td>" + entry.statusName + "</td>";
                        html += "</tr>";
                    });
                    $("#divNewUser").show();
                    BindNewUse(userId);
                }
                else {
                    html += "<tr>";
                    html += "<td colspan='6' style='text-align:center'>No Record Found!</td>";
                    html += "</tr>";
                    $("#divNewUser").hide();
                }

                html += "</table>";
                document.getElementById("divUnitReallocation").innerHTML = html;
                dataTable = $("#UnitReallocationList").DataTable({
                    "font- size": '1em',
                    dom: 'lBfrtip',
                    "bDestroy": true,
                    "paging": true,
                    "searching": true,
                    "ordering": true,
                    "scrollX": true,
                    "info": false,

                    language: {
                        searchPlaceholder: "Search records",
                        emptyTable: "No data found",
                        width: '100%',
                    },
                })
            }
        }
    })

}

function checkAll(bx) {
    debugger
    var cbs = document.getElementsByTagName('input');
    for (var i = 0; i < cbs.length; i++) {
        if (cbs[i].type == 'checkbox') {
            cbs[i].checked = bx.checked;
            var bookingId = cbs[i].dataset['bookingid'];
            if (bx.checked && bookingId != undefined) {
                if (!selectedBookingIds.includes(bookingId)) {
                    selectedBookingIds.push(bookingId);
                }
            }
            else {
                if (bookingId != undefined) {
                    var index = selectedBookingIds.indexOf(bookingId);
                    if (index !== -1) {
                        selectedBookingIds.splice(index, 1);
                    }
                }

            }
        }
    }
}
function check(bx) {
    var cbs = document.getElementById('chkheader');
    cbs.checked = bx.checked;
    var bookingId = bx.dataset['bookingid'];
    if (cbs.checked) {
        if (!selectedBookingIds.includes(bookingId)) {
            selectedBookingIds.push(bookingId);
        }
    }
    else {
        var index = selectedBookingIds.indexOf(bookingId);
        if (index !== -1) {
            selectedBookingIds.splice(index, 1);
        }
    }
}

function ReallocateUnit() {
    var roleName = $("#ddlRole").val();
    var userId = $("#ddlUser").val();
    var newUserId = $("#ddlNewUser").val();
    if (roleName == 0) {
        ErrorMessage('Please select role.');
        return
    }
    if (userId == 0) {
        ErrorMessage('Please select user.');
        return
    }
    if (selectedBookingIds.length == 0) {
        ErrorMessage("Please select at least one booking.")
        return;
    }
    if (newUserId == 0) {
        ErrorMessage('Please select new user.');
        return
    }


    $('#Loader').show();
    $.ajax({
        url: '/api/ReallocationController/SaveBookingReallocation?FromUserID=' + userId + '&ToUserID=' + newUserId + '&BookingIds=' + selectedBookingIds,
        type: 'GET',
        contentType: 'application/json', 
        success: function (res) {
            debugger
            $('#Loader').hide();
            if (res.success) {
                SuccessMessage(res.data);
            //    ClearData(); 
                
            }
            else {
                $('#Loader').hide();
                ErrorMessage(res.data);
            }
        }
    })
}
function ClearData() {
    $("#hdnSchemeID").val('');
    $("#txtSchemeName").val('');
    $("#txtSchemeDesc").val('');
    document.getElementById("planLabel").innerHTML = "Add Scheme";
}






