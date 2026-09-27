import { useMemo } from 'react';
import { useMenu } from '../context/MenuContext';
import { useCurrentTabId, useTabs } from '../context/TabsContext';
import { montarCaminhoRotina } from './caminhoRotina';
import type { CaminhoRotina } from './caminhoRotina';

/** Caminho no menu da tela da aba atual (spec de breadcrumb, R1/R8); `null` se não estiver no menu. */
export function useCaminhoRotina(): CaminhoRotina | null {
  const { modulos } = useMenu();
  const { abas } = useTabs();
  const tabId = useCurrentTabId();
  const rota = abas.find((a) => a.id === tabId)?.rota;

  return useMemo(() => montarCaminhoRotina(modulos, rota), [modulos, rota]);
}
