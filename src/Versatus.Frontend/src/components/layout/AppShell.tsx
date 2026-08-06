import React from 'react';
import { Box } from '@mui/material';
import { TopBar } from './TopBar';
import { TabBar } from './TabBar';
import { ContextualSidebar } from './ContextualSidebar';
import { useTabs } from '../../context/TabsContext';

// Importações de todas as páginas — keep-alive: ficam montadas, apenas ocultas por CSS
import { FEntidade } from '../../pages/AcessoGlobal/FEntidade';
import { FParametro } from '../../pages/AcessoGlobal/FParametro';
import { FCondicaoPagamento } from '../../pages/AcessoGlobal/FCondicaoPagamento';
import { WelcomeScreen } from './WelcomeScreen';

/** Mapa de rota → componente correspondente */
const PAGINA_MAP: Record<string, React.ComponentType> = {
  '/acesso-global/entidade': FEntidade,
  '/acesso-global/parametro': FParametro,
  '/acesso-global/condicao-pagamento': FCondicaoPagamento,
};

export const AppShell: React.FC = () => {
  const { abas, abaAtivaId } = useTabs();

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', bgcolor: 'background.default' }}>
      {/* Barra superior fixa */}
      <TopBar />

      {/* Barra de abas — só aparece quando há abas abertas */}
      <TabBar />

      <Box sx={{ display: 'flex', flexGrow: 1, position: 'relative', overflow: 'hidden' }}>
        {/* Sidebar de navegação */}
        <ContextualSidebar />

        {/* Área de conteúdo principal */}
        <Box
          component="main"
          sx={{
            flexGrow: 1,
            minWidth: 0,
            overflowX: 'auto',
            bgcolor: 'background.default',
            position: 'relative',
          }}
        >
          {/* Tela de boas-vindas quando não há abas abertas */}
          {abas.length === 0 && <WelcomeScreen />}

          {/* Keep-Alive: cada aba fica montada, apenas oculta via display:none */}
          {abas.map(aba => {
            const Pagina = PAGINA_MAP[aba.rota];
            if (!Pagina) return null;
            return (
              <Box
                key={aba.id}
                sx={{
                  display: abaAtivaId === aba.id ? 'block' : 'none',
                  height: '100%',
                }}
              >
                <Pagina />
              </Box>
            );
          })}
        </Box>
      </Box>
    </Box>
  );
};
