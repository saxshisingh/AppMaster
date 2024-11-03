$(document).ready(function () {
    $('#empLoader').hide();

    ProjectConfigureList();
    SearchData("Company")
   
    
    $('#locationDropdown').select2();
    $('#projectDropdown').select2();
    $('#towerDropdown').select2();
   
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

$('#chkAllTower').change(function () {
    if (this.checked) {
        $('#towerDropdown').empty();
        $('#towerDropdown').prop('disabled', true);
    }
    else {
        $('#towerDropdown').prop('disabled', false);
        SearchData('Tower');
    }
});
function ProjectConfigureList() {
    counter = 0;
    dataTable = $("#projectUser").DataTable({

        ajax: {
            'type': 'GET',
            'url': '/api/AuthController/userList',
            'contentType': 'application/json',
            data: {
                type: 'UBMUser'
            }
        },
        columns: [
            {
                'data': 'userId',
                'render': function (data, type, row, meta) {
                    return `<input type='checkbox' class='userCheckbox' data-userid='${row.userId}' />`;
                },
                'width': '2%'
            },
            {
                'data': 'userId', 'render': function (data, type, row) {
                    counter++; // Increment counter
                    return `<span>${counter}</span>`;
                },
                'width': '5%', 'font-size': '6px'
            },
            {
                'data': 'username', 'render': function (data, type, row) {
                    return `<span>${row.username}</span>`;
                },
                'width': '15%', 'font-size': '6px'
            },
            {
                'data': 'roleName', 'render': function (data, type, row) {
                    return `<span>${row.roleName}</span>`;
                },
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'userId',
                'render': function (data, type, row) {
                    return `<button data-toggle="modal" onclick=ViewProjectPermission("${row.userId}","${row.username}")  href="" data-backdrop="static" data-keyboard="false"  data-target="#addProjectmodal" class="btn btn-primary">View</button>`
                    
                },
                'width': '15%',
                'font-size': '6px'
            },

        ],

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

    }
    );

    $('#projectUser').on('change', '.userCheckbox', function () {
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

    /* dataTable.on('order.dt ', function () {
         dataTable.column(0, { order: 'applied' }).nodes().each(function (cell, i) {
             cell.innerHTML = i + 1;
         });
     }).draw();*/
}

function SearchData(type) {
  
  
    var dataToSend = {
        type: type,
        projectAllData: {}
    };
    if (type == "Company") {
        dataToSend.projectAllData.company = [];
    } else if (type == "Location") {
        var selectedCompanies = $('#companyDropdown').val();
        if (selectedCompanies) {
            dataToSend.projectAllData.company = [{ companyID: selectedCompanies }];
            /*  dataToSend.projectAllData.company = { companyID: selectedCompanies };*/

            /* dataToSend.projectAllData.company = selectedCompanies.map(function (companyID) {
                  return { companyID: companyID };
              });*/
        }
    }
    else if (type == "Project") {
         
        var selectedCompanies = $('#companyDropdown').val();
        if (selectedCompanies) {
            dataToSend.projectAllData.company = [{ companyID: selectedCompanies }];
            /*  dataToSend.projectAllData.company = { companyID: selectedCompanies };*/

            /* dataToSend.projectAllData.company = selectedCompanies.map(function (companyID) {
                  return { companyID: companyID };
              });*/
        }
        var selectedLocation = $('#locationDropdown').val();
        if (selectedLocation) {
            dataToSend.projectAllData.location = [{ locationID: selectedLocation }];
        }
        else {
            return;
        }
    }
    else if (type == "Tower") {
        var selectedCompanies = $('#companyDropdown').val();
        if (selectedCompanies) {
            dataToSend.projectAllData.company = [{ companyID: selectedCompanies }];
            /*  dataToSend.projectAllData.company = { companyID: selectedCompanies };*/

            /* dataToSend.projectAllData.company = selectedCompanies.map(function (companyID) {
                  return { companyID: companyID };
              });*/
        }
        var selectedLocation = $('#locationDropdown').val();
        if (selectedLocation) {
            dataToSend.projectAllData.location = [{ locationID: selectedLocation }];
        }
        else {
            return;
        }
        var selectedProject = $('#projectDropdown').val();
        if (selectedProject) {
            dataToSend.projectAllData.project = [{ projectID: selectedProject }];
        }
        else {
            return;
        }
    }

   
    $.ajax({
        url: '/api/projectController/SearchData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),
        success: function (data) {
            if (data.success) {
             
              
                if (type == "Company") {
                    $('#projectDropdown').empty();
                    $('#towerDropdown').empty();
                    $('#locationDropdown').empty();
                    fillcompanyDropdown(data.data);
                } else if (type == "Location") {
                    $('#projectDropdown').empty();
                    $('#towerDropdown').empty();
                    fillLocationDropdown(data.data);
                }
                else if (type == "Project") { 
                    $('#towerDropdown').empty();
                    fillProjectDropdown(data.data)
                }
                else if (type == "Tower") {
                    fillTowerDropdown(data.data)
                }
            }
            else {
                ErrorMessage(data.message)
               
            }
        }
    })
}


// BIND COMPANY DROPDOWN DATA

function fillcompanyDropdown(results) {
    var $companyDropdown = $('#companyDropdown');
    $companyDropdown.empty();

    var defaultOption = new Option('Select Location', '', false, false);
    $companyDropdown.append(defaultOption);
    results.forEach(function (item) {
        if (item && item.company && Array.isArray(item.company) && item.company.length > 0) {
            item.company.forEach(function (company) {
                if (company.companyName && company.companyID) {
                    var option = new Option(company.companyName, company.companyID, false, false);
                    $companyDropdown.append(option);
                }
            });
        }
    });
    if (!$companyDropdown.data('select2')) {
        $companyDropdown.select2({
            closeOnSelect: true,
            placeholder: "Select Company"
        });
    } else {
        $companyDropdown.trigger('change.select2');
    }
    $('#locationDropdown').append('<option value="">Select Location</option>');
    $('#projectDropdown').append('<option value="">Select Project</option>');
}





// BIND LOCATION DROPDOWN DATA

function fillLocationDropdown(results) {
    var $locationDropdown = $('#locationDropdown');
    $locationDropdown.empty();

    var defaultOption = new Option('Select Location', '', false, false);
    $locationDropdown.append(defaultOption);
    results.forEach(function (item) {
        if (item && item.location && Array.isArray(item.location) && item.location.length > 0) {
            item.location.forEach(function (location) {
                if (location.locationName && location.locationID) {
                    var option = new Option(location.locationName, location.locationID, false, false);
                    $locationDropdown.append(option);
                }
            });
        }
    });
    if (!$locationDropdown.data('select2')) {
        $locationDropdown.select2({
            closeOnSelect: true,
            placeholder: "Select Location"
        });
    } else {
        $locationDropdown.trigger('change.select2');
    }
    $('#projectDropdown').append('<option value="">Select Project</option>');
}


// BIND PROJECT DROPDOWN DATA

function fillProjectDropdown(results) {
            debugger
    var $projectDropdown = $('#projectDropdown');
    $projectDropdown.empty();

    var defaultOption = new Option('Select Project', '', false, false);
    $projectDropdown.append(defaultOption);
    results.forEach(function (item) {
        if (item && item.project && Array.isArray(item.project) && item.project.length > 0) {
            item.project.forEach(function (project) {
                if (project.projectName && project.projectID) {
                    var option = new Option(project.projectName, project.projectID, false, false);
                    $projectDropdown.append(option);
                }
            });
        }
    });
    if (!$projectDropdown.data('select2')) {
        $projectDropdown.select2({
            closeOnSelect: true,
            placeholder: "Select Project"
        });
    } else {
        $projectDropdown.select2({
            closeOnSelect: true,
            placeholder: "Select Project"
        });
        $projectDropdown.trigger('change.select2');
    }
}


// BIND TOWER DROPDOWN DATA

function fillTowerDropdown(results) {
    var $towerDropdown = $('#towerDropdown');
    $towerDropdown.empty();

    var defaultOption = new Option('Select Tower', '', false, false);
    $towerDropdown.append(defaultOption);
    results.forEach(function (item) {
        if (item && item.tower && Array.isArray(item.tower) && item.tower.length > 0) {
            item.tower.forEach(function (tower) {
                if (tower.towerName && tower.towerID) {
                    var option = new Option(tower.towerName, tower.towerID, false, false);
                    $towerDropdown.append(option);
                } 
            });
        } 
    });
    if (!$towerDropdown.data('select2')) {
        $towerDropdown.select2({
            closeOnSelect: true,
            placeholder: "Select Tower"
        });
    } else {
        $towerDropdown.trigger('change.select2'); 
    }
}


function SaveProjectPermission() {

    console.log(selectedUserIds);

    var dataToSend = {
        useridList: selectedUserIds,
        companyID: document.getElementById("companyDropdown").value,
        locationID: document.getElementById("locationDropdown").value,
        projectID: document.getElementById("projectDropdown").value,
        towerList: $('#towerDropdown').val()

    };

    if (selectedUserIds.length == 0) {
        ErrorMessage("Kindly Select Users in CheckBoxes!")
        return;
    }
    else if (document.getElementById("companyDropdown").value == "") {
        ErrorMessage("Kindly Select Company!");
        return;
    }
    else if (document.getElementById("locationDropdown").value == "") {
        ErrorMessage("Kindly Select Location!");
        return;
    }
    else if (document.getElementById("projectDropdown").value == "") {
        ErrorMessage("Kindly Select Project!");
        return;
    }


    $('#empLoader').show();
    $.ajax({
        url: '/api/projectController/addProjectPermission',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend),
        success: function (data) {
            if (data.success) {
                SuccessMessage(data.message);
                counter = 0;
                $('#projectUser').DataTable().ajax.reload();
                while (selectedUserIds.length > 0) {
                    selectedUserIds.pop();
                }
                $('#companyDropdown').empty();
                $('#locationDropdown').empty();
                $('#projectDropdown').empty();
                $('#towerDropdown').empty();
                window.location.reload();
               // SearchData("Company")
            }
            else {
                ErrorMessage(data.message)
            }
            $('#empLoader').hide();
        }
    })

}




function ViewProjectPermission(userID, userName) {
    document.getElementById("SelecteduserName").innerHTML = userName

    dataTable = $("#viewProjectPermissionTable").DataTable({
        ajax: {
            'type': 'GET',
            'url': '/api/projectController/projectPermissionlist',
            'contentType': 'application/json',
            data: {
                userID: userID
            }
        },
        columns: [
            {
                'data': 'companyName', 
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'locationName', 
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'projectName',
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'towerName', 'render': function (data, type, row) {
                    
                    if (row.towerName != "") {
                        return `<span>${row.towerName}</span>`;
                        
                    }
                    else {
                        return `<span>All Tower</span>`;
                    }
                    
                },
                'width': '10%', 'font-size': '6px'
            },
            {
                'data': 'userID',
                'render': function (data, type, row) {
                    return `<button onclick=deletePermission("${row.projectPermissionID}")   class="btn btn-danger">Delete</button>`
                },
                'width': '15%',
                'font-size': '6px'
            },

        ],

        "font- size": '1em',
        dom: 'lBfrtip',
        "bDestroy": true,

        language: {
            searchPlaceholder: "Search records",
            emptyTable: "No data found",
            width: '100%',
        },

    }
    );
}

function deletePermission(permissionID) {

    Swal.fire({
        title: 'Are you sure Delete this Permission?',
        text: "",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#empLoader').show();
            $.ajax({
                type: 'GET',
                url: '/api/projectController/deletePermission',
                data: {
                    permissionID: permissionID
                },
                success: function (data) {
                    if (data.success == true) {
                        SuccessMessage(data.message);
                        $('#viewProjectPermissionTable').DataTable().ajax.reload();
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
