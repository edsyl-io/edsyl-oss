import { Blazor, registerEvents } from "./src";

/** @see https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/startup#javascript-initializers */
export function afterStarted(blazor: Blazor) {
  registerEvents(blazor);
}
