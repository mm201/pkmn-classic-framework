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
