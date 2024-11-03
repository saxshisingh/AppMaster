
$(document).ready(function () {
    GetDashboardSummary();
});
function GetDashboardSummary() {

    $('#eLoader').show();
    $.ajax({

        'type': 'GET',
        'url': '/api/DashboardController/GetDashboardSummary',
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {
            
            $('#eLoader').hide();
            if (response.success == false) {
                ErrorMessage(response.data);
            }
            else {
                $("#totalUnit").html(response.data.totalUnit);
                $("#soldUnit").html(response.data.soldUnit);
                $("#progressUnit").html(response.data.progressUnit);
                $("#unSoldUnit").html(response.data.unsoldUnit);
                BindSaleSummary(response.data.saleSummary);
                CollectionSummary(response.data.bookingAmount);
                SaleProgress(response.data.unitSaleProgress);
            }
        }
    })

}
function BindSaleSummary(saleSummary) {
    let xValues = [];
    let yValues = [];
    for (var i = 0; i < saleSummary.length; i++) {
        xValues.push(saleSummary[i].saleMonth);
        yValues.push(saleSummary[i].totalUnit);
    }


    new Chart("myChart", {
        type: "line",
        data: {
            labels: xValues,
            datasets: [{
                fill: false,
                lineTension: 0,
                backgroundColor: "rgba(0,0,255,1.0)",
                borderColor: "rgba(0,0,255,0.1)",
                data: yValues
            }]
        },
        options: {
            legend: { display: false },
            scales: {
                yAxes: [{ ticks: { min: 1, max: 100 } }],
            }
        }
    });
}
function CollectionSummary(bookingAmount) {


    let xValues = [];
    let yValues = [];
    var maxamt = Number.parseInt(Math.max(...bookingAmount.map(o => o.amount)));
    var minamt = Number.parseInt(Math.min(...bookingAmount.map(o => o.amount)));
    for (var i = 0; i < bookingAmount.length; i++) {
        xValues.push(bookingAmount[i].collectionMonth);
        var amt = Number.parseFloat(bookingAmount[i].amount);
        yValues.push(amt);
    }
    var barColors = ["#CD6155", "#C39BD3", "#2980B9", "#76D7C4", "#73C6B6", "#82E0AA", "#F9E79F", "#F8C471", "#F0B27A", "#DC7633", "#2C3E50", "#FFBF00"];

    new Chart("myBarChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: barColors,
                data: yValues
            }]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: "Amount in Crores"
            },
            scales: {
                yAxes: [{ ticks: { min: 1, max: 100 } }]
            }
        }
    });
}

function SaleProgress(unitSaleProgress) {
   
    // $("#userEmail").val("");
    var sessionValue = $('#sessionRoleValue').text();
   

    var html = "<table id='SaleProgress' class='table table-bordered table-hover' style='width:100%'>";
    html += "<thead>";
    html += "<tr>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>S.No.</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Project</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Unit No</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Type</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Status</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Assign Date</th>";
    html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Assign Person</th>";
    if (sessionValue == "Admin/CFO") {
        html += "<th style='font-family: 'sans-serif'; font-weight: normal;width:120px'>Actions</th>";
    }
    html += "</tr>";
    html += "</thead>";
    var i = 1;
    if (unitSaleProgress.length > 0) {
        unitSaleProgress.forEach(function (entry) {
            $("#userEmail").val(entry.email);
            $("#lblMailConfigureID").val(entry.mailConfigureID);
            html += "<tr>";
            html += "<td>" + i++ + "</td>";
            html += "<td>" + entry.projectName + "</td>";
            html += "<td>" + entry.unitNo + "</td>";
            html += "<td>" + entry.bookingType + "</td>";          
            html += "<td>" + entry.statusName + "</td>";
            html += "<td>" + entry.statusDate + "</td>";
            html += "<td>" + entry.salesPersonName + "</td>";
            if (sessionValue == "Admin/CFO") {
                html += "<td style='width: 150px;'> <button type='button' onclick='GetUbmEmails(" + JSON.stringify(entry.ubmID) + ")' data-toggle='modal' data-backdrop='static' data-keyboard='false' data-target='#sendMailModal' class='btn btn-primary'><i class='fa fa-envelope'></i> Send Mail</button> </td>";
            }
            html += "</tr>";
        });
    }
    else {
        html += "<tr>";
        html += "<td colspan='8' style='text-align:center'>No Record Found!</td>";
        html += "</tr>";
    }

    html += "</table>";
    document.getElementById("divSaleProgress").innerHTML = html;
    
}
  
function GetUbmEmails(ubmId) {

    $('#eLoader').show();
    $.ajax({

        'type': 'GET',
        'url': '/api/DashboardController/GetUbmEmails',
        'contentType': 'application/json',
        dataType: 'json',
        data: {
            UbmID: ubmId 
        },
        success: function (response) {
            debugger
            $('#eLoader').hide();
            if (response.success == false) {
                ErrorMessage(response.data);
            }
            else {
                $("#userEmail").val(response.data);
            }
        }
    })

}

function SendTestConfigure() {
    var mailConfigureID = $("#lblMailConfigureID").val();
    var userEmail = $("#userEmail").val();
    var message = $("#txtMessage").val();
    var subject = $("#subject").val();
   
    if (subject == '') { ErrorMessage('Please enter subject.'); return; }
    else if (message == '') {
        ErrorMessage('Please enter message.'); return;
    }
    var formdata = {
        MailConfigureID: mailConfigureID,
        Type: "Email",
        ToEmail: userEmail,
        Message: message,
        Subject: subject
    };
    $('#eLoader').show();
    $.ajax({

        'type': 'POST',
        'url': '/api/MailConfigureController/AlertMail',
        'contentType': 'application/json',
        dataType: 'json',
        data: JSON.stringify(formdata),
        success: function (response) {
            $('#eLoader').hide();
            if (response.success) {
                SuccessMessage(response.message);
                //$("#lblMailConfigureID").val();
                //$("#lblConfigureType").val();
                $("#txtTestToEmail").val('');
                $("#txtTestToMobile").val('');
                $("#txtTestTemplateId").val('');
                $("#txtMessage").val('');
                $('#sendMailModal').modal('hide');
                $('.modal-backdrop').remove();
            }
            else
                ErrorMessage(response.message);
        }

    });

}