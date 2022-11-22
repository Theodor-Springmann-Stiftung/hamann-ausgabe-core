const ACTIVATEGOTOFILTER = function(filter, button) {
    let f = filter.value;
    if (f === "") {
        button.disabled = true;
        return;
    }
    button.disabled = false;
}

const ACTIVATEZHSEARCH = function(volume, page, button) {
    let vol = volume.options[volume.selectedIndex].value;
    let pg = page.value;
    if (pg === "") {
        button.disabled = true;
        return;
    }
    button.disabled = false;
}

const SUBMITZHSEARCH = function(volume, page) {
    let vol = volume.options[volume.selectedIndex].value;
    let pg = page.value;
    window.location.href = "/HKB/" + vol + "/" + pg;
}

const ACTIVATESEARCHFILTER = function(filter, button) {
    let f = filter.value;
    if (f === "") {
        button.disabled = true;
        return;
    }
    button.disabled = false;
}

// Go to letter HKB number
let gotofilter = document.getElementById("ha-gotoletternumber");
let gotosubmitbtn = document.getElementById("ha-gotoformsubmit");
let gotoform = document.getElementById("ha-gotoform");
ACTIVATEGOTOFILTER(gotofilter, gotosubmitbtn);
gotofilter.addEventListener("input", () => ACTIVATEGOTOFILTER(gotofilter, gotosubmitbtn));

// ZH Volume / Page Lookup
let vol = document.getElementById("ha-zhformvolume");
let pg = document.getElementById("ha-zhformpage");
let zhsubmitbtn = document.getElementById("ha-zhformsubmit");
let zhsearchform = document.getElementById("ha-zhform");
ACTIVATEZHSEARCH(vol, pg, zhsubmitbtn);
vol.addEventListener("change", () => ACTIVATEZHSEARCH(vol, pg, zhsubmitbtn));
pg.addEventListener("input", () => ACTIVATEZHSEARCH(vol, pg, zhsubmitbtn));
// Need to implement send bc razor tag helpers do not work here
zhsearchform.addEventListener("submit", (ev) => { 
    ev.preventDefault();
    SUBMITZHSEARCH(vol, pg);
});

// Full-text search
let searchfilter = document.getElementById("ha-searchformtext");
let searchsubmitbtn = document.getElementById("ha-searchformsubmit");
let searchform = document.getElementById("ha-searchform");
ACTIVATESEARCHFILTER(searchfilter, searchsubmitbtn);
searchfilter.addEventListener("input", () => ACTIVATESEARCHFILTER(searchfilter, searchsubmitbtn));