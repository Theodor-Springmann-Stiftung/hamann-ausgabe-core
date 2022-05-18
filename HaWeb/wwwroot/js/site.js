const openmenu = function () {
    var x = document.getElementById("ha-topnav");
    if (x !== null) x.className += " ha-topnav-collapsed";
    let oldbutton = document.getElementById("openmenubutton");
    if (oldbutton !== null) oldbutton.setAttribute('class', 'hidden');
    let newbutton = document.getElementById("closemenubutton");
    if (newbutton !== null) newbutton.setAttribute('class', '');
}

const closemenu = function () {
    var x = document.getElementById("ha-topnav");
    if (x !== null) x.className = "ha-topnav";
    let oldbutton = document.getElementById("closemenubutton");
    if (oldbutton !== null) oldbutton.setAttribute('class', 'hidden');
    let newbutton = document.getElementById("openmenubutton");
    if (newbutton !== null) newbutton.setAttribute('class', '');

}

const markactive_startswith = function (element) {
    // Marks links as active which target URL starts with the current URL
    var all_links = element.getElementsByTagName("a"),
        i = 0, len = all_links.length,
        full_path = location.href.split('#')[0].toLowerCase(); //Ignore hashes

    for (; i < len; i++) {
        if (full_path.startsWith(all_links[i].href.toLowerCase())) {
            all_links[i].className += " active";
        }
    }
}

const markactive_exact = function (element) {
    var all_links = element.getElementsByTagName("a"),
        i = 0, len = all_links.length,
        full_path = location.href.split('#')[0].toLowerCase(); //Ignore hashes

    for (; i < len; i++) {
        if (full_path == all_links[i].href.toLowerCase()) {
            all_links[i].className += " active";
        }
    }
}

const getLineHeight = function (element) {
    var temp = document.createElement(element.nodeName), ret;
    temp.setAttribute("class", element.className);
    temp.innerHTML = "A";

    element.parentNode.appendChild(temp);
    ret = temp.clientHeight;
    temp.parentNode.removeChild(temp);

    return ret;
}

const sidebarcollapse = function (selector) {
    let backlinkboxes = document.querySelectorAll(selector);
    let clientrects = [];

    for (element of backlinkboxes) {
        clientrects.push([element, element.getBoundingClientRect()]);
    }

    let lineheight = 1;

    if (backlinkboxes.length >= 1) {
        lineheight = getLineHeight(backlinkboxes[0]);
    }

    for (var i = 0; i < clientrects.length; i++) {
        if (i < clientrects.length-1) {
            if (clientrects[i][1].bottom >= clientrects[i+1][1].top) {
                let overlap = clientrects[i][1].bottom - clientrects[i+1][1].top;
                let newlength = clientrects[i][1].height - overlap;
                let remainder = newlength % lineheight;
                newlength = newlength - remainder;
                clientrects[i][0].style.height = newlength + 'px';
                clientrects[i][0].style.overflowX = "hidden";
                clientrects[i][0].style.overflowY = "scroll";
            }
        }
    }
}



window.addEventListener('load', function() {
    document.getElementById("openmenubutton").addEventListener('click', openmenu);
    document.getElementById("closemenubutton").addEventListener('click', closemenu);
    markactive_startswith(document.getElementById("ha-topnav"));
    markactive_exact(document.getElementById("ha-register-nav"));
    sidebarcollapse(".ha-neuzeit .ha-letlinks");
    sidebarcollapse(".ha-forschung .ha-letlinks");
})

