import React, { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { Box } from '@mui/material';
import { TopBar } from './TopBar';
import { TabBar } from './TabBar';
import { ContextualSidebar } from './ContextualSidebar';
import { useTabs, TabScopeContext } from '../../context/TabsContext';

// Importações de todas as páginas — keep-alive: ficam montadas, apenas ocultas por CSS
import { FEntidade } from '../../pages/AcessoGlobal/FEntidade';
import { FParametro } from '../../pages/AcessoGlobal/FParametro';
import { FCondicaoPagamento } from '../../pages/AcessoGlobal/FCondicaoPagamento';
import { DashboardScreen } from './DashboardScreen';

/** Mapa de rota → componente correspondente */
const PAGINA_MAP: Record<string, React.ComponentType> = {
  '/acesso-global/entidade': FEntidade,
  '/acesso-global/parametro': FParametro,
  '/acesso-global/condicao-pagamento': FCondicaoPagamento,
};

/** Mapa de rota → título legível da aba */
const ROTA_TITULO_MAP: Record<string, string> = {
  '/acesso-global/entidade': 'Cadastro Entidade',
  '/acesso-global/parametro': 'Parâmetros',
  '/acesso-global/condicao-pagamento': 'Condições de Pagamento',
};

/** Componente memoizado para congelar renderizações de abas inativas */
const TabKeepAliveWrapper = React.memo<{
  isAtiva: boolean;
  children: React.ReactNode;
}>(
  ({ isAtiva, children }) => (
    <Box
      sx={{
        display: isAtiva ? 'block' : 'none',
        height: '100%',
      }}
    >
      {children}
    </Box>
  ),
  (prevProps, nextProps) => prevProps.isAtiva === nextProps.isAtiva
);

export const AppShell: React.FC = () => {
  const { abas, abaAtivaId, abrirAba } = useTabs();
  const location = useLocation();
  const initializedRef = useRef(false);
  const lastPathRef = useRef<string | null>(null);

  // Sincronização da URL inicial / mudança de URL no navegador
  useEffect(() => {
    const currentPath = location.pathname;
    const titulo = ROTA_TITULO_MAP[currentPath];

    if (titulo) {
      const pathMudou = lastPathRef.current !== null && lastPathRef.current !== currentPath;
      if (!initializedRef.current || pathMudou) {
        initializedRef.current = true;
        abrirAba({ titulo, rota: currentPath });
      }
    }
    lastPathRef.current = currentPath;
  }, [location.pathname, abrirAba]);

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', bgcolor: 'background.default' }}>
      {/* Barra superior fixa */}
      <TopBar />

      <Box sx={{ display: 'flex', flexGrow: 1, position: 'relative', overflow: 'hidden' }}>
        {/* Sidebar de navegação */}
        <ContextualSidebar />

        {/* Coluna Direita: Barra de Abas + Conteúdo Principal */}
        <Box sx={{ display: 'flex', flexDirection: 'column', flexGrow: 1, minWidth: 0, overflow: 'hidden' }}>
          {/* Barra de abas — posicionada à direita da Sidebar */}
          <TabBar />

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
            {/* Dashboard institucional quando não há abas de rotinas abertas */}
            {abas.length === 0 && <DashboardScreen />}

            {/* Keep-Alive Memoizado: cada aba fica montada em memória e isolada com seu próprio ID no TabScopeContext */}
            {abas.map(aba => {
              const Pagina = PAGINA_MAP[aba.rota];
              if (!Pagina) return null;
              return (
                <TabKeepAliveWrapper key={aba.id} isAtiva={aba.id === abaAtivaId}>
                  <TabScopeContext.Provider value={aba.id}>
                    <Pagina />
                  </TabScopeContext.Provider>
                </TabKeepAliveWrapper>
              );
            })}
          </Box>
        </Box>
      </Box>
    </Box>
  );
};
