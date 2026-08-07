import React, { useState } from 'react';
import {
  Box,
  Card,
  Button,
  Typography,
  Breadcrumbs,
  Link,
  IconButton,
  Alert,
  Stack
} from '@mui/material';
import {
  Plus,
  Save,
  RotateCcw,
  Trash2,
  ArrowLeft
} from 'lucide-react';
import type { BaseCadastroProps } from '../../types/cadastro';
import { useTabs } from '../../context/TabsContext';

/**
 * BaseCadastro — Componente base herdado por todos os formulários CRUD do ERP.
 *
 * Controla a transição entre dois modos de exibição na aba:
 *   • 'list': exibe a grid/listagem (children do modo browse)
 *   • 'form': exibe o formulário inline (children do modo insert/edit)
 *
 * REGRA: Nenhum modal/dialog é aberto. O formulário ocupa a área de conteúdo da aba.
 * Ao salvar com sucesso (onSalvar) ou cancelar (onDesfazer/onSair): retorna para 'list'.
 * Se houver alterações não salvas ao cancelar ou fechar, solicita confirmação do usuário.
 */
export const BaseCadastro: React.FC<BaseCadastroProps> = ({
  titulo,
  state,
  onAdicionar,
  onEditar,
  onSalvar,
  onDesfazer,
  onExcluir,
  onSair,
  children
}) => {
  const { abas, abaAtivaId, marcarDirty } = useTabs();
  const [confirmarCancelamento, setConfirmarCancelamento] = useState(false);

  const abaAtiva = abas.find(a => a.id === abaAtivaId);
  const isDirty = abaAtiva?.isDirty ?? false;

  const isBrowse = state === 'browse';
  const viewMode = isBrowse ? 'list' : 'form';

  const subtituloBreadcrumb = state === 'insert'
    ? 'Novo'
    : state === 'edit'
    ? 'Editar'
    : null;

  const handleSairClick = () => {
    if (isDirty) {
      setConfirmarCancelamento(true);
    } else {
      if (onSair) onSair();
    }
  };

  const handleConfirmarDescarte = () => {
    setConfirmarCancelamento(false);
    if (abaAtivaId) {
      marcarDirty(abaAtivaId, false);
    }
    if (onSair) onSair();
  };

  return (
    <Box sx={{ px: { xs: 2, sm: 3 }, pt: 2.5, pb: 3, minHeight: '100%', bgcolor: 'background.default' }}>

      {/* 1. Breadcrumbs */}
      <Breadcrumbs aria-label="breadcrumb" sx={{ mb: 1 }}>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Dashboard
        </Link>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Cadastros
        </Link>
        {subtituloBreadcrumb && (
          <Link
            underline="hover"
            color="inherit"
            onClick={handleSairClick}
            sx={{ fontSize: '0.85rem', cursor: 'pointer' }}
          >
            {titulo}
          </Link>
        )}
        <Typography color="text.primary" sx={{ fontSize: '0.85rem', fontWeight: 500 }}>
          {subtituloBreadcrumb || titulo}
        </Typography>
      </Breadcrumbs>

      {/* 2. Top Header — Título + Botões de Ação CRUD */}
      <Box
        sx={{
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          justifyContent: 'space-between',
          alignItems: { xs: 'flex-start', sm: 'center' },
          gap: 2,
          mb: 4
        }}
      >
        <Box sx={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 1 }}>
          {/* Botão voltar — visível no modo form para voltar à listagem */}
          {!isBrowse && onSair && (
            <IconButton
              onClick={handleSairClick}
              size="small"
              sx={{ mr: 1 }}
              title="Voltar para a listagem"
            >
              <ArrowLeft size={20} />
            </IconButton>
          )}
          <Typography variant="h5" component="h1" sx={{ color: 'text.primary', fontWeight: 700, fontSize: '1.35rem' }}>
            {subtituloBreadcrumb ? `${subtituloBreadcrumb} ${titulo}` : titulo}
          </Typography>
        </Box>

        {/* Barra de Ferramentas CRUD — exibida condicionalmente por viewMode */}
        <Box sx={{ display: 'flex', flexDirection: 'row', gap: 1.5, flexWrap: 'wrap' }}>

          {/* Modo LIST: botões Novo, Editar, Excluir */}
          {viewMode === 'list' && (
            <>
              <Button
                variant="contained"
                color="primary"
                startIcon={<Plus size={16} />}
                onClick={onAdicionar}
              >
                Novo
              </Button>

              {onEditar && (
                <Button
                  variant="outlined"
                  color="inherit"
                  onClick={onEditar}
                >
                  Alterar
                </Button>
              )}

              {onExcluir && (
                <Button
                  variant="outlined"
                  color="error"
                  startIcon={<Trash2 size={16} />}
                  onClick={onExcluir}
                >
                  Excluir
                </Button>
              )}
            </>
          )}

          {/* Modo FORM: botões Salvar, Desfazer, Cancelar */}
          {viewMode === 'form' && (
            <>
              <Button
                variant="contained"
                color="primary"
                startIcon={<Save size={16} />}
                onClick={() => {
                  setConfirmarCancelamento(false);
                  if (abaAtivaId) marcarDirty(abaAtivaId, false);
                  onSalvar();
                }}
              >
                Salvar
              </Button>

              <Button
                variant="outlined"
                color="inherit"
                startIcon={<RotateCcw size={16} />}
                onClick={onDesfazer}
                sx={{ borderColor: 'divider' }}
              >
                Desfazer
              </Button>

              {onSair && (
                <Button
                  variant="outlined"
                  color="inherit"
                  onClick={handleSairClick}
                  sx={{ borderColor: 'divider' }}
                >
                  Cancelar
                </Button>
              )}
            </>
          )}
        </Box>
      </Box>

      {/* 3. Banner Inline de Confirmação de Cancelamento (quando há alterações não salvas) */}
      {confirmarCancelamento && (
        <Alert
          severity="warning"
          variant="outlined"
          action={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <Button
                color="warning"
                variant="contained"
                size="small"
                onClick={handleConfirmarDescarte}
              >
                Descartar e Sair
              </Button>
              <Button
                color="inherit"
                variant="outlined"
                size="small"
                onClick={() => setConfirmarCancelamento(false)}
                sx={{ borderColor: 'divider' }}
              >
                Continuar Editando
              </Button>
            </Stack>
          }
          sx={{
            mb: 3,
            bgcolor: 'warning.50',
            borderColor: 'warning.main',
            alignItems: 'center',
            borderRadius: 1.5,
          }}
        >
          <Typography variant="subtitle2" sx={{ fontWeight: 700, color: 'warning.dark' }}>
            Alterações não salvas
          </Typography>
          <Typography variant="body2" sx={{ color: 'text.primary', mt: 0.25 }}>
            Você possui alterações não salvas no formulário. Deseja realmente cancelar e descartar as alterações?
          </Typography>
        </Alert>
      )}

      {/* 4. Área de conteúdo — grid (list) ou formulário (form) */}
      <Card sx={{ p: { xs: 2, sm: 4 }, overflow: 'visible' }}>
        {children}
      </Card>
    </Box>
  );
};
