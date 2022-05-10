import { html, LitElement } from 'lit'
import { customElement } from 'lit/decorators.js'

@customElement('ha-footer')
export class HaFooter extends LitElement {
  render() {
    return html`
      <div class="bg-slate-50 max-w-[1280px] mt-4 mx-auto p-2 md:p-4 lg:p-8 text-lg font-serif right text-right">
        <a href="/kontakt.html">Kontakt</a> · <a href="/datenschutzerklaerung.html">Datenschutzerklärung</a>
      </div>
    `
  }

  protected firstUpdated() {
    // Marks links as active
    var all_links = this.getElementsByTagName("a"),
    i=0, len=all_links.length,
    full_path = location.href.split('#')[0]; //Ignore hashes
    
    for(; i<len; i++) {
        if(all_links[i].href.split("#")[0] == full_path) {
            all_links[i].className += "active";
        }
    }
}

  protected createRenderRoot(): Element | ShadowRoot {
      return this;
  }
}