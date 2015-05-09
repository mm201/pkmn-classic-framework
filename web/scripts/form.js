function labelTextBox_Change()
{
    if (this.value.length > 0)
        $(this).addClass("hasContent");
    else
        $(this).removeClass("hasContent");
}

$(document).ready(function ()
{
    $("input").change(labelTextBox_Change);
    $("input").each(labelTextBox_Change);
})

// foreign lookup
function pfHandleLookupKeypress(id_outer, id_value, id_results, count, url)
{
    if (pfShowLookupResults(id_value, id_results))
    {
        pfHandleLookup(id_outer, id_value, id_results, count, url);
    }
}

// keyup
function pfHandleLookupKeypress3(id_outer, id_value, id_results, count, url)
{
    var ctrl_text = document.getElementById(id_value);

    if (ctrl_text.prevValue && ctrl_text.prevValue == ctrl_text.value)
    {
        ctrl_text.prevValue = ctrl_text.value;
        return;
    }
    ctrl_text.prevValue = ctrl_text.value;
    var ctrl_value = document.getElementById(id_outer + "_hdSelectedValue");

    ctrl_value.value = "";
    if (ctrl_value.onchange) ctrl_value.onchange();
    pfHandleLookupKeypress(id_outer, id_value, id_results, count, url);
}

// keypress
function pfHandleLookupKeypress2(id_outer, id_value, id_results, count, url, event)
{
    var ctrl_value = document.getElementById(id_outer + "_hdSelectedValue");
    var ctrl_text = document.getElementById(id_value);
    if (event && pfLookupResultsVisible(id_results) && (event.keyCode == 13 || event.keyCode == 9))
    {
        var ctrl_result1 = document.getElementById(id_outer + "_result1");
        if (ctrl_result1)
        {
            ctrl_value.value = ctrl_result1.getAttribute("data-value");
            if (ctrl_value.onchange) ctrl_value.onchange();
            ctrl_text.value = ctrl_result1.getAttribute("data-text");
        }
        pfHideLookupResults(id_results);
        ctrl_text.prevValue = ctrl_text.value;
        return false;
    }
    else if (event && (event.keyCode == 27))
    {
        pfHideLookupResults(id_results);
        ctrl_text.prevValue = ctrl_text.value;
        return true;
    }
    else if (event)
    {
        pfHandleLookupKeypress(id_outer, id_value, id_results, count, url);
        return true;
    }
    else
    {
        return true;
    }
}

function pfHandleLookup(id_outer, id_value, id_results, count, url)
{
    var ctrl_value = document.getElementById(id_value);

    if (ctrl_value.value.length > 1)
    {
        var ctrl_results = document.getElementById(id_results);
        if (ctrl_results.pfLookupBlocked == undefined) ctrl_results.pfLookupBlocked = false;
        if (ctrl_results.pfLastLookup == undefined) ctrl_results.pfLastLookup = "";
        var search = encodeURIComponent(ctrl_value.value);

        if (ctrl_results.pfLastLookup == search) return;
        if (ctrl_results.pfLookupBlocked) return; // todo: timeout this to deal with slow server performance
        ctrl_results.pfLastLookup = search;

        var request = $.ajax(url, 
            {
                complete: function (response, status, xhr)
                {
                    pfReceiveLookupResults(response.responseText, response.status, id_outer, id_value, id_results, count, url);
                },
                data: { n: count, q: search, c: id_outer },
                method: "POST"
            })
        ctrl_results.pfLookupBlocked = true;
    }
}

function pfShowLookupResults(id_value, id_results)
{
    if (document.getElementById(id_value).value.length > 1)
    {
        document.getElementById(id_results).style.visibility = "visible";
        return true;
    }
    else
    {
        document.getElementById(id_results).style.visibility = "hidden";
        return false;
    }
}

function pfReceiveLookupResults(response, status, id_outer, id_value, id_results, count, url)
{
    var ctrl_results = document.getElementById(id_results);
    if (ctrl_results.pfLastLookup == undefined) ctrl_results.pfLastLookup = "";
    ctrl_results.pfLookupBlocked = false;

    if (status == "200")
    {
        ctrl_results.innerHTML = "<div class=\"results_inner\">" + response + "</div>";
    }
    else
    {
        ctrl_results.innerHTML = "server error";
    }

    var search = encodeURIComponent(document.getElementById(id_value).value);
    if (ctrl_results.pfLastLookup != search) pfHandleLookup(id_outer, id_value, id_results, count, url);
}

function pfHideLookupResults(id)
{
    document.getElementById(id).style.visibility = "hidden";
}

function pfSelectLookupResult(id_main, id_value, value, id_text, text)
{
    document.getElementById(id_value).value = value;
    document.getElementById(id_text).value = text;
    document.getElementById(id_main).blur();
    var ctrl_value = document.getElementById(id_value);
    if (ctrl_value.onchange) ctrl_value.onchange();
}

function pfLookupResultsVisible(id)
{
    return document.getElementById(id).style.visibility == "visible";
}
