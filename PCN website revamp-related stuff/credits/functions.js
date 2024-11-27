// Opens links in the original browser window
function openInOrig (page) {
    window.opener.open(page);
    window.close();
}
