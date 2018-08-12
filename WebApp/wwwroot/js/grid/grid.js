﻿var _childGridId, _childPropertyName, _spacingPatterns, _spacingInserts, _refreshInterval = 10000;
function ClearTitles(grid) {
    var tdElements = document.getElementsByTagName("td");
    for (var i = 0; i < tdElements.length; i++) {
        tdElements[i].title = "";
    }
}
function SetRowIds(rows) {
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (row.id != null) {
            var id = row.id.split("/");
            row.id = id[id.length - 1];
            if (id.length > 12) {
                row.parentEntityName = id[id.length - 3];
            }
            row.id = row.id.replace(/ /g, "_");
            row.id = row.id.replace(/,/g, "");
            row.id = row.id.replace(/\(/g, "");
            row.id = row.id.replace(/\)/g, "");
        }
    }
}
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).jqGrid("getLocalRow", parentRowKey);
    var childRows = parentRow[_childPropertyName];
    var columns = GetChildColumns(_childGridId);
    $("#" + parentRowId).html("<table id='" + _childGridId + "'></table>");
    $("#" + _childGridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: childRows,
        loadComplete: ClearTitles,
        sortname: "name",
        rowNum: 50
    });
}
function LoadGrid(gridId, rows, columns) {
    SetRowIds(rows);
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: OnGridLoad,
        subGrid: _childGridId != null,
        subGridOptions: {
            "openicon": "ui-icon-arrowreturnthick-1-e"
        },
        subGridRowExpanded: LoadSubGrid,
        pager: "gridPager",
        sortname: "name",
        height: "auto",
        rowNum: 10
    });
    for (var i = 0; i < rows.length; i++) {
        var rowId = rows[i].id;
        CreateTipTop(rowId + "_cancel", "Cancel");
        CreateTipTop(rowId + "_reindex", "Reindex");
        CreateTipTop(rowId + "_publish", "Publish");
        CreateTipTop(rowId + "_edit", "Edit");
        CreateTipTop(rowId + "_delete", "Delete");
    }
}
function ReloadGrid(gridId, relativeUrl, columns) {
    if (window.location.href.indexOf("disableRefresh") == -1) {
        $.get(relativeUrl,
            function (rows) {
                SetRowIds(rows);
                $("#" + gridId).jqGrid("clearGridData");
                $("#" + gridId).jqGrid("setGridParam", {
                    datatype: "local",
                    data: rows
                });
                $("#" + gridId).trigger("reloadGrid");
            }
        );
    }
}
function OnGridLoad(grid) {
    ClearTitles(grid);
    if (_childGridId == "transformJobOutputs") {
        var rows = grid.rows;
        for (var i = 0; i < rows.length; i++) {
            var jobState = rows[i]["properties.state"];
            if (jobState == "Processing") {
                var rowId = rows[i].id;
                $(this).expandSubGridRow(rowId);
            }
        }
    }
}
