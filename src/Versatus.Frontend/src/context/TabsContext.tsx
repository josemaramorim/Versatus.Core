import React, { createContext, useContext, useState, useCallback } from 'react';
import type { TabItem } from '../types/tabs';

// ── Contrato do Contexto ────────────────────────────────────────────────────

interface TabsContextState {
  /** Lista completa de abas abertas */
  abas: TabItem[];
  /** ID da aba atualmente visível */
  abaAtivaId: string | null;
  /**
   * ID da aba que solicitou fechamento mas possui alterações não salvas.
   * Quando preenchido, o formulário ativo exibe o banner de confirmação inline.
   */
  idAbaParaFechar: string | null;
  /**
   * Abre sempre uma NOVA aba (permite duplicatas).
   * Se a mesma rota já existir, ainda assim cria outra aba independente.
   */
  abrirAba: (item: Omit<TabItem, 'id'>) => void;
  /** Remove uma aba pelo ID. Se possuir alterações não salvas, aciona banner inline em vez de window.confirm(). */
  fecharAba: (id: string, force?: boolean) => void;
  /** Cancela a solicitação de fechamento da aba (usuário escolheu "Continuar Editando") */
  cancelarFechamento: () => void;
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
  const [idAbaParaFechar, setIdAbaParaFechar] = useState<string | null>(null);

  const abrirAba = useCallback((item: Omit<TabItem, 'id'>) => {
    const novaAba: TabItem = { ...item, id: crypto.randomUUID() };
    setAbas(prev => [...prev, novaAba]);
    setAbaAtivaId(novaAba.id);
  }, []);



  const fecharAba = useCallback((id: string, force = false) => {
    setAbas(prev => {
      const abaParaFechar = prev.find(a => a.id === id);

      // Se a aba tem alterações e não é forçado: ativar a aba e sinalizar banner inline
      if (abaParaFechar?.isDirty && !force) {
        setAbaAtivaId(id);
        setIdAbaParaFechar(id);
        return prev; // não fecha ainda
      }

      // Fecha imediatamente (sem alterações ou force=true)
      const index = prev.findIndex(a => a.id === id);
      const novaLista = prev.filter(a => a.id !== id);

      setIdAbaParaFechar(null);
      setAbaAtivaId(current => {
        if (current !== id) return current;
        if (novaLista.length === 0) return null;
        const novoIndex = Math.max(0, index - 1);
        return novaLista[novoIndex]?.id ?? null;
      });

      return novaLista;
    });
  }, []);

  const cancelarFechamento = useCallback(() => {
    setIdAbaParaFechar(null);
  }, []);

  const ativarAba = useCallback((id: string) => {
    setAbaAtivaId(id);
  }, []);

  const marcarDirty = useCallback((id: string, dirty: boolean) => {
    setAbas(prev => prev.map(a => a.id === id ? { ...a, isDirty: dirty } : a));
  }, []);

  return (
    <TabsContext.Provider value={{
      abas,
      abaAtivaId,
      idAbaParaFechar,
      abrirAba,
      fecharAba,
      cancelarFechamento,
      ativarAba,
      marcarDirty
    }}>
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
