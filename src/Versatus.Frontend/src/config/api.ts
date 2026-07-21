/**
 * Configuração centralizada de URLs e segurança da aplicação.
 * As variáveis são lidas a partir do ambiente do Vite (injetadas no build).
 * Em desenvolvimento local, os valores padrão são usados.
 */
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string) || '';
const API_KEY = (import.meta.env.VITE_API_KEY as string) || '';

export function getApiBaseUrl(): string {
  return API_BASE_URL;
}

export function buildApiEndpoint(path: string): string {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  return `${API_BASE_URL}${normalizedPath}`;
}

/**
 * Retorna os cabeçalhos padrão para as requisições à API,
 * incluindo o X-Api-Key se configurado.
 */
export function getApiHeaders(extra?: Record<string, string>): Record<string, string> {
  const headers: Record<string, string> = { ...extra };
  if (API_KEY) {
    headers['X-Api-Key'] = API_KEY;
  }
  return headers;
}
