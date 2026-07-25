export interface ModuloMenuDto {
  idModulo: number;
  nome: string;
  prefixoRota: string | null;
  iconeMui: string | null;
  corHex: string | null;
  ordem: number;
  menus: MenuItemDto[];
}

export interface MenuItemDto {
  idMenu: number;
  descricao: string;
  ordem: number;
  subMenus: MenuItemDto[];
  rotinas: RotinaItemDto[];
}

export interface RotinaItemDto {
  idRotina: number;
  nome: string;
  objeto: string | null;
  rotaWeb: string | null;
  ordem: number;
  rotaCompleta: string;
}

export interface FavoritoDto {
  idFavorito: number;
  idRotina: number;
  nomeRotina: string;
  rotaCompleta: string;
  nomeModulo: string;
  corHex: string | null;
  caminhoCompleto: string;
}
