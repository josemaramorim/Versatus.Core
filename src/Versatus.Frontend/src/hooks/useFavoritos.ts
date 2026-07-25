import { useState, useEffect, useCallback } from 'react';
import type { FavoritoDto } from '../types/menu';
import { buildApiEndpoint, getApiHeaders } from '../config/api';

export interface ToastState {
  open: boolean;
  message: string;
  severity: 'success' | 'error';
}

export function useFavoritos() {
  const [favoritos, setFavoritos] = useState<FavoritoDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [toast, setToast] = useState<ToastState>({
    open: false,
    message: '',
    severity: 'success'
  });

  const hideToast = useCallback(() => {
    setToast((prev) => ({ ...prev, open: false }));
  }, []);

  const showToast = useCallback((message: string, severity: 'success' | 'error') => {
    setToast({ open: true, message, severity });
  }, []);

  const fetchFavoritos = useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await fetch(buildApiEndpoint('/api/menu/favoritos'), {
        headers: getApiHeaders()
      });
      if (response.ok) {
        const data: FavoritoDto[] = await response.json();
        setFavoritos(data);
      }
    } catch (err) {
      console.error('Erro ao carregar favoritos:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const adicionarFavorito = useCallback(
    async (idRotina: number, nomeRotina?: string) => {
      try {
        const response = await fetch(buildApiEndpoint('/api/menu/favoritos'), {
          method: 'POST',
          headers: getApiHeaders({ 'Content-Type': 'application/json' }),
          body: JSON.stringify({ idRotina })
        });

        if (response.ok) {
          showToast(`"${nomeRotina || 'Item'}" adicionado aos favoritos!`, 'success');
          await fetchFavoritos();
        } else {
          showToast('Não foi possível adicionar aos favoritos.', 'error');
        }
      } catch (err) {
        console.error('Erro ao adicionar favorito:', err);
        showToast('Erro de conexão ao adicionar favorito.', 'error');
      }
    },
    [fetchFavoritos, showToast]
  );

  const removerFavorito = useCallback(
    async (idRotina: number, nomeRotina?: string) => {
      try {
        const response = await fetch(buildApiEndpoint(`/api/menu/favoritos/${idRotina}`), {
          method: 'DELETE',
          headers: getApiHeaders()
        });

        if (response.ok) {
          showToast(`"${nomeRotina || 'Item'}" removido dos favoritos!`, 'success');
          await fetchFavoritos();
        } else {
          showToast('Não foi possível remover o favorito.', 'error');
        }
      } catch (err) {
        console.error('Erro ao remover favorito:', err);
        showToast('Erro de conexão ao remover favorito.', 'error');
      }
    },
    [fetchFavoritos, showToast]
  );

  useEffect(() => {
    fetchFavoritos();
  }, [fetchFavoritos]);

  return {
    favoritos,
    isLoading,
    adicionarFavorito,
    removerFavorito,
    refetch: fetchFavoritos,
    toast,
    hideToast
  };
}
