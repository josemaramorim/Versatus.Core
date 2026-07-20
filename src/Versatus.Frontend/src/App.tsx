import { useState } from 'react';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box, AppBar, Toolbar, Typography, Tab, Tabs } from '@mui/material';
import { theme } from './theme';
import { FEntidade } from './pages/FEntidade';
import { FParametro } from './pages/FParametro';

function App() {
  const [tabIndex, setTabIndex] = useState(1); // Inicia na aba de Parâmetros por padrão

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static" color="default" elevation={1} sx={{ bgcolor: 'background.paper' }}>
          <Toolbar variant="dense">
            <Typography variant="h6" color="inherit" component="div" sx={{ mr: 4, fontWeight: 700 }}>
              Versatus ERP
            </Typography>
            <Tabs value={tabIndex} onChange={(_, val) => setTabIndex(val)} aria-label="Navegação de telas">
              <Tab label="Entidades" id="nav-tab-entidade" sx={{ fontWeight: 600 }} />
              <Tab label="Parâmetros" id="nav-tab-parametro" sx={{ fontWeight: 600 }} />
            </Tabs>
          </Toolbar>
        </AppBar>
        <Box sx={{ mt: 1 }}>
          {tabIndex === 0 && <FEntidade />}
          {tabIndex === 1 && <FParametro />}
        </Box>
      </Box>
    </ThemeProvider>
  );
}

export default App;
