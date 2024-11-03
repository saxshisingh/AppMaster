
$(document).ready(function () {
    document.getElementById("btnAddAppDoc").innerHTML = "Save";
    appDocList();
    // ErrorMessage('hi');
});



function addAppDoc() {

    var e = document.getElementById("ddlApplicationType")
    var apptype = e.options[e.selectedIndex].value;
    var docname = document.getElementById("docname").value;
    var Mandatory = document.getElementById("IsRequired").checked; 

    if (apptype == 'Select') {
        ErrorMessage('Please select Application Type');
        return
    }

    if (docname == "") {
        ErrorMessage('Please enter document name');
        return
    }
    var dataToSend = {
        DocumentID: $("#hdnDocID").val() == '' ? '0' : $("#hdnDocID").val(),
        ApplicationType: apptype,
        DocumentName: docname,
        Mandatory: Mandatory

    }
    $('#empLoader').show();
    $.ajax({
        url: '/api/ApplicationDocController/SaveApplicationDocument',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),

        success: function (data) {
            $('#empLoader').hide();    
            if (data.success) {
               
                SuccessMessage(data.message);               
                ClearData(); 
                $('#addAppDocument').modal('hide');
                $('.modal-backdrop').remove();
                $("#hdnDocID").val(0);
                appDocList(); 
                document.getElementById("btnAddAppDoc").innerHTML = "Save";
            
            }
            else {
                $('#empLoader').hide();    
                ErrorMessage(data.message);
            }
        }
    })
}
function appDocList() {
    $('#empLoader').show();    
    $.ajax({

        'type': 'GET',
        'url': '/api/ApplicationDocController/GetApplicationDocList',
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {
            $('#empLoader').hide();    
            if (response.success == false) {
                ErrorMessage(response.data);
            }

            else {


                var html = "<table id='AppDocList' class='table table-bordered table-hover' style='width:100%'>";
                html += "<thead>";
                html += "<tr>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>S. No</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Application Type</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Document Name</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>IsMandatory</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Actions</th>";
                html += "</tr>";
                html += "</thead>";
                var i = 1;
                if (response.data.length > 0) {
                    response.data.forEach(function (entry) {
                        html += "<tr>";
                        html += "<td>" + i++ + "</td>";
                        html += "<td>" + entry.applicationType + "</td>";
                        html += "<td>" + entry.documentName + "</td>";
                        html += "<td>" + entry.mandatory + "</td>";
                        html += "<td><button data-row=" + JSON.stringify(entry) + " data-toggle='modal' href='' data-target='#addAppDocument' onclick ='EditAppDoc(" + JSON.stringify(entry) + ")' type = 'button' class='btn btn-primary'> <i class='fa fa-edit'></i>&nbsp;Edit</button>&nbsp;";
                        html += "<button  data-row=" + JSON.stringify(entry.documentID) + " onclick='DeleteAppDoc(" + JSON.stringify(entry) + ")' type='button' class='btn btn-danger'><i class='fa fa-bitbucket'></i>&nbsp;Delete</button>";
                        html += "</td>";
                        html += "</tr>";
                    });
                }
                else {
                    html += "<tr>";
                    html += "<td colspan='5' style='text-align:center'>No Record Found!</td>";
                    html += "</tr>";
                }

                html += "</table>";
                document.getElementById("divAppDocList").innerHTML = html;
                dataTable = $("#AppDocList").DataTable({
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


function EditAppDoc(rowData) {
    $("#hdnDocID").val(rowData["documentID"]);
    $("#docname").val(rowData['documentName']);
    $("#ddlApplicationType").val(rowData['applicationType']);
    $("#IsRequired").prop("checked", rowData['mandatory']);
    document.getElementById("btnAddAppDoc").innerHTML = "Update";
}

function ClearData() {
    $("#docname").val('');
    $("#ddlApplicationType").val('Select');
    $("#IsRequired").prop("checked", false);
}

function DeleteAppDoc(rowData) {
  
    var id = rowData["documentID"];
    Swal.fire({
        title: 'Are you want to delete document?',
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
                url: '/api/ApplicationDocController/DeleteApplicationDocument',
                type: 'GET',
                contentType: 'application/json',
                data: {
                    id: id
                },
                success: function (data) {
                    $('#empLoader').hide();    
                    if (data.success) {
                        SuccessMessage(data.message);
                        appDocList();
                    }
                    else {
                        ErrorMessage(data.message);
                    }
                }
            })
        };
    })
}




