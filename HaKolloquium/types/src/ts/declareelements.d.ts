import { HaFooter } from "./hafooter";
import { HaMenu } from "./hamenu";
declare global {
    interface HTMLElementTagNameMap {
        'ha-menu': HaMenu;
        'ha-footer': HaFooter;
    }
}
export {};
