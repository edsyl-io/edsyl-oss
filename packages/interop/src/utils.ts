/**
 * Set the value of the property by the given path.
 * @param path - path to a property.
 * @param value - value to set.
 */
export function set(path: string, value: any) {
  const parts = path.split(".");
  const field = parts.pop()!;

  // locate target
  let target: any = window;
  for (const part of parts) {
    target = target[part];
  }

  // set value
  target[field] = value;
}

export function innerHtmlOf(selector: string) {
  return document.querySelector(selector)?.innerHTML;
}

export function saveAs(filename: string, content: BlobPart, options: BlobPropertyBag) {
  const blob = new Blob([content], options);

  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = filename;
  link.style.display = "none";

  const { body } = document;
  body.appendChild(link);
  link.click();
  body.removeChild(link);
}
