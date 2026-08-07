import React, { createContext, useContext, useState, useEffect, useMemo, useCallback } from 'react';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { getTheme } from '../theme';

export type PaletteMode = 'light' | 'dark';

interface ThemeContextState {
  /** Modo ativo do tema ('light' | 'dark') */
  mode: PaletteMode;
  /** Alterna entre os temas claro e escuro */
  toggleTheme: () => void;
  /** Define diretamente o modo de tema */
  setThemeMode: (mode: PaletteMode) => void;
}

const ThemeContext = createContext<ThemeContextState | undefined>(undefined);

const DEFAULT_USER_LOGIN = 'admin';

/** Retorna a chave do LocalStorage individual por usuário logado */
function getStorageKey(userLogin = DEFAULT_USER_LOGIN): string {
  return `versatus_theme_mode_${userLogin}`;
}

export const CustomThemeProvider: React.FC<{ children: React.ReactNode; userLogin?: string }> = ({
  children,
  userLogin = DEFAULT_USER_LOGIN
}) => {
  const [mode, setMode] = useState<PaletteMode>(() => {
    // 1. Camada 1: Tenta ler o tema salvo no LocalStorage individual do usuário
    try {
      const saved = localStorage.getItem(getStorageKey(userLogin));
      if (saved === 'light' || saved === 'dark') {
        return saved;
      }
    } catch {
      // Ignore storage errors
    }
    // Fallback: Modo 'light' por padrão
    return 'light';
  });

  // Atualizar tema quando mudar o login do usuário
  useEffect(() => {
    try {
      const saved = localStorage.getItem(getStorageKey(userLogin));
      if (saved === 'light' || saved === 'dark') {
        setMode(saved);
      }
    } catch {
      // Ignore
    }
  }, [userLogin]);

  // Alterna entre Claro e Escuro e persiste no LocalStorage
  const toggleTheme = useCallback(() => {
    setMode((prevMode) => {
      const nextMode: PaletteMode = prevMode === 'light' ? 'dark' : 'light';
      try {
        localStorage.setItem(getStorageKey(userLogin), nextMode);
        // Camada 2: Estrutura pronta para sincronização com API de perfil no futuro
        // syncWithApi(userLogin, nextMode);
      } catch {
        // Ignore
      }
      return nextMode;
    });
  }, [userLogin]);

  const setThemeMode = useCallback(
    (newMode: PaletteMode) => {
      setMode(newMode);
      try {
        localStorage.setItem(getStorageKey(userLogin), newMode);
      } catch {
        // Ignore
      }
    },
    [userLogin]
  );

  const muiTheme = useMemo(() => getTheme(mode), [mode]);

  return (
    <ThemeContext.Provider value={{ mode, toggleTheme, setThemeMode }}>
      <ThemeProvider theme={muiTheme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </ThemeContext.Provider>
  );
};

export const useThemeMode = (): ThemeContextState => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useThemeMode deve ser utilizado dentro de um CustomThemeProvider');
  }
  return context;
};
