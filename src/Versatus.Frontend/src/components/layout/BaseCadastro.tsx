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

  return (
    <Box sx={{ p: 4, minHeight: '100vh', bgcolor: 'background.default' }}>
      
      {/* 1. Breadcrumbs (Estilo Minimals.cc) */}
      <Breadcrumbs aria-label="breadcrumb" sx={{ mb: 1 }}>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Dashboard
        </Link>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Cadastros
        </Link>
        <Typography color="text.primary" sx={{ fontSize: '0.85rem', fontWeight: 500 }}>
          {titulo}
        </Typography>
      </Breadcrumbs>

      {/* 2. Top Header (Título + Botões de Ações CRUD) */}
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
          {onSair && (
            <IconButton onClick={onSair} size="small" sx={{ mr: 1 }}>
              <ArrowLeft size={20} />
            </IconButton>
          )}
          <Typography variant="h4" component="h1" sx={{ color: 'text.primary', fontWeight: 800 }}>
            {titulo}
          </Typography>
        </Box>

        {/* Barra de Ferramentas CRUD */}
        <Box sx={{ display: 'flex', flexDirection: 'row', gap: 1.5, flexWrap: 'wrap' }}>
          {/* Adicionar */}
          <Button
            variant="contained"
            color="primary"
            startIcon={<Plus size={16} />}
            onClick={onAdicionar}
            disabled={!isBrowse}
          >
            Novo
          </Button>

          {/* Editar */}
          {onEditar && (
            <Button
              variant="outlined"
              color="inherit"
              onClick={onEditar}
              disabled={!isBrowse}
            >
              Alterar
            </Button>
          )}

          {/* Salvar */}
          <Button
            variant="contained"
            color="primary"
            startIcon={<Save size={16} />}
            onClick={onSalvar}
            disabled={isBrowse}
          >
            Salvar
          </Button>

          {/* Desfazer / Cancelar */}
          <Button
            variant="outlined"
            color="inherit"
            startIcon={<RotateCcw size={16} />}
            onClick={onDesfazer}
            disabled={isBrowse}
            sx={{ borderColor: 'divider' }}
          >
            Desfazer
          </Button>

          {/* Excluir */}
          {onExcluir && (
            <Button
              variant="outlined"
              color="error"
              startIcon={<Trash2 size={16} />}
              onClick={onExcluir}
              disabled={!isBrowse}
            >
              Excluir
            </Button>
          )}
        </Box>
      </Box>

      {/* 3. Container Card do Formulário específico (children) */}
      <Card sx={{ p: 4, overflow: 'visible' }}>
        {children}
      </Card>
    </Box>
  );
};
