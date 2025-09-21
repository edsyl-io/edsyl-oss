import { use } from "./mutation.ts";
import { observe, ObserveCallbacks } from "./observe.ts";

/**
 * Register a mediator by a particular name.
 * @param name - name of the mediator for further reference.
 */
export function mediator<R, T extends any[], E extends Element = HTMLElement>(name: string) {
  return function (ctor: MediatorType<R, T, E>) {
    registry[name] = ctor;
  };
}

/**
 * Mount mediator by a given name on the provided HTML element.
 * @param el - HTML element to hold the mediator instance.
 * @param name - name of the mediator to create.
 * @param args - arguments to pass for mediator constructor if any.
 */
export function mediate(el: HTMLElement, name: string, args: any[]): any {
  const ctor = registry[name];
  if (ctor) {
    const instance = new ctor(el, ...args);
    observe(el, ctor.observe, instance);
    return use(el, instance);
  }
}

const registry: Record<string, MediatorType> = {};
interface MediatorType<R = any, T extends any[] = any, E extends Element = any> {
  observe?: ObserveCallbacks<R>;
  new (el: E, ...args: T): R;
}
