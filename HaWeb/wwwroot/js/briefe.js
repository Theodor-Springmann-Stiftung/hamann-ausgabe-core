// // Code specifically for the letter view

const startup_briefe = function () {
    let activetab = null;
    let activetabbtn = null;
    let activetabbtn2 = null;
    let tabbtnlist = document.querySelectorAll(".ha-tabbtn");
    let tablist = document.querySelectorAll(".ha-tab");

    for (let i = 0; i < tabbtnlist.length; i++) {
        tablist[i % tablist.length].classList.add("hidden");
        tabbtnlist[i].addEventListener("click", () => {
            if (activetab != null)
                activetab.classList.add("hidden");
            if (activetabbtn != null)
                activetabbtn.classList.remove("active");
            if (activetabbtn2 != null)
                activetabbtn2.classList.remove("active");

            tablist[i % tablist.length].classList.remove("hidden");
            tabbtnlist[i].classList.add("active");
            tabbtnlist[(i + tablist.length) % tabbtnlist.length].classList.add("active");
            activetab = tablist[i % tablist.length];
            activetabbtn = tabbtnlist[i];
            activetabbtn2 = tabbtnlist[(i + tablist.length) % tabbtnlist.length];

            // if (resetall != null) { 
            //     console.log("RESET MARG")
            //     requestAnimationFrame(() => { resetall(); });
            // }
        });
    }

    if (tabbtnlist.length > 0)
        tabbtnlist[0].click();
};

startup_briefe();