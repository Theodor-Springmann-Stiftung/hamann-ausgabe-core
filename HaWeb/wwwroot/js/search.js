const startup_search = function () {

    const ACTIVATESEARCHFILTER = function (filter, button) {
        let f = filter.value;
        if (f === "") {
            button.disabled = true;
            return;
        }
        button.disabled = false;
    }

    let searchfilter = document.getElementById("ha-searchformtext");
    let searchsubmitbtn = document.getElementById("ha-searchformsubmit");
    let searchform = document.getElementById("ha-searchform");
    ACTIVATESEARCHFILTER(searchfilter, searchsubmitbtn);
    searchfilter.addEventListener("input", () => ACTIVATESEARCHFILTER(searchfilter, searchsubmitbtn));
}

startup_search();
