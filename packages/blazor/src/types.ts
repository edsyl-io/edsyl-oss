export interface Blazor {
  registerCustomEventType<E extends Event>(eventName: string, options: EventTypeOptions<E>): void;
}

export interface DotNet {
  createJSObjectReference(instance: any): JsRef;
  disposeJSObjectReference(reference: JsRef): any;
}

export interface DotNetRef {
  invokeMethod(name: string, ...args: any): unknown;
  invokeMethodAsync(name: string, ...args: any): Promise<unknown>;
}

export type JsRef = unknown;

export interface EventTypeOptions<E extends Event = Event> {
  browserEventName?: string;
  createEventArgs?: (event: E) => unknown;
}
