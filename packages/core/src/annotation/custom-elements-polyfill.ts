import { WebComponent } from "./component.ts";

(function () {
  // initialize existing elements on a page
  for (const node of document.querySelectorAll("[is]")) {
    upgrade(node);
  }

  // setup observer to process elements added after the page load
  new MutationObserver(mutations => {
    for (const mutation of mutations) {
      for (const node of mutation.addedNodes) {
        if (isElement(node)) upgrade(node);
      }
    }
  }).observe(document.body, { childList: true, subtree: true });

  function upgrade(el: Element) {
    const is = el.getAttribute("is")!;
    const ctor = is && customElements.get(is);
    if (ctor && Object.getPrototypeOf(el) != ctor.prototype) {
      // use new prototype
      Object.setPrototypeOf(el, ctor!.prototype);

      // simulate callbacks if necessary
      if (hasConnectCallback(el)) el.connectedCallback();

      // track changes if necessary
      if (hasAttributeChangedCallback(el)) {
        const attributes: string[] = (ctor as any).observedAttributes;
        if (attributes && attributes.length) {
          new MutationObserver(attributeMutationCallback).observe(el, {
            attributes: true,
            attributeOldValue: el.attributeChangedCallback.length >= 2,
            attributeFilter: attributes,
          });
        }
      }
    }
  }
})();

function attributeMutationCallback(mutations: MutationRecord[]) {
  for (const mutation of mutations) {
    const name = mutation.attributeName!;
    (mutation.target as WebComponent).attributeChangedCallback!(name, mutation.oldValue, (mutation.target as Element).getAttribute(name));
  }
}

function isElement(el: Node): el is Element {
  return typeof (el as Element).getAttribute === "function";
}

function hasConnectCallback(el: Node): el is Node & Required<Pick<WebComponent, "connectedCallback">> {
  return typeof (el as WebComponent).connectedCallback === "function";
}

function hasAttributeChangedCallback(el: Node): el is Node & Required<Pick<WebComponent, "attributeChangedCallback">> {
  return typeof (el as WebComponent).attributeChangedCallback === "function";
}
