const openmenu = function () {
  var x = document.getElementById("ha-topnav");
  if (x !== null) x.className += " ha-topnav-collapsed";
  let oldbutton = document.getElementById("openmenubutton");
  if (oldbutton !== null) oldbutton.setAttribute("class", "hidden");
  let newbutton = document.getElementById("closemenubutton");
  if (newbutton !== null) newbutton.setAttribute("class", "");
};

const closemenu = function () {
  var x = document.getElementById("ha-topnav");
  if (x !== null) x.className = "ha-topnav";
  let oldbutton = document.getElementById("closemenubutton");
  if (oldbutton !== null) oldbutton.setAttribute("class", "hidden");
  let newbutton = document.getElementById("openmenubutton");
  if (newbutton !== null) newbutton.setAttribute("class", "");
};

const markactive_startswith = function (element) {
  // Marks links as active which target URL starts with the current URL
  var all_links = element.getElementsByTagName("a"),
    i = 0,
    len = all_links.length,
    full_path = location.href.split("#")[0].toLowerCase(); //Ignore hashes

  for (; i < len; i++) {
    if (full_path.startsWith(all_links[i].href.toLowerCase())) {
      all_links[i].className += " active";
    }
  }
};

const markactive_exact = function (element) {
  var all_links = element.getElementsByTagName("a"),
    i = 0,
    len = all_links.length,
    full_path = location.href.split("#")[0].toLowerCase(); //Ignore hashes

  for (; i < len; i++) {
    if (full_path == all_links[i].href.toLowerCase()) {
      all_links[i].className += " active";
    }
  }
};

const getLineHeight = function (element) {
  var temp = document.createElement(element.nodeName),
    ret;
  temp.setAttribute("class", element.className);
  temp.innerHTML = "A";

  element.parentNode.appendChild(temp);
  ret = temp.clientHeight;
  temp.parentNode.removeChild(temp);

  return ret;
};

const collapsebox = function (element, height) {
  element.style.maxHeight = height + "px";
  element.classList.add("ha-collapsed-box");
  element.classList.remove("ha-expanded-box");
};

const uncollapsebox = function (element) {
  element.classList.remove("ha-collapsed-box");
  element.classList.add("ha-expanded-box");
};

const addbuttoncaollapsebox = function (element, height, hoverfunction) {
  const btn = document.createElement("div");
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

  //element.appendChild(btn);
  //element.insertBefore(btn, element.firstChild);
  element.parentNode.insertBefore(btn, element);
};

/* TODO: need a resize watcher to undo and reapply the effect on breakpoint */
const overlappingcollapsebox = function (selector, hoverfunction) {
  let boxes = document.querySelectorAll(selector);
  let clientrects = [];
  let lineheight = 1;

  if (boxes.length >= 1) {
    lineheight = getLineHeight(boxes[0]);
  }

  for (element of boxes) {
    clientrects.push([element, element.getBoundingClientRect()]);
  }

  for (var i = 0; i < clientrects.length; i++) {
    if (i < clientrects.length - 1) {
      let overlap = clientrects[i][1].bottom - clientrects[i + 1][1].top;
      if (overlap >= 0) {
        let newlength = clientrects[i][1].height - overlap;
        let remainder = newlength % lineheight;
        newlength = newlength - remainder;
        collapsebox(clientrects[i][0], newlength);
        addbuttoncaollapsebox(clientrects[i][0], newlength, hoverfunction);
      }
    }
  }
};

window.addEventListener("load", function () {
  if (
    document.getElementById("openmenubutton") != null &&
    document.getElementById("closemenubutton") != null
  ) {
    document
      .getElementById("openmenubutton")
      .addEventListener("click", openmenu);
    document
      .getElementById("closemenubutton")
      .addEventListener("click", closemenu);
  }
  if (document.getElementById("ha-topnav") != null)
    markactive_startswith(document.getElementById("ha-topnav"));
  if (document.getElementById("ha-register-nav") != null)
    markactive_exact(document.getElementById("ha-register-nav"));
  overlappingcollapsebox(".ha-neuzeit .ha-letlinks", true);
  overlappingcollapsebox(".ha-forschung .ha-letlinks", true);
  overlappingcollapsebox(".ha-lettertext .ha-marginalbox", true);
});
