import { html, LitElement } from 'lit'
import { customElement, property } from 'lit/decorators.js'

@customElement('ha-menu')
export class HaMenu extends LitElement {

  @property()
  title = 'Internationales Hamann Kolloquium'

  render() {
    return html`
      <header class="sticky w-100 font-serif mb-6">
        <nav class="flex flex-row w-100 bg-slate-50 border-t-[5px] border-gray-300 py-4 px-3 lg:px-8 flex-wrap">
          <a href="/" class="text-xl 2xl:text-4xl grow-0 shrink-0 text-black hover:text-black">
            ${this.title}
          </a>
          <div class="flex grow shrink">
      
          </div>
          <div class="main__item inline-block lg:hidden pr-3 align-bottom">
            <a href="javascript:void(0);" @click=${this._burgerclick}>
              <svg role="img" id="openmenubutton" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"
                aria-labelledby="hamburgerIconTitle">
                <title id="hamburgerIconTitle">Menu</title>
                <path d="M6 7L18 7M6 12L18 12M6 17L18 17"></path>
              </svg>
              <svg class="hidden" role="img" id="closemenubutton" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"
                aria-labelledby="closeIconTitle">
                <title id="closeIconTitle">Close</title>
                <path d="M6.34314575 6.34314575L17.6568542 17.6568542M6.34314575 17.6568542L17.6568542 6.34314575"></path>
              </svg>
            </a>
          </div>
          <div class="topnav" id="topnav">
            <a class="" href="/">
              Aktuelles
            </a>
            <a class="" href="/kolloquien.html">
              Hamann-Kolloquien
            </a>
            <a class="" href="/acta-baende.html">
              Acta-Bände
            </a>
            <a class="" href="/studien.html">
              Hamann-Studien
            </a>
            <a class="" href="https://hamann-ausgabe.de/Supplementa/Forschung">
              Forschungsbibliographie
            </a>
            <a class="pr-2 2xl:pr-4" href="https://www.hamann-ausgabe.de">
              Kommentierte Briefausgabe (&thinsp;H&thinsp;K&thinsp;B&thinsp;)
            </a>
          </div>
        </nav>
      </header>
    `
  }

  private _burgerclick() {
    var x = document.getElementById("topnav");
    if (x !== null)
      if (x.className === "topnav") {
        x.className += " responsive";
        let oldbutton = document.getElementById("openmenubutton");
        if (oldbutton !== null) {
          oldbutton.setAttribute('class', 'hidden');
        }
        let newbutton = document.getElementById("closemenubutton");
        if (newbutton !== null) {
          newbutton.setAttribute('class', '');
        }
      } else {
        x.className = "topnav";
        let oldbutton = document.getElementById("closemenubutton");
        if (oldbutton !== null) {
          oldbutton.setAttribute('class', 'hidden');
        }
        let newbutton = document.getElementById("openmenubutton");
        if (newbutton !== null) {
          newbutton.setAttribute('class', '');
        }
      }
  }

  protected firstUpdated() {
    // Marks links as active
    var all_links = this.getElementsByTagName("a"),
      i = 0, len = all_links.length,
      full_path = location.href.split('#')[0]; //Ignore hashes

    for (; i < len; i++) {
      if (all_links[i].href.split("#")[0] == full_path) {
        all_links[i].className += " active";
      }
    }
  }

  protected createRenderRoot(): Element | ShadowRoot {
    return this;
  }
}