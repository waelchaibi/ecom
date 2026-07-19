import { environment } from '../../../environments/environment';

/** API origin without trailing slash (e.g. https://localhost:7018). */
export function apiOrigin(): string {
  return environment.apiBaseUrl.replace(/\/api\/?$/, '');
}

/**
 * Resolves product/cart image paths for display.
 * Absolute http(s) URLs (seed Unsplash links) pass through;
 * uploaded paths like /uploads/products/x.jpg are prefixed with the API host.
 */
export function resolveMediaUrl(url: string | null | undefined): string | null {
  if (!url?.trim()) return null;
  const value = url.trim();
  if (/^https?:\/\//i.test(value) || value.startsWith('data:')) {
    return value;
  }
  if (value.startsWith('/')) {
    return `${apiOrigin()}${value}`;
  }
  return `${apiOrigin()}/${value}`;
}
