(function ($) {
    var selectedRows = new Array();

    $.fn.selectableGrid = function (options) {
        var settings = $.extend({}, $.fn.selectableGrid.defaults, options);

        $(settings.deleteItemID).attr('disabled', 'disabled');
        $(settings.checkAllID).prop("checked", false);
        selectedRows = new Array();

        $(settings.checkAllID).click(function () {
            var status = $(this).prop("checked");
            $(settings.itemCheckBoxClass).prop("checked", status);
            var idArray = Array();
            $(settings.itemCheckBoxClass).each(function (index, item) {
                idArray.push($(item).val());
            });
            onAllSelected(idArray, status);
        });

        $(this).on('click', settings.itemCheckBoxClass, function (event) {
            var id = $(this).val();
            var status = $(this).prop('checked');
            onRowSelected(id, status);
        });

        $(this).on('click', '.delete', function (event) {
            onRowSelected($(this).attr('data-id'), true);
            $(settings.deleteModalID).modal('show');
            $(settings.deleteModalID + " .confirm").unbind();
            $(settings.deleteModalID + " .confirm").click(function (event) {
                createDeleteFormAndSubmit();
            });
        });

        $(settings.deleteItemID).click(function () {
            $(settings.deleteModalID).modal('show');
            $(settings.deleteModalID + " .confirm").unbind();
            $(settings.deleteModalID + " .confirm").click(function (event) {
                createDeleteFormAndSubmit();
            });
        });

        return this;
    };

    $.fn.selectableGrid.defaults = {
        deleteItemID: "#deleteitems",
        checkAllID: "#checkall",
        itemCheckBoxClass: ".itemcheckbox",
        deleteModalID: "#deleteModal"
    };

    function onAllSelected(rowIDs, status) {
        if (status) {
            selectedRows.addUnique(rowIDs);
            $($.fn.selectableGrid.defaults.deleteItemID).removeAttr("disabled");
        } else {
            selectedRows = new Array();
            $($.fn.selectableGrid.defaults.deleteItemID).attr('disabled', 'disabled');
        }
    };

    function onRowSelected(rowid, status) {
        if (status) {
            var found = false;
            for (var i = 0; i < selectedRows.length; i++) {
                if (selectedRows[i] == rowid) {
                    found = true;
                    break;
                }
            }
            if (!found)
                selectedRows.push(rowid);
        } else {
            var newItems = Array();
            for (var i = 0; i < selectedRows.length; i++) {
                if (selectedRows[i] != rowid) {
                    newItems.push(selectedRows[i]);
                }
            }
            selectedRows = newItems;
        }

        if (selectedRows.length > 0)
            $($.fn.selectableGrid.defaults.deleteItemID).removeAttr("disabled");
        else
            $($.fn.selectableGrid.defaults.deleteItemID).attr('disabled', 'disabled');

        if ($($.fn.selectableGrid.defaults.itemCheckBoxClass).length == selectedRows.length)
            $($.fn.selectableGrid.defaults.checkAllID).prop("checked", true);
        else
            $($.fn.selectableGrid.defaults.checkAllID).prop("checked", false);

        return selectedRows;
    };

    function createDeleteFormAndSubmit() {
        var postUrl = $($.fn.selectableGrid.defaults.deleteItemID).attr("data");
        var newform = document.createElement("form");
        newform.setAttribute('method', "post");
        newform.setAttribute('action', postUrl);

        for (var i = 0; i < selectedRows.length; i++) {
            var item = document.createElement("input"); //input element, text
            item.setAttribute('type', "hidden");
            item.setAttribute('name', "ids");
            item.setAttribute('value', selectedRows[i]);
            newform.appendChild(item);
        }

        document.getElementsByTagName('body')[0].appendChild(newform);
        newform.submit();
    }
}(jQuery));

/***********************************************************************************
            End of Grid delete selection plugin
***********************************************************************************/


new function ($) {
    //SET CURSOR POSITION
    $.fn.setCursorPosition = function (pos) {
        this.each(function (index, elem) {
            if (elem.setSelectionRange) {
                elem.setSelectionRange(pos, pos);
            } else if (elem.createTextRange) {
                var range = elem.createTextRange();
                range.collapse(true);
                range.moveEnd('character', pos);
                range.moveStart('character', pos);
                range.select();
            }
        });
        return this;
    };

    $.fn.getCursorPosition = function () {
        var pos = 0;
        var el = $(this).get(0);
        // IE Support
        if (document.selection) {
            el.focus();
            var Sel = document.selection.createRange();
            var SelLength = document.selection.createRange().text.length;
            Sel.moveStart('character', -el.value.length);
            pos = Sel.text.length - SelLength;
        }
            // Firefox support
        else if (el.selectionStart || el.selectionStart == '0')
            pos = el.selectionStart;
        return pos;
    }
}(jQuery);


/***********************************************************************************
            End of cursor position plugin
***********************************************************************************/

function OnBeforeLoad(selectedRows) {
    $("#deleteitems").attr('disabled', 'disabled');
    $("#checkall").prop("checked", false);
    selectedRows = new Array();
}

function OnAllRowSelected(selectedRows, aRowids, status) {
    if (status) {
        selectedRows.addUnique(aRowids);
        $("#deleteitems").removeAttr("disabled");
    } else {
        selectedRows = new Array();
        $("#deleteitems").attr('disabled', 'disabled');
    }
}

function OnRowSelected(selectedRows, rowid, status) {
    console.log(selectedRows.length)
    if (status) {
        var found = false;
        for (var i = 0; i < selectedRows.length; i++) {
            if (selectedRows[i] == rowid) {
                found = true;
                break;
            }
        }
        if (!found)
            selectedRows.push(rowid);
    } else {
        var newItems = Array();
        for (var i = 0; i < selectedRows.length; i++) {
            if (selectedRows[i] != rowid) {
                newItems.push(selectedRows[i]);
            }
        }
        selectedRows = newItems;
    }

    if (selectedRows.length > 0)
        $("#deleteitems").removeAttr("disabled");
    else
        $("#deleteitems").attr('disabled', 'disabled');

    if ($(".itemcheckbox").length == selectedRows.length)
        $("#checkall").prop("checked", true);
    else
        $("#checkall").prop("checked", false);

    return selectedRows;
}

function createDeleteFormAndSubmit(selectedRows, fieldName, postUrl) {
    var newform = document.createElement("form");
    newform.setAttribute('method', "post");
    newform.setAttribute('action', postUrl);

    for (var i = 0; i < selectedRows.length; i++) {
        var item = document.createElement("input"); //input element, text
        item.setAttribute('type', "hidden");
        item.setAttribute('name', fieldName);
        item.setAttribute('value', selectedRows[i]);
        newform.appendChild(item);
    }

    document.getElementsByTagName('body')[0].appendChild(newform);
    newform.submit();
}

function createFormAndSubmit(fieldName, fieldValue, postUrl) {
    var newform = document.createElement("form");
    newform.setAttribute('method', "post");
    newform.setAttribute('action', postUrl);

    var item = document.createElement("input"); //input element, text
    item.setAttribute('type', "hidden");
    item.setAttribute('name', fieldName);
    item.setAttribute('value', fieldValue);
    newform.appendChild(item);

    document.getElementsByTagName('body')[0].appendChild(newform);
    newform.submit();
}

/**********************************************************************
Helper method to show modal popup in shortcut 
**********************************************************************/
function ShowModal(initiatorID, modalID, actionUrl, loaderImageUrl, headerText, cancelButtonText, actionButtonText, actionButtonAction) {
    $("#" + initiatorID).click(function (e) {
        e.preventDefault();
        $('#' + modalID + ' .modal-header h3').html(headerText);
        $('#' + modalID + ' .modal-footer .btn').html(cancelButtonText);
        $('#' + modalID + ' .modal-footer .btn').show();
        $('#' + modalID + ' .modal-footer .btn-primary').html(actionButtonText);
        $('#' + modalID + ' .modal-footer .btn-primary').show();
        
        $.ajax({
            url: actionUrl,
            beforeSend: function (xhr) {
                $('#' + modalID + ' .modal-body').html("<img style='margin:30px 45%;' src='" + loaderImageUrl + "' alt='loader' />");
            },
            cache: false
        }).done(function (html) {
            $('#' + modalID +' .modal-body').html(html);
        });
        $('#' + modalID + ' .modal-footer .btn-primary').unbind();
        $('#' + modalID + ' .modal-footer .btn-primary').click(actionButtonAction);
        $('#' + modalID).modal('show');
    });
}

function ShowModal2(modalID, actionUrl, loaderImageUrl, headerText, cancelButtonText, actionButtonText, actionButtonAction) {
    $('#' + modalID + ' .modal-header h3').html(headerText);
    $('#' + modalID + ' .modal-footer .btn').html(cancelButtonText);
    $('#' + modalID + ' .modal-footer .btn').show();
    $('#' + modalID + ' .modal-footer .btn-primary').html(actionButtonText);
    $('#' + modalID + ' .modal-footer .btn-primary').show();

    $.ajax({
        url: actionUrl,
        beforeSend: function (xhr) {
            $('#' + modalID + ' .modal-body').html("<img style='margin:30px 45%;' src='" + loaderImageUrl + "' alt='loader' />");
        },
        cache: false
    }).done(function (html) {
        $('#' + modalID + ' .modal-body').html(html);
    });
    $('#' + modalID + ' .modal-footer .btn-primary').unbind();
    $('#' + modalID + ' .modal-footer .btn-primary').click(actionButtonAction);
    $('#' + modalID).modal('show');
}

function ModalAction(modalID, gridID, callUrl, jsonData, loaderImageUrl) {
    var button = $(this);
    
    $.ajax({
        url: callUrl,
        type: "POST",
        dataType: "json",
        timeout: 10000,
        contentType: "application/json",
        data: JSON.stringify(jsonData),
        beforeSend: function (xhr) {
            $('#' + modalID + ' .modal-body').html("<img style='margin:30px 45%;' src='" + loaderImageUrl + "' alt='loader' />");
            $('#' + modalID + ' .modal-footer .btn').hide();
            $('#' + modalID + ' .modal-footer .btn-primary').hide();
        },
        cache: false
    })
    .done(function (data) {
        $('#' + gridID).trigger('reloadGrid');
        $(document).trigger("clear-alerts");
        if (data.status) {
            if (data.status == "fail") {
                $(document).trigger("add-alerts", [
                    {
                        'message': data.message,
                        'priority': 'error'
                    }
                ]);
            } else if (data.status == "success") {
                $(document).trigger("add-alerts", [
                    {
                        'message': data.message,
                        'priority': 'success'
                    }
                ]);
            }
            else {
                $(document).trigger("add-alerts", [
                    {
                        'message': data.message,
                        'priority': 'error'
                    }
                ]);
            }
        }
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        $(document).trigger("clear-alerts");
        $(document).trigger("add-alerts", [
            {
                'message': commonError,
                'priority': 'error'
            }
        ]);
    })
    .always(function () {
        $('#' + modalID).modal('hide');
    });
}

function AjaxCall(callUrl, callType, containerID) {
    $.ajax({
        url: callUrl,
        type: callType,
        cache: false
    })
    .done(function (data) {
        $("#" + containerID).html(data);
    })
    .fail(function () {
        $(document).trigger("clear-alerts");
        $(document).trigger("add-alerts", [
            {
                'message': commonError,
                'priority': 'error'
            }
        ]);
    })
    .always(function () {
    });
}

function AjaxCallWithData(callUrl, callType, jsonData) {
    $.ajax({
        url: callUrl,
        type: callType,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jsonData),
        cache: false
    })
    .done(function (data) {
        $(document).trigger("clear-alerts");
        var type = "success";
        if (data.status != "success") {
            type = "error";
        }

        $(document).trigger("add-alerts", [
            {
                'message': data.message,
                'priority': type
            }
        ]);
    })
    .fail(function () {
        $(document).trigger("clear-alerts");
        $(document).trigger("add-alerts", [
            {
                'message': "There was a problem, please try again",
                'priority': 'error'
            }
        ]);
    })
    .always(function () {
    });
}

function AjaxCallWithDataAndCallback(callUrl, callType, jsonData, beforeBeginCallback, successCallback, errorCallback, alwaysCallback) {
    $.ajax({
        url: callUrl,
        type: callType,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(jsonData),
        beforeSend: beforeBeginCallback,
        cache: false
    })
    .done(function (data) {
        $(document).trigger("clear-alerts");
        var type = "success";
        if (data.status != "success") {
            type = "error";
        }

        $(document).trigger("add-alerts", [
            {
                'message': data.message,
                'priority': type
            }
        ]);

        successCallback(data);
    })
    .fail(function () {
        $(document).trigger("clear-alerts");
        $(document).trigger("add-alerts", [
            {
                'message': "There was a problem, please try again",
                'priority': 'error'
            }
        ]);

        errorCallback();
    })
    .always(function () {
        alwaysCallback();
    });
}

function AddGridCommonButtons(gridID, loaderImageUrl, addUrl, deleteUrl, dataCallback) {
    $('#t_' + gridID)
        .append('<button id="newitem" class="btn btn-primary"><i class="icon-plus"></i> ' + gridNewButtonText + '</button>') // gridNewButtonText comes from i18n/framework/translator{*}.js file
        .append('&nbsp;<button id="deleteitems" class="btn btn-primary" disabled="disabled"><i class="icon-trash"></i> ' + gridDeleteButtonText + '</button>'); // gridDeleteButtonText comes from i18n/framework/translator{*}.js file

    // attaching click event to the "+New" button
    ShowModal("newitem", "rootModal", addUrl,
        loaderImageUrl, addNewItemText, cancelText, saveText, function () {
        // If validation pass, then we process the request
        if ($("#actionForm").validationEngine('validate')) {
            // binding an action with the modal button and refreshing the grid
            ModalAction("rootModal", gridID, addUrl, dataCallback(), loaderImageUrl);
        }
    });
        
    // attaching click event to the "+Delete" button
    ShowModal("deleteitems", "rootModal", deleteUrl,
        loaderImageUrl, deleteConfirmationText, cancelText, confirmText, function () {
            // collecting data entered in the form fields
            var deleteJsonData = {
            __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val(),
            IDs: selectedRows
        };
        // binding an action with the modal button and refreshing the grid
        ModalAction("rootModal", gridID, deleteUrl, deleteJsonData, loaderImageUrl);
    });

    // Attaching edit button click event
    $('#' + gridID).on('click', 'a[action=edit]', function (event) {
        event.preventDefault();
        var url = $(this).attr("href");

        // attaching click event to the "+New" button. modalTitleForEditText,cancelText,saveText
        // comes from i18n/framework/translator{*}.js file
        ShowModal2("rootModal", url, loaderImageUrl, modalTitleForEditText, cancelText, saveText, function () {
            // If validation pass, then we process the request
            if ($("#actionForm").validationEngine('validate')) {
                // binding an action with the modal button and refreshing the grid
                ModalAction("rootModal", gridID, url, dataCallback(), loaderImageUrl);
            }
        });

        return false;
    });

    // Attaching delete button click event
    $('#' + gridID).on('click', 'a[action=delete]', function (event) {
        event.preventDefault();
        var url = $(this).attr("href");

        // attaching click event to the "Delete" button. deleteConfirmationText, cancelText, confirmText
        // comes from i18n/framework/translator{*}.js file
        ShowModal2("rootModal", url, loaderImageUrl, deleteConfirmationText, cancelText, confirmText, function () {
            var deleteJsonData = {
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            };

            // binding an action with the modal button and refreshing the grid
            ModalAction("rootModal", gridID, url, deleteJsonData, loaderImageUrl);
        });

        return false;
    });

    // Attaching validators
    $("#actionForm").validationEngine('init', { promptPosition: "centerRight", scroll: false });
    $("#actionForm").validationEngine('attach');
}