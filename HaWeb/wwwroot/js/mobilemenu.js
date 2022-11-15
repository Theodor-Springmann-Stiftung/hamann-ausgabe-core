// Code for showing and hiding the mobile menu
const openmenu = function () {
    var x = document.getElementById("ha-topnav");
    if (x !== null) x.className += " ha-topnav-collapsed";
    let oldbutton = document.getElementById("openmenubutton");
    if (oldbutton !== null) oldbutton.setAttribute("class", "hidden");
    let newbutton = document.getElementById("closemenubutton");
    if (newbutton !== null) newbutton.setAttribute("class", "");
};

const closemenu = function () {
    var x = document.getElementById("ha-topnav");
    if (x !== null) x.className = "ha-topnav";
    let oldbutton = document.getElementById("closemenubutton");
    if (oldbutton !== null) oldbutton.setAttribute("class", "hidden");
    let newbutton = document.getElementById("openmenubutton");
    if (newbutton !== null) newbutton.setAttribute("class", "");
};

// Menu: Show / Hide Buttons for mobile View
if (
    document.getElementById("openmenubutton") !== null &&
    document.getElementById("closemenubutton") !== null
) {
    document
        .getElementById("openmenubutton")
        .addEventListener("click", openmenu);
    document
        .getElementById("closemenubutton")
        .addEventListener("click", closemenu);
}
