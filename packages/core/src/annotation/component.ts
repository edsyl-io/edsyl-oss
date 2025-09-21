import "./custom-elements-polyfill";

export interface WebComponent {
  connectedCallback?(): void;
  attributeChangedCallback?(name: string, oldValue: string | null, newValue: string | null): void;
}

export interface ComponentOptions {
  id?: string;
  classes?: string[];
  styles?: Partial<CSSStyleDeclaration>;
  observe?: string[];
}

/** Register a custom element with provided options. */
export function Component(name: string, { id, classes, styles, observe }: ComponentOptions = {}) {
  return function (target: CustomElementConstructor) {
    const { connectedCallback } = target.prototype;

    if (observe) {
      (target as any).observedAttributes = observe;
    }

    if (id || classes || styles)
      (target.prototype as WebComponent).connectedCallback = function (this: HTMLElement) {
        if (id) this.id = id;
        if (classes) this.classList.add(...classes);
        if (styles) Object.assign(this.style, styles);
        connectedCallback?.apply(this);
      };

    customElements.define(name, target);
  };
}
