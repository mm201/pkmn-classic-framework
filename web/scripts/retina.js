function setPixelRatioCookie(scale)
{
    var expires = new Date();
    expires.setDate(expires.getDate() + 21);
    document.cookie = "pixelRatio=" + scale + ";path=/;expires=" + expires;
}
setPixelRatioCookie(getPixelRatio());

function getPixelRatio()
{
    if (window.devicePixelRatio != undefined)
        return window.devicePixelRatio;
    else if (window.screen.deviceXDPI != undefined)
        return window.screen.deviceXDPI / 96.0;
    else if (window.matchMedia != undefined)
        return bisectScale(1.0, 2.0, 0.005);
    else
        return 1.0;
}

function matchScale(scale)
{
    var dpi = scale * 96.0;
    var dpi_int = Math.ceil(dpi); // ceil to get mathmatically correct results for non-int dpi

    return window.matchMedia("(min--moz-device-pixel-ratio: " + scale.toString() + ")").matches || // firefox 8-15
           window.matchMedia("(-o-min-device-pixel-ratio: " + dpi_int.toString() + "/96)").matches ||
           window.matchMedia("(-webkit-min-device-pixel-ratio: " + scale.toString() + ")").matches ||
           window.matchMedia("(min-resolution: " + dpi.toString() + "dpi)").matches ||
           window.matchMedia("(min-resolution: " + dpi_int.toString() + "dpi)").matches // firefox 7- doesn't understand non-int dpi units
}

function bisectScale(min, max, tolerance)
{
    if (min + tolerance > max) return min;
    if (matchScale(max)) return max;
    if (!matchScale(min)) return min;
    if (matchScale((min + max) * 0.5)) return bisectScale((min + max) * 0.5, max, tolerance);
    return bisectScale(min, (min + max) * 0.5, tolerance);
}

function checkRetina(element, scale)
{
    try
    {
        var data_design_width = element.getAttribute("data-design-width");
        var data_design_height = element.getAttribute("data-design-height");

        if (data_design_width !== null &&
            data_design_height !== null)
        {
            var scaleX = $(element).width() / parseFloat(data_design_width);
            var scaleY = $(element).height() / parseFloat(data_design_height);
            scale *= Math.max(scaleX, scaleY);
        }
        else if (data_design_width !== null)
        {
            scale *= $(element).width() / parseFloat(data_design_width);
        }
        else if (data_design_height !== null)
        {
            scale *= $(element).height() / parseFloat(data_design_height);
        }

        var sizes = new Array();
        var x = 0;
        for (x in element.attributes)
        {
            var attrName = element.attributes[x].name;
            if (attrName != undefined && attrName.length > 11 &&
                attrName.substring(0, 11).toLowerCase() == "data-hires-")
            {
                sizes.push(parseFloat(attrName.substring(11)));
            }
        }
        sizes.sort();

        var best = 0.0;
        for (x in sizes)
        {
            var f = sizes[x];
            if (f === undefined) continue;
            if (best > 0.0 && f > scale && best >= scale) break;
            best = f;
        }

        if ($(element).hasClass("keephr") && element.retinaActiveScale && element.retinaActiveScale >= best) return;

        element.src = element.getAttribute("data-hires-" + best.toString());
        element.retinaActiveScale = best;
    }
    catch (err)
    {
    }
}

function checkAllRetina()
{
    var scale = getPixelRatio();
    $(".retina").each(function ()
    {
        checkRetina(this, scale);
    });
    setPixelRatioCookie(scale);
}

function checkRetinaIn(element)
{
    var scale = getPixelRatio();
    $(".retina", element).each(function ()
    {
        checkRetina(this, scale);
    });
}

function handleBeforePrint()
{
    window.rjsPrinting = true;
    checkAllRetina();
}

function handleAfterPrint()
{
    window.rjsPrinting = false;
    checkAllRetina();
}

function handlePrintListener(mql)
{
    window.rjsPrinting = mql.matches;
    checkAllRetina();
}

window.rjsPrinting = false;

if (document.addEventListener) document.addEventListener("DOMContentLoaded", checkAllRetina, false);
else if (document.attachEvent) document.attachEvent("DOMContentLoaded", checkAllRetina, false);
if (window.addEventListener) window.addEventListener("resize", checkAllRetina, false);
else if (window.attachEvent) window.attachEvent("resize", checkAllRetina, false);
if (window.addEventListener) window.addEventListener("beforeprint", handleBeforePrint, false);
else if (window.attachEvent) window.attachEvent("beforeprint", handleBeforePrint, false);
if (window.addEventListener) window.addEventListener("afterprint", handleAfterPrint, false);
else if (window.attachEvent) window.attachEvent("afterprint", handleAfterPrint, false);
if (window.matchMedia) window.matchMedia("print").addListener(handlePrintListener);
