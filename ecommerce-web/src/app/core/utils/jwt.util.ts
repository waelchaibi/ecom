export interface JwtPayload {
  exp?: number;
  role?: string | string[];
  [key: string]: unknown;
}

export function parseJwtPayload(token: string): JwtPayload | null {
  try {
    const part = token.split('.')[1];
    if (!part) return null;
    const json = atob(part.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

export function jwtRoles(payload: JwtPayload): string[] {
  const roles: string[] = [];
  const short = payload.role;
  if (typeof short === 'string') roles.push(short);
  if (Array.isArray(short)) roles.push(...short.filter((r): r is string => typeof r === 'string'));

  const longKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  const long = payload[longKey];
  if (typeof long === 'string') roles.push(long);
  if (Array.isArray(long)) roles.push(...long.filter((r): r is string => typeof r === 'string'));

  return roles;
}

export function isJwtExpired(payload: JwtPayload): boolean {
  if (!payload.exp) return true;
  return payload.exp * 1000 <= Date.now();
}

export function jwtHasRole(token: string | null, role: string): boolean {
  if (!token) return false;
  const payload = parseJwtPayload(token);
  if (!payload || isJwtExpired(payload)) return false;
  return jwtRoles(payload).includes(role);
}
