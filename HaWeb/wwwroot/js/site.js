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
  temp.innerHTML = "Ü";
  element.parentNode.appendChild(temp);
  ret = temp.getBoundingClientRect().height;
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

/* TODO: need a resize watcher to undo and reapply the effect on breakpoint */
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
      let nextrect = boxes[i+1].getBoundingClientRect();
      let overlap = thisrect.bottom - nextrect.top;
      if (overlap >= 0 && !(window.getComputedStyle(element).display === "none")) {
        let newlength = thisrect.height - overlap;
        let remainder = newlength % lineheight;
        newlength = newlength - remainder - 1;
        requestAnimationFrame(() => { collapsebox(element, newlength) });
        requestAnimationFrame(() => { addbuttoncaollapsebox(element, newlength, hoverfunction) });
      }
    }
  }
};

/* Button to hide / show traditions, marginals and the text of the letter */
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
        if (hiddenelement !== null) hiddenelement.classList.add("hide");
      }

      if (button !== null) button.classList.add("active");
      if (div !== null) div.classList.remove("hide");
    });
  }
};

// Functions for switching theme
const go_to_dark = function() {
  localStorage.setItem('theme', 'ha-toggledark');
  document.documentElement.classList.add('dark');
}

const go_to_twilight = function() {
  document.documentElement.classList.remove('dark')
  let elements = document.getElementsByClassName("ha-twilighttogglebar");
  for (let el of elements) {
    el.classList.add("dark");
  }
  localStorage.setItem('theme', 'ha-toggletwilight');
}

const go_to_bright = function() {
  document.documentElement.classList.remove('dark');
  let elements = document.getElementsByClassName("ha-twilighttogglebar");
  for (let el of elements) {
    el.classList.remove("dark");
  }
  localStorage.setItem('theme', 'ha-togglebright');
}

// Functions for reading theme settings
const get_theme_settings = function(standard) {
  var theme = localStorage.getItem('theme');
  if (theme === null) theme = standard;
  let toggleSwitch = document.getElementById(theme).click();
}

const collapseboxes = function() {
  overlappingcollapsebox(".ha-neuzeit .ha-letlinks", true);
  overlappingcollapsebox(".ha-forschung .ha-letlinks", true);
  overlappingcollapsebox(".ha-lettertext .ha-marginalbox", true);
}


//////////////////////////////// ONLOAD ////////////////////////////////////
window.addEventListener("load", function () {
  // Menu: Show / Hide Buttons for mobile View
  if (
    document.getElementById("openmenubutton") !== null &&
    document.getElementById("closemenubutton") !== null
  ) {
    document
      .getElementById("openmenubutton")
      .addEventListener("click", openmenu);
    document
      .getElementById("closemenubutton")
      .addEventListener("click", closemenu);
  }

  // Menu / Register / Search View: Mark active link
  if (document.getElementById("ha-topnav") != null)
    markactive_startswith(document.getElementById("ha-topnav"));
  if (document.getElementById("ha-register-nav") != null)
    markactive_exact(document.getElementById("ha-register-nav"));

  // Letter / Register View: Collapse all unfit boxes + resize observer
  collapseboxes();
  var doit;
  this.window.addEventListener('resize', function() {
    this.clearTimeout(doit);
    this.setTimeout(collapseboxes, 250);
  });


  // Letter View: Show / Hide Tabs
  let buttonlist = ["ha-lettertextbtn", "ha-additionsbtn", "ha-marginalsbtn"];
  let divlist = ["ha-lettertext", "ha-additions", "ha-marginals"];

  if (this.document.getElementById("ha-lettertextbtn") !== null) {
    showhidebutton("ha-lettertextbtn", "ha-lettertext", buttonlist, divlist, false);
    showhidebutton("ha-additionsbtn", "ha-additions", buttonlist, divlist, true);
    showhidebutton("ha-marginalsbtn", "ha-marginals", buttonlist, divlist, true);
  } else {
    showhidebutton("ha-lettertextbtn", "ha-lettertext", buttonlist, divlist, true);
    showhidebutton("ha-additionsbtn", "ha-additions", buttonlist, divlist, false);
    showhidebutton("ha-marginalsbtn", "ha-marginals", buttonlist, divlist, true);
  }

  // Theme: Get saved theme from memory and check the box accordingly
  // Register theme toggler
  if (
    document.getElementById("ha-togglebright") !== null &&
    document.getElementById("ha-toggletwilight") !== null &&
    this.document.getElementById("ha-toggledark") !== null
  ) {
    document
      .getElementById("ha-togglebright")
      .addEventListener("click", go_to_bright);
    document
      .getElementById("ha-toggledark")
      .addEventListener("click", go_to_dark);
    document
      .getElementById("ha-toggletwilight")
      .addEventListener("click", go_to_twilight);
  }
  get_theme_settings("ha-togglebright");
});

// import resolveConfig from 'tailwindcss/resolveConfig'
// import tailwindConfig from './tailwind.config.js'

// const fullConfig = resolveConfig(tailwindConfig)

// fullConfig.theme.width[4]
// // => '1rem'

// fullConfig.theme.screens.md
// // => '768px'

// fullConfig.theme.boxShadow['2xl']
// // => '0 25px 50px -12px rgba(0, 0, 0, 0.25)'
