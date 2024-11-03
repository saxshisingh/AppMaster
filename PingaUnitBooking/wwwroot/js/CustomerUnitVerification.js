$(document).ready(function () {
    $('#empLoader').hide();
    let urlParams = new URLSearchParams(window.location.search);
    var ubmID = urlParams.get('ubmID');
    $('#applicantState').select2();
   
    UnitBookingDetails(ubmID);
   
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
function validateNumber(e) {
    const pattern = /^[0-9]$/;

    return pattern.test(e.key)
}

/*document.getElementById("tncCheckBox").addEventListener("change", function () {
    const finalSubmitBtn = document.getElementById("finalSubmitBtn");

    if (this.checked) {
        finalSubmitBtn.disabled = false;
    } else {
        finalSubmitBtn.disabled = true;
    }
});*/


function stateList() {
    $.ajax({
        url: '/api/StateCityController/StateList',
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                $('#applicantState').append('<option> Select </option>');
                for (i = 0; i < data.data.length; i++) {
                    $('#applicantState').append('<option value=' + data.data[i]['stateID'] + '> ' + data.data[i]['stateName'] + ' </option>')
                }
                $('#applicantState').select2();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    });
}

function CityList() {
    var StateID = $('#applicantState').val();
    $.ajax({
        url: '/api/StateCityController/CityList?stateID=' + StateID,
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                $('#applicantCity').empty();
                $('#applicantCity').append('<option> Select </option>');
                for (i = 0; i < data.data.length; i++) {
                    $('#applicantCity').append('<option value=' + data.data[i]['cityID'] + '> ' + data.data[i]['cityName'] + ' </option>')
                }
                $('#applicantCity').select2();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    });
}



function proceedPaymentTab() {
    $('#pd-tab').tab('show');
    $('#pd-tab').addClass('active');
    $('#pd-tab-home').addClass('active show');
    ubmPaymentList($('#ubmID').text());
}
function previousPaymentTab() {
    $('#unit-tab').tab('show');
    $('#unit-tab').addClass('active');
    $('#unit-tab-home').addClass('active show');
}

function previousApplicantTab() {
    $('#pd-tab').tab('show');
    $('#pd-tab').addClass('active');
    $('#pd-tab-home').addClass('active show');
}
function previousCoApplicantTab() {
    $('#ad-tab').tab('show');
    $('#ad-tab').addClass('active');
    $('#ad-tab-home').addClass('active show');
}
function previousDocumentTab() {
    $('#cad-tab').tab('show');
    $('#cad-tab').addClass('active');
    $('#cad-tab-home').addClass('active show');
}
function previousTMCTab() {
    $('#du-tab').tab('show');
    $('#du-tab').addClass('active');
    $('#du-tab-home').addClass('active show');
}

function UnitBookingDetails(ubmID) {
    document.getElementById("ubmID").innerHTML = ubmID;
    $.ajax({
        url: '/api/bookingController/ubmDetailsByUnitID?ubmID=' + ubmID,
        type: 'GET',
        contentType: 'application/json',

        success: function (data) {
            if (data.success) {
                document.getElementById("custMobileNo").value = data.data[0].customerMobileNo;
                document.getElementById("custEmail").value = data.data[0].customerEmail;
                var datePart = data.data[0].releaseUnitDate.split('T')[0];
                document.getElementById("releaseUD").value = datePart;
                document.getElementById("appType").value = data.data[0].applicationType;
                document.getElementById("projectID").value = data.data[0].projectName;
                document.getElementById("towerID").value = data.data[0].towerName;
                document.getElementById("floorID").value = data.data[0].floorName;
                document.getElementById("unitNo").value = data.data[0].unitNo;
                document.getElementById("unitID").value = data.data[0].unitID;
                GetUnitInfo()
            }
            else {
                ErrorMessage(data.message);

            }
        }
    });
}

function addPayment() {


    var payMode = document.getElementById("payMode").value;
    var paymentDate = document.getElementById("paymentDate").value;
    var ChequeNo = document.getElementById("ChequeNo").value;
    var bankName = document.getElementById("bankName").value;
    var transactionNo = document.getElementById("transactionNo").value;
    var paymentAmount = document.getElementById("paymentAmount").value;
    var ubmID = document.getElementById("ubmID").innerHTML;


    if (payMode == "Select") {
        return ErrorMessage("Kindly select paymode");
    }
    else if (payMode == "Cheque") {
        if (ChequeNo == "") return ErrorMessage("Please enter cheque no");
        else if (bankName == "") return ErrorMessage("Please enter bank name");
        else if (paymentAmount == "") return ErrorMessage("Kindly add amount no");
        else if (paymentDate == "") return ErrorMessage("Kindly select date");
    }
    else if (payMode == "NEFT" || payMode == "RTGS") {
        if (transactionNo == "") return ErrorMessage("Please enter transaction no");
        else if (paymentAmount == "") return ErrorMessage("Kindly add amount no");
        else if (paymentDate == "") return ErrorMessage("Kindly select date");
    }
    else if (payMode == "Bank Transfer") {
        if (transactionNo == "") return ErrorMessage("Please enter transaction no");
        else if (bankName == "") return ErrorMessage("Please enter bank name");
        else if (paymentAmount == "") return ErrorMessage("Kindly add amount no");
        else if (paymentDate == "") return ErrorMessage("Kindly select date");
    }

    var payMode = document.getElementById("payMode").value;
    var paymentDate = document.getElementById("paymentDate").value;
    var ChequeNo = document.getElementById("ChequeNo").value;
    var bankName = document.getElementById("bankName").value;
    var transactionNo = document.getElementById("transactionNo").value;
    var paymentAmount = document.getElementById("paymentAmount").value;
    var ubmID = document.getElementById("ubmID").innerHTML;


    var payLoad = {
        PaymentMode: payMode,
        PaymentDate: paymentDate,
        ChequeNo: ChequeNo,
        BankName: bankName,
        TransactionNo: transactionNo,
        Amount: paymentAmount,
        ubmID: ubmID
    }
    $.ajax({
        url: '/api/bookingController/addPaymentModel',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                $('#payMode').val('Select');
                $('#paymentDate').val('');
                $('#ChequeNo').val('');
                $('#bankName').val('');
                $('#transactionNo').val('');
                $('#paymentAmount').val('');

                $('#transactionNoDiv').hide();
                $('#ChequeNoDiv').hide();
                $('#bankDiv').hide();



                ubmPaymentList(ubmID)
                /* $("#ubmPaymentTable").DataTable().ajax.reload();*/

            }
            else {
                ErrorMessage(data.message);

            }
        }
    })

}

function ubmPaymentList(ubmID) {

    dataTable = $("#ubmPaymentTable").DataTable({

        ajax: {
            url: '/api/bookingController/getPaymentModelList?ubmID=' + ubmID,
            type: 'GET',
            contentType: 'application/json',
        },
        columns: [
            {
                'data': 'paymentMode',
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'formatPaymentDate',
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'chequeNo',
                'width': '15%', 'font-size': '6px'
            },
            {
                'data': 'bankName',
                'width': '15%', 'font-size': '6px'
            },
            {
                'data': 'transactionNo',
                'width': '15%', 'font-size': '6px'
            },
            {
                'data': 'amount',
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'ubmPaymentId',
                'render': function (data, type, row) {
                    return `<button id="paymentDeleteIcon" onclick=deletePaymentPlan("${row.ubmPaymentId}") type="button" class="btn btn-danger"><i class="fa fa-trash-o"></i> </button>`
                },
                'width': '5%',
                'font-size': '6px'
            },

        ],

        "font- size": '1em',
        dom: 'lBfrtip',
        "bDestroy": true,
        "paging": false,
        "searching": false,
        "ordering": false,
        "scrollX": true,
        "info": false,

        language: {
            searchPlaceholder: "Search records",
            emptyTable: "-",
            width: '100%',
        },

    }
    );

}


function paymentSave() {
    $('#ad-tab').tab('show');
    $('#ad-tab').addClass('active');
    $('#ad-tab-home').addClass('active show');
    getApplicantDetails(1);
    stateList();
    blockFutureDate();
}


function applicantDocumentList() {
    var ubmID = document.getElementById("ubmID").innerHTML;

    $.ajax({
        url: '/api/bookingController/getApplicantDocument?ubmID=' + ubmID,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                var dataTable = $("#ApplicantDocTable").DataTable({
                    data: response.data,
                    columns: [
                        {
                            'data': null,
                            'width': '1%',
                            'font-size': '6px'
                        },
                        {
                            'data': 'documentName',
                            'render': function (data, type, row) {
                                if (row.mandatory != true) {
                                    return `<span>${row.documentName}</span>`;
                                } else {
                                    return `<span >${row.documentName} <span class="red-star">*</span></span>`;
                                }
                            },
                            'width': '3%',
                            'font-size': '6px'
                        },
                        {
                            'data': 'documentUrl',
                            'render': function (data, type, row) {
                                if (row.documentUrl != "") {
                                    let fileType = (row.documentUrl)
                                    if (fileType.includes("pdf")) {
                                        return `<div style="text-align: center;">
                                                          <a target="_blank" href="../../${row.documentUrl}">
                                                           <i class="fa fa-file-pdf-o" style="font-size:48px;color:red"></i>  
                                                           </a>
                                                   </div>`;

                                    } else {
                                        return `<div style="text-align: center;">
                                                          <a target="_blank" href="../../${row.documentUrl}">
                                                                  <img src="../../${row.documentUrl}" style="width:80px; height:60px;">
                                                           </a>
                                                   </div>`;
                                    }

                                } else {
                                    return `<input type="file" id="ApplicantFile" class="form-control" accept="image/jpeg,image/png,application/pdf" placeholder="File" aria-label="File browser example">`;
                                }
                            },
                            'width': '25%',
                            'font-size': '6px'
                        },
                        {
                            'data': 'documentID',
                            'render': function (data, type, row) {
                                if (row.ubmKycid == 0) {
                                    return `<button data-row='${JSON.stringify(row)}' onclick="uploadApplicantDocs(this)" type="button" class="btn btn-primary">Upload</button>`;
                                } else {
                                    return `<button data-row='${JSON.stringify(row)}' onclick="deleteAttachments(this)" type="button" class="btn btn-danger">Remove</button>`;
                                }
                            },
                            'width': '3%',
                            'font-size': '6px'
                        }
                    ],

                    "font-size": '1em',
                    dom: 'lBfrtip',
                    "bDestroy": true,
                    "paging": false,
                    "searching": false,
                    "ordering": true,
                    "scrollX": false,
                    "info": false,

                    language: {
                        searchPlaceholder: "Search records",
                        emptyTable: "No data found",
                        width: '100%',
                    }
                });

                // Add index column
                dataTable.on('order.dt', function () {
                    dataTable.column(0, { order: 'applied' }).nodes().each(function (cell, i) {
                        cell.innerHTML = i + 1;
                    });
                }).draw();
            }
            else {
                ErrorMessage(response.message);
            }

        }
    });
}


function uploadApplicantDocs(button) {

    var rowData = JSON.parse(button.getAttribute('data-row'));
    var ubmID = document.getElementById("ubmID").innerHTML;
    let docFile = $("#ApplicantFile")[0].files;
    if (docFile != null) {
        var formData = new FormData();
        for (i = 0; i < docFile.length; i++) {
            formData.append('DocumentFile', docFile[i]);
            formData.append('UbmID', ubmID);
            formData.append('DocumentID', rowData.documentID)
            formData.append('DocumentName', rowData.documentName)
            formData.append('MobileNo', rowData.mobileNo)
            formData.append('UnitID', rowData.unitID)
        }
        $.ajax({
            type: 'Post',
            url: "/api/bookingController/addApplicantDocument",
            async: false,
            cache: false,
            contentType: false,
            enctype: 'multipart/form-data',
            processData: false,
            data: formData,
            success: function (data) {
                if (data.success) {
                    applicantDocumentList()
                    /*  $("#ApplicantDocTable").DataTable().ajax.reload();*/
                    SuccessMessage(data.message);

                }
                else {
                    ErrorMessage(data.message);
                }
            }
        });
    }
    else {


    }
}


function deleteAttachments(button) {
    var rowData = JSON.parse(button.getAttribute('data-row'));

    var payLoad = {
        documentUrl: rowData.documentUrl,
        UbmKycid: rowData.ubmKycid,
    }
    $.ajax({
        url: '/api/bookingController/deleteAttachments',
        type: 'DELETE',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                applicantDocumentList();
            }
            else {
                ErrorMessage(data.message);
            }
        }
    })


}

function addApplicantDetails() {
    /* var applicantUnitDate = document.getElementById("applicantUnitDate").value;*/
    var applicantBuyerName = document.getElementById("applicantBuyerName").value;
    var applicantswd = document.getElementById("applicantswd").value;
    var applicantdob = document.getElementById("applicantdob").value;
    var applicantAge = document.getElementById("applicantAge").value;
    var applicantNationality = document.getElementById("applicantNationality").value;
    var applicantOccupation = document.getElementById("applicantOccupation").value;
    var applicantPan = document.getElementById("applicantPan").value;
    var applicantAdharNo = document.getElementById("applicantAdharNo").value;
    var applicantCorrAddress = document.getElementById("applicantCorrAddress").value;
    var applicantCity = document.getElementById("applicantCity").value;
    var applicantState = document.getElementById("applicantState").value;
    var applicantCountry = document.getElementById("applicantCountry").value;
    var applicantPIN = document.getElementById("applicantPIN").value;
    var applicantEmail = document.getElementById("applicantEmail").value;
    var applicantMob = document.getElementById("applicantMob").value;


    var ubmID = document.getElementById("ubmID").innerHTML;
    var ApplicantID = document.getElementById("applicantID").innerHTML;


    if (applicantBuyerName == "" || applicantswd == "" || applicantdob == "" || applicantAge == "" || applicantNationality == "" ||
        applicantOccupation == "" || applicantOccupation == "" || applicantPan == "" || applicantAdharNo == "" || applicantCorrAddress == ""
        || applicantCountry == "" || applicantPIN == "" || applicantEmail == "" ||  applicantMob == "" || applicantCity == "" || applicantState == "" || applicantCity == "Select" || applicantState == "Select"
    ) {
        return ErrorMessage("Please Fill All Mandatory Fileds");
    }
    else if ((emailValidation(applicantEmail)) == false) {
        return ErrorMessage("Invalid Email Address");
    }
    else if ((phoneNumberValidation(applicantMob)) == false) {
        return ErrorMessage("Invalid Phone No.");
    }
    else if ((panValidation(applicantPan)) == false) {
        return ErrorMessage("Invalid Pan No.");
    }
    else if ((aadharNoValidation(applicantAdharNo)) == false) {
        return ErrorMessage("Invalid Adhar No.");
    }
    var payLoad = {
        applicantType: 1,
        /* applicantUnitDate: applicantUnitDate,*/
        applicantBuyerName: applicantBuyerName,
        applicantswd: applicantswd,
        applicantdob: applicantdob,
        applicantAge: applicantAge,
        applicantNationality: applicantNationality,
        applicantOccupation: applicantOccupation,
        applicantPan: applicantPan,
        applicantAdharNo: applicantAdharNo,
        applicantCorrAddress: applicantCorrAddress,
        applicantCityID: applicantCity,
        applicantStateID: applicantState,
        applicantCountry: applicantCountry,
        applicantPIN: applicantPIN,
        applicantEmail: applicantEmail,
        applicantMob: applicantMob,
        ubmID: ubmID,
        applicantID: ApplicantID != "" ? ApplicantID : null
    }
    $.ajax({
        url: '/api/bookingController/addApplicantDetails',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                //SuccessMessage(data.message);
                $('#cad-tab').tab('show');
                $('#cad-tab').addClass('active');
                $('#cad-tab-home').addClass('active show');
                applicantList(2);
                CoApplicantstateList();
                clearApplicant();
                document.getElementById("coApplicantDiv").style.display = "none";
            }
            else {
                ErrorMessage(data.message);

            }
        }
    })
}
function ProceedApplicantDoc() {
    $('#du-tab').tab('show');
    $('#du-tab').addClass('active');
    $('#du-tab-home').addClass('active show');
    applicantDocumentList();
}


function ProceeddocumentUpload() {
    $('#tmc-tab').tab('show');
    $('#tmc-tab').addClass('active');
    $('#tmc-tab-home').addClass('active show');
    getTncTemplate();
}
function clearApplicant() {
    document.getElementById("applicantUnitDate").value = "";
    document.getElementById("applicantBuyerName").value = "";
    document.getElementById("applicantswd").value = "";
    document.getElementById("applicantdob").value = "";
    document.getElementById("applicantAge").value = "";
    document.getElementById("applicantNationality").value = "";
    document.getElementById("applicantOccupation").value = "";
    document.getElementById("applicantPan").value = "";
    document.getElementById("applicantAdharNo").value = "";
    document.getElementById("applicantCorrAddress").value = "";
    document.getElementById("applicantCity").value = "";
    document.getElementById("applicantState").value = "";
    document.getElementById("applicantCountry").value = "";
    document.getElementById("applicantPIN").value = "";
    document.getElementById("applicantEmail").value = "";
    document.getElementById("applicantMob").value = "";
}

function CoApplicantDetails() {
    var applicantBuyerName = document.getElementById("coApplicantName").value;
    var applicantswd = document.getElementById("coApplicantSwd").value;
    var applicantdob = document.getElementById("coApplicantDOB").value;
    var applicantAge = document.getElementById("coApplicantAge").value;
    var applicantNationality = document.getElementById("coApplicantNationality").value;
    var applicantOccupation = document.getElementById("coApplicantOccupation").value;
    var applicantPan = document.getElementById("coApplicantPAN").value;
    var applicantAdharNo = document.getElementById("coApplicantAdharNo").value;
    var applicantCorrAddress = document.getElementById("coApplicantAddress").value;
    var applicantCity = document.getElementById("coApplicantCityID").value;
    var applicantState = document.getElementById("coApplicantStateID").value;
    var applicantCountry = document.getElementById("coApplicantCountry").value;
    var applicantPIN = document.getElementById("coApplicantPIN").value;
    var applicantEmail = document.getElementById("coApplicantEmail").value;
    var applicantMob = document.getElementById("coApplicantMobile").value;
    var ubmID = document.getElementById("ubmID").innerHTML;

    if (applicantBuyerName == "" || applicantswd == "" || applicantdob == "" || applicantAge == "" || applicantNationality == "" ||
        applicantOccupation == "" || applicantOccupation == "" || applicantPan == "" || applicantAdharNo == "" || applicantCorrAddress == "" || applicantState == "" || applicantState == "Select" || applicantCity == "" || applicantCity == "Select"
        || applicantCountry == "" || applicantPIN == "" || applicantEmail == "" ||  applicantMob == "") {
        return ErrorMessage("Please Fill All Mandatory Fileds");
    }
    else if ((emailValidation(applicantEmail)) == false) {
        return ErrorMessage("Invalid Email Address");
    }
    else if ((phoneNumberValidation(applicantMob)) == false) {
        return ErrorMessage("Invalid Phone No.");
    }
    else if ((panValidation(applicantPan)) == false) {
        return ErrorMessage("Invalid Pan No.");
    }
    else if ((aadharNoValidation(applicantAdharNo)) == false) {
        return ErrorMessage("Invalid Adhar No.");
    }


    var payLoad = {
        applicantType: 2,
        applicantBuyerName: applicantBuyerName,
        applicantswd: applicantswd,
        applicantdob: applicantdob,
        applicantAge: applicantAge,
        applicantNationality: applicantNationality,
        applicantOccupation: applicantOccupation,
        applicantPan: applicantPan,
        applicantAdharNo: applicantAdharNo,
        applicantCorrAddress: applicantCorrAddress,
        applicantCityID: applicantCity,
        applicantStateID: applicantState,
        applicantCountry: applicantCountry,
        applicantPIN: applicantPIN,
        applicantEmail: applicantEmail,
        applicantMob: applicantMob,
        ubmID: ubmID
    }
    $.ajax({
        url: '/api/bookingController/addApplicantDetails',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                $("#coApplicantTable").DataTable().ajax.reload();
                clearCoApplicant();
                document.getElementById("coApplicantDiv").style.display = "none";
            }
            else {
                ErrorMessage(data.message);

            }
        }
    })
}



function clearCoApplicant() {
    document.getElementById("coApplicantName").value = "";
    document.getElementById("coApplicantSwd").value = "";
    document.getElementById("coApplicantDOB").value = "";
    document.getElementById("coApplicantAge").value = "";
    document.getElementById("coApplicantNationality").value = "";
    document.getElementById("coApplicantOccupation").value = "";
    document.getElementById("coApplicantPAN").value = "";
    document.getElementById("coApplicantAdharNo").value = "";
    document.getElementById("coApplicantAddress").value = "";
    document.getElementById("coApplicantCityID").value = "";
    document.getElementById("coApplicantStateID").value = "";
    document.getElementById("coApplicantCountry").value = "";
    document.getElementById("coApplicantPIN").value = "";
    document.getElementById("coApplicantEmail").value = "";
    document.getElementById("coApplicantMobile").value = "";
}


function CoApplicantstateList() {
    $.ajax({
        url: '/api/StateCityController/StateList',
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                $('#coApplicantStateID').append('<option> Select </option>');
                for (i = 0; i < data.data.length; i++) {
                    $('#coApplicantStateID').append('<option value=' + data.data[i]['stateID'] + '> ' + data.data[i]['stateName'] + ' </option>')
                }
                $('#coApplicantStateID').select2();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    });
}

function CoApplicantCityList() {
    var StateID = $('#coApplicantStateID').val();
    $.ajax({
        url: '/api/StateCityController/CityList?stateID=' + StateID,
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                $('#coApplicantCityID').empty();
                $('#coApplicantCityID').append('<option> Select </option>');
                for (i = 0; i < data.data.length; i++) {
                    $('#coApplicantCityID').append('<option value=' + data.data[i]['cityID'] + '> ' + data.data[i]['cityName'] + ' </option>')
                }
                $('#coApplicantCityID').select2();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    });
}


function getApplicantDetails(appType) {
    var ubmID = document.getElementById("ubmID").innerHTML;

    $.ajax({
        url: '/api/bookingController/getApplicantList?ubmID=' + ubmID + '&appType=' + appType,
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {

                $('#applicantunitno').text($('#spUnitNo').text());
                if (data.data.length == 0) {
                    document.getElementById("applicantEmail").value = $('#custEmail').val();
                    document.getElementById("applicantMob").value = $('#custMobileNo').val();
                }
                document.getElementById("applicantID").innerHTML = data.data[0].applicantID;
                document.getElementById("applicantBuyerName").value = data.data[0].applicantBuyerName;
                document.getElementById("applicantswd").value = data.data[0].applicantswd;
                var formatapplicantdob = data.data[0].applicantdob.split('T')[0];
                document.getElementById("applicantdob").value = formatapplicantdob;
                document.getElementById("applicantAge").value = data.data[0].applicantAge;
                document.getElementById("applicantNationality").value = data.data[0].applicantNationality;
                document.getElementById("applicantOccupation").value = data.data[0].applicantOccupation;
                document.getElementById("applicantPan").value = data.data[0].applicantPan;
                document.getElementById("applicantAdharNo").value = data.data[0].applicantAdharNo;
                document.getElementById("applicantCorrAddress").value = data.data[0].applicantCorrAddress;

                
                $('#applicantState').val(data.data[0].applicantStateID).trigger('change');
                setTimeout(function () {
                    $('#applicantCity').val(data.data[0].applicantCityID).trigger('change');
                }, 500); 
               
                document.getElementById("applicantCountry").value = data.data[0].applicantCountry;
                document.getElementById("applicantPIN").value = data.data[0].applicantPIN;
                document.getElementById("applicantEmail").value = data.data[0].applicantEmail;
                document.getElementById("applicantMob").value = data.data[0].applicantMob;
            }
            else {
                ErrorMessage(data.message);

            }
        }
    })
}

function applicantList(appType) {
    var ubmID = document.getElementById("ubmID").innerHTML;

    dataTable = $("#coApplicantTable").DataTable({

        ajax: {
            url: '/api/bookingController/getApplicantList?ubmID=' + ubmID + '&appType=' + appType,
            type: 'GET',
            contentType: 'application/json',
        },
        columns: [
            {
                'data': null,
                'width': '1%', 'font-size': '6px'
            },
            {
                'data': 'applicantBuyerName',
                'width': '20%', 'font-size': '6px'
            },
            {
                'data': 'applicantCorrAddress',
                'width': '70%', 'font-size': '6px'
            },

            {
                'data': 'applicantID',
                'render': function (data, type, row) {
                    return `<button data-row='${JSON.stringify(row)}' onclick="deleteCoApplicant(this)"  type="button" class="btn btn-danger">Delete </button>`

                },
                'width': '9%',
                'font-size': '6px'
            },

        ],

        "font- size": '1em',
        dom: 'lBfrtip',
        "bDestroy": true,
        "paging": false,
        "searching": false,
        "ordering": true,
        "scrollX": false,
        "info": false,

        language: {
            searchPlaceholder: "Search records",
            emptyTable: "No data found",
            width: '100%',
        },

    }
    );
    dataTable.on('order.dt ', function () {
        dataTable.column(0, { order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

}


function deleteCoApplicant(button) {
    var rowData = JSON.parse(button.getAttribute('data-row'));

    var payLoad = {
        applicantID: rowData.applicantID,
    }
    $.ajax({
        url: '/api/bookingController/deleteCoApplicant',
        type: 'DELETE',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                $("#coApplicantTable").DataTable().ajax.reload();
            }
            else {
                ErrorMessage(data.message);
            }
        }
    })


}

function ChangeUbmAuthorization() {
    var ubmID = document.getElementById("ubmID").innerHTML;
    var remarks = document.getElementById("remarks").value;

    if (remarks == "") return ErrorMessage("Please Enter Remarks")

    var payLoad = {
        UbmID: ubmID,
        Remarks: remarks,
    }
    $.ajax({
        url: '/api/bookingController/ChangeUbmAuthorization',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                window.location.replace("../Success")

            }
            else {
                ErrorMessage(data.message);
            }
        }
    })
}

function openAuthorizationModel(button) {
    var rowData = JSON.parse(button.getAttribute('data-row'));
    document.getElementById("ubmID").innerHTML = rowData.ubmID;
}


function deletePaymentPlan(paymentID) {
    var ubmID = $('#ubmID').text();
    $.ajax({
        url: '/api/bookingController/deletepaymentPlan?paymentID=' + paymentID,
        type: 'DELETE',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                ubmPaymentList(ubmID)
            }
            else {
                ErrorMessage(data.message);
            }
        }
    })
}

function getTncTemplate() {
    var ubmID = $('#ubmID').text();

    $.ajax({
        url: '/api/bookingController/getTncTemplate?ubmID=' + ubmID,
        type: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                document.getElementById("tncTemplate").innerHTML = data.data;
            }
            else {
                ErrorMessage(data.message);
            }

        }
    })

}

function previousTNCTab() {
    $('#du-tab').tab('show');
    $('#du-tab').addClass('active');
    $('#du-tab-home').addClass('active show');
}

function showCoApplicantDiv() {
    document.getElementById("coApplicantDiv").style.display = "block";
}



function GetUnitInfo() {

    var UnitId = $('#unitID').val();
    if (UnitId == 'Select' || UnitId == '')
        return;
    $.ajax({
        url: '/api/bookingController/GetUnitInfo?UnitID=' + UnitId,
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {
            if (res.success) {
                $('#spFlatType').html(res.data["unitTypeName"]);
                $('#spTowerName').html(res.data["blockName"]);
                $('#spUnitNo').html(res.data["unitNo"]);
                $('#spUnitCarpetArea').html(res.data["unitCarpetArea"]);
                $('#spUnitScheme').html(res.data["schemeName"]);
                $('#txtInterestPlan').val(res.data["intPlanName"]);
                $('#hdnIntPayPlan').val(res.data["payPlanID"]);
                if (res.data["payPlanID"] > 0)
                    $('#divPayPlan').show();
                else
                    $('#divPayPlan').hide();
                $('#btnPayPlan').html(res.data["payPlanName"]);

            }
            else {
                ErrorMessage(res.data);
            }
        }

    });
}

function ShowPayplan() {

    var paymentPlanId = $('#hdnIntPayPlan').val();
    var paymentPlan = $("#btnPayPlan").html();
    $.ajax({
        url: '/api/UnitController/GetPaymentPlan?PayPlanID=' + paymentPlanId,
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {

            if (res.success) {
                var page = "<!DOCTYPE html><html><head><style>";
                page += "body {vertical-align: middle;}";
                page += "table { font-family: arial, sans-serif; border-collapse: collapse; width: 100%;}";
                page += "td, th { border: 1px solid #dddddd; text-align: left; padding: 8px;}";
                page += "tr:nth-child(even) { background-color: #dddddd;}";
                page += ".table-container { max-height: 550px; overflow-y: auto; }";
                page += "</style></head><body>";
                page += "<h2>Payment Plan: " + paymentPlan + "</h2>";
                page += "<div class='table-container'>";
                page += "<table><tr><th>S. No.</th><th>Stage Name</th><th>Charge Name</th><th>Due %</th></tr>";
                var i = 1;
                if (res.data.length > 0) {
                    var prestageId = 0;
                    res.data.forEach(function (entry) {
                        page += "<tr>";
                        if (prestageId != entry.stageID) {
                            page += "<td>" + i++ + "</td>";
                            page += "<td>" + entry.stageName + "</td>";
                            page += "<td>" + entry.chargeName + "</td>";
                            page += "<td>" + entry.duePercentage + "</td>";
                            page += "</tr>";
                        }
                        else {
                            page += "<td></td>";
                            page += "<td></td>";
                            page += "<td>" + entry.chargeName + "</td>";
                            page += "<td>" + entry.duePercentage + "</td>";
                            page += "</tr>";
                        }

                        prestageId = entry.stageID;
                    });
                }
                else {
                    page += "<tr>";
                    page += "<td colspan='4'>No Record Found!</td>";
                    page += "</tr>";
                }
                page += "</table>";
                page += "</div>";
                page += "</body></html>";
                var wnd = window.open("PayPlan:blank", "", "_blank");
                wnd.document.write(page);
            }
            else {
                ErrorMessage(res.data);
            }
        }
    });
}




function ShowDocuments() {
    var ApplicationType = $('#appType').val();
    $.ajax({
        url: '/api/bookingController/GetApplicationDocument?ApplicationType=' + ApplicationType,
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {

            if (res.success) {
                var page = "<!DOCTYPE html><html><head><style>";
                page += "body {vertical-align: middle;}   table { font-family: arial, sans-serif; border-collapse: collapse;width: 100 %;}";
                page += "td, th { border: 1px solid #dddddd;  text-align: left;  padding: 8px;}";
                page += "tr:nth-child(even) { background-color: #dddddd;}</style></head><body>";
                page += "<h2>Required Document</h2>";
                page += "<table><tr><th>S. No.</th><th>Document Name</th></tr>";

                var i = 1;
                if (res.data.length > 0) {
                    res.data.forEach(function (entry) {
                        page += "<tr>";
                        page += "<td>" + i++ + "</td>";
                        page += "<td>" + entry + "</td>";
                        page += "</tr>";
                    });
                }
                else {
                    page += "<tr>";
                    page += "<td colspan='2'>No record found</td>";
                    page += "</tr>";
                }


                page += "</table>";
                page += "</body></html>";
                var wnd = window.open("about:blank", "", "_blank");
                wnd.document.write(page);
            }
            else {
                ErrorMessage(res.data);
            }
        }
    });
}


function paymodeChange() {
    var payMode = $("#payMode").val();
    if (payMode == "Cheque") {
        $('#transactionNoDiv').hide();
        $('#ChequeNoDiv').show();
        $('#bankDiv').show();
    }
    else if (payMode == "Bank Transfer") {
        $('#transactionNoDiv').show();
        $('#ChequeNoDiv').hide();
        $('#bankDiv').show();
    }
    else {
        $('#transactionNoDiv').show();
        $('#ChequeNoDiv').hide();
        $('#bankDiv').hide();
    }
}


function previousPaymentButton() {
    $('#unit-tab').tab('show');
    $('#unit-tab').addClass('active');
    $('#unit-tab-home').addClass('active show');
}

function blockFutureDate() {
    var today = new Date().toISOString().split('T')[0];
    document.getElementById('applicantdob').setAttribute('max', today);
    document.getElementById('coApplicantDOB').setAttribute('max', today);
}


function checkTncClick() {

    var tnc1 = $('input:checkbox[name=tncCheckBox]').is(':checked');
    var tnc2 = $('input:checkbox[name=tnc2CheckBox]').is(':checked');
    const finalSubmitBtn = document.getElementById("finalSubmitBtn");
    if (tnc1 == true && tnc2 == true) {
        finalSubmitBtn.disabled = false;
    }
    else {
        finalSubmitBtn.disabled = true;
    }
}