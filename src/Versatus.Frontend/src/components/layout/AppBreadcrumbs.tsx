import React from 'react';
import { Breadcrumbs, Link, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useTabs } from '../../context/TabsContext';
import { useCaminhoRotina } from '../../hooks/useCaminhoRotina';

// Spec: docs/spec_navegacao_breadcrumb.md v1.0.
//   Início › Módulo › Menu(s) › Rotina [› Novo|Editar]
// Clicáveis: só "Início" (R4) e, no formulário, a rotina = voltar para a lista (R7).
// Módulo e menus são texto (R5/R6). Nenhum href="#" (R9).

export interface IAppBreadcrumbsProps {
  /** Título da tela — usado só quando a rota não está no menu (R8). */
  tituloTela: string;
  /** "Novo" / "Editar" no modo formulário. */
  sufixo?: string;
  /** Volta para a lista (com o aviso de alterações não salvas de quem chama). */
  onVoltarLista?: () => void;
  sx?: object;
}

const estiloItem = { fontSize: '0.85rem' };

export const AppBreadcrumbs: React.FC<IAppBreadcrumbsProps> = ({ tituloTela, sufixo, onVoltarLista, sx }) => {
  const caminho = useCaminhoRotina();
  const { irParaInicio } = useTabs();
  const navigate = useNavigate();

  const niveisTexto = caminho ? [caminho.modulo, ...caminho.menus] : [];
  const nomeRotina = caminho?.rotina ?? tituloTela;

  return (
    <Breadcrumbs aria-label="breadcrumb" sx={sx}>
      <Link
        component="button"
        type="button"
        underline="hover"
        color="inherit"
        onClick={() => {
          irParaInicio();
          navigate('/');
        }}
        sx={estiloItem}
      >
        Início
      </Link>

      {niveisTexto.map((nivel, i) => (
        <Typography key={`${i}-${nivel}`} color="text.secondary" sx={estiloItem}>
          {nivel}
        </Typography>
      ))}

      {sufixo && onVoltarLista ? (
        <Link component="button" type="button" underline="hover" color="inherit" onClick={onVoltarLista} sx={estiloItem}>
          {nomeRotina}
        </Link>
      ) : (
        <Typography color="text.primary" sx={{ ...estiloItem, fontWeight: 500 }}>
          {nomeRotina}
        </Typography>
      )}

      {sufixo && (
        <Typography color="text.primary" sx={{ ...estiloItem, fontWeight: 500 }}>
          {sufixo}
        </Typography>
      )}
    </Breadcrumbs>
  );
};
