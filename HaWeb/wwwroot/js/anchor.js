// Marks anchors, if anchors are present in the URL
const markanchor = function () {
    var currentUrl = document.URL,
        urlParts = currentUrl.split('#');
    var anchor = (urlParts.length > 1) ? urlParts[1] : null;
    if (anchor != null) {
        var element = document.getElementById(anchor);
        var pointerelement = document.createElement("span");
        pointerelement.classList.add("ha-location");
        pointerelement.prepend("☛");
        if (element !== null && element.classList.contains("ha-linecount")) {
            pointerelement.classList.add("-left-5");
            pointerelement.classList.add("-top-1.5")
            element.prepend(pointerelement);
        } else if (element !== null && element.classList.contains("ha-commenthead")) {
            pointerelement.classList.add("-left-6");
            element.prepend(pointerelement);
        }
    }
}

markanchor();