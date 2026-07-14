import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { theme } from './theme';
import { FEntidade } from './pages/FEntidade';

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <FEntidade />
    </ThemeProvider>
  );
}

export default App;
