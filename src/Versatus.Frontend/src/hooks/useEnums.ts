import { useState, useEffect } from 'react';
import { buildApiEndpoint, getApiHeaders } from '../config/api';

export interface EnumOption {
  value: number;
  label: string;
}

const enumCache: Record<number, EnumOption[]> = {};
const pendingRequests: Record<number, Promise<EnumOption[]> | null> = {};

/**
 * Hook para buscar opções de um enumerado do banco de dados (GloTipoEnumerado).
 * Realiza cache em memória para evitar chamadas redundantes.
 * 
 * @param idPai Identificador do grupo pai do enumerado no banco de dados.
 */
export function useEnumOptions(idPai: number) {
  const [options, setOptions] = useState<EnumOption[]>(enumCache[idPai] || []);
  const [loading, setLoading] = useState<boolean>(!enumCache[idPai]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    if (enumCache[idPai]) {
      setOptions(enumCache[idPai]);
      setLoading(false);
      return;
    }

    const fetchOptions = async (): Promise<EnumOption[]> => {
      const response = await fetch(buildApiEndpoint(`/api/TipoEnumerado/${idPai}`), {
        headers: getApiHeaders({
          'Accept': 'application/json'
        })
      });

      if (!response.ok) {
        throw new Error(`Falha ao buscar enumerado ${idPai}`);
      }

      return response.json();
    };

    if (!pendingRequests[idPai]) {
      pendingRequests[idPai] = fetchOptions()
        .then(data => {
          enumCache[idPai] = data;
          pendingRequests[idPai] = null;
          return data;
        })
        .catch(err => {
          pendingRequests[idPai] = null;
          throw err;
        });
    }

    pendingRequests[idPai]!
      .then(data => {
        if (active) {
          setOptions(data);
          setLoading(false);
        }
      })
      .catch(err => {
        if (active) {
          setError(err instanceof Error ? err.message : 'Erro ao buscar enum');
          setLoading(false);
        }
      });

    return () => {
      active = false;
    };
  }, [idPai]);

  return { options, loading, error };
}
