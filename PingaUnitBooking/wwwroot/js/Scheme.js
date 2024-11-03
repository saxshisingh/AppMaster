
$(document).ready(function () {
    document.getElementById("btnAddScheme").innerHTML = "Save";
    appSchemeList();
    // ErrorMessage('hi');
});



function addScheme() {

 
    var schemeName = document.getElementById("txtSchemeName").value;
    var schemeDesc = document.getElementById("txtSchemeDesc").value; 
    if (schemeName == "") {
        ErrorMessage('Please enter scheme');
        return
    }
    var dataToSend = {
        SchemeID: $("#hdnSchemeID").val() == '' ? '0' : $("#hdnSchemeID").val(),
        SchemeName: schemeName,
        SchemeDesc: schemeDesc 
    }
    $('#empLoader').show();
    $.ajax({
        url: '/api/SchemeController/SaveScheme',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),

        success: function (data) {
            $('#empLoader').hide();    
            if (data.success) {
                SuccessMessage(data.message);               
                ClearData(); 
                $('#addScheme').modal('hide');
                $('.modal-backdrop').remove();
                $("#hdnSchemeID").val(0);
                appSchemeList(); 
                document.getElementById("btnAddScheme").innerHTML = "Save";
                document.getElementById("planLabel").innerHTML = "Add Scheme";
            }
            else {
                $('#empLoader').hide();    
                ErrorMessage(data.message);
            }
        }
    })
}
function appSchemeList() {
    $('#empLoader').show();    
    $.ajax({

        'type': 'GET',
        'url': '/api/SchemeController/GetSchemeList',
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {
            $('#empLoader').hide();    
            if (response.success == false) {
                ErrorMessage(response.data);
            }

            else {


                var html = "<table id='SchemeList' class='table table-bordered table-hover' style='width:100%'>";
                html += "<thead>";
                html += "<tr>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>S. No</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Scheme Name</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Scheme Desc</th>"; 
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Actions</th>";
                html += "</tr>";
                html += "</thead>";
                var i = 1;
                if (response.data.length > 0) {
                    response.data.forEach(function (entry) {
                        html += "<tr>";
                        html += "<td>" + i++ + "</td>";
                        html += "<td>" + entry.schemeName + "</td>";
                        html += "<td>" + entry.schemeDesc + "</td>"; 
                        html += "<td><button data-row=" + JSON.stringify(entry) + " data-toggle='modal' href='' data-target='#addScheme' onclick ='EditScheme(" + JSON.stringify(entry) + ")' type = 'button' class='btn btn-primary'> <i class='fa fa-edit'></i>&nbsp;Edit</button>&nbsp;";
                        html += "<button  data-row=" + JSON.stringify(entry.schemeID) + " onclick='DeleteScheme(" + JSON.stringify(entry) + ")' type='button' class='btn btn-danger'><i class='fa fa-bitbucket'></i>&nbsp;Delete</button>";
                        html += "</td>";
                        html += "</tr>";
                    });
                }
                //else {
                //    html += "<tr>";
                //    html += "<td colspan='4' style='text-align:center'>No Record Found!</td>";
                //    html += "</tr>";
                //}

                html += "</table>";
                document.getElementById("divSchemeList").innerHTML = html;
                dataTable = $("#SchemeList").DataTable({
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


function EditScheme(rowData) {
    $("#hdnSchemeID").val(rowData["schemeID"]);
    $("#txtSchemeName").val(rowData['schemeName']);
    $("#txtSchemeDesc").val(rowData['schemeDesc']);
    document.getElementById("planLabel").innerHTML = "Update Scheme";
    document.getElementById("btnAddScheme").innerHTML = "Update";
}

function ClearData() {
    $("#hdnSchemeID").val('');
    $("#txtSchemeName").val('');
    $("#txtSchemeDesc").val('');
    document.getElementById("planLabel").innerHTML = "Add Scheme";
}

function DeleteScheme(rowData) {
  
    var SchemeID = rowData["schemeID"];
    Swal.fire({
        title: 'Are you want to delete scheme?',
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
                url: '/api/SchemeController/DeleteScheme',
                type: 'GET',
                contentType: 'application/json',
                data: {
                    SchemeId: SchemeID
                },
                success: function (data) {
                    $('#empLoader').hide();    
                    if (data.success) {
                        SuccessMessage(data.message);
                        appSchemeList();
                    }
                    else {
                        ErrorMessage(data.message);
                    }
                }
            })
        };
    })
}




