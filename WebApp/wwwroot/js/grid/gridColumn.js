﻿var defaultWidth = 100, nameWidth = 300, dateTimeWidth = 120;
function GetParentColumns(gridId, nameLabel) {
    var columns;
    switch (gridId) {
        case "storageAccounts":
            columns = [
                {
                    label: "Storage Account Name",
                    name: "id",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "Type",
                    name: "type",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "videoIndexerInsights":
            columns = [
                {
                    label: "Insight Id",
                    name: "id",
                    align: "center",
                    width: 200
                },
                {
                    label: nameLabel,
                    name: "name",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth
                }
            ];
            break;
        default:
            columns = [
                {
                    label: nameLabel,
                    name: "name",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "Created",
                    name: "properties.created",
                    formatter: FormatDateTime,
                    align: "center",
                    width: dateTimeWidth
                },
            ];
    }
    if (gridId != "storageAccounts" &&
        gridId != "streamingPolicies" &&
        gridId != "streamingLocators" &&
        gridId != "videoIndexerInsights") {
        var modified = {
            label: "Modified",
            name: "properties.lastModified",
            formatter: FormatDateTime,
            align: "center",
            width: dateTimeWidth
        };
        columns.splice(columns.length, 0, modified);
    }
    switch (gridId) {
        case "transformJobs":
            var priority = {
                label: "Priority",
                name: "properties.priority",
                align: "center",
                width: defaultWidth
            };
            var state = {
                label: "State",
                name: "properties.state",
                align: "center",
                width: defaultWidth
            };
            columns.splice(1, 0, priority, state);
            break;
        case "streamingLocators":
            var streamingPolicyName = {
                label: "Streaming Policy Name",
                name: "properties.streamingPolicyName",
                align: "center",
                width: nameWidth
            };
            var startTime = {
                label: "Start Time",
                name: "properties.startTime",
                formatter: FormatDateTime,
                align: "center",
                width: dateTimeWidth
            };
            var endTime = {
                label: "End Time",
                name: "properties.endTime",
                formatter: FormatDateTime,
                align: "center",
                width: dateTimeWidth
            };
            columns.splice(1, 0, streamingPolicyName, startTime, endTime);
            break;
    }
    if (gridId != "storageAccounts") {
        var actions = {
            label: "Actions",
            formatter: FormatActions,
            align: "center",
            sortable: false,
            width: defaultWidth
        };
        columns.splice(columns.length, 0, actions);
    }
    return columns;
}
function GetChildColumns(gridId) {
    var columns;
    switch (gridId) {
        case "assetFiles":
            columns = [
                {
                    label: "Asset File Name",
                    name: "name",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth + (dateTimeWidth * 2) + 30
                }
            ];
            break;
        case "transformOutputs":
            columns = [
                {
                    label: "Preset Name",
                    name: "preset.presetName",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth + 10
                },
                {
                    label: "Priority",
                    name: "relativePriority",
                    align: "center",
                    width: dateTimeWidth + 5
                },
                {
                    label: "On Error",
                    name: "onError",
                    formatter: FormatValue,
                    align: "center",
                    width: dateTimeWidth + 5
                }
            ];
            break;
        case "transformJobOutputs":
            columns = [
                {
                    label: "Asset Name",
                    name: "assetName",
                    formatter: FormatName,
                    align: "center",
                    width: nameWidth + 10
                },
                {
                    label: "State",
                    name: "state",
                    align: "center",
                    width: defaultWidth + 5
                },
                {
                    label: "Progress",
                    name: "progress",
                    formatter: FormatProgress,
                    align: "center",
                    width: defaultWidth + 5
                }
            ];
            break;
    }
    return columns;
}
function FormatName(value, grid, row) {
    if (row.preset != null && row.preset.audioLanguage != null) {
        value = row.preset.audioInsightsOnly == null ? "Audio Analyzer" : "Video Analyzer";
    }
    value = FormatValue(value, grid, row);
    var description = row["properties.description"];
    if (description != null) {
        value = "<span onclick=DisplayMessage('Description','" + description + "')>" + value + "</span>";
    }
    return value;
}
function FormatValue(value, grid, row) {
    for (var i = 0; i < _spacingPatterns.length; i++) {
        var expression = new RegExp(_spacingPatterns[i], "g");
        value = value.replace(expression, _spacingInserts[i]);
    }
    return value;
}
function FormatProgress(value, grid, row) {
    return value + "%";
}
function FormatDateTime(value, grid, row) {
    if (value == null) {
        value = "N/A";
    } else {
        value = value.slice(11, 19) + "<br>" + value.slice(0, 10);
    }
    return value;
}
function FormatActions(value, grid, row) {
    var actionsHtml = "";
    var entityName = row.name;
    var entityType = GetEntityType();
    if (entityName.indexOf("Predefined_") > -1) {
        actionsHtml = "N/A";
    } else {
        var cancelHtml = "";
        if (grid.gid == "transformJobs") {
            var onCancel = "CancelJob('" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
            var cancelHtml = "<button id='" + row.id + "_cancel' class='siteButton' onclick=" + onCancel + ">";
            cancelHtml = cancelHtml + "<img src='" + _storageCdnUrl + "/MediaEntityCancel.png'></button>";
        }
        var editHtml = "";
        if (window.location.href.indexOf("/account") == -1) {
            if (grid.gid == "transforms") {
                editHtml = "<button id='" + row.id + "_edit' class='siteButton' onclick=SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')>";
                editHtml = editHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
            }
        }
        var onDelete = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
        var deleteHtml = "<button id='" + row.id + "_delete' class='siteButton' onclick=" + onDelete + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
        actionsHtml = cancelHtml + editHtml + deleteHtml;
    }
    return actionsHtml
}