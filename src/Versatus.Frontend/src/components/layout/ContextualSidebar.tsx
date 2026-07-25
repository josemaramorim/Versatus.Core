import React, { useState, useEffect, useRef } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import {
  Box,
  Drawer,
  Typography,
  IconButton,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
  alpha
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import StarIcon from '@mui/icons-material/Star';
import StarBorderIcon from '@mui/icons-material/StarBorder';
import FolderIcon from '@mui/icons-material/Folder';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import { useMenu } from '../../context/MenuContext';
import type { MenuItemDto, RotinaItemDto } from '../../types/menu';

const SIDEBAR_WIDTH_EXPANDED = 240;
const SIDEBAR_WIDTH_COLLAPSED = 64;

export const ContextualSidebar: React.FC = () => {
  const { modulos, moduloAtivo, setModuloAtivo, favoritos, adicionarFavorito, removerFavorito } = useMenu();
  const location = useLocation();
  const prevPathRef = useRef(location.pathname);

  const [collapsed, setCollapsed] = useState<boolean>(false);
  const [openAccordionIds, setOpenAccordionIds] = useState<number[]>([]);

  const corHex = moduloAtivo?.corHex || '#2065D1';

  // Sincroniza o módulo ativo APENAS quando a rota da URL de fato muda por navegação (ex: via Favoritos ou Link direto)
  useEffect(() => {
    if (!modulos || modulos.length === 0) return;

    if (prevPathRef.current !== location.pathname) {
      prevPathRef.current = location.pathname;
      const currentPath = location.pathname;

      const targetModulo = modulos.find(
        (m) => m.prefixoRota && m.prefixoRota !== '/' && currentPath.startsWith(m.prefixoRota)
      );

      if (targetModulo) {
        setModuloAtivo(targetModulo);
      }
    }
  }, [location.pathname, modulos, setModuloAtivo]);

  // Carrega estado dos accordions do localStorage por módulo
  useEffect(() => {
    if (!moduloAtivo) return;
    const key = `versatus_sidebar_${moduloAtivo.idModulo}`;
    try {
      const saved = localStorage.getItem(key);
      if (saved) {
        setOpenAccordionIds(JSON.parse(saved));
      } else {
        // Abre os primeiros menus por padrão
        const initialOpen = moduloAtivo.menus.map((m) => m.idMenu);
        setOpenAccordionIds(initialOpen);
      }
    } catch {
      setOpenAccordionIds([]);
    }
  }, [moduloAtivo]);

  // Salva estado dos accordions no localStorage
  const handleToggleAccordion = (idMenu: number) => {
    if (!moduloAtivo) return;
    setOpenAccordionIds((prev) => {
      const next = prev.includes(idMenu) ? prev.filter((id) => id !== idMenu) : [...prev, idMenu];
      try {
        localStorage.setItem(`versatus_sidebar_${moduloAtivo.idModulo}`, JSON.stringify(next));
      } catch (e) {
        console.warn(e);
      }
      return next;
    });
  };

  const isFavorited = (idRotina: number) => favoritos.some((f) => f.idRotina === idRotina);

  const renderRotinaItem = (rotina: RotinaItemDto, level = 0) => {
    const active = location.pathname === rotina.rotaCompleta;
    const favorited = isFavorited(rotina.idRotina);

    return (
      <Tooltip
        key={rotina.idRotina}
        title={rotina.nome}
        placement="right"
        enterDelay={400}
        enterNextDelay={200}
        arrow
      >
        <ListItemButton
          component={NavLink}
          to={rotina.rotaCompleta}
          sx={{
            pl: collapsed ? 2 : 2 + level * 1.5,
            pr: 1,
            py: 0.75,
            minHeight: 38,
            borderRadius: 1,
            mb: 0.2,
            borderLeft: active ? `3px solid ${corHex}` : '3px solid transparent',
            bgcolor: active ? alpha(corHex, 0.08) : 'transparent',
            color: active ? corHex : 'text.secondary',
            fontWeight: active ? 700 : 500,
            '&:hover': {
              bgcolor: alpha(corHex, 0.05),
              color: 'text.primary',
              '& .star-action-icon': {
                opacity: 1
              }
            }
          }}
        >
          {!collapsed && (
            <ListItemIcon sx={{ minWidth: 24, color: active ? corHex : 'text.disabled' }}>
              <InsertDriveFileIcon style={{ fontSize: 16 }} />
            </ListItemIcon>
          )}
          <ListItemText
            primary={rotina.nome}
            slotProps={{
              primary: {
                variant: 'body2',
                noWrap: true,
                sx: {
                  fontSize: '0.8125rem',
                  fontWeight: active ? 700 : 500
                }
              }
            }}
          />

          {!collapsed && (
            <IconButton
              size="small"
              className="star-action-icon"
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                if (favorited) {
                  removerFavorito(rotina.idRotina, rotina.nome);
                } else {
                  adicionarFavorito(rotina.idRotina, rotina.nome);
                }
              }}
              sx={{
                p: 0.25,
                opacity: favorited ? 1 : 0,
                transition: 'opacity 0.2s',
                color: favorited ? '#F59E0B' : 'text.disabled',
                '&:hover': { color: '#F59E0B' }
              }}
            >
              {favorited ? <StarIcon style={{ fontSize: 16 }} /> : <StarBorderIcon style={{ fontSize: 16 }} />}
            </IconButton>
          )}
        </ListItemButton>
      </Tooltip>
    );
  };

  const renderMenuItem = (menu: MenuItemDto, level = 0) => {
    const isOpen = openAccordionIds.includes(menu.idMenu);

    if (collapsed) {
      return (
        <Box key={menu.idMenu} sx={{ py: 0.5 }}>
          {menu.rotinas.map((r) => renderRotinaItem(r, level))}
          {menu.subMenus.map((sub) => renderMenuItem(sub, level + 1))}
        </Box>
      );
    }

    return (
      <Accordion
        key={menu.idMenu}
        expanded={isOpen}
        onChange={() => handleToggleAccordion(menu.idMenu)}
        elevation={0}
        sx={{
          boxShadow: 'none',
          '&:before': { display: 'none' },
          borderRadius: 0,
          bgcolor: 'transparent',
          m: '0 !important'
        }}
      >
        <AccordionSummary
          expandIcon={<ExpandMoreIcon sx={{ fontSize: 18 }} />}
          sx={{
            minHeight: 36,
            px: 1,
            py: 0,
            borderRadius: 1,
            '&:hover': { bgcolor: alpha('#000', 0.03) }
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <FolderIcon style={{ fontSize: 18, color: alpha('#212B36', 0.6) }} />
            <Typography variant="body2" sx={{ fontWeight: 600, fontSize: '0.8125rem', color: 'text.primary' }}>
              {menu.descricao}
            </Typography>
          </Box>
        </AccordionSummary>
        <AccordionDetails sx={{ p: 0, pl: 1 }}>
          <List disablePadding>
            {menu.rotinas.map((r) => renderRotinaItem(r, level + 1))}
            {menu.subMenus.map((sub) => renderMenuItem(sub, level + 1))}
          </List>
        </AccordionDetails>
      </Accordion>
    );
  };

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: collapsed ? SIDEBAR_WIDTH_COLLAPSED : SIDEBAR_WIDTH_EXPANDED,
        flexShrink: 0,
        whiteSpace: 'nowrap',
        boxSizing: 'border-box',
        '& .MuiDrawer-paper': {
          width: collapsed ? SIDEBAR_WIDTH_COLLAPSED : SIDEBAR_WIDTH_EXPANDED,
          transition: 'width 0.3s ease',
          overflowX: 'hidden',
          top: 64, // Abaixo da TopBar
          height: 'calc(100vh - 64px)',
          borderRight: '1px solid rgba(145, 158, 171, 0.12)',
          bgcolor: 'background.paper',
          overflowY: 'auto',
          '&::-webkit-scrollbar': { width: 4 },
          '&::-webkit-scrollbar-thumb': { bgcolor: alpha('#000', 0.12), borderRadius: 2 }
        }
      }}
    >
      {/* Botão para colapsar/expandir a Sidebar */}
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: collapsed ? 'center' : 'space-between',
          px: 2,
          py: 1,
          borderBottom: '1px solid rgba(145, 158, 171, 0.12)'
        }}
      >
        {!collapsed && (
          <Typography variant="overline" sx={{ color: 'text.disabled', fontWeight: 700, letterSpacing: 1.1 }}>
            NAVEGAÇÃO
          </Typography>
        )}
        <IconButton size="small" onClick={() => setCollapsed(!collapsed)}>
          {collapsed ? <ChevronRightIcon /> : <ChevronLeftIcon />}
        </IconButton>
      </Box>

      {/* Árvore de Menus do Módulo Ativo */}
      <Box sx={{ px: 1, pt: 1, pb: 4 }}>
        {moduloAtivo?.menus.map((menu) => renderMenuItem(menu))}
      </Box>
    </Drawer>
  );
};
