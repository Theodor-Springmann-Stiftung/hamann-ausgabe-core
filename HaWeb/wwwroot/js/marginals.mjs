// Script for auto collapsing marginal boxes
const startup_marginals = function () {
    let debounce_resize;
    let collapsedboxes = [];

    const getLineHeight = function (element) {
        var temp = document.createElement(element.nodeName),
            ret;
        temp.setAttribute("class", element.className);
        temp.innerHTML = "Üj";
        element.parentNode.appendChild(temp);
        ret = temp.getBoundingClientRect().height;
        temp.parentNode.removeChild(temp);
        return ret;
    };

    const collapsebox = function (element, height, lineheight) {
        element.style.maxHeight = height + "px";
        element.classList.add("ha-collapsed-box");
        element.classList.remove("ha-expanded-box");
        setTimeout(function () {
            element.classList.remove("transition-all");
        }, 130);
    };

    const uncollapsebox = function (element) {
        element.classList.add("transition-all");
        element.classList.remove("ha-collapsed-box");
        element.classList.add("ha-expanded-box");
    };

    const addbuttoncaollapsebox = function (element, height, hoverfunction, topmove) {
        let btn = document.createElement("div");
        btn.classList.add("ha-btn-collapsed-box");

        if (element.classList.contains("ha-collapsed-box")) {
            btn.classList.add("ha-open-btn-collapsed-box");
        } else {
            btn.classList.add("ha-close-btn-collapsed-box");
        }

        btn.addEventListener("click", function (ev) {
            ev.stopPropagation();
            if (element.classList.contains("ha-collapsed-box")) {
                uncollapsebox(element);
                if (topmove > 0) element.style.bottom = "5px";
                btn.classList.add("ha-close-btn-collapsed-box");
                btn.classList.add("ha-collapsed-box-manually-toggled");
            } else {
                collapsebox(element, height, 0);
                
                btn.classList.remove("ha-close-btn-collapsed-box");
                btn.classList.remove("ha-collapsed-box-manually-toggled");
            }
        });

        if (hoverfunction) {
            let timer = null;

            element.addEventListener("mouseenter", function (ev) {
                ev.stopPropagation();
                timer = setTimeout(function () {
                    if (element.classList.contains("ha-collapsed-box")) {
                        uncollapsebox(element);
                        if (topmove > 0) element.style.bottom = "5px";
                        btn.classList.add("ha-close-btn-collapsed-box");
                    }
                }, 80);
            });

            element.addEventListener("mouseleave", function (ev) {
                ev.stopPropagation();
                if (timer != null) {
                    clearTimeout(timer);
                }
                if (
                    element.classList.contains("ha-expanded-box") &&
                    !btn.classList.contains("ha-collapsed-box-manually-toggled")
                ) {
                    collapsebox(element, height, 0);
                    btn.classList.remove("ha-close-btn-collapsed-box");
                }
            });
        }
        element.parentNode.insertBefore(btn, element);
    };

    const overlappingcollapsebox = function (selector, hoverfunction, containerid) {
        let container = document.getElementById(containerid);
        if (!container) return;
        container.classList.add("overflow-hidden");
        let containerrect = document.getElementById(containerid).getBoundingClientRect();;
        let boxes = document.querySelectorAll(selector);
        let lineheight = 1;

        if (boxes.length >= 1) {
            lineheight = getLineHeight(boxes[0]);
        }

        for (var i = 0; i < boxes.length; i++) {
            let element = boxes[i];
            let thisrect = element.getBoundingClientRect();
            let overlap = -2;
            let topmove = 0;
            if (thisrect.bottom > containerrect.bottom) {
                overlap = thisrect.bottom - containerrect.bottom;
                topmove = thisrect.bottom - containerrect.bottom;
                console.log("topmove", topmove);
            } else if (i < boxes.length - 1) {
                let nextrect = boxes[i + 1].getBoundingClientRect();
                overlap = thisrect.bottom - nextrect.top;
            }
            if (
                // -1 for catching lines that perfectly close up on each other
                overlap >= -1 &&
                !(window.getComputedStyle(element).display === "none")
            ) {
                let newlength = 0;
                if (overlap >= 0)
                    newlength = thisrect.height - overlap;
                else
                    newlength = thisrect.height - lineheight;
                if (newlength % (lineheight * 3) <= 2)
                    newlength -= lineheight;
                let remainder = newlength % lineheight;
                newlength = newlength - remainder;

                // Line clamping for Marginals
                if (element.classList.contains("ha-marginalbox")) {
                    let marginals = element.querySelectorAll(".ha-marginal");
                    let h = 0;
                    for (let m of marginals) {
                        let cr = m.getBoundingClientRect();
                        let eh = cr.bottom - cr.top;
                        h += eh;
                        if (h >= newlength) {
                            let lines = Math.floor(eh / lineheight);
                            let cutoff = Math.floor((h - newlength) / lineheight);
                            m.style.cssText += "-webkit-line-clamp: " + (lines - cutoff) + ";";
                            m.style.cssText += "line-clamp: " + (lines - cutoff) + ";";
                        }
                    }
                }

                requestAnimationFrame(() => {
                    collapsedboxes.push(element);
                    collapsebox(element, newlength, lineheight);
                    addbuttoncaollapsebox(element, newlength, hoverfunction, topmove);
                });

            }
        }
    };

    const marginalboxwidthset = function (containerid, classes) {
        let lt = document.getElementById(containerid);
        if (lt !== null) {
            let mg = lt.querySelectorAll(classes);
            if (mg.length > 0) {
                let ltbcr = lt.getBoundingClientRect();
                let mgbcr = mg[0].getBoundingClientRect();
                let nw = ltbcr.right - mgbcr.left - 20;

                for (let element of mg) {
                    element.style.width = nw + "px";
                }
            }
        }
    };

    const clearcollapsedboxes = function () {
        let elements = document.querySelectorAll(".ha-marginalbox");
        elements.forEach(element => {
            element.removeAttribute("style");
        });
        collapsedboxes.forEach(element => {
            element.classList.remove("ha-expanded-box");
            element.classList.remove("ha-collapsed-box");
            element.outerHTML = element.outerHTML;
        });
        collapsedboxes = [];
        elements = document.querySelectorAll(".ha-btn-collapsed-box");
        elements.forEach(element => {
            element.remove();
        });
    };

    const resetall = function () {
        clearcollapsedboxes();
        marginalboxwidthset("ha-letterbody", ".ha-marginalbox");
        marginalboxwidthset("ha-register-body", ".ha-letlinks");
        startup_marginals();
    };

    const collapseboxes = function () {
        overlappingcollapsebox(".ha-letlinks", true, "ha-register-body");
        overlappingcollapsebox(".ha-marginalbox", true, "ha-letterbody");
    };

    window.addEventListener("resize", function () {
        clearTimeout(debounce_resize);
        debounce_resize = setTimeout(resetall, 17);
    });

    marginalboxwidthset("ha-letterbody", ".ha-marginalbox");
    marginalboxwidthset("ha-register-body", ".ha-letlinks");
    collapseboxes();
};

export { startup_marginals };
