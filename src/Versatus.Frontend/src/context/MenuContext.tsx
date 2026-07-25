import React, { createContext, useContext } from 'react';
import type { ModuloMenuDto, FavoritoDto } from '../types/menu';
import { useMenuArvore } from '../hooks/useMenuArvore';
import { useFavoritos } from '../hooks/useFavoritos';
import { Snackbar, Alert } from '@mui/material';

interface MenuContextState {
  modulos: ModuloMenuDto[];
  moduloAtivo: ModuloMenuDto | null;
  setModuloAtivo: (m: ModuloMenuDto) => void;
  favoritos: FavoritoDto[];
  adicionarFavorito: (idRotina: number, nomeRotina?: string) => void;
  removerFavorito: (idRotina: number, nomeRotina?: string) => void;
  isLoading: boolean;
}

const MenuContext = createContext<MenuContextState | undefined>(undefined);

export const MenuProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { modulos, moduloAtivo, setModuloAtivo, isLoading: isLoadingArvore } = useMenuArvore();
  const { favoritos, adicionarFavorito, removerFavorito, isLoading: isLoadingFavs, toast, hideToast } = useFavoritos();

  const isLoading = isLoadingArvore || isLoadingFavs;

  return (
    <MenuContext.Provider
      value={{
        modulos,
        moduloAtivo,
        setModuloAtivo,
        favoritos,
        adicionarFavorito,
        removerFavorito,
        isLoading
      }}
    >
      {children}
      <Snackbar
        open={toast.open}
        autoHideDuration={2500}
        onClose={hideToast}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert onClose={hideToast} severity={toast.severity} sx={{ width: '100%', boxShadow: 3 }}>
          {toast.message}
        </Alert>
      </Snackbar>
    </MenuContext.Provider>
  );
};

export const useMenu = (): MenuContextState => {
  const context = useContext(MenuContext);
  if (!context) {
    throw new Error('useMenu deve ser utilizado dentro de um MenuProvider');
  }
  return context;
};
