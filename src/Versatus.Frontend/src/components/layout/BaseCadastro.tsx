import React from 'react';
import {
  Box,
  Card,
  Button,
  Typography,
  Breadcrumbs,
  Link,
  IconButton
} from '@mui/material';
import {
  Plus,
  Save,
  RotateCcw,
  Trash2,
  ArrowLeft
} from 'lucide-react';
import type { BaseCadastroProps } from '../../types/cadastro';

/**
 * BaseCadastro — Componente base herdado por todos os formulários CRUD do ERP.
 *
 * Controla a transição entre dois modos de exibição na aba:
 *   • 'list': exibe a grid/listagem (children do modo browse)
 *   • 'form': exibe o formulário inline (children do modo insert/edit)
 *
 * REGRA: Nenhum modal/dialog é aberto. O formulário ocupa a área de conteúdo da aba.
 * Ao salvar com sucesso (onSalvar) ou cancelar (onDesfazer/onSair): retorna para 'list'.
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
  const isBrowse = state === 'browse';

  // viewMode interno: derivado do state para manter compatibilidade com o código existente
  // browse → list | insert/edit → form
  const viewMode = isBrowse ? 'list' : 'form';

  // Título dinâmico da aba/breadcrumb baseado no modo
  const subtituloBreadcrumb = state === 'insert'
    ? 'Novo'
    : state === 'edit'
    ? 'Editar'
    : null;

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
            onClick={onSair}
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
              onClick={onSair}
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
                onClick={onSalvar}
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
                  onClick={onSair}
                  sx={{ borderColor: 'divider' }}
                >
                  Cancelar
                </Button>
              )}
            </>
          )}
        </Box>
      </Box>

      {/* 3. Área de conteúdo — grid (list) ou formulário (form) */}
      <Card sx={{ p: { xs: 2, sm: 4 }, overflow: 'visible' }}>
        {children}
      </Card>
    </Box>
  );
};
