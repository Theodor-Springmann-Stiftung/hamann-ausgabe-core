// Script for auto collapsing marginal boxes
const getLineHeight = function (element) {
    var temp = document.createElement(element.nodeName),
        ret;
    temp.setAttribute("class", element.className);
    temp.innerHTML = "Ü";
    element.parentNode.appendChild(temp);
    ret = temp.getBoundingClientRect().height;
    temp.parentNode.removeChild(temp);
    return ret;
};

const collapsebox = function (element, height, lineheight) {
    element.style.maxHeight = height + "px";
    element.classList.add("ha-collapsed-box");
    element.classList.remove("ha-expanded-box");
};

const uncollapsebox = function (element) {
    element.classList.remove("ha-collapsed-box");
    element.classList.add("ha-expanded-box");
};

const addbuttoncaollapsebox = function (element, height, hoverfunction) {
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
            btn.classList.remove("ha-open-btn-collapsed-box");
            btn.classList.add("ha-close-btn-collapsed-box");
            btn.classList.add("ha-collapsed-box-manually-toggled");
        } else {
            collapsebox(element, height);
            btn.classList.remove("ha-close-btn-collapsed-box");
            btn.classList.remove("ha-collapsed-box-manually-toggled");
            btn.classList.add("ha-open-btn-collapsed-box");
        }
    });

    if (hoverfunction) {
        let timer = null;

        element.addEventListener("mouseenter", function (ev) {
            ev.stopPropagation();
            timer = setTimeout(function () {
                if (element.classList.contains("ha-collapsed-box")) {
                    uncollapsebox(element);
                    btn.classList.remove("ha-open-btn-collapsed-box");
                    btn.classList.add("ha-close-btn-collapsed-box");
                }
            }, 200);
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
                collapsebox(element, height);
                btn.classList.remove("ha-close-btn-collapsed-box");
                btn.classList.add("ha-open-btn-collapsed-box");
            }
        });
    }
    element.parentNode.insertBefore(btn, element);
};

const overlappingcollapsebox = function (selector, hoverfunction) {
    let boxes = document.querySelectorAll(selector);
    let lineheight = 1;

    if (boxes.length >= 1) {
        lineheight = getLineHeight(boxes[0]);
    }

    for (var i = 0; i < boxes.length; i++) {
        if (i < boxes.length - 1) {
            let element = boxes[i];
            let thisrect = element.getBoundingClientRect();
            let nextrect = boxes[i + 1].getBoundingClientRect();
            let overlap = thisrect.bottom - nextrect.top;
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
                newlength = newlength - remainder - 1;

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
                        }
                    }
                }

                requestAnimationFrame(() => {
                    collapsebox(element, newlength, lineheight);
                });
                requestAnimationFrame(() => {
                    addbuttoncaollapsebox(element, newlength, hoverfunction);
                });
            }
        }
    }
};

const marginalboxwidthset = function () {
    let lt = document.getElementById("ha-letterbody");
    if (lt !== null) {
        let mg = lt.querySelectorAll(".ha-lettertext .ha-marginalbox");
        if (mg.length > 0) {
            let ltbcr = lt.getBoundingClientRect();
            let mgbcr = mg[0].getBoundingClientRect();
            let nw = ltbcr.right - mgbcr.left - 18;

            for (let element of mg) {
                element.style.width = nw + "px";
            }
        }
    }
}



const collapseboxes = function () {
    overlappingcollapsebox(".ha-neuzeit .ha-letlinks", true);
    overlappingcollapsebox(".ha-forschung .ha-letlinks", true);
    overlappingcollapsebox(".ha-commentlist .ha-letlinks", true);
    overlappingcollapsebox(".ha-lettertext .ha-marginalbox", true);
};

marginalboxwidthset();
collapseboxes();
var doit;
this.window.addEventListener("resize", function () {
    this.clearTimeout(doit);
    marginalboxwidthset();
    doit = this.setTimeout(collapseboxes, 250);
});
