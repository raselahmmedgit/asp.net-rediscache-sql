
var dataTablePatientProfile;

var PatientProfile = function () {

    var htmlTemplateAction = function (data) {
        if (data) {
            var html = "<button data-id='" + data + "' title='Details' class='btn btn-sm btn-primary ml-10 btnDetailPatientProfile'>Details</button>";
            html += "<button data-id='" + data + "' title='Edit' class='btn btn-sm btn-secondary ml-10 btnEditPatientProfile'>Edit</button>";
            html += "<button data-id='" + data + "' title='Delete' class='btn btn-sm btn-danger ml-10 btnDeletePatientProfile'>Delete</button>";
            return html;
        }
    };

    var htmlTemplateData = function (row) {
        if (row) {
            var html = "<div class='row dataTablesCard'>";
            if (row.firstName === null) {
                row.firstName = "";
            }
            html += "<div class='col-6'>";

            html += "<div class='row mb-1'>";
            html += "<div class='col-4'><strong>First Name: </strong></div>";
            html += "<div class='col-8'>" + row.firstName + "</div>";
            html += "</div>";

            html += "</div>";

            html += "<div class='col-6'>";
            
            html += "<div class='row mb-1'>";
            html += "<div class='col-4'><strong>LastName: </strong></div>";
            html += "<div class='col-8'>" + row.lastName + "</div>";
            html += "</div>";

            html += "</div>";

            html += "</div>";

            return html;
        }
    };

    var loadDataTables = function (dataTableId, iDisplayLength, sAjaxSourceUrl) {

        $.fn.dataTable.ext.errMode = () => alert('We are facing some problem while processing the current request. Please try again later.');

        dataTablePatientProfile = $('#' + dataTableId).DataTable({
            "bJQueryUI": true,
            "bAutoWidth": true,
            "sPaginationType": "full_numbers",
            "bPaginate": true,
            "iDisplayLength": iDisplayLength,
            "bSort": false,
            "bFilter": true,
            "bSortClasses": false,
            "lengthChange": false,
            "oLanguage": {
                "sLengthMenu": "Display _MENU_ records per page",
                "sZeroRecords": "Data not found.",
                "sInfo": "Page _START_ to _END_ (Total _TOTAL_)",
                "sInfoEmpty": "Page 0 to 0 (Total 0)",
                "sInfoFiltered": "",
                "sProcessing": "<div class='row'><div class='col-12 dataTables_processing'>Loading...</div></div>"
            },
            "bProcessing": true,
            "bServerSide": true,
            "initComplete": function (settings, json) {
                App.SetDataTableSearch(dataTableId);
            },
            "drawCallback": function (settings) {
            },
            "scrollX": false,
            ajax: {
                url: sAjaxSourceUrl,
                cache: false,
                type: 'GET',
                beforeSend: function () {
                    App.LoaderShow();
                },
                error: function (jqXHR, ajaxOptions, thrownError) {
                    //alert(thrownError + "\r\n" + jqXHR.statusText + "\r\n" + jqXHR.responseText + "\r\n" + ajaxOptions.responseText);
                    var respText = jqXHR.responseText;
                    var messageText = respText;
                    console.log(messageText);
                    App.ToastrNotifierError(messageText);
                    App.LoaderHide();
                },
                complete: function () {
                    App.LoaderHide();
                }
            },
            /*ajax: sAjaxSourceUrl,*/
            columns: [
                {
                    name: 'PatientProfileId',
                    data: 'patientProfileId',
                    title: "PatientProfileId",
                    sortable: false,
                    searchable: false,
                    visible: false
                },
                {
                    name: 'FirstName',
                    data: 'firstName',
                    title: "First Name",
                    sortable: false,
                    searchable: false
                },
                {
                    name: 'LastName',
                    data: 'lastName',
                    title: "Last Name",
                    sortable: false,
                    searchable: false
                }
                ,
                {
                    name: 'EmailAddress',
                    data: 'emailAddress',
                    title: "Email Address",
                    sortable: false,
                    searchable: false
                }
                ,{
                    name: 'PatientProfileId',
                    data: "patientProfileId",
                    title: "Actions",
                    sortable: false,
                    searchable: false,
                    className: "w-200px text-center",
                    "mRender": function (data, type, row) {
                        return htmlTemplateAction(data);
                    }
                }
            ]

        });

    };

    function reloadDataTable() {
        if (dataTablePatientProfile != null && dataTablePatientProfile != null) {
            dataTablePatientProfile.draw()
        }
    };

    var modalPatientProfileBegin = function () {

        App.LoaderShow();
    }

    var modalPatientProfileComplete = function () {

        App.LoaderHide();

    }

    var modalPatientProfileSuccess = function (response) {

        if (response != undefined || response != null) {

            if (response.success == true) {

                App.ToastrNotifierSuccess(response.message);

                //App.AjaxFormReset('FormPatientProfile');

                reloadDataTable();

                //close bootstrap modal
                $('#modalPatientProfileAddOrEdit').modal('hide');
                $("#modalContainer").html('');
            }
            else {

                App.ToastrNotifierError(response.message);
            }
        }
        //check null

        App.LoaderHide();
    }

    var modalPatientProfileFailure = function () {

        App.ToastrNotifierError(appMessage.Error);
    }

    function addPatientProfileModal() {

        $.ajax({
            type: "GET",
            url: "/PatientProfile/AddAjax",
            beforeSend: function () {
                App.LoaderShow();
            },
            success: function (result) {

                //console.log(response);
                $("#modalContainer").html('');
                $("#modalContainer").html(result);
                $('#modalPatientProfile').modal('hide');
                $('#modalPatientProfileAddOrEdit .modal-title').html('PatientProfile Add');
                $('#modalPatientProfileAddOrEdit').modal('show');

                App.LoaderHide();
            },
            error: function (objAjaxRequest, strError) {
                var respText = objAjaxRequest.responseText;
                var messageText = respText;
                console.log(messageText);
                App.ToastrNotifierError(messageText);
                App.LoaderHide();
            },
            complete: function () {
                App.LoaderHide();
            }
        });

    };

    function editPatientProfileModal(id) {

        var id = id != null ? id : 0;
        var dataParam = {
            id: id
        };

        $.ajax({
            type: "GET",
            url: "/PatientProfile/EditAjax",
            data: dataParam,
            beforeSend: function () {
                App.LoaderShow();
            },
            success: function (result) {

                //console.log(response);
                $("#modalContainer").html('');
                $("#modalContainer").html(result);
                $('#modalPatientProfile').modal('hide');
                $('#modalPatientProfileAddOrEdit .modal-title').html('PatientProfile Edit');
                $('#modalPatientProfileAddOrEdit').modal('show');

                App.LoaderHide();
            },
            error: function (objAjaxRequest, strError) {
                var respText = objAjaxRequest.responseText;
                var messageText = respText;
                console.log(messageText);
                App.ToastrNotifierError(messageText);
                App.LoaderHide();
            },
            complete: function () {
                App.LoaderHide();
            }
        });

    };

    function detailPatientProfileModal(id) {

        var id = id != null ? id : 0;
        var dataParam = {
            id: id
        };

        $.ajax({
            type: "GET",
            url: "/PatientProfile/DetailsAjax",
            data: dataParam,
            beforeSend: function () {
                App.LoaderShow();
            },
            success: function (result) {

                //console.log(response);
                $("#modalContainer").html('');
                $("#modalContainer").html(result);
                $('#modalPatientProfileAddOrEdit').modal('hide');
                $('#modalPatientProfile').modal('show');

                App.LoaderHide();
            },
            error: function (objAjaxRequest, strError) {
                var respText = objAjaxRequest.responseText;
                var messageText = respText;
                console.log(messageText);
                App.ToastrNotifierError(messageText);
                App.LoaderHide();
            },
            complete: function () {
                App.LoaderHide();
            }
        });

    };

    var initPatientProfile = function () {

        $('body').on('click', '#btnAddPatientProfile', function () {
            //Open modal dialog for add
            addPatientProfileModal();

            return false;
        });

        $('body').on('click', '.btnEditPatientProfile', function () {
            var id = $(this).attr("data-id");
            //Open modal dialog for edit
            editPatientProfileModal(id);

            return false;
        });

        $('body').on('click', '.btnDetailPatientProfile', function () {
            var id = $(this).attr("data-id");
            //Open modal dialog for details
            detailPatientProfileModal(id);

            return false;
        });

        $('body').on('click', '.btnDeletePatientProfile', function () {
            var id = $(this).attr("data-id");
            if (id != null && confirm('Are you sure you want to delete this item?')) {
                $.ajax({
                    type: "POST",
                    url: '/PatientProfile/DeleteAjax',
                    data: { 'id': id },
                    success: function (data) {
                        if (data.status) {
                            //Refresh DataTable
                            reloadDataTable();
                            $("#modalContainer").html('');
                            $('#modalPatientProfileAddOrEdit').modal('hide');
                            $('#modalPatientProfile').modal('hide');
                        }
                    },
                    error: function () {
                        alert('Failed');
                    }
                })
            }
        });

    };

    return {
        LoadDataTables: loadDataTables,
        ModalPatientProfileBegin: modalPatientProfileBegin,
        ModalPatientProfileComplete: modalPatientProfileComplete,
        ModalPatientProfileSuccess: modalPatientProfileSuccess,
        ModalPatientProfileFailure: modalPatientProfileFailure,
        InitPatientProfile: initPatientProfile
    };
}();