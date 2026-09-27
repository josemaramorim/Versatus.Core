import type { MenuItemDto, ModuloMenuDto } from '../types/menu';

// Caminho de navegação de uma tela, derivado da árvore do menu (GET /api/menu/arvore).
// Spec: docs/spec_navegacao_breadcrumb.md — regras R1, R2, R3, R6 (NB4) e R8.

export interface CaminhoRotina {
  modulo: string;
  menus: string[];
  rotina: string;
}

/** Normaliza para comparar nomes sem diferenciar maiúsculas nem acentos (R3). */
const normalizar = (texto: string): string =>
  texto.normalize('NFD').replace(/[̀-ͯ]/g, '').trim().toLowerCase();

const CONECTIVOS = new Set(['de', 'da', 'do', 'das', 'dos', 'e']);

/**
 * NB4 — texto todo em caixa alta vindo do banco ("GESTÃO FINANCEIRA") é exibido em formato de
 * título ("Gestão Financeira"). Texto com caixa mista é mantido como está.
 */
export function formatarRotulo(texto: string): string {
  const temLetra = /\p{L}/u.test(texto);
  if (!temLetra || texto !== texto.toUpperCase()) return texto;

  return texto
    .toLowerCase()
    .split(' ')
    .map((palavra, i) =>
      i > 0 && CONECTIVOS.has(palavra) ? palavra : palavra.charAt(0).toUpperCase() + palavra.slice(1),
    )
    .join(' ');
}

/**
 * Procura a rotina cuja `rotaCompleta` é `rota`. Ordem igual à de `MenuService.ProcessarMenuParaMap`
 * no backend: rotinas do menu antes dos submenus; a primeira ocorrência vence (R2).
 */
function buscarNoMenu(menu: MenuItemDto, rota: string, trilha: string[]): { menus: string[]; rotina: string } | null {
  const caminho = [...trilha, menu.descricao];

  const rotina = menu.rotinas.find((r) => r.rotaCompleta === rota);
  if (rotina) return { menus: caminho, rotina: rotina.nome };

  for (const sub of menu.subMenus) {
    const achou = buscarNoMenu(sub, rota, caminho);
    if (achou) return achou;
  }
  return null;
}

/**
 * Monta `Módulo › Menu(s) › Rotina` para a rota (R1). Retorna `null` quando a rota não está no
 * menu — quem chama usa só `Início › {título da tela}` (R8), nunca um caminho inventado.
 */
export function montarCaminhoRotina(modulos: ModuloMenuDto[], rota: string | null | undefined): CaminhoRotina | null {
  if (!rota) return null;

  for (const modulo of modulos) {
    for (const menu of modulo.menus) {
      const achou = buscarNoMenu(menu, rota, []);
      if (!achou) continue;

      const nomeModulo = normalizar(modulo.nome);
      return {
        modulo: formatarRotulo(modulo.nome),
        // R3 — omite o nível de menu que repete o nome do módulo.
        menus: achou.menus.filter((m) => normalizar(m) !== nomeModulo).map(formatarRotulo),
        rotina: formatarRotulo(achou.rotina),
      };
    }
  }
  return null;
}
