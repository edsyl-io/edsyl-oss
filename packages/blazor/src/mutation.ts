export interface WatchCallback<T extends Element = Element> {
  /**
   * Handle a new HTML element occurence.
   * @param el - HTML element added to the document.
   * @returns resource to be disposed after HTML element removal.
   */
  (el: T): Disposable | any;
}

/**
 * Adds a disposable resource to the HTML element to be cleaned up upon element removal.
 * @param el - HTML resource to hold the resource.
 * @param resource - resource to be disposed after HTML element removal.
 */
export function use<T extends Disposable>(el: Element, resource: T): T {
  if (disposable(resource)) {
    ((el as any)[$disposables] ??= []).push(resource);
  }

  return resource;
}

/**
 * Watch when the provided attribute appears on the HTML elements.
 * @param attribute -
 * @param callback
 */
export function watch<T extends Element>(attribute: string, callback: WatchCallback<T>) {
  if (attribute in callbacks) throw new Error(`duplicate attribute watch: ` + attribute);
  callbacks[attribute] = callback;
  observer.observe(document, options);
}

const $disposables = Symbol("disposables");
const callbacks: Record<string, WatchCallback<any>> = {};
const observer = new MutationObserver(handleMutations);
const options: MutationObserverInit = { childList: true, subtree: true };

const removes = new Set<Element>();
const inserts = new Set<Element>();

function handleMutations(mutations: MutationRecord[]) {
  mutations.forEach(collect);
  inserts.forEach(initialize);
  removes.forEach(destroy);
  removes.clear();
  inserts.clear();
}

function collect(mutation: MutationRecord) {
  // start with removals since elements might be moved within the document
  mutation.addedNodes.forEach(handleAdded);
  mutation.removedNodes.forEach(handleRemoved);
}

function handleAdded(node: Node) {
  switch (node.nodeType) {
    case 1:
      inserts.add(node as Element);
      break;
  }
}

function handleRemoved(node: Node) {
  switch (node.nodeType) {
    case 1:
      removes.add(node as Element);
      break;
  }
}

let disposables: Disposable[] = [];

function initialize(el: Element) {
  // element has been moved within the document
  if (removes.delete(el)) return;

  for (const attr of el.attributes) {
    const instance = construct(el, callbacks[attr.name]);
    if (instance && Symbol.dispose in instance) {
      disposables.push(instance);
    }
  }

  if (disposables.length > 0) {
    (el as any)[$disposables] = disposables;
    disposables = [];
  }
}

function construct(el: Element, callback?: WatchCallback<any>): Disposable | any {
  return callback?.(el);
}

function disposable(resource: any): resource is Disposable {
  return resource && Symbol.dispose in resource;
}

function destroy(el: Element) {
  walk(el, finalize);
}

function finalize(el: Element) {
  ((el as any)[$disposables] as Disposable[])?.forEach(dispose);
}

function dispose(disposable: Disposable) {
  disposable[Symbol.dispose]();
}

function walk(el: Element, consume: (el: Element) => void) {
  const it = document.createNodeIterator(el, 1);
  for (let node = it.nextNode(); node; node = it.nextNode()) {
    consume(node as Element);
  }
}
