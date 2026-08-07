import React, { createContext, useContext, useState, useCallback } from 'react';
import type { TabItem } from '../types/tabs';

// ── Contrato do Contexto ────────────────────────────────────────────────────

interface TabsContextState {
  /** Lista completa de abas abertas */
  abas: TabItem[];
  /** ID da aba atualmente visível */
  abaAtivaId: string | null;
  /**
   * Abre sempre uma NOVA aba (permite duplicatas).
   * Se a mesma rota já existir, ainda assim cria outra aba independente.
   */
  abrirAba: (item: Omit<TabItem, 'id'>) => void;
  /** Remove uma aba pelo ID. Se possuir alterações não salvas, pede confirmação. */
  fecharAba: (id: string, force?: boolean) => void;
  /** Torna uma aba a ativa sem criar nem fechar nenhuma. */
  ativarAba: (id: string) => void;
  /** Marca ou desmarca a aba como "suja" (alterações não salvas) */
  marcarDirty: (id: string, dirty: boolean) => void;
}

// ── Criação do Contexto ─────────────────────────────────────────────────────

const TabsContext = createContext<TabsContextState | undefined>(undefined);

// ── Provider ────────────────────────────────────────────────────────────────

export const TabsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [abas, setAbas] = useState<TabItem[]>([]);
  const [abaAtivaId, setAbaAtivaId] = useState<string | null>(null);

  const abrirAba = useCallback((item: Omit<TabItem, 'id'>) => {
    const novaAba: TabItem = { ...item, id: crypto.randomUUID() };
    setAbas(prev => [...prev, novaAba]);
    setAbaAtivaId(novaAba.id);
  }, []);

  const fecharAba = useCallback((id: string, force = false) => {
    setAbas(prev => {
      const abaParaFechar = prev.find(a => a.id === id);
      if (abaParaFechar?.isDirty && !force) {
        const confirmou = window.confirm(
          `A aba "${abaParaFechar.titulo}" possui alterações não salvas. Deseja realmente fechar e descartar as alterações?`
        );
        if (!confirmou) return prev;
      }

      const index = prev.findIndex(a => a.id === id);
      const novaLista = prev.filter(a => a.id !== id);

      setAbaAtivaId(current => {
        if (current !== id) return current;
        if (novaLista.length === 0) return null;
        // Ativa a aba anterior ou a próxima disponível
        const novoIndex = Math.max(0, index - 1);
        return novaLista[novoIndex]?.id ?? null;
      });

      return novaLista;
    });
  }, []);

  const ativarAba = useCallback((id: string) => {
    setAbaAtivaId(id);
  }, []);

  const marcarDirty = useCallback((id: string, dirty: boolean) => {
    setAbas(prev => prev.map(a => a.id === id ? { ...a, isDirty: dirty } : a));
  }, []);

  return (
    <TabsContext.Provider value={{ abas, abaAtivaId, abrirAba, fecharAba, ativarAba, marcarDirty }}>
      {children}
    </TabsContext.Provider>
  );
};

// ── Hook de consumo ─────────────────────────────────────────────────────────

export const useTabs = (): TabsContextState => {
  const context = useContext(TabsContext);
  if (!context) {
    throw new Error('useTabs deve ser utilizado dentro de um TabsProvider');
  }
  return context;
};
