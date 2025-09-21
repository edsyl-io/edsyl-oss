import { Blazor } from "./types.ts";

/**
 * Dispatch {@link CustomEvent} to a target element.
 * @param el - target element.
 * @param name - name of the event.
 * @param detail - event payload.
 */
export function dispatch<T extends object>(el: HTMLElement, name: string, detail?: T) {
  el.dispatchEvent(new CustomEvent(name, { bubbles: true, detail }));
}

/**
 * Registers event to be recognized by Blazor.
 * @param browserEventName - name of the browser event.
 * @param eventName - name of the Blazor event handler in C#.
 * @param factory - optional factory to process event arguments.
 */
export function channel<T extends Event = CustomEvent>(browserEventName: string, eventName: string, factory?: EventFactory<T>): string {
  registry.push([browserEventName, eventName, factory]);
  return browserEventName;
}

/**
 * Registers all {@link channel} events to the provided blazor instance.
 * @param blazor - blazor runtime instance.
 */
export function registerEvents(blazor: Blazor) {
  while (registry.length > 0) {
    const [browserEventName, eventName, factory] = registry.pop()!;
    blazor.registerCustomEventType(eventName, {
      browserEventName,
      createEventArgs: factory ?? createEventArgs,
    });
  }
}

interface EventFactory<T extends Event> {
  (e: T): unknown;
}

type Registry = [string, string, EventFactory<any>?][];
const $registry = Symbol.for("__blazor__EventRegistry__");
const registry: Registry = ((global as any)[$registry] ??= []);

function createEventArgs(e: CustomEvent) {
  return e.detail;
}
