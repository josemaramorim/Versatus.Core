import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Autocomplete, CircularProgress, TextField } from '@mui/material';
import { buildApiEndpoint, getApiHeaders } from '../../../config/api';

/** Item de `GET /api/financeiro/lookups/...` (`ItemLookupDto`). */
export interface IItemLookup {
  id: number;
  descricao: string;
}

export interface ILookupFieldProps {
  label: string;
  /** Rota relativa, ex.: `/api/financeiro/lookups/agencias`. */
  endpoint: string;
  value: number | null;
  onChange: (id: number | null, item: IItemLookup | null) => void;
  required?: boolean;
  disabled?: boolean;
  error?: boolean;
  helperText?: React.ReactNode;
}

const buscar = async (endpoint: string, texto: string): Promise<IItemLookup[]> => {
  const url = `${buildApiEndpoint(endpoint)}?texto=${encodeURIComponent(texto)}`;
  const resposta = await fetch(url, { headers: getApiHeaders({ Accept: 'application/json' }) });
  return resposta.ok ? ((await resposta.json()) as IItemLookup[]) : [];
};

/**
 * Campo de busca (lookup) com floating label — substitui os LookupEdit do legado.
 * Busca no servidor pelo texto digitado (código ou descrição) e resolve a descrição do valor atual.
 */
export const LookupField: React.FC<ILookupFieldProps> = ({
  label,
  endpoint,
  value,
  onChange,
  required,
  disabled,
  error,
  helperText,
}) => {
  const [opcoes, setOpcoes] = useState<IItemLookup[]>([]);
  const [selecionado, setSelecionado] = useState<IItemLookup | null>(null);
  const [texto, setTexto] = useState('');
  const [carregando, setCarregando] = useState(false);
  const temporizador = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Resolve a descrição do valor carregado do registro.
  useEffect(() => {
    let ativo = true;
    if (value == null || value <= 0) {
      setSelecionado(null);
      return;
    }
    if (selecionado?.id === value) return;

    buscar(endpoint, String(value))
      .then((itens) => {
        if (ativo) setSelecionado(itens.find((i) => i.id === value) ?? { id: value, descricao: `#${value}` });
      })
      .catch(() => ativo && setSelecionado({ id: value, descricao: `#${value}` }));

    return () => {
      ativo = false;
    };
  }, [value, endpoint]); // eslint-disable-line react-hooks/exhaustive-deps

  // Busca com atraso enquanto o usuário digita.
  useEffect(() => {
    if (disabled) return;
    if (temporizador.current) clearTimeout(temporizador.current);
    temporizador.current = setTimeout(() => {
      setCarregando(true);
      buscar(endpoint, texto)
        .then(setOpcoes)
        .catch(() => setOpcoes([]))
        .finally(() => setCarregando(false));
    }, 300);
    return () => {
      if (temporizador.current) clearTimeout(temporizador.current);
    };
  }, [texto, endpoint, disabled]);

  const opcoesComSelecionado = useMemo(
    () => (selecionado && !opcoes.some((o) => o.id === selecionado.id) ? [selecionado, ...opcoes] : opcoes),
    [opcoes, selecionado],
  );

  return (
    <Autocomplete<IItemLookup>
      size="small"
      fullWidth
      disabled={disabled}
      options={opcoesComSelecionado}
      value={selecionado}
      filterOptions={(x) => x}
      getOptionLabel={(o) => o.descricao}
      isOptionEqualToValue={(a, b) => a.id === b.id}
      loading={carregando}
      noOptionsText="Nenhum registro encontrado"
      loadingText="Buscando..."
      onInputChange={(_, novoTexto, motivo) => {
        if (motivo === 'input') setTexto(novoTexto);
      }}
      onChange={(_, item) => {
        setSelecionado(item);
        onChange(item?.id ?? null, item);
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label={label}
          required={required}
          error={error}
          helperText={helperText}
          slotProps={{
            ...params.slotProps,
            input: {
              ...params.slotProps.input,
              endAdornment: (
                <>
                  {carregando ? <CircularProgress color="inherit" size={16} /> : null}
                  {params.slotProps.input.endAdornment}
                </>
              ),
            },
          }}
        />
      )}
    />
  );
};
