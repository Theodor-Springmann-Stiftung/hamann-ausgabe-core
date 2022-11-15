const hideshowfiles = function() {
    let elem = document.getElementById("ha-availablefileslist");
    if (elem.classList.contains('hidden')) {
        
        elem.classList.remove('hidden');
        elem.classList.add('block');
    }
    else {
        elem.classList.add('hidden');
        elem.classList.remove('block');
    }
    }

    function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

var filesbutton = document.getElementById("ha-availablefiles");
if (filesbutton !== null)
    filesbutton.addEventListener("click", () => hideshowfiles());
