import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { theme } from './theme';
import { MenuProvider } from './context/MenuContext';
import { TabsProvider } from './context/TabsContext';
import { AppShell } from './components/layout/AppShell';

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <MenuProvider>
        <TabsProvider>
          <AppShell />
        </TabsProvider>
      </MenuProvider>
    </ThemeProvider>
  );
}

export default App;
