import { useState, useEffect, useCallback } from 'react';
import type { ModuloMenuDto } from '../types/menu';
import { buildApiEndpoint, getApiHeaders } from '../config/api';

const LOCAL_STORAGE_KEY_MODULO = 'versatus_modulo_ativo';

export function useMenuArvore() {
  const [modulos, setModulos] = useState<ModuloMenuDto[]>([]);
  const [moduloAtivo, setModuloAtivoState] = useState<ModuloMenuDto | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const setModuloAtivo = useCallback((modulo: ModuloMenuDto) => {
    setModuloAtivoState(modulo);
    try {
      localStorage.setItem(LOCAL_STORAGE_KEY_MODULO, JSON.stringify(modulo.idModulo));
    } catch (e) {
      console.warn('Erro ao salvar módulo ativo no localStorage:', e);
    }
  }, []);

  const fetchArvore = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await fetch(buildApiEndpoint('/api/menu/arvore'), {
        headers: getApiHeaders()
      });

      if (!response.ok) {
        throw new Error(`Erro ao carregar menu (${response.status})`);
      }

      const data: ModuloMenuDto[] = await response.json();
      setModulos(data);

      if (data.length > 0) {
        let moduloSelecionado: ModuloMenuDto | undefined;

        // 1. Tenta encontrar módulo correspondente à URL atual no carregamento
        const currentPath = window.location.pathname;
        if (currentPath && currentPath !== '/') {
          moduloSelecionado = data.find(
            (m) => m.prefixoRota && m.prefixoRota !== '/' && currentPath.startsWith(m.prefixoRota)
          );
        }

        // 2. Se não encontrou pela URL, busca no localStorage
        if (!moduloSelecionado) {
          try {
            const savedId = localStorage.getItem(LOCAL_STORAGE_KEY_MODULO);
            if (savedId) {
              const parsedId = JSON.parse(savedId);
              moduloSelecionado = data.find((m) => m.idModulo === parsedId);
            }
          } catch {
            // Fallback
          }
        }

        // 3. Fallback para Acesso Global (id 1) ou primeiro módulo
        if (!moduloSelecionado) {
          moduloSelecionado = data.find((m) => m.idModulo === 1) || data[0];
        }

        setModuloAtivoState(moduloSelecionado);
      }
    } catch (err: any) {
      console.error(err);
      setError(err?.message || 'Falha ao carregar árvore de menus');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchArvore();
  }, [fetchArvore]);

  return {
    modulos,
    moduloAtivo,
    setModuloAtivo,
    isLoading,
    error,
    refetch: fetchArvore
  };
}
