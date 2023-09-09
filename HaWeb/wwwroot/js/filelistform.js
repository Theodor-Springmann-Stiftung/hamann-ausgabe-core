function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

const USESubmit = async function (oFormElement, file = null) {
    let fd = new FormData(oFormElement);
    document.getElementById("ha-filelistbutton").style.pointerEvents = "none";
    document.getElementById("ha-lds-ellipsis-load").style.display = "inline-block";
    await fetch(oFormElement.action, {
        method: 'POST',
        headers: {
        'RequestVerificationToken': getCookie('RequestVerificationToken')
        },
        body: fd
    })
    .then(response => response.json())
    .then(json => {
        if ("Error" in json) {
            document.getElementById("ha-filelistbutton").style.pointerEvents = "auto";
            document.getElementById("ha-lds-ellipsis-load").style.display = "none";
        document.getElementById("ha-filelistoutput").textContent = json.Error;
        } 
        else {
            document.getElementById("ha-filelistbutton").style.pointerEvents = "auto";
            document.getElementById("ha-lds-ellipsis-load").style.display = "none";
        location.reload(); 
        }
    })
    .catch ((e) => {
        document.getElementById("ha-filelistbutton").style.pointerEvents = "auto";
        document.getElementById("ha-lds-ellipsis-load").style.display = "none";
        document.getElementById("ha-filelistoutput").textContent = e;
    })
}

const YEARSUBMIT = async function (oFormElement, file = null) {
    let fd = new FormData(oFormElement);
    document.getElementById("ha-setendyearbutton").style.pointerEvents = "none";
    await fetch(oFormElement.action, {
        method: 'POST',
        headers: {
        'RequestVerificationToken': getCookie('RequestVerificationToken')
        },
        body: fd
    })
    .then(response => response.json())
    .then(json => {
        document.getElementById("ha-setendyearbutton").style.pointerEvents = "auto";
        location.reload();
    })
    .catch ((e) => {
        document.getElementById("ha-setendyearbutton").style.pointerEvents = "auto";
    })
}