import React, { useState, useEffect, useMemo, useCallback } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Checkbox,
  FormControlLabel,
  Stack,
  CircularProgress,
  Divider,
  Alert,
  Snackbar,
  Card,
  CardContent,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import SaveIcon from '@mui/icons-material/Save';
import RefreshIcon from '@mui/icons-material/Refresh';
import SearchIcon from '@mui/icons-material/Search';

import { buildApiEndpoint, getApiHeaders } from '../../../config/api';
import type { IParametroForm } from './types';

const AGRUPADOR_NAMES: Record<number, string> = {
  0: 'Geral',
  1: 'Faturamento & Vendas',
  2: 'Financeiro & Contábil',
  3: 'Estoque & Materiais',
  4: 'Segurança & Auditoria',
  5: 'Fiscal & NFe',
  10: 'Configurações Globais',
};

const getAgrupadorLabel = (id: number) => AGRUPADOR_NAMES[id] || `Módulo de Configuração ${id}`;

interface PerfilOption {
  idPerfil: number;
  descricao: string;
}

// Subcomponente dinâmico para renderizar opções de enumerado carregadas do backend
const EnumField: React.FC<{
  objeto: string;
  value: string;
  onChange: (val: string) => void;
  disabled: boolean;
}> = ({ objeto, value, onChange, disabled }) => {
  const [options, setOptions] = useState<{ value: string; label: string }[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!objeto) return;
    setLoading(true);
    fetch(buildApiEndpoint(`/api/parametro/enum-opcoes?enumNome=${objeto}`), {
      headers: getApiHeaders(),
    })
      .then((res) => {
        if (!res.ok) throw new Error();
        return res.json();
      })
      .then((data) => {
        setOptions(data);
        setLoading(false);
      })
      .catch(() => setLoading(false));
  }, [objeto]);

  return (
    <FormControl fullWidth size="small" disabled={disabled || loading}>
      <InputLabel id={`enum-select-${objeto}-label`}>
        {loading ? 'Carregando...' : 'Selecione'}
      </InputLabel>
      <Select
        labelId={`enum-select-${objeto}-label`}
        value={value || ''}
        onChange={(e) => onChange(e.target.value as string)}
        label={loading ? 'Carregando...' : 'Selecione'}
      >
        <MenuItem value="">
          <em>Nenhum</em>
        </MenuItem>
        {options.map((opt) => (
          <MenuItem key={opt.value} value={opt.value}>
            {opt.label}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
};

export const FParametro: React.FC = () => {
  // Estado de escopo
  const [escopo, setEscopo] = useState<number>(159); // Padrão: 159 = Sistema
  const [perfis, setPerfis] = useState<PerfilOption[]>([]);
  const [perfilId, setPerfilId] = useState<number | ''>('');

  // Estado dos parâmetros
  const [parametros, setParametros] = useState<IParametroForm[]>([]);
  const [originalParametros, setOriginalParametros] = useState<IParametroForm[]>([]);
  const [loading, setLoading] = useState(false);

  // Filtros de busca
  const [searchChave, setSearchChave] = useState('');
  const [searchDescricao, setSearchDescricao] = useState('');

  // Notificações
  const [notification, setNotification] = useState<{
    open: boolean;
    message: string;
    severity: 'success' | 'error' | 'info';
  }>({
    open: false,
    message: '',
    severity: 'success',
  });

  // Carregar perfis se escopo for Perfil
  useEffect(() => {
    fetch(buildApiEndpoint('/api/parametro/perfis'), {
      headers: getApiHeaders(),
    })
      .then((res) => res.json())
      .then((data) => setPerfis(data))
      .catch(() => console.error('Erro ao buscar perfis.'));
  }, []);

  // Carregar parâmetros quando escopo ou perfil mudam
  const carregarParametros = useCallback(() => {
    if (escopo === 161 && perfilId === '') {
      setParametros([]);
      return;
    }

    setLoading(true);
    const url = buildApiEndpoint(
      `/api/parametro/escopo?tipoParametro=${escopo}&idPerfil=${escopo === 161 ? perfilId : ''}`
    );

    fetch(url, { headers: getApiHeaders() })
      .then((res) => {
        if (!res.ok) throw new Error('Erro ao carregar parâmetros');
        return res.json();
      })
      .then((data: any[]) => {
        const mapped = data.map((item) => ({
          id: item.id,
          chave: item.chave,
          descricao: item.descricao,
          valor: item.valor || '',
          tipo: item.tipo,
          agrupador: item.agrupador,
          visivel: item.visivel,
          idRotina: item.idRotina,
          tipoParametro: item.tipoParametro,
          idParametroValor: item.idParametroValor,
          valorConfigurado: item.valorConfigurado || '',
          marcado: item.marcado,
        }));
        setParametros(mapped);
        setOriginalParametros(JSON.parse(JSON.stringify(mapped)));
        setLoading(false);
      })
      .catch((err) => {
        setNotification({
          open: true,
          message: err.message || 'Erro ao carregar os parâmetros.',
          severity: 'error',
        });
        setLoading(false);
      });
  }, [escopo, perfilId]);

  useEffect(() => {
    carregarParametros();
  }, [carregarParametros]);

  // Alterações de valores inline
  const handleMarcadoChange = (id: number, checked: boolean) => {
    setParametros((prev) =>
      prev.map((p) => {
        if (p.id === id) {
          return {
            ...p,
            marcado: checked,
            // Se desmarcar, limpa o valor configurado
            valorConfigurado: checked ? p.valorConfigurado : '',
          };
        }
        return p;
      })
    );
  };

  const handleValorChange = (id: number, val: string) => {
    setParametros((prev) =>
      prev.map((p) => (p.id === id ? { ...p, valorConfigurado: val } : p))
    );
  };

  // Filtragem local dos parâmetros na memória
  const filteredParametros = useMemo(() => {
    return parametros.filter((p) => {
      const matchChave = p.chave.toLowerCase().includes(searchChave.toLowerCase());
      const matchDesc = p.descricao.toLowerCase().includes(searchDescricao.toLowerCase());
      return matchChave && matchDesc;
    });
  }, [parametros, searchChave, searchDescricao]);

  // Agrupamento por Categoria (Agrupador)
  const groupedParametros = useMemo(() => {
    const groups: Record<number, IParametroForm[]> = {};
    filteredParametros.forEach((p) => {
      if (!groups[p.agrupador]) {
        groups[p.agrupador] = [];
      }
      groups[p.agrupador].push(p);
    });
    return groups;
  }, [filteredParametros]);

  // Verificar se há alterações para habilitar botão salvar
  const temAlteracoes = useMemo(() => {
    return JSON.stringify(parametros) !== JSON.stringify(originalParametros);
  }, [parametros, originalParametros]);

  // Salvar alterações em lote
  const handleSave = () => {
    const alterados = parametros.filter((p, index) => {
      const orig = originalParametros[index];
      return p.marcado !== orig.marcado || p.valorConfigurado !== orig.valorConfigurado;
    });

    if (alterados.length === 0) return;

    setLoading(true);
    const body = {
      tipoParametro: escopo,
      idPerfil: escopo === 161 ? perfilId : null,
      valores: alterados.map((p) => ({
        idParametro: p.id,
        valorConfigurado: p.valorConfigurado,
        marcado: p.marcado,
      })),
    };

    fetch(buildApiEndpoint('/api/parametro/salvar-valores'), {
      method: 'PUT',
      headers: getApiHeaders({
        'Content-Type': 'application/json',
      }),
      body: JSON.stringify(body),
    })
      .then((res) => {
        if (!res.ok) throw new Error('Erro ao salvar os parâmetros.');
        setNotification({
          open: true,
          message: 'Parâmetros atualizados com sucesso!',
          severity: 'success',
        });
        carregarParametros();
      })
      .catch((err) => {
        setNotification({
          open: true,
          message: err.message || 'Erro ao persistir alterações.',
          severity: 'error',
        });
        setLoading(false);
      });
  };

  const handleCancel = () => {
    setParametros(JSON.parse(JSON.stringify(originalParametros)));
  };

  // Renderizar o controle de input correto baseado no tipo
  const renderInputControl = (p: IParametroForm) => {
    const disabled = !p.marcado;
    const value = p.valorConfigurado || '';
    const onChange = (val: string) => handleValorChange(p.id, val);

    switch (p.tipo) {
      case 156: // Smallint / Boolean (Sim/Não)
        return (
          <FormControl fullWidth size="small" disabled={disabled}>
            <InputLabel id={`select-bool-${p.id}-label`}>Opção</InputLabel>
            <Select
              labelId={`select-bool-${p.id}-label`}
              value={value === 'Sim' || value === 'True' || value === 'true' ? 'Sim' : value === 'Não' || value === 'False' || value === 'false' ? 'Não' : ''}
              onChange={(e) => onChange(e.target.value as string)}
              label="Opção"
            >
              <MenuItem value="">
                <em>Nenhum</em>
              </MenuItem>
              <MenuItem value="Sim">Sim</MenuItem>
              <MenuItem value="Não">Não</MenuItem>
            </Select>
          </FormControl>
        );

      case 157: // DateTime (Data)
        return (
          <TextField
            type="date"
            size="small"
            fullWidth
            value={value}
            disabled={disabled}
            onChange={(e) => onChange(e.target.value)}
            slotProps={{
              inputLabel: { shrink: true }
            }}
          />
        );

      case 153: // Int (Inteiro)
        return (
          <TextField
            type="number"
            size="small"
            fullWidth
            value={value}
            disabled={disabled}
            onChange={(e) => onChange(e.target.value)}
            slotProps={{
              htmlInput: { step: 1 }
            }}
          />
        );

      case 154: // Numeric (Decimal)
        return (
          <TextField
            type="number"
            size="small"
            fullWidth
            value={value}
            disabled={disabled}
            onChange={(e) => onChange(e.target.value)}
            slotProps={{
              htmlInput: { step: 0.01 }
            }}
          />
        );

      case 234: // Enumerado
        return (
          <EnumField
            objeto={p.valor} // p.valor no C# contém o nome do enum (Ex: StatusCampo)
            value={value}
            disabled={disabled}
            onChange={onChange}
          />
        );

      default: // String ou outros
        return (
          <TextField
            size="small"
            fullWidth
            value={value}
            disabled={disabled}
            onChange={(e) => onChange(e.target.value)}
            placeholder="Digite o valor..."
          />
        );
    }
  };

  return (
    <Box sx={{ p: 3, display: 'flex', flexDirection: 'column', gap: 3 }}>
      {/* Cabeçalho da Página */}
      <Paper sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 'bold', color: 'primary.main' }}>
          Configuração de Parâmetros
        </Typography>

        <Stack direction="row" spacing={2}>
          <Button
            variant="contained"
            color="primary"
            startIcon={<SaveIcon />}
            disabled={!temAlteracoes || loading}
            onClick={handleSave}
          >
            Confirmar (Salvar)
          </Button>
          <Button
            variant="outlined"
            color="secondary"
            startIcon={<RefreshIcon />}
            disabled={!temAlteracoes || loading}
            onClick={handleCancel}
          >
            Cancelar
          </Button>
        </Stack>
      </Paper>

      {/* Seletores de Escopo e Filtros */}
      <Card>
        <CardContent>
          <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 3, alignItems: 'center', width: '100%' }}>
            {/* Definido por (Escopo) */}
            <Box sx={{ flex: 1, width: '100%' }}>
              <FormControl fullWidth size="small">
                <InputLabel id="escopo-label">Definido por:</InputLabel>
                <Select
                  labelId="escopo-label"
                  value={escopo}
                  onChange={(e) => {
                    setEscopo(Number(e.target.value));
                    setPerfilId('');
                  }}
                  label="Definido por:"
                >
                  <MenuItem value={159}>Sistema</MenuItem>
                  <MenuItem value={160}>Filial</MenuItem>
                  <MenuItem value={161}>Perfil de Acesso</MenuItem>
                  <MenuItem value={350}>Grupo</MenuItem>
                  <MenuItem value={351}>Empresa</MenuItem>
                </Select>
              </FormControl>
            </Box>

            {/* Perfil de Acesso se escopo for Perfil */}
            {escopo === 161 && (
              <Box sx={{ flex: 1, width: '100%' }}>
                <FormControl fullWidth size="small">
                  <InputLabel id="perfil-label">Perfil:</InputLabel>
                  <Select
                    labelId="perfil-label"
                    value={perfilId}
                    onChange={(e) => setPerfilId(e.target.value as number)}
                    label="Perfil:"
                  >
                    <MenuItem value="">
                      <em>Selecione o perfil...</em>
                    </MenuItem>
                    {perfis.map((p) => (
                      <MenuItem key={p.idPerfil} value={p.idPerfil}>
                        {p.descricao}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Box>
            )}

            {/* Busca por Chave */}
            <Box sx={{ flex: 1, width: '100%' }}>
              <TextField
                label="Buscar por Chave"
                size="small"
                fullWidth
                value={searchChave}
                onChange={(e) => setSearchChave(e.target.value)}
                slotProps={{
                  input: {
                    startAdornment: <SearchIcon sx={{ color: 'action.active', mr: 1 }} />
                  }
                }}
              />
            </Box>

            {/* Busca por Descrição */}
            <Box sx={{ flex: 1, width: '100%' }}>
              <TextField
                label="Buscar por Descrição"
                size="small"
                fullWidth
                value={searchDescricao}
                onChange={(e) => setSearchDescricao(e.target.value)}
                slotProps={{
                  input: {
                    startAdornment: <SearchIcon sx={{ color: 'action.active', mr: 1 }} />
                  }
                }}
              />
            </Box>
          </Box>
        </CardContent>
      </Card>

      {/* Lista de Parâmetros Agrupados */}
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}>
          <CircularProgress />
        </Box>
      ) : escopo === 161 && perfilId === '' ? (
        <Alert severity="info">Por favor, selecione um Perfil de Acesso para visualizar e configurar os parâmetros correspondentes.</Alert>
      ) : Object.keys(groupedParametros).length === 0 ? (
        <Alert severity="info">Nenhum parâmetro encontrado com os filtros atuais.</Alert>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {Object.keys(groupedParametros)
            .map(Number)
            .sort((a, b) => a - b)
            .map((agrupadorId) => {
              const items = groupedParametros[agrupadorId];
              return (
                <Accordion key={agrupadorId} defaultExpanded>
                  <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                    <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                      {getAgrupadorLabel(agrupadorId)} ({items.length} {items.length === 1 ? 'parâmetro' : 'parâmetros'})
                    </Typography>
                  </AccordionSummary>
                  <AccordionDetails sx={{ p: 2 }}>
                    <Stack spacing={2} divider={<Divider flexItem />}>
                      {items.map((p) => (
                        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 2, alignItems: 'center', width: '100%' }} key={p.id}>
                          {/* Marcado Checkbox */}
                          <Box sx={{ width: { xs: '100%', sm: '120px' }, display: 'flex', flexShrink: 0 }}>
                            <FormControlLabel
                              control={
                                <Checkbox
                                  checked={p.marcado}
                                  onChange={(e) => handleMarcadoChange(p.id, e.target.checked)}
                                  color="primary"
                                />
                              }
                              label="Ativar"
                              sx={{ m: 0 }}
                            />
                          </Box>

                          {/* Chave e Descrição */}
                          <Box sx={{ flex: 1, width: '100%' }}>
                            <Typography variant="body1" sx={{ fontWeight: '600', color: p.marcado ? 'text.primary' : 'text.disabled' }}>
                              {p.chave}
                            </Typography>
                            <Typography variant="caption" sx={{ color: 'text.secondary', display: 'block' }}>
                              {p.descricao}
                            </Typography>
                            {p.valor && (
                              <Typography variant="caption" sx={{ color: 'text.disabled', fontStyle: 'italic' }}>
                                Valor base padrão: {p.valor}
                              </Typography>
                            )}
                          </Box>

                          {/* Campo de Edição do Valor */}
                          <Box sx={{ width: { xs: '100%', sm: '350px' }, flexShrink: 0 }}>
                            {renderInputControl(p)}
                          </Box>
                        </Box>
                      ))}
                    </Stack>
                  </AccordionDetails>
                </Accordion>
              );
            })}
        </Box>
      )}

      {/* Alertas */}
      <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={() => setNotification((prev) => ({ ...prev, open: false }))}
      >
        <Alert
          onClose={() => setNotification((prev) => ({ ...prev, open: false }))}
          severity={notification.severity}
          sx={{ width: '100%' }}
        >
          {notification.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};
