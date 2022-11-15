// Code specifically for the letter view
const showhidebutton = function (
    buttonid,
    divid,
    buttonlist,
    divlist,
    starthidden
) {
    let button = document.getElementById(buttonid);
    let div = document.getElementById(divid);

    if (starthidden && div !== null) {
        div.classList.add("hide");
    }

    if (!starthidden && button !== null) {
        button.classList.add("active");
    }

    if (button !== null) {
        button.addEventListener("click", function () {
            for (let btn of buttonlist) {
                let inactivebutton = document.getElementById(btn);
                if (inactivebutton !== null) inactivebutton.classList.remove("active");
            }

            for (let element of divlist) {
                let hiddenelement = document.getElementById(element);
                if (hiddenelement !== null) {
                    hiddenelement.classList.add("hide")
                    hiddenelement.classList.remove("flow-root");
                };
            }

            if (button !== null) button.classList.add("active");
            if (div !== null) {
                div.classList.add("flow-root");
                div.classList.remove("hide");
            }
        });
    }
};

// Letter View: Show / Hide Tabs
let buttonlist = ["ha-lettertextbtn", "ha-additionsbtn", "ha-marginalsbtn"];
let divlist = ["ha-lettertext", "ha-additions", "ha-marginals"];

if (this.document.getElementById("ha-lettertextbtn") !== null) {
    showhidebutton(
        "ha-lettertextbtn",
        "ha-lettertext",
        buttonlist,
        divlist,
        false
    );
    showhidebutton(
        "ha-additionsbtn",
        "ha-additions",
        buttonlist,
        divlist,
        true
    );
    showhidebutton(
        "ha-marginalsbtn",
        "ha-marginals",
        buttonlist,
        divlist,
        true
    );
} else {
    showhidebutton(
        "ha-lettertextbtn",
        "ha-lettertext",
        buttonlist,
        divlist,
        true
    );
    showhidebutton(
        "ha-additionsbtn",
        "ha-additions",
        buttonlist,
        divlist,
        false
    );
    showhidebutton(
        "ha-marginalsbtn",
        "ha-marginals",
        buttonlist,
        divlist,
        true
    );
}