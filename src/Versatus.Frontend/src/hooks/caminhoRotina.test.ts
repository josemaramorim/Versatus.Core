import { describe, it, expect } from 'vitest';
import { formatarRotulo, montarCaminhoRotina } from './caminhoRotina';
import type { MenuItemDto, ModuloMenuDto } from '../types/menu';

// docs/spec_navegacao_breadcrumb.md — C7: R2, R3, R8 (+ NB4).

const rotina = (idRotina: number, nome: string, rotaCompleta: string) => ({
  idRotina,
  nome,
  objeto: null,
  rotaWeb: null,
  ordem: idRotina,
  rotaCompleta,
});

const menu = (idMenu: number, descricao: string, extra: Partial<MenuItemDto> = {}): MenuItemDto => ({
  idMenu,
  descricao,
  ordem: idMenu,
  subMenus: [],
  rotinas: [],
  ...extra,
});

const modulo = (idModulo: number, nome: string, menus: MenuItemDto[]): ModuloMenuDto => ({
  idModulo,
  nome,
  prefixoRota: null,
  iconeMui: null,
  corHex: null,
  ordem: idModulo,
  menus,
});

// Espelha o banco: módulo 2 → menu raiz "GESTÃO FINANCEIRA" → submenu "Cadastros" → rotina 29.
const arvore: ModuloMenuDto[] = [
  modulo(1, 'ACESSO GLOBAL', [
    menu(1, 'ACESSO GLOBAL', {
      subMenus: [menu(22, 'Cadastro financeiro', { rotinas: [rotina(24, 'Condição de pagamento', '/acesso-global/condicao-pagamento')] })],
    }),
  ]),
  modulo(2, 'GESTÃO FINANCEIRA', [
    menu(2, 'GESTÃO FINANCEIRA', {
      subMenus: [
        menu(30, 'Cadastros', { rotinas: [rotina(29, 'Caixa/Conta', '/financeiro/caixabanco')] }),
        menu(31, 'Caixa/Banco', {
          subMenus: [menu(40, 'Consultas', { rotinas: [rotina(90, 'Caixa/Conta (consulta)', '/financeiro/caixabanco')] })],
        }),
      ],
    }),
  ]),
];

describe('montarCaminhoRotina', () => {
  it('R1: monta Módulo › Menus › Rotina pela rota', () => {
    expect(montarCaminhoRotina(arvore, '/financeiro/caixabanco')).toEqual({
      modulo: 'Gestão Financeira',
      menus: ['Cadastros'],
      rotina: 'Caixa/Conta',
    });
  });

  it('R3: omite o menu raiz que repete o nome do módulo (sem diferenciar acento/caixa)', () => {
    const caminho = montarCaminhoRotina(arvore, '/acesso-global/condicao-pagamento');
    expect(caminho?.menus).toEqual(['Cadastro financeiro']);
    expect(caminho?.modulo).toBe('Acesso Global');
  });

  it('R2: rota em dois lugares do menu — vale a primeira ocorrência', () => {
    expect(montarCaminhoRotina(arvore, '/financeiro/caixabanco')?.rotina).toBe('Caixa/Conta');
  });

  it('percorre submenus em mais de um nível', () => {
    const outra = [
      modulo(3, 'Faturamento', [
        menu(5, 'Vendas', { subMenus: [menu(6, 'Pedidos', { subMenus: [menu(7, 'Relatórios', { rotinas: [rotina(1, 'Pedidos do mês', '/faturamento/pedidos-mes')] })] })] }),
      ]),
    ];
    expect(montarCaminhoRotina(outra, '/faturamento/pedidos-mes')?.menus).toEqual(['Vendas', 'Pedidos', 'Relatórios']);
  });

  it('R8: rota fora do menu devolve null', () => {
    expect(montarCaminhoRotina(arvore, '/financeiro/nao-existe')).toBeNull();
    expect(montarCaminhoRotina(arvore, null)).toBeNull();
    expect(montarCaminhoRotina([], '/financeiro/caixabanco')).toBeNull();
  });
});

describe('formatarRotulo (NB4)', () => {
  it('converte texto todo em caixa alta para formato de título', () => {
    expect(formatarRotulo('GESTÃO FINANCEIRA')).toBe('Gestão Financeira');
    expect(formatarRotulo('GESTÃO DE MATERIAL')).toBe('Gestão de Material');
  });

  it('mantém texto com caixa mista como está', () => {
    expect(formatarRotulo('Caixa/Conta')).toBe('Caixa/Conta');
    expect(formatarRotulo('Cadastro financeiro')).toBe('Cadastro financeiro');
  });
});
