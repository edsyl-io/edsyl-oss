export { mediate } from "./src";

export function invoke(el: any, name: string, args: any) {
  return (el[name] as Function).apply(el, args);
}

export function identity<T>(x: T) {
  return x;
}
