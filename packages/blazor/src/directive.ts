import { ObserveCallbacks } from "./observe.ts";
import { watch } from "./mutation";
import { observe } from "./observe";

interface DirectiveType<T extends HTMLElement, R> {
  observe?: ObserveCallbacks<R>;
  new (el: T): R;
}

/**
 * Register a directive that automatically mounts on an HTML element with the provided attribute.
 * @param name - name of the HTML attribute watch for
 */
export function directive<T extends HTMLElement, R>(name: string) {
  return function (ctor: DirectiveType<T, R>) {
    watch(name, function (el: T): Disposable | any {
      const instance = new ctor(el);
      observe(el, ctor.observe, instance);
      return instance;
    });
  };
}
