import "../css/site.css";
import { startup_marginals } from "./marginals.mjs";
import { startup_theme } from "./theme.mjs";
import { startup_menu } from "./menu.mjs";
import { startup_markanchor } from "./anchor.mjs";
import { startup_mobilemenu } from "./mobilemenu.mjs";
import { startup_scrollbutton } from "./scrollbutton.mjs";
import { startup_clipboard } from "./clipboard.mjs";
import { XMLStateHelper } from "./filelistform.mjs";
import { startup_index } from "./index.mjs";
import { startup_briefe } from "./briefe.mjs";
import { startup_websocket } from "./websocket.mjs";
import { startup_search } from "./search.mjs";

const startup_default = function () {
  startup_marginals();
  startup_theme();
  startup_scrollbutton();
  startup_menu();
  startup_mobilemenu();
  startup_markanchor();
  startup_clipboard();
};

export {
  startup_search,
  startup_websocket,
  startup_briefe,
  startup_index,
  XMLStateHelper,
  startup_default,
  startup_clipboard,
  startup_mobilemenu,
  startup_markanchor,
  startup_menu,
  startup_scrollbutton,
  startup_marginals,
  startup_theme,
};

