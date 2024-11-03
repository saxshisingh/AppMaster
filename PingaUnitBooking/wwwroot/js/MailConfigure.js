
$(document).ready(function () {
    document.getElementById("btnMailConfigure").innerHTML = "Save";
    MailConfigureList();
    // ErrorMessage('hi');
});

function onChangeConfigType(val) {
    if (val == 'Sms') {
        $("#divSms").show();
        $("#divEmail").hide();
        $("#divWhatsApp").hide();
    }
    else if (val == 'Email') {
        $("#divEmail").show();
        $("#divSms").hide();
        $("#divWhatsApp").hide();
    }
    else if (val == 'WhatsApp') {
        $("#divWhatsApp").show();
        $("#divSms").hide();
        $("#divEmail").hide();
    }
    else {
        $("#divSms").hide();
        $("#divEmail").hide();
        $("#divWhatsApp").hide();
    }
}

function AddMailConfigure() {
   
    var mailConfigureID = $("#hdnMailConfigureID").val() == '' ? '0' : $("#hdnMailConfigureID").val();
    var configureType = $("#ddlConfigureType").val();
    var isActive = document.getElementById("IsActive").checked;
    var formdata;
    if (configureType == 'Email') {
        var emailProvider = $("#ddlEmailProvider").val();
        if (emailProvider == 'Select') {
            ErrorMessage('Please select email provide.'); return;
        }
        var senderName = $("#txtSender").val();
        if (senderName == '') {
            ErrorMessage('Please enter sender name.'); return;
        }
        var smtpServer = $("#txtSmtpServer").val();
        if (smtpServer == '') {
            ErrorMessage('Please enter smtp server.'); return;
        }
        var userName = $("#txtUserName").val();
        if (userName == '') {
            ErrorMessage('Please enter username.'); return;
        }
        var password = $("#txtPassword").val();
        if (password == '') {
            ErrorMessage('Please enter password.'); return;
        }
        var portNo = $("#txtPortNo").val();
        if (portNo == '') {
            ErrorMessage('Please enter port no.'); return;
        }
        var basedOn = $("#ddlBasedOn").val();
        if (basedOn == 'Select') {
            ErrorMessage('Please select based on.'); return;
        }
        formdata = {
            MailConfigureID: mailConfigureID,
            ConfigureType: configureType,
            Provider: emailProvider,
            SenderName: senderName,
            SMTPServer: smtpServer,
            UserName: userName,
            Password: password,
            PortNo: portNo,
            BasedOn: basedOn,
            TokenWA: '',
            PhoneWA: '',
            SmsUrl: '',
            IsActive: isActive
        };
    }
    else if (configureType == 'Sms') {
        var smsuserName = $("#txtSmsUsername").val();
        if (smsuserName == '') {
            ErrorMessage('Please enter username.'); return;
        }
        var smspassword = $("#txtSmsPassword").val();
        if (smspassword == '') {
            ErrorMessage('Please enter password.'); return;
        }
        var smssender = $("#txtSmsSender").val();
        if (smssender == '') {
            ErrorMessage('Please enter sender id.'); return;
        }
        var smsurl = $("#txtSmsURL").val();
        if (smsurl == '') {
            ErrorMessage('Please enter sms url as given example.'); return;
        }
        formdata = {
            MailConfigureID: mailConfigureID,
            ConfigureType: configureType,
            Provider: '',
            SenderName: smssender,
            SMTPServer: '',
            UserName: smsuserName,
            Password: smspassword,
            PortNo: 0,
            BasedOn: '',
            SmsUrl: smsurl,
            TokenWA: '',
            PhoneWA: '',
            IsActive: isActive
        };
    }
    else {

        var WPProvider = $("#ddlWPProvider").val();

        var wpToken = $("#txtWPToken").val();
        if (wpToken == '') {
            ErrorMessage('Please enter token.'); return;
        }
        var wpPhoneNo = $("#txtWPPhoneNo").val();
        if (wpPhoneNo == '') {
            ErrorMessage('Please enter phone no.'); return;
        }
        var whatsappurl = $("#txtWPUrl").val();
        if (whatsappurl == '') {
            ErrorMessage('Please enter url.'); return;
        }
        formdata = {
            MailConfigureID: mailConfigureID,
            ConfigureType: configureType,
            Provider: WPProvider,
            TokenWA: wpToken,
            SenderName: '',
            SMTPServer: '',
            PortNo: 0,
            PhoneWA: wpPhoneNo,
            SmsUrl: whatsappurl,
            BasedOn: '',
            IsActive: isActive
        };
    } 
    $('#empLoader').show();
    $.ajax({
        url: '/api/MailConfigureController/SaveMailConfigure',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formdata),
        success: function (data) {
            $('#empLoader').hide();
            if (data.success) {
                SuccessMessage(data.message);
                ClearData();
                $('#addConfiguration').modal('hide');
                $('.modal-backdrop').remove();
                $("#hdnMailConfigureID").val(0);
                MailConfigureList();
                document.getElementById("btnMailConfigure").innerHTML = "Save";

            }
            else {
                $('#empLoader').hide();
                ErrorMessage(data.message);
            }
        }
    })
}
function MailConfigureList() {
    $('#empLoader').show();
    $.ajax({

        'type': 'GET',
        'url': '/api/MailConfigureController/GetMailConfigure',
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {
            $('#empLoader').hide();
            
            var html = "<table id='MailConfigurationList' class='table table-bordered table-hover' style='width:100%'>";
            html += "<thead>";
            html += "<tr>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>S No.</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Type</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Sender Name</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Based On</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Provider</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Is Active</th>";
            html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Actions</th>";
            html += "</tr>";
            html += "</thead>";
            if (response.success == false) {
                ErrorMessage(response.data);
            }

            else {
                var i = 1;
                if (response.data.length > 0) {
                    response.data.forEach(function (entry) {
                        var isActiveStatus = entry.isActive != false ? "Yes":"No"
                        html += "<tr>";
                        html += "<td>" + i++ + "</td>";
                        html += "<td>" + entry.configureType + "</td>";
                        html += "<td><button data-row=" + JSON.stringify(entry) + " data-toggle='modal' href='' data-backdrop='static' data-keyboard='false' data-target='#TestConfiguration' onclick ='OpenTestConfiguration(" + JSON.stringify(entry) + ")' type = 'button' class='btn btn-primary'>" + entry.senderName + "</button></td>";
                        html += "<td>" + entry.basedOn + "</td>";
                        html += "<td>" + entry.provider + "</td>";
                        html += "<td>" + isActiveStatus + "</td>";
                        html += "<td><button data-row=" + JSON.stringify(entry) + " data-toggle='modal' href='' data-backdrop='static' data-keyboard='false' data-target='#addConfiguration' onclick ='EditMailConfigure(" + JSON.stringify(entry) + ")' type = 'button' class='btn btn-primary'> <i class='fa fa-edit'></i>&nbsp;Edit</button>&nbsp;";
                        html += "<button  data-row=" + JSON.stringify(entry.mailConfigureID) + " onclick='DeleteMailConfigure(" + JSON.stringify(entry) + ")' type='button' class='btn btn-danger'><i class='fa fa-bitbucket'></i>&nbsp;Delete</button>";
                        html += "</td>";
                        html += "</tr>";
                    });
                }
                else {
                    html += "<tr>";
                    html += "<td colspan='7' style='text-align:center'>No Record Found!</td>";
                    html += "</tr>";
                }

                html += "</table>";
                document.getElementById("divConfigurationList").innerHTML = html;
                dataTable = $("#MailConfigurationList").DataTable({
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


function EditMailConfigure(rowData) {
    ClearData();
    
    $("#hdnMailConfigureID").val(rowData["mailConfigureID"]);
    $("#ddlConfigureType").val(rowData['configureType']);
    if (rowData['configureType'] == 'Email') {
       
        $("#ddlEmailProvider").val(rowData['provider']);
        $("#txtSender").val(rowData['senderName']);
        $("#txtSmtpServer").val(rowData['smtpServer']);
        $("#txtUserName").val(rowData['userName']);
        $("#txtPassword").val(rowData['password']);
        $("#txtPortNo").val(rowData['portNo']);
        $("#ddlBasedOn").val(rowData['basedOn']);
        $("#divEmail").show();
        $("#divWhatsApp").hide();
        $("#divSms").hide();
    }
    else if (rowData['configureType'] == 'Sms') {
      
        $("#txtSmsSender").val(rowData['senderName']);
        $("#txtSmsUsername").val(rowData['userName']);
        $("#txtSmsPassword").val(rowData['password']);
        $("#txtSmsURL").val(rowData['smsUrl']);
        $("#divSms").show();
        $("#divWhatsApp").hide();
        $("#divEmail").hide();
    }
    else {
        $("#divWhatsApp").show();
        $("#divSms").hide();
        $("#divEmail").hide();
        $("#txtWPToken").val(rowData['tokenWA']);
        $("#txtWPPhoneNo").val(rowData['phoneWA']);
        $("#txtWPUrl").val(rowData['smsUrl']);
    }

    $("#IsActive").prop("checked", rowData['isActive']);
    document.getElementById("btnMailConfigure").innerHTML = "Update";
}

function ClearData() {
    $("#hdnMailConfigureID").val(0)
    $("#divSms").hide();
    $("#divWhatsApp").hide();
    $("#divEmail").hide();
    $("#ddlConfigureType").val('Select');
    $("#ddlEmailProvider").val('Select');
    $("#txtSender").val('');
    $("#txtSmtpServer").val('');
    $("#txtUserName").val('');
    $("#txtPassword").val('');
    $("#txtPortNo").val('');
    $("#ddlBasedOn").val('Select');
    $("#IsActive").prop("checked", false);

    $("#txtSmsUsername").val('');
    $("#txtSmsPassword").val('');
    $("#txtSmsSender").val('');
    $("#txtSmsURL").val('');
    $("#txtWPToken").val('');
    $("#txtWPPhoneNo").val('');
    $("#txtWPUrl").val('');
}

function DeleteMailConfigure(rowData) {

    var id = rowData["mailConfigureID"];
    Swal.fire({
        title: 'Are you want to delete configuration?',
        text: "",
        width: 350,
        height: 5,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#empLoader').show();
            $.ajax({
                url: '/api/MailConfigureController/DeleteMailConfigure',
                type: 'GET',
                contentType: 'application/json',
                data: {
                    MailConfigureID: id
                },
                success: function (data) {
                    $('#empLoader').hide();
                    if (data.success) {
                        SuccessMessage(data.message);
                        MailConfigureList();
                    }
                    else {
                        ErrorMessage(data.message);
                    }
                }
            })
        };
    })
}

function OpenTestConfiguration(rowData) {
    
    $("#lblMailConfigureID").val();
    $("#lblConfigureType").val();
    $("#txtTestToEmail").val('');
    $("#txtTestToMobile").val('');
    $("#txtTestTemplateId").val('');
    $("#txtMessage").val('');
    $("#lblConfigureType").val(rowData["configureType"]);
    $("#lblMailConfigureID").val(rowData["mailConfigureID"]);
    if (rowData["configureType"] == 'Sms') {
        $("#lblMHeader").html('Test Sms Configure')
        $("#divTestEmail").hide();
        $("#divTestMobile").show();
        $("#divTestSmsWhatsApp").show();
    }
    else if (rowData["configureType"] == 'WhatsApp') {
        $("#divTestEmail").hide();
        $("#divTestMobile").show();
        $("#divTestSmsWhatsApp").show();
        $("#lblMHeader").html('Test WhatsApp Configure')
    }
    else {
        $("#divTestEmail").show();
        $("#divTestMobile").hide();
        $("#divTestSmsWhatsApp").hide();
        $("#lblMHeader").html('Test Email Configure')
    }
}
document.getElementById('txtTestToEmail').addEventListener('input', function () {
    const email = this.value.trim();
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (emailPattern.test(email)) {
        document.getElementById("errorTestToEmail").style.display = "none";
    } else {
        document.getElementById("errorTestToEmail").style.display = "block";

    }
});
document.getElementById('txtTestToMobile').addEventListener('input', function () {
    const value = this.value.trim();
    const isValidMobile = /^\d{10}$/.test(value);

    if (!isValidMobile) {
        document.getElementById("errorTestToMobile").style.display = "block";
    } else {
        document.getElementById("errorTestToMobile").style.display = "none";
    }
});
function validateNumber(e) {
    const pattern = /^[0-9]$/;

    return pattern.test(e.key)
}
function validateEmail(email) {
    var re = /\S+@\S+\.\S+/;
    return re.test(email);
}
function SendTestConfigure() {
    

    var mailConfigureID = $("#lblMailConfigureID").val();
    var configurType = $("#lblConfigureType").val();
    var testToEmail = $("#txtTestToEmail").val();

    if (configurType == "Email") {
        if (testToEmail == '') {
            ErrorMessage('Please enter email id.'); return;
        }
        else {
            if (!validateEmail(testToEmail)) {
                ErrorMessage('Please enter email id.'); return;
            }
        }
    }
    var testToMobile = $("#txtTestToMobile").val();
    var testTemplateID = $("#txtTestTemplateId").val();
    if (configurType == "Sms" || configurType == "WhatsApp") {
        if (testToMobile == '') {
            ErrorMessage('Please enter mobile no.'); return;
        }
    }

    var message = $("#txtMessage").val();
    if (message == '') {
        ErrorMessage('Please enter message.'); return;
    }
    var formdata = {
        MailConfigureID: mailConfigureID,
        Type: configurType,
        ToEmail: testToEmail,
        ToMobileNo: testToMobile,
        TemplateID: testTemplateID,
        Message: message
    };
    $('#empLoader').show();
    $.ajax({

        'type': 'POST',
        'url': '/api/MailConfigureController/TestMailConfigure',
        'contentType': 'application/json',
        dataType: 'json',
        data: JSON.stringify(formdata),
        success: function (response) {
            $('#empLoader').hide();
            if (response.success) {
                SuccessMessage(response.message);
                //$("#lblMailConfigureID").val();
                //$("#lblConfigureType").val();
                $("#txtTestToEmail").val('');
                $("#txtTestToMobile").val('');
                $("#txtTestTemplateId").val('');
                $("#txtMessage").val('');
            }               
            else
                ErrorMessage(response.message);
        }

    });

}

