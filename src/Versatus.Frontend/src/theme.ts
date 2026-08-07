import { createTheme } from '@mui/material/styles';

/**
 * Função geradora de tema dinâmico (Light / Dark) com estética premium Minimals.
 */
export const getTheme = (mode: 'light' | 'dark' = 'light') => {
  const isDark = mode === 'dark';

  return createTheme({
    palette: {
      mode,
      primary: {
        main: isDark ? '#3385FF' : '#2065D1', // Azul Royal ajustado no modo escuro
        light: '#76B0F1',
        dark: '#103996',
        contrastText: '#fff',
      },
      secondary: {
        main: isDark ? '#A27BFC' : '#845ADF',
        light: '#C3B0F9',
        dark: '#512DA8',
        contrastText: '#fff',
      },
      background: {
        default: isDark ? '#161C24' : '#F4F6F8', // Dark Minimals vs Light Minimals
        paper: isDark ? '#212B36' : '#FFFFFF',   // Fundo elevado dos cards
      },
      text: {
        primary: isDark ? '#FFFFFF' : '#212B36',
        secondary: isDark ? '#919EAB' : '#637381',
        disabled: isDark ? '#637381' : '#919EAB',
      },
      divider: isDark ? 'rgba(145, 158, 171, 0.24)' : 'rgba(145, 158, 171, 0.2)',
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
        textTransform: 'none',
        fontWeight: 700,
      },
    },
    shape: {
      borderRadius: 12,
    },
    components: {
      MuiCard: {
        styleOverrides: {
          root: {
            backgroundImage: 'none',
            boxShadow: isDark
              ? '0 0 2px 0 rgba(0, 0, 0, 0.24), 0 12px 24px -4px rgba(0, 0, 0, 0.24)'
              : '0 0 2px 0 rgba(145, 158, 171, 0.2), 0 12px 24px -4px rgba(145, 158, 171, 0.12)',
            borderRadius: 16,
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
              background: isDark
                ? 'linear-gradient(135deg, #3385FF 0%, #103996 100%)'
                : 'linear-gradient(135deg, #2065D1 0%, #103996 100%)',
              '&:hover': {
                background: isDark
                  ? 'linear-gradient(135deg, #2065D1 0%, #082163 100%)'
                  : 'linear-gradient(135deg, #103996 0%, #082163 100%)',
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
          size: 'small',
        },
      },
      MuiFormLabel: {
        styleOverrides: {
          asterisk: {
            color: '#FF4842 !important',
            fontWeight: 'bold',
          },
        },
      },
      MuiInputLabel: {
        styleOverrides: {
          asterisk: {
            color: '#FF4842 !important',
            fontWeight: 'bold',
          },
        },
      },
      MuiSelect: {
        defaultProps: {
          size: 'small',
        },
      },
    },
  });
};

/** Tema estático padrão para compatibilidade retroativa */
export const theme = getTheme('light');
