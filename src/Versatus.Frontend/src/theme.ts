import { createTheme } from '@mui/material/styles';

// Cores premium inspiradas no template Minimals (minimals.cc)
export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#2065D1', // Azul Royal Minimals
      light: '#76B0F1',
      dark: '#103996',
      contrastText: '#fff',
    },
    secondary: {
      main: '#845ADF', // Roxo Sutil
      light: '#C3B0F9',
      dark: '#512DA8',
      contrastText: '#fff',
    },
    background: {
      default: '#F4F6F8', // Cinza extra-claro de fundo
      paper: '#FFFFFF',   // Fundo branco puro dos Cards
    },
    text: {
      primary: '#212B36',   // Cinza escuro de alta legibilidade
      secondary: '#637381', // Cinza médio para descrições/legendas
      disabled: '#919EAB',
    },
    divider: 'rgba(145, 158, 171, 0.2)',
  },
  typography: {
    fontFamily: '"Public Sans", "Inter", "Outfit", "Roboto", sans-serif',
    h1: { fontWeight: 800 },
    h2: { fontWeight: 800 },
    h3: { fontWeight: 700 },
    h4: { fontWeight: 700 },
    h5: { fontWeight: 600 },
    h6: { fontWeight: 600 },
    subtitle1: { fontWeight: 600 },
    subtitle2: { fontWeight: 600 },
    body1: { fontSize: '0.9375rem', lineHeight: 1.5 },
    body2: { fontSize: '0.875rem', lineHeight: 1.5 },
    button: {
      textTransform: 'none', // Sem caixa-alta forçada (visual limpo)
      fontWeight: 700,
    },
  },
  shape: {
    borderRadius: 12, // Cantos arredondados generosos
  },
  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          boxShadow: '0 0 2px 0 rgba(145, 158, 171, 0.2), 0 12px 24px -4px rgba(145, 158, 171, 0.12)',
          borderRadius: 16, // Cards mais arredondados estilo Minimals
          padding: '24px',
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: ({ ownerState }) => ({
          borderRadius: 8,
          padding: '6px 16px',
          boxShadow: 'none',
          '&:hover': {
            boxShadow: 'none',
          },
          ...(ownerState.variant === 'contained' && ownerState.color === 'primary' && {
            background: 'linear-gradient(135deg, #2065D1 0%, #103996 100%)',
            '&:hover': {
              background: 'linear-gradient(135deg, #103996 0%, #082163 100%)',
            },
          }),
        }),
      },
    },
    MuiTab: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontWeight: 600,
          fontSize: '0.9375rem',
          minWidth: 100,
          marginRight: '8px',
          borderRadius: '8px 8px 0 0',
        },
      },
    },
    MuiTextField: {
      defaultProps: {
        variant: 'outlined',
        size: 'small', // Campos mais compactos e discretos
      },
    },
    MuiSelect: {
      defaultProps: {
        size: 'small',
      },
    },
  },
});
