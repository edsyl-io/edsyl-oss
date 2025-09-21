/**
 * Get attribute by name.
 * @param el - element to get attribute from.
 * @param name - name of the attribute to get.
 */
export function attr(el: Element, name: string) {
  return el.getAttribute(name);
}
