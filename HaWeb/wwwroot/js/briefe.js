// // Code specifically for the letter view
let activetab = null;
let activetabbtn = null;
let tabbtnlist = document.querySelectorAll(".ha-tabbtn");
let tablist = document.querySelectorAll(".ha-tab");

for (let i = 0; i < tabbtnlist.length; i++) {
    tablist[i].classList.add("hidden");
    tabbtnlist[i].addEventListener("click", () => {
        if (activetab != null) 
            activetab.classList.add("hidden");
        if (activetabbtn != null)
            activetabbtn.classList.remove("active");

        tablist[i].classList.remove("hidden");
        tabbtnlist[i].classList.add("active");
        activetab = tablist[i];
        activetabbtn = tabbtnlist[i];
    });
}

if (tabbtnlist.length > 0) 
    tabbtnlist[0].click();