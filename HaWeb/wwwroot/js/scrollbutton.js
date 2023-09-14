// Script for showing and acting upon the "scroll to top button"
const scrollFunction = function () {
    button = document.getElementById("ha-scrollbutton");
    if (button !== null) {
        if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 300) {
            // button.style.display = "block";
            button.style.pointerEvents = "auto";
            button.style.opacity = "1";
        } else {
            // button.style.display = "none";
            button.style.pointerEvents = "none";
            button.style.opacity = "0";
        }
    }
}

// Scroll button
if (document.getElementById("ha-scrollbutton") !== null) {
    scrollFunction();
    document.getElementById("ha-scrollbutton").addEventListener("click", () => {
        document.body.scrollTop = 0; // For Safari
        document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
    })
    // TODO: workaround, bc window does not recieve scroll events anymore
    setInterval(() => scrollFunction(), 2500);
}
