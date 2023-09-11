function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

const USESubmit = async function (oFormElement, file = null) {
    let fd = new FormData(oFormElement);
    document.getElementById("ha-filelistbutton").style.pointerEvents = "none";
    document.getElementById("ha-filelistbutton").classList.add("loading");
    await fetch(oFormElement.action, {
        method: 'POST',
        headers: {
        'RequestVerificationToken': getCookie('RequestVerificationToken')
        },
        body: fd
    })
    .then(response => response.json())
    .then(json => {
        document.getElementById("ha-filelistbutton").classList.remove("loading");
        document.getElementById("ha-filelistbutton").style.pointerEvents = "auto";
        if ("Error" in json) {
            document.getElementById("ha-filelistoutput").textContent = json.Error;
        } 
        else {
            location.reload(); 
        }
    })
    .catch ((e) => {
        document.getElementById("ha-filelistbutton").classList.remove("loading");
        document.getElementById("ha-filelistbutton").style.pointerEvents = "auto";
        document.getElementById("ha-filelistoutput").textContent = e;
    })
}

const GETSyntaxCheck = async function (oFormElement, file = null) {
    document.getElementById("ha-scbutton").style.pointerEvents = "none";
    document.getElementById("ha-scbutton").classList.toggle("loading");
    await fetch(oFormElement.action)
        .then(response => response.json())
        .then(j => {
            Object.entries(j).forEach(([key, value]) => {
                var e = document.getElementById(key);
                if (e !== null && !e.classList.contains("red")) {
                    var h = e.querySelector(".ha-managedfileheader");
                    var i = e.querySelector(".ha-filestatusicon");
                    var a = e.querySelector(".ha-managedfileannotations");
                    if (value.errors === null) {
                        h.classList.add("green");
                    } else {
                        var icon = i.querySelector("svg");
                        icon.remove();
                        i.insertAdjacentHTML("afterbegin", '<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 24 24"><title>alert-decagram-outline</title><path d="M23,12L20.56,14.78L20.9,18.46L17.29,19.28L15.4,22.46L12,21L8.6,22.47L6.71,19.29L3.1,18.47L3.44,14.78L1,12L3.44,9.21L3.1,5.53L6.71,4.72L8.6,1.54L12,3L15.4,1.54L17.29,4.72L20.9,5.54L20.56,9.22L23,12M20.33,12L18.5,9.89L18.74,7.1L16,6.5L14.58,4.07L12,5.18L9.42,4.07L8,6.5L5.26,7.09L5.5,9.88L3.67,12L5.5,14.1L5.26,16.9L8,17.5L9.42,19.93L12,18.81L14.58,19.92L16,17.5L18.74,16.89L18.5,14.1L20.33,12M11,15H13V17H11V15M11,7H13V13H11V7" /></svg>');
                        h.classList.add("expandable");
                        h.classList.add("orange");
                        h.addEventListener("click", () => {
                            h.classList.toggle("expanded");
                        });
                        var t = document.createElement("table");
                        var thr = document.createElement("tr");
                        var thl = document.createElement("th");
                        var thc = document.createElement("th");
                        var thm = document.createElement("th");
                        thl.append("Zeile");
                        thc.append("Spalte");
                        thm.append("Fehler");
                        thr.append(thl, thc, thm);
                        t.append(thr);
                        value.errors.forEach((error) => {
                            var tr = document.createElement("tr");
                            var tdl = document.createElement("td");
                            var tdc = document.createElement("td");
                            var tdm = document.createElement("td");
                            tdl.append(error.line);
                            tdc.append(error.column);
                            tdm.append(error.message);
                            tr.append(tdl, tdc, tdm);
                            t.append(tr);
                        })
                        a.append(t);
                    }
                }
                console.log(e, h, i, a);
            });
            // let coll = document.getElementsByClassName("ha-managedfile");
            // for (i = 0; i < coll.length; i++) {
            //     let e = coll[i];
            //     if (j[e.id] !== null) {
            //         if(j[e.id].errors === null) {
            //             console.log(e.id + " hat keine errors");
            //         } else {
            //             console.log(e.id + " hat errors");
            //         }
            //     }
            // }
            document.getElementById("ha-scbutton").classList.toggle("hidden");

        })
        .catch ((e) => {
            console.log(e);
            document.getElementById("ha-scbutton").classList.toggle("loading");
            document.getElementById("ha-scbutton").style.pointerEvents = "auto";
        })
}

var coll = document.getElementsByClassName("expandable");

for (i = 0; i < coll.length; i++) {
    let element = coll[i]
    coll[i].addEventListener("click", () => {
        element.classList.toggle("expanded");
    });
}