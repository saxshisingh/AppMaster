$(document).ready(function () {

    $('#ddlUserProjects').select2();
    $('#ddlTower').select2();
    $('#interestPlan').select2();
    $('#ddlScheme').select2();
    $('#interestPlan').append('<option>Select</option>');
    $('#ddlScheme').append('<option>Select</option>');
    UnitConfigureList();
    GetUserProjects();
    GetScheme();
});

function searchCondition() {
    var searchType = $("#searchType").val();
    if (searchType == "Project&Tower") {
        document.getElementById("projectDiv").style.display="block"
        document.getElementById("towerDiv").style.display ="block"
    } else {
        document.getElementById("projectDiv").style.display = "none"
        document.getElementById("towerDiv").style.display = "none"
    }
}


function GetScheme() {
    $('#ddlScheme').empty();
    $('#empLoader').show();
    $.ajax({
        url: '/api/UnitController/GetScheme',
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {
            $('#empLoader').hide();

            if (res.success) {

                $('#ddlScheme').append('<option>Select</option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlScheme').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                }
                $('#ddlScheme').select2();
            }
            else {
                ErrorMessage(res.data)
                $('#ddlScheme').empty();
                $('#ddlScheme').append('<option>Select</option>');
            }
        }
    })
}
var unitcounter = 0;


function validateNumber(e) {
    const pattern = /^[0-9]$/;
    return pattern.test(e.key)
}
function UnitConfigureList() {
    unitcounter = 0;
    $('#empLoader').show();
    var projectid = $("#ddlUserProjects").val() == null ? 0 : $("#ddlUserProjects").val();
    var towerid = $("#ddlTower").val() == null ? 0 : $("#ddlTower").val();
    var statusType = $("#searchType").val() == "Select" ? "" : $("#searchType").val()
    $.ajax({
        url: '/api/UnitController/unitDetailsList',
        type: 'GET',
        contentType: 'application/json',
        data: {
            ProjectID: projectid,
            TowerID: towerid,
            statusType, statusType
        },
        success: function (res) {
            $('#empLoader').hide();
            
            if (!res.success) {
                ErrorMessage(res.data);
                return
            }
            dataTable = $('#unitConfigureListTable').DataTable({
                data: res.data,
                columns: [
                    /*{
                        'data': 'unitID', 'render': function (data, type, row) {
                            unitcounter++; // Increment counter
                            return `<span>${unitcounter}</span>`;
                        },
                        'width': '1%', 'font-size': '6px'
                    },*/
                    {
                        'data': null, 'render': function (data, type, row, meta) {
                            return meta.row + 1; // Index
                        },
                        'width': '1%', 'font-size': '6px'
                    },
                    {
                        'data': 'unitNo',
                        'render': function (data, type, row) {
                            return `<a href="" data-backdrop="static" data-keyboard="false"  data-toggle="modal" data-target="#addUnitmodal" data-row='${JSON.stringify(row)}' onclick="viewUnitDetails(this)"">${row.unitNo}</a>`
                        },
                        'width': '10%',
                        'font-size': '6px'
                    },
                    {
                        'data': 'towerName',
                        'width': '15%', 'font-size': '6px'
                    },
                    {
                        'data': 'projectName',
                        'width': '15%', 'font-size': '6px'
                    },
                    {
                        'data': 'locationName',
                        'width': '10%', 'font-size': '6px'
                    },
                    {
                        'data': 'netAmount',
                        'width': '15%', 'font-size': '6px'
                    },
                    {
                        'data': 'createdBy',
                        'width': '15%', 'font-size': '6px'
                    },

                    {
                        'data': 'unitID',
                        'render': function (data, type, row) {
                            if (row.roleName == "Admin/CFO") {

                                if (row.unitStatus == 1) {
                                    return `<button type="button" onclick=changeUnitStatus("${row.unitID}") class="btn btn-warning"><i class="fa fa-clock-o mr-2"></i>Change Status</button>`
                                }
                                else if (row.unitStatus == 2)
                                    return ` <button  type="button" class="btn btn-success"><i class="fa fa-check mr-2"></i>Approved</button>`
                                else
                                    return `<apn></span>`
                            }
                            else {

                                if (row.unitStatus == 0) {
                                    return `<button data-row='${JSON.stringify(row)}' data-toggle="modal" data-backdrop="static" data-keyboard="false" onclick="getUnitDetailsByID(this)" data-target="#addUnitmodal" type="button" class="btn btn-primary"><i class="fa fa-edit mr-2"></i> Request for approval</button>`
                                }
                                else if (row.unitStatus == 1) {
                                    return `<button type="button" class="btn btn-warning"><i class="fa fa-clock-o mr-2"></i>Approval Pending</button>`
                                }
                                else if (row.unitStatus == 2)
                                    return ` <button  type="button" class="btn btn-success"><i class="fa fa-check mr-2"></i>Approved</button>`
                            }

                        },
                        'width': '22%',
                        'font-size': '6px'
                    },

                ],

                "font- size": '1em',
                dom: 'lBfrtip',
                "bDestroy": true,
                "paging": true,
                "searching": true,
                "ordering": false,
                "scrollX": true,
                "info": false,

                language: {
                    searchPlaceholder: "Search records",
                    emptyTable: "No data found",
                    width: '100%',
                },
            });
        }
    });



    //dataTable = $("#unitConfigureListTable").DataTable({

    //    ajax: {
    //        'type': 'GET',
    //        'url': '/api/UnitController/unitDetailsList',
    //        'contentType': 'application/json',
    //        data: {
    //            ProjectID: projectid,
    //            TowerID: towerid
    //        }
    //    },


    //    columns: [
    //        {

    //            'data': 'unitID', 'render': function (data, type, row) {
    //                unitcounter++; // Increment counter
    //                return `<span>${unitcounter}</span>`;
    //            },
    //            'width': '1%', 'font-size': '6px'
    //        },
    //        {
    //            'data': 'unitNo', 'render': function (data, type, row) {
    //                return `<span>${row.unitNo}</span>`;
    //            },
    //            'width': '10%', 'font-size': '6px'
    //        },
    //        {
    //            'data': 'towerName', 'render': function (data, type, row) {
    //                return `<span>${row.towerName}</span>`;
    //            },
    //            'width': '15%', 'font-size': '6px'
    //        },
    //        {
    //            'data': 'projectName', 'render': function (data, type, row) {
    //                return `<span>${row.projectName}</span>`;
    //            },
    //            'width': '15%', 'font-size': '6px'
    //        },
    //        {
    //            'data': 'locationName', 'render': function (data, type, row) {
    //                return `<span>${row.locationName}</span>`;
    //            },
    //            'width': '10%', 'font-size': '6px'
    //        },
    //        {
    //            'data': 'netAmount', 'render': function (data, type, row) {
    //                return `<span>₹ ${row.netAmount}</span>`;
    //            },
    //            'width': '15%', 'font-size': '6px'
    //        },

    //        {
    //            'data': 'unitID',
    //            'render': function (data, type, row) {
    //                if (row.roleName == "Admin/CFO") {

    //                    if (row.unitStatus == 1) {
    //                        return `<button type="button" onclick=changeUnitStatus("${row.unitID}") class="btn btn-warning"><i class="fa fa-clock-o mr-2"></i>Change Status</button>`
    //                    }
    //                    else if (row.unitStatus == 2)
    //                        return ` <button  type="button" class="btn btn-success"><i class="fa fa-check mr-2"></i>Approved</button>`
    //                    else
    //                        return `<apn></span>`
    //                }
    //                else {

    //                    if (row.unitStatus == 0) {
    //                        return `<button data-row='${JSON.stringify(row)}' data-toggle="modal" onclick="getUnitDetailsByID(this)" data-target="#addUnitmodal" type="button" class="btn btn-primary"><i class="fa fa-edit mr-2"></i>Approval Request </button>`
    //                    }
    //                    else if (row.unitStatus == 1) {
    //                        return `<button type="button" class="btn btn-warning"><i class="fa fa-clock-o mr-2"></i>Approval Pending</button>`
    //                    }
    //                    else if (row.unitStatus == 2)
    //                        return ` <button  type="button" class="btn btn-success"><i class="fa fa-check mr-2"></i>Approved</button>`
    //                }

    //            },
    //            'width': '22%',
    //            'font-size': '6px'
    //        },

    //    ],

    //    "font- size": '1em',
    //    dom: 'lBfrtip',
    //    "bDestroy": true,
    //    "paging": true,
    //    "searching": true,
    //    "ordering": true,
    //    "scrollX": true,
    //    "info": false,

    //    language: {
    //        searchPlaceholder: "Search records",
    //        emptyTable: "No data found",
    //        width: '100%',
    //    },

    //}
    //);

}

function viewUnitDetails(button) {
    $("#addUnitmodalDiv :input").prop("disabled", true);
    getUnitDetailsByID(button, 1)
}




function getUnitDetailsByID(button, type = null) {
    if (type == null) {
        $("#addUnitmodalDiv :input").prop("disabled", false);
    }
    var rowData = JSON.parse(button.getAttribute('data-row'));
    document.getElementById("projectName").value = rowData.projectName;
    document.getElementById("categoryName").value = rowData.categoryName;
    document.getElementById("towerName").value = rowData.towerName;
    document.getElementById("unitNo").value = rowData.unitNo;
    document.getElementById("unitID").value = rowData.unitID;
    document.getElementById("floorName").value = rowData.floorName;
    document.getElementById("area").value = rowData.area;
    document.getElementById("superArea").value = rowData.unitSuperArea;
    document.getElementById("rate").value = rowData.rate;
    document.getElementById("builtUpArea").value = rowData.unitBuiltUpArea;
    document.getElementById("superAreaRate").value = rowData.unitSuperAreaRate;
    document.getElementById("terraceArea").value = rowData.unitTerraceArea;
    document.getElementById("builtUpAreaRate").value = rowData.unitBuiltUpAreaRate;
    document.getElementById("terraceAreaRate").value = rowData.unitTerraceAreaRate;
    document.getElementById("carpetArea").value = rowData.unitCarpetArea;
    document.getElementById("balconyArea").value = rowData.unitBalconyArea;
    document.getElementById("basicAmount").value = rowData.basicAmount;
    document.getElementById("additionalCharge").value = rowData.additionalCharge;
    rowData.discountAmount != 0 ? document.getElementById("discountAmount").value = rowData.discountAmount : document.getElementById("discountAmount").value = 0;
    rowData.minSaleAmount != 0 ? document.getElementById("minSaleAmount").value = rowData.minSaleAmount : document.getElementById("minSaleAmount").value = 0;
    rowData.maxSaleAmount != 0 ? document.getElementById("maxSaleAmount").value = rowData.maxSaleAmount : document.getElementById("maxSaleAmount").value = 0;
    netAmount();
    getPaymentPlan(rowData.floorID, rowData.unitID, rowData.companyID, rowData.locationID);
    getIntrestPlan(rowData.companyID, rowData.locationID);
    
   
    /*  if (rowData.intPlanID != 0) {*/
    setTimeout(function () {
        $("#ddlScheme").val(rowData.schemeID).trigger('change');
    }, 100);
    setTimeout(function () {
        $('#interestPlan').val(rowData.intPlanID).trigger('change');
    }, 100);
    setTimeout(function () {
        $('#paymentPlan').val(rowData.payplanID).trigger('change');
    }, 100);
    /*} */
}

function getPaymentPlan(blockID, unitID, companyID, locationID) {
    $.ajax({
        url: '/api/UnitController/paymentPlanList',
        type: 'GET',
        contentType: 'application/json',
        data: {
            blockID: blockID,
            unitID: unitID,
            companyID: companyID,
            locationID: locationID
        },
        success: function (data) {
            $('#paymentPlan').empty(); // Clear existing options
            $('#paymentPlan').append('<option value="">Select</option>');
            if (data.success) {
                for (var i = 0; i < data.data.length; i++) {

                    $('#paymentPlan').append('<option value="' + data.data[i]['payplanID'] + '">' + data.data[i]['payplanName'] + '</option>');
                }
                $('#paymentPlan').select2();

            }

            else {
                ErrorMessage(data.message);

            }
        }
    })
}

function getIntrestPlan(companyID, locationID) {
    $.ajax({
        url: '/api/UnitController/intrestPlanList',
        type: 'GET',
        contentType: 'application/json',
        data: {
            companyID: companyID,
            locationID: locationID
        },
        success: function (data) {
            if (data.success) {
                $('#interestPlan').empty(); // Clear existing options
                $('#interestPlan').append('<option value="">Select</option>');

                for (var i = 0; i < data.data.length; i++) {

                    $('#interestPlan').append('<option value="' + data.data[i]['intPlanID'] + '">' + data.data[i]['intPlanName'] + '</option>');
                }
                $('#interestPlan').select2();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    })
}

function netAmount() {


    var basicAmount = $('#basicAmount').val();
    var discountAmount = $('#discountAmount').val();
    if (parseInt(discountAmount) > parseInt(basicAmount)) {
        document.getElementById("discountAmount").value = 0;
        document.getElementById("netAmount").value =
            parseInt(document.getElementById("basicAmount").value) +
            parseInt(document.getElementById("additionalCharge").value) -
            parseInt(document.getElementById("discountAmount").value != "" ? document.getElementById("discountAmount").value : '0')

        return alert("Discount Amount cannot not be greater than basic amount");
    }
    else {
        document.getElementById("netAmount").value =
            parseInt(document.getElementById("basicAmount").value) +
            parseInt(document.getElementById("additionalCharge").value) -
            parseInt(document.getElementById("discountAmount").value != "" ? document.getElementById("discountAmount").value : '0')

    }


}


function minSaleAmount() {
    var netAmount = $('#netAmount').val();
    var minSaleAmt = $('#minSaleAmount').val();
    if (parseInt(minSaleAmt) < parseInt(netAmount)) {
        return false;
    } else {
        return true
    }
}


function addUbmUnit() {

    var UnitID = document.getElementById("unitID").value;
    var BasicAmount = document.getElementById("basicAmount").value;
    var AdditionalAmount = document.getElementById("additionalCharge").value;
    var DiscountAmount = document.getElementById("discountAmount").value;
    var NetAmount = document.getElementById("netAmount").value;
    var minSaleAmount = document.getElementById("minSaleAmount").value;
    var maxSaleAmount = document.getElementById("maxSaleAmount").value;
    var IntPlanID = document.getElementById("interestPlan").value == "" ? 0 : document.getElementById("interestPlan").value;
    var PayPlanID = document.getElementById("paymentPlan").value;
    var schemeId = document.getElementById("ddlScheme").value;

    if (minSaleAmount == ""  || DiscountAmount == "" || PayPlanID == "" ) {
        ErrorMessage('Please fill all required fields');
        return;
    }
    else if (parseInt(minSaleAmount) == 0) {
        ErrorMessage('Minimum Sale Amount cannot not be greater than Net amount');
        return;
    }
    else if (parseInt(minSaleAmount) > parseInt(NetAmount)) {
        ErrorMessage('Minimum Sale Amount cannot not be greater than Net amount');
        return;
    }

    var dataToSend = {
        UnitID: UnitID,
        BasicAmount: BasicAmount,
        additionalCharge: AdditionalAmount,
        DiscountAmount: DiscountAmount,
        NetAmount: NetAmount,
        minSaleAmount: minSaleAmount,
        maxSaleAmount: maxSaleAmount,
        payment: { payplanID: PayPlanID },
        intrest: { intPlanID: IntPlanID },
        SchemeID: schemeId != "" ? schemeId : null 
    }

    $.ajax({
        url: '/api/UnitController/addUbmUnit',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                location.reload();
            }
            else {
                ErrorMessage(data.message);

            }
        }
    })
}
function changeUnitStatus(unitID) {


    Swal.fire({
        title: "Approve Unit for Sale?",
        showDenyButton: true,
        showCloseButton: true,
        showCancelButton: false,
        confirmButtonText: "Approved",
        denyButtonText: `Not Approved`
    }).then((result) => {

        if (result.isConfirmed) {
            $.ajax({
                type: 'GET',
                url: '/api/UnitController/changeUnitStatus?unitID=' + unitID + '&status=2',
                success: function (data) {
                    if (data.success == true) {
                        SuccessMessage(data.message);
                        UnitConfigureList();
                    }
                    else {
                        ErrorMessage(data.message);

                    }
                }
            });
        }
        else if (result.isDenied) {
            $.ajax({
                type: 'GET',
                url: '/api/UnitController/changeUnitStatus?unitID=' + unitID + '&status=0',
                success: function (data) {
                    if (data.success == true) {
                        SuccessMessage(data.message);
                        UnitConfigureList();
                    }
                    else {
                        ErrorMessage(data.message);


                    }
                }
            });
        }

    });

}
function GetUserProjects() {
    $('#ddlUserProjects').empty();
    $.ajax({
        url: '/api/UnitController/GetUserProjects',
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {
            if (res.success) {

                $('#ddlUserProjects').append('<option>Select</option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlUserProjects').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                }
                $('#ddlUserProjects').select2();
            }
            else {
                ErrorMessage(res.data)

            }
        }
    })
    $('#ddlTower').append('<option>Select</option>');
}



function GetTowerByProjectId(projectId) {

    if (projectId == 0)
        return
    $('#ddlTower').empty();
    $.ajax({
        url: '/api/UnitController/GetTowerByProjectId',
        type: 'GET',
        contentType: 'application/json',
        data: {
            ProjectID: projectId
        },
        success: function (res) {
            if (res.success) {
                $('#ddlTower').append('<option>Select</option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlTower').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                }
                $('#ddlTower').select2();
            }
            else {
                ErrorMessage(res.data);
            }
        }
    })

    $('#ddlTower').select2({
        closeOnSelect: true,
        placeholder: "Select Tower"
    });
}

function ChangePayplan(sel) {

    var text = sel.options[sel.selectedIndex].text;
    var payplan = sel.options[sel.selectedIndex].value
    $("#hdnPayPlan").val(text);
    if (payplan > 0) {
        $("#divPayPlan").show()
    }
    else {
        $("#divPayPlan").hide()
    }
}
function ShowPayplan() {

    var paymentPlanId = $('#paymentPlan').val();
    var paymentPlan = $("#hdnPayPlan").val();
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
