$(document).ready(function () {
    GetTemplates();
    BindDropdown();
    
    $("#divTemplateType").show();
    $('#ddlTemplateType').select2();
    // $('#ddlProcessType').select2();
    $('#ddlAppType').select2();
});

function BindDropdown() {
    clear();
    ProcessTypeList();
    GetProject();

}
function GetProject() {
    $('#ddlProject').empty();
    $.ajax({
        url: '/api/TemplateController/GetProjects',
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {
            if (res.success) {

                $('#ddlProject').append('<option> Select </option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlProject').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                }
                $('#ddlProject').select2();
            }
            else {
                ErrorMessage(res.data)

            }
        }
    })
}

function ProcessTypeList() {


    $('#empLoader').show();
    $.ajax({
        url: '/api/TemplateController/ProcessTypeList',
        type: 'GET',
        contentType: 'application/json',
        success: function (res) {

            $('#empLoader').hide();
            if (res.success) {
                $('#ddlProcessType').empty();
                $('#ddlProcessType').append('<option value="0"> Select </option>');
                for (i = 0; i < res.data.length; i++) {
                    $('#ddlProcessType').append('<option value=' + res.data[i].key + '>' + res.data[i].value + '</option>')
                }
                $('#ddlProcessType').select2();
            }
            else {
                ErrorMessage(res.data)
            }
        }
    });
}
function changeProcessType(Process) {
     
    if (Process == '')
        return
    var ProcessType = Process == 1 ? 'Initiate Booking' : Process == 2 ? 'Final Booking' : 'T&C';
    $("#ddlEmbnamelist").empty();
    if (ProcessType == 'Initiate Booking') {
       
        $("#divTemplateType").show();
        $("#divEmbededField").show();
    }
    else if (ProcessType == 'Final Booking') {
       
        $("#divTemplateType").show();
        $("#divEmbededField").show();
    }
    else if (ProcessType == 'T&C') {
        $("#divTemplateType").hide();
        
        $("#divEmbededField").hide();
    }
    else {
       
        $("#divTemplateType").show();
        $("#divEmbededField").show();

    }


    if (ProcessType == 'Final Booking' || ProcessType == 'Initiate Booking') {


        $.ajax({
            url: '/api/TemplateController/GetTemplateEmbList?ProcessType=' + ProcessType,
            type: 'GET',
            contentType: 'application/json',
            success: function (res) {
                $('#ddlEmbnamelist').empty();
                if (res.success) {

                    for (i = 0; i < res.data.length; i++) {
                        $('#ddlEmbnamelist').append('<option value=' + res.data[i].key + '> ' + res.data[i].value + ' </option>')
                    }
                }
                else {
                    ErrorMessage(res.data);
                }
            }
        });
    }
}


//$("#ddlAppType").change(function () {

//    var e = document.getElementById("ddlAppType")
//    var apptype = e.options[e.selectedIndex].value;
//    $.ajax({
//        url: '/api/TemplateController/GetAppDocList',
//        type: 'GET',
//        contentType: 'application/json',
//        data: {
//            ApplicationType: apptype
//        },
//        success: function (res) {
//            if (res.success) {
//                //  Editor.data.set(res.data);

//                Editor.model.change(writer => {
//                    var html = res.data;
//                    var viewFragment = Editor.data.processor.toView(html);
//                    var modelFragment = Editor.data.toModel(viewFragment);

//                    var insertPosition = Editor.model.document.selection.getFirstPosition();

//                    Editor.model.insertContent(modelFragment, insertPosition);
//                });
//            }
//            else {
//                ErrorMessage(res.message);
//            }
//        }
//    });
//});

function chooseEmbeded(sel) {
    var text = sel.options[sel.selectedIndex].text;
    var templateType = $("#ddlTemplateType").val();
    if (templateType == "SMS") {
        var cursorPos = $('#txteditor').prop('selectionStart');
        var v = $('#txteditor').val();
        var textBefore = v.substring(0, cursorPos);
        var textAfter = v.substring(cursorPos, v.length);
        $('#txteditor').val(textBefore + text + textAfter);
    }
    else {
        Editor.model.change(writer => {
            var html = text;
            var viewFragment = Editor.data.processor.toView(html);
            var modelFragment = Editor.data.toModel(viewFragment);

            var insertPosition = Editor.model.document.selection.getFirstPosition();

            Editor.model.insertContent(modelFragment, insertPosition);
        });
    }

}
function changeTemplateType(val) {

    if (Editor !== null)
        Editor.data.set('');
  
    if (val == 'SMS') {
        Editor.destroy()
        Editor = null;
        $("#divVendorTemplateId").show();
    } 
    else {
        if (val == 'WhatsApp') {
            $("#divVendorTemplateId").show();
        }
        else {
            $("#divVendorTemplateId").hide();
        }
     
        if (Editor == null) {
            ClassicEditor
                .create(document.querySelector('#txteditor'))
                .then(editor => {
                    // editor.data.set(data.data[0]['documentName']);
                    Editor = editor;

                })
                .catch(error => {
                    console.error(error);
                });
        }
    }
} 
function SaveTemplate() {
    
    var templateId = $("#hdnTemplateId").val() == '' ? '0' : $("#hdnTemplateId").val();
    var processType = $("#ddlProcessType option:selected").text(); //$("#ddlProcessType").val();
    var projectId = $("#ddlProject").val() == 'Select' || $("#ddlProject").val() == null ? '0' : $("#ddlProject").val();
    var templateType = $("#ddlTemplateType").val();
    var vendorTemplateId = $("#txtVendorTemplateId").val();
    var template;
    if (templateType == "SMS") {
        template = $("#txteditor").val();
    }
    else {
        template = Editor.getData();
    }
    if (processType == '0') {
        ErrorMessage('Please select process type!');
        return
    }
    if (projectId == 0) {
        ErrorMessage('Please select process type!');
        return
    }
    if (processType !== 'T&C') {
        if (templateType == '') {
            ErrorMessage('Please select template type!');
            return
        }
    }
    if (template == '') {
        ErrorMessage('Please create template!');
        return
    }
    var formdata = {
        TemplateID: templateId,
        ProcessType: processType,
        TemplateType: templateType,
        ProjectID: projectId,
        TemplateMsg: template,
        VendorTemplateID: vendorTemplateId
    };
    
    $('#empLoader').show();
    $.ajax({
        url: '/api/TemplateController/SaveTemplate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formdata),
        success: function (res) {
            $('#empLoader').hide();
            if (res.success) {
                GetTemplates();
                SuccessMessage(res.message);
                $("#hdnTemplateId").val(0);
                $('#addTemplate').modal('hide');
                $('.modal-backdrop').remove();
                clear();
            }
            else {
                $('#empLoader').hide();
                ErrorMessage(res.message);
            }
        }
    })
}
function clear() {
    
    $("#hdnTemplateId").val(0);
    $("#ddlProject").val(0).trigger('change');;
    $("#ddlTemplateType").val('Select').trigger('change');;
    Editor.data.set('');
    $("#ddlEmbnamelist").empty();
    $("#txtVendorTemplateId").val();
}
function GetTemplates() {
    $('#empLoader').show();
    $.ajax({

        'type': 'GET',
        'url': '/api/TemplateController/GetTemplateList',
        'contentType': 'application/json',
        dataType: 'json',
        success: function (response) {

            $('#empLoader').hide();
            if (response.success == false) {
                ErrorMessage(response.data);
            }

            else {


                var html = "<table id='TempList' class='table table-bordered table-hover' style='width:100%'>";
                html += "<thead>";
                html += "<tr>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>S. no</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Process</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Project</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Type</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Message</th>";
                html += "<th style='font-family: 'sans-serif'; font-weight: normal;'>Actions</th>";
                html += "</tr>";
                html += "</thead>";
                var i = 1;
                if (response.data.length > 0) {
                    response.data.forEach(function (entry) {
                        html += "<tr>";
                        html += "<td>" + i++ + "</td>";
                        html += "<td>" + entry.processType + "</td>";
                        html += "<td>" + entry.projectName + "</td>";
                        html += "<td>" + entry.templateType + "</td>";
                        html += "<td><button type='button' data-toggle='modal' href='' data-target='#vTemplate'  onclick ='ViewTemplate(" + JSON.stringify(entry.templateMsg) + ")'  class='btn btn-info' style='float:right'>View Template</button></td>";
                        html += "<td><button data-row=" + entry + " data-toggle='modal' href='' data-target='#addTemplate' onclick ='clear();EditTemplate(" + JSON.stringify(entry) + ")' type = 'button' class='btn btn-primary'> <i class='fa fa-edit'></i>&nbsp;Edit</button>&nbsp;";
                        html += "<button  data-row=" + JSON.stringify(entry.templateID) + " onclick='DeleteTemplate(" + JSON.stringify(entry) + ")' type='button' class='btn btn-danger'><i class='fa fa-bitbucket'></i>&nbsp;Delete</button>";
                        html += "</td>";
                        html += "</tr>";
                    });
                }


                html += "</table>";
                document.getElementById("divTempList").innerHTML = html;

                dataTable = $("#TempList").DataTable({
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
function EditTemplate(rowData) {
    
    // $("#ddlTemplateType").val(rowData['templateID']).trigger('change');    
    $("#hdnTemplateId").val(rowData["templateID"]);
    $("#ddlProject").val(rowData['projectID']).trigger('change');
    $("#ddlTemplateType").val(rowData['templateType']).trigger('change');
   var processtypeid = rowData['processType'] == 'Initiate Booking' ? 1 : rowData['processType'] == 'Final Booking' ? 2 : 3;

    $("#ddlProcessType").val(processtypeid).trigger('change');
    $("#txtVendorTemplateId").val(rowData['vendorTemplateID']);
    if (rowData['templateType'] == "SMS") {
        $("#txteditor").val(rowData['templateMsg']);
        $("#divVendorTemplateId").show();
        
    }
    else {
        Editor.data.set(rowData['templateMsg']);
        $("#divVendorTemplateId").hide();
        if (rowData['templateType'] == "WhatsApp") { 
            $("#divVendorTemplateId").show();
        }
    } 

}
function ViewTemplate(val) {
    $('#dvTemp').html(val);
}
function DeleteTemplate(rowData) {
    var id = rowData["templateID"];
    Swal.fire({
        title: 'Are you want to delete template?',
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
                url: '/api/TemplateController/DeleteTemplate',
                type: 'GET',
                contentType: 'application/json',
                data: {
                    id: id
                },
                success: function (data) {
                    $('#empLoader').hide();
                    if (data.success) {
                        SuccessMessage(data.message);
                        GetTemplates();
                    }
                    else {
                        ErrorMessage(data.message);
                    }
                }
            })
        };
    })
}


