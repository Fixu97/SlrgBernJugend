
$(document).ready(function () {

    AllEntries = [];
    Url = window.location.href;

});
function defined(obj) {
    return obj !== undefined
        && obj !== null;
};
$("#displayAll").click(function (handler) {
    var button = handler.currentTarget;
    if (button.innerHTML == "Display all") {
        button.innerHTML = "Display active";
        ShowAllEntries();
    } else {
        button.innerHTML = "Display all";
        ShowOnlyActivePeople();
    }
});
$("#createNew").click(function () {
    window.location.href = Url + "/Create";
});

function GetAllEntries(controller, successHandler) {
    $.ajax({
        url: "/" + controller + "/GetAll",
        async: true,
        dataType: "json",
        type: 'post',
        success: function (returnVal) {
            if (successHandler) {
                successHandler(returnVal);
            }
        },
        error: function () {
            alert("An error occoured while trying to retrieve data from server!");
        }
    });
}
function AppendRowsToTable(items, tbody) {

    var itemsCount = items.length;
    var row;

    for (var i = 0; i < itemsCount; i++) {

        row = $("<tr onclick='redirectToEntry(" + items[i]["Pk"] + ")'>");
        row.append($("<td>" + items[i]["DiscName"] + "</td>"));
        row.append($("<td>" + items[i]["Meters"] + "m</td>"));

        //tbody.append(row);
        $(row).appendTo(tbody).show('slow');

    }

    $("#entriesCount").innerText = items.length + " Entries";
}
function ShowAllEntries() {
    removeTableContent($("#valuesList"));
    AppendRowsToTable(AllEntries, $("#valuesList"));
}
function redirectToEntry(pk) {
    window.location.href = Url + "/Details/" + pk;
}
function removeTableContent(tableParam) {
    var table = tableParam[0];
    while (table.firstChild) {
        table.removeChild(table.firstChild);
    }
    $("#entriesCount").innerText = "0 Entries";
}

function CreateDropdown(name, displayData, valueData, defaultValue, additionalAttribute) {

    var dropdown = $("<select id='" + name + "' name='" + name + "'" + additionalAttribute + "></select>");
    var option;
    var isDefault;

    if (!defaultValue) {
        defaultValue = 0;
        dropdown.append($("<option selected disabled>---Please select an entry!---</option>"));
    }

    $(displayData).each(function (i, displayVal) {
        isDefault = valueData[i] == defaultValue ? " selected" : "";
        option = $("<option value='" + valueData[i] + "' " + isDefault + ">" + displayVal + "</option>");
        dropdown.append(option);
    });
    return dropdown;
}

function CreatePkArray(list) {
    var pkArr = [];
    $(list).each(function(i, value) {
        pkArr[i] = value.Pk;
    });
    return pkArr;
}
function CreatePeopleNameList(people) {
    var displayNames = [];

    $(people).each(function (i, value) {
        displayNames[i] = value.Prename + " " + value.LastName.charAt(0) + ".";
    });

    return displayNames;
}
function CreateDisciplinesNameList(disciplines) {
    var displayNames = [];

    $(disciplines).each(function (i, value) {
        displayNames[i] = value.DiscName + " " + value.Meters + "m";
    });

    return displayNames;
}
function CreateUsersNameList(users) {
    var displayNames = [];

    $(users).each(function (i, value) {
        displayNames[i] = value.Username;
    });

    return displayNames;
}
function CreateRelaysNameList(relays) {
    var displayNames = [];

    $(relays).each(function (i, value) {
        displayNames[i] = value.Relay;
    });

    return displayNames;
}

function CreatePeopleDropdown(fk, td) {
    DataAccess.getAllPeople(
        function (people) {
            var nameList = CreatePeopleNameList(people);
            var pkList = CreatePkArray(people);
            var dropdown = CreateDropdown("FK_P", nameList, pkList, fk);

            $(td).append(dropdown);
        },
        undefined);
}
function CreateDisciplinesDropdown(fk, td) {
    DataAccess.getAllDisciplines(
        function (disciplines) {
            var nameList = CreateDisciplinesNameList(disciplines);
            var pkList = CreatePkArray(disciplines);
            var dropdown = CreateDropdown("FK_D", nameList, pkList, fk);

            $(td).append(dropdown);
        },
        undefined);
}
function CreateUsersDropdown(fk, td) {
    DataAccess.getAllUsers(
        function (users) {
            var nameList = CreateUsersNameList(users);
            var pkList = CreatePkArray(users);
            var dropdown = CreateDropdown("FK_U", nameList, pkList, fk);

            $(td).append(dropdown);
        },
        undefined);
}
function CreateRelaysDropdown(fk, td) {
    DataAccess.getAllRelays(
        function (relays) {
            var nameList = CreateRelaysNameList(relays);
            var pkList = CreatePkArray(relays);
            var dropdown = CreateDropdown("FK_R", nameList, pkList, fk);

            $(td).prepend(dropdown);
        },
        undefined);
}

function CreateDisciplinesMultiSelect(td, selectId) {
    DataAccess.getAllDisciplines(
        function (disciplines) {
            var nameList = CreateDisciplinesNameList(disciplines);
            var pkList = CreatePkArray(disciplines);
            var dropdown = CreateDropdown(selectId, nameList, pkList, null, "multiple");

            $(td).prepend(dropdown);
        },
        undefined);
}


function AppendChart(container, charts) {
    $(charts).each(function (index, item) {

        // Remove old chart
        $(container).empty();

        var chartContainer = $("<div class='chartContainer'></div>");
        chartContainer.append("<h1>" + item.title + "</h2>");
        var canvas = $("<canvas class='chart'></canvas>");
        chartContainer.append(canvas);
        chartContainer.append("<hr/>");
        container.append(chartContainer);

        // Draw the chart
        var chart = new Chart($(canvas).get(0).getContext("2d")).Line(item, item.options);
        var legend = chart.generateLegend();

        container.append(legend);
    });
}
function CreateChart(container, pk) {
    $.ajax({
        url: '/Chart/GetChartsForDiscipline/' + pk,
        async: true,
        dataType: 'json',
        type: 'GET',
        success: function (charts) {
            AppendChart(container, charts);
            AppendHighScoreTable(container, pk);
        },
        error: function (error) {
            errorHandler(error, container);
        }
    });
}
function CreateChartForDisciplineByGender(container, pk, male) {
    $.ajax({
        url: '/Chart/GetChartsForDisciplineByGender/',
        async: true,
        dataType: 'json',
        data: {id: pk, male: male},
        type: 'GET',
        success: function (charts) {

            // append chart
            AppendChart(container, charts);
            AppendHighScoreTable(container, pk, male);
        },
        error: function (error) {
            errorHandler(error, container);
        }
    });
}
function CreateChartForPersonForDiscipline(container, personId, disciplineId) {

    // First get chart
    $.ajax({
        url: '/Chart/GetChartsForPersonForDiscipline/',
        async: true,
        dataType: 'json',
        data: { personId: personId, disciplineId: disciplineId },
        type: 'GET',
        success: function (charts) {
            AppendChart(container, charts);
            AppendPersonHighScore(container, personId, disciplineId);
        },
        error: function (error) {
            errorHandler(error, container);
        }
    });
}

function AppendHighScoreTable(container, disciplineId, male) {

    var self = {
        container: container,
        disciplineId: disciplineId,
        male: male,
        topN: 5
    };

    var methodName = "GetTopNForDiscipline";

    if ($("#topN").val() != undefined) {
        self.topN = $("#topN").val();
    }

    var data = { n: self.topN, disciplineId: disciplineId};
    if (male !== undefined && male !== null) {
        methodName += "ByGender";
        data.male = male;
    }

    // Then get best 5 people
    $.ajax({
        url: '/Times/' + methodName,
        async: true,
        dataType: 'json',
        data: data,
        type: 'GET',
        success: function (topN) {
            $(".highscoreContainer").remove();
            var highscoreElem = $("<div class='highscoreContainer'></div>");

            var possibleSelections = [5, 10, 20];
            var topX = "<select id='topN'>";
            for (var i = 0; i < possibleSelections.length; i++) {
                topX += "<option" + (self.topN == possibleSelections[i] ? " selected" : "") + ">" + possibleSelections[i] + "</option>";
            }
            topX += "</select>";

            highscoreElem.append("<h3>Top " + topX + "</h3>");

            var highscoreTable = $("<table class='table table-striped table-bordered'><tr><th>Person</th><th>Date</th><th>Seconds</th></tr></table>");
            var emptyRow = "<tr><td>&nbsp;</td><td></td><td></td></tr>";

            if (topN) {
                $(topN).each(function (i, time) {

                    if (!time) {
                        highscoreTable.append(emptyRow);
                        return true; // continue
                    }

                    var date = new Date(time.Date);
                    var dateStr = ('0' + date.getDate()).slice(-2) + '.'
                     + ('0' + (date.getMonth() + 1)).slice(-2) + '.'
                     + date.getFullYear();

                    highscoreTable.append("<tr><td> " + time.Person.DisplayName + "</td><td> " + dateStr + "</td><td> " + time.DisplayTime + "</td></tr>");
                });
            } else {
                // if no highscores are available, append an empty row
                highscoreTable.append(emptyRow);
            }

            highscoreElem.append(highscoreTable);

            container.append(highscoreElem);

            $("#topN").change(function () {
                AppendHighScoreTable(self.container, self.disciplineId, self.male);
            });
        },
        error: function (error) {
            errorHandler(error, container);
        }
    });
}
function AppendPersonHighScore(container, personId, disciplineId) {

    $.ajax({
        url: '/Times/GetHighScoreForPersonForDiscipline/',
        async: true,
        dataType: 'json',
        data: { personId: personId, disciplineId: disciplineId},
        type: 'GET',
        success: function (highscore) {

            var highscoreElem = $("<span class='highScore'>Best: " + highscore.DisplayTime + "</span>");

            container.append(highscoreElem);
        },
        error: function (error) {
            errorHandler(error, container);
        }
    });
}

function displaySuccessInfo(text) {
    $("#message").remove();
    var msg = $("<p id='message' class='bg-success'>" + text + "</p>");
    $("body").append(msg).fadeIn(300);
    msg.delay(1000).fadeOut(300, function () { $(this).remove(); });
}
function displayErrorInfo(text) {
    $("#message").remove();
    var msg = $("<p id='message' class='bg-danger'>" + text + "</p>");
    $("body").append(msg).fadeIn(300);
    msg.delay(10000).fadeOut(300, function () { $(this).remove(); });
}

function errorHandler(serverError, container) {

    // make sure container is JQuery
    container = $(container);

    container.empty();
    container.append(serverError.responseText);
}