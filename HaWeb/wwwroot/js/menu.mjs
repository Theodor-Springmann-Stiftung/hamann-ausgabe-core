const startup_menu = function () {
    // Gives active classes to links with active URLs
    // Marks links as active which target URL starts with the current URL
    const markactive_startswith = function (element) {
        var all_links = element.getElementsByTagName("a"),
            i = 0,
            len = all_links.length,
            full_path = location.href.split("#")[0].toLowerCase(); //Ignore hashes

        for (; i < len; i++) {
            if (full_path.startsWith(all_links[i].href.toLowerCase())) {
                all_links[i].className += " active";
            }
        }
    };

    // Marks links active which target URL is exact the same as the current URL
    const markactive_exact = function (element) {
        var all_links = element.getElementsByTagName("a"),
            i = 0,
            len = all_links.length,
            full_path = location.href.split("#")[0].toLowerCase(); //Ignore hashes

        for (; i < len; i++) {
            if (full_path == all_links[i].href.toLowerCase() || full_path == all_links[i].href.toLowerCase() + "/") {
                all_links[i].className += " active";
            }
        }
    };

    // Marks links as active, single links mutch match exactly, dropdown only the beginning
    const markactive_menu = function (element) {
        var all_links = element.getElementsByTagName("a"),
            i = 0,
            len = all_links.length,
            marked = false,
            full_path = location.href.split("#")[0].toLowerCase(); //Ignore hashes
        full_path = full_path.split("?")[0];

        for (; i < len; i++) {
            if (all_links[i].parentNode.classList.contains("ha-topnav-dropdown")) {
                if (full_path.startsWith(all_links[i].href.toLowerCase())) {
                    all_links[i].className += " active pointer-events-none";
                    marked = true;
                }
            } else {
                if (full_path == all_links[i].href.toLowerCase() || full_path == all_links[i].href.toLowerCase() + "/") {
                    all_links[i].className += " active pointer-events-none";
                    marked = true;
                }
            }
        }

        i = 0;
        if (marked === false) {
            for (; i < len; i++) {
                if (all_links[i].classList.contains("ha-active-default")) {
                    all_links[i].className += " active";
                }
            }
        }
    };

    if (document.getElementById("ha-topnav") !== null)
        markactive_menu(document.getElementById("ha-topnav"));
    if (document.getElementById("ha-register-nav") !== null)
        markactive_exact(document.getElementById("ha-register-nav"));
    if (document.getElementById("ha-adminuploadfields") !== null)
        markactive_exact(document.getElementById("ha-adminuploadfields"));
};


export { startup_menu };