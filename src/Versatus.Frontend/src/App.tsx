import { Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { theme } from './theme';
import { MenuProvider } from './context/MenuContext';
import { AppShell } from './components/layout/AppShell';
import { FEntidade } from './pages/AcessoGlobal/FEntidade';
import { FParametro } from './pages/AcessoGlobal/FParametro';
import { FCondicaoPagamento } from './pages/AcessoGlobal/FCondicaoPagamento';

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <MenuProvider>
        <Routes>
          <Route path="/" element={<AppShell />}>
            <Route index element={<Navigate to="/acesso-global/parametro" replace />} />
            <Route path="/acesso-global/entidade" element={<FEntidade />} />
            <Route path="/acesso-global/parametro" element={<FParametro />} />
            <Route path="/acesso-global/condicao-pagamento" element={<FCondicaoPagamento />} />
          </Route>
        </Routes>
      </MenuProvider>
    </ThemeProvider>
  );
}

export default App;
