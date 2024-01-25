// Functions for switching theme
const startup_theme = function () {

    const go_to_dark = function () {
        localStorage.setItem("theme", "ha-toggledark");
        document.documentElement.classList.add("dark");
    };

    const go_to_bright = function () {
        document.documentElement.classList.remove("dark");
        localStorage.setItem("theme", "ha-togglebright");
    };

    // Functions for reading theme settings
    const get_theme_settings = function (standard) {
        var theme = localStorage.getItem("theme");
        if (theme === null) theme = standard;
        let toggleSwitch = document.getElementById(theme).click();
    };

    if (
        document.getElementById("ha-togglebright") !== null &&
        this.document.getElementById("ha-toggledark") !== null
    ) {
        document
            .getElementById("ha-togglebright")
            .addEventListener("click", go_to_bright);
        document
            .getElementById("ha-toggledark")
            .addEventListener("click", go_to_dark);
    }
    get_theme_settings("ha-togglebright");
};

startup_theme();