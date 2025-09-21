import { attr } from "./utils.ts";

export interface ObserveCallback<$ = any> {
  (this: $, value: string, old: string): void;
}

export type ObserveCallbacks<$ = any> = Record<string, ObserveCallback<$>>;

/**
 * Create {@link MutationObserver} to monitor element attribute modifications.
 * @param el - element containing attributes to observe.
 * @param callbacks - callbacks to call when attribute value changes.
 * @param $this - to be used as the `this` for every callback.
 */
export function observe<$ = any>(el: Element, callbacks?: ObserveCallbacks<$>, $this?: $) {
  if (callbacks) {
    return new MutationObserver(handler.bind([$this, callbacks])).observe(el, {
      attributes: true,
      attributeFilter: Object.keys(callbacks),
      attributeOldValue: Object.values(callbacks).some(requiresOldValue),
    });
  }
}

type Context = [any, ObserveCallbacks];

function handler(this: Context, mutations: MutationRecord[]) {
  mutations.forEach(process, this);
}

function process(this: Context, mutation: MutationRecord) {
  const [self, callbacks] = this;
  const name = mutation.attributeName!;
  const el = mutation.target as Element;
  const cb = callbacks[name] as Function;
  if (typeof cb == "function")
    switch (cb.length) {
      case 0:
        return cb.apply(self);
      case 1:
        return cb.call(self, attr(el, name));
      default:
        return cb.call(self, attr(el, name), mutation.oldValue!);
    }
}

function requiresOldValue(callback: ObserveCallback) {
  return callback.length > 1;
}
