/**
 * Configuração centralizada de URLs da aplicação.
 * A URL base da API é lida a partir da variável de ambiente VITE_API_BASE_URL
 * definida no momento do build (em produção, configurada no painel ICP).
 * Em desenvolvimento local, o valor é vazio, usando rotas relativas (/api/...).
 */
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string) || '';

export function getApiBaseUrl(): string {
  return API_BASE_URL;
}

export function buildApiEndpoint(path: string): string {
  // Garante que o path começa com /
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  return `${API_BASE_URL}${normalizedPath}`;
}
