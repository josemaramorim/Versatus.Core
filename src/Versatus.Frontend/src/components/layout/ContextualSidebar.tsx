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
  Popover,
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

const SIDEBAR_WIDTH_EXPANDED = 260;
const SIDEBAR_WIDTH_COLLAPSED = 88;

export const ContextualSidebar: React.FC = () => {
  const { modulos, moduloAtivo, setModuloAtivo, favoritos, adicionarFavorito, removerFavorito } = useMenu();
  const location = useLocation();
  const prevPathRef = useRef(location.pathname);

  const [collapsed, setCollapsed] = useState<boolean>(false);
  const [openAccordionIds, setOpenAccordionIds] = useState<number[]>([]);

  // Estado para o Menu Popover Flutuante (Flyout) no modo colapsado
  const [flyoutAnchor, setFlyoutAnchor] = useState<{ el: HTMLElement; menu: MenuItemDto } | null>(null);

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
        const initialOpen = moduloAtivo.menus.map((m) => m.idMenu);
        setOpenAccordionIds(initialOpen);
      }
    } catch {
      setOpenAccordionIds([]);
    }
  }, [moduloAtivo]);

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

  // Manipuladores do Flyout Popover no modo colapsado
  const handleOpenFlyout = (event: React.MouseEvent<HTMLElement>, menu: MenuItemDto) => {
    setFlyoutAnchor({ el: event.currentTarget, menu });
  };

  const handleCloseFlyout = () => {
    setFlyoutAnchor(null);
  };

  // Renderiza rotina (folha) no modo expandido ou dentro do flyout
  const renderRotinaItem = (rotina: RotinaItemDto, level = 0, onRotinaClick?: () => void) => {
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
          onClick={onRotinaClick}
          sx={{
            pl: collapsed && onRotinaClick ? 1.5 : 2 + level * 1.5,
            pr: 1,
            py: 0.75,
            minHeight: 38,
            borderRadius: 1.5,
            mb: 0.3,
            borderLeft: active && !collapsed ? `3px solid ${corHex}` : '3px solid transparent',
            bgcolor: active ? alpha(corHex, 0.1) : 'transparent',
            color: active ? corHex : 'text.secondary',
            fontWeight: active ? 700 : 500,
            '&:hover': {
              bgcolor: alpha(corHex, 0.06),
              color: 'text.primary',
              '& .star-action-icon': { opacity: 1 }
            }
          }}
        >
          <ListItemIcon sx={{ minWidth: 24, color: active ? corHex : 'text.disabled' }}>
            <InsertDriveFileIcon style={{ fontSize: 16 }} />
          </ListItemIcon>

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
        </ListItemButton>
      </Tooltip>
    );
  };

  // Renderiza item de menu no modo colapsado (Estilo Minimals Pro: Ícone + Label Vertical + Chevron + Flyout)
  const renderCollapsedMenuItem = (menu: MenuItemDto) => {
    const hasChildren = menu.subMenus.length > 0 || menu.rotinas.length > 0;
    const isChildActive =
      menu.rotinas.some((r) => r.rotaCompleta === location.pathname) ||
      menu.subMenus.some((s) => s.rotinas.some((r) => r.rotaCompleta === location.pathname));

    return (
      <Box key={menu.idMenu} sx={{ display: 'flex', justifyContent: 'center', mb: 1, position: 'relative' }}>
        <ListItemButton
          onClick={(e) => hasChildren && handleOpenFlyout(e, menu)}
          sx={{
            width: 72,
            height: 68,
            borderRadius: 2.5,
            flexDirection: 'column',
            justifyContent: 'center',
            alignItems: 'center',
            p: 0.75,
            position: 'relative',
            bgcolor: isChildActive ? alpha(corHex, 0.12) : 'transparent',
            color: isChildActive ? corHex : 'text.secondary',
            border: isChildActive ? `1px solid ${alpha(corHex, 0.3)}` : '1px solid transparent',
            '&:hover': {
              bgcolor: alpha(corHex, 0.08),
              color: 'text.primary'
            }
          }}
        >
          <FolderIcon style={{ fontSize: 24, color: isChildActive ? corHex : alpha('#212B36', 0.6) }} />

          <Typography
            variant="caption"
            sx={{
              fontSize: '0.65rem',
              fontWeight: isChildActive ? 700 : 600,
              color: isChildActive ? corHex : 'text.secondary',
              textAlign: 'center',
              lineHeight: 1.1,
              mt: 0.5,
              width: '100%',
              display: '-webkit-box',
              WebkitLineClamp: 2,
              WebkitBoxOrient: 'vertical',
              overflow: 'hidden'
            }}
          >
            {menu.descricao}
          </Typography>

          {hasChildren && (
            <ChevronRightIcon
              sx={{
                fontSize: 14,
                position: 'absolute',
                right: 3,
                top: 10,
                color: isChildActive ? corHex : 'text.disabled'
              }}
            />
          )}
        </ListItemButton>
      </Box>
    );
  };

  // Renderiza item de menu no modo expandido (Accordion)
  const renderExpandedMenuItem = (menu: MenuItemDto, level = 0) => {
    const isOpen = openAccordionIds.includes(menu.idMenu);

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
            minHeight: 38,
            px: 1.5,
            py: 0,
            borderRadius: 1.5,
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
            {menu.subMenus.map((sub) => renderExpandedMenuItem(sub, level + 1))}
          </List>
        </AccordionDetails>
      </Accordion>
    );
  };

  return (
    <Box sx={{ position: 'relative' }}>
      {/* Botão Flutuante Circular de Toggle (< ou >) na linha divisória da Sidebar */}
      <IconButton
        size="small"
        onClick={() => setCollapsed(!collapsed)}
        sx={{
          position: 'fixed',
          top: 80,
          left: collapsed ? SIDEBAR_WIDTH_COLLAPSED - 12 : SIDEBAR_WIDTH_EXPANDED - 12,
          zIndex: (theme) => theme.zIndex.drawer + 2,
          width: 24,
          height: 24,
          bgcolor: 'background.paper',
          border: '1px solid rgba(145, 158, 171, 0.24)',
          boxShadow: '0 2px 6px rgba(0,0,0,0.12)',
          transition: 'left 0.3s ease, bgcolor 0.2s',
          '&:hover': {
            bgcolor: 'background.paper',
            borderColor: 'primary.main',
            boxShadow: '0 3px 8px rgba(0,0,0,0.18)'
          }
        }}
      >
        {collapsed ? (
          <ChevronRightIcon sx={{ fontSize: 14, color: 'text.secondary' }} />
        ) : (
          <ChevronLeftIcon sx={{ fontSize: 14, color: 'text.secondary' }} />
        )}
      </IconButton>

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
        {/* Cabeçalho da Sidebar */}
        <Box sx={{ px: 2, pt: 2, pb: 1 }}>
          <Typography
            variant="overline"
            sx={{
              color: 'text.disabled',
              fontWeight: 700,
              letterSpacing: 1.1,
              fontSize: '0.675rem',
              display: 'block',
              textAlign: collapsed ? 'center' : 'left'
            }}
          >
            {collapsed ? 'MENU' : 'NAVEGAÇÃO'}
          </Typography>
        </Box>

        {/* Conteúdo da Árvore de Menus */}
        <Box sx={{ px: collapsed ? 0.5 : 1, pb: 4 }}>
          {moduloAtivo?.menus.map((menu) =>
            collapsed ? renderCollapsedMenuItem(menu) : renderExpandedMenuItem(menu)
          )}
        </Box>
      </Drawer>

      {/* Popover Flutuante (Flyout) para submenus no modo colapsado */}
      <Popover
        open={Boolean(flyoutAnchor)}
        anchorEl={flyoutAnchor?.el}
        onClose={handleCloseFlyout}
        anchorOrigin={{
          vertical: 'top',
          horizontal: 'right'
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left'
        }}
        slotProps={{
          paper: {
            elevation: 8,
            sx: {
              ml: 1,
              p: 1.5,
              minWidth: 200,
              maxWidth: 280,
              borderRadius: 2.5,
              border: '1px solid rgba(145, 158, 171, 0.16)',
              backdropFilter: 'blur(8px)',
              bgcolor: 'rgba(255, 255, 255, 0.95)'
            }
          }
        }}
      >
        {flyoutAnchor && (
          <>
            <Typography
              variant="caption"
              sx={{ px: 1, py: 0.5, display: 'block', fontWeight: 800, color: 'text.secondary', textTransform: 'uppercase' }}
            >
              {flyoutAnchor.menu.descricao}
            </Typography>

            <List disablePadding sx={{ mt: 0.5 }}>
              {flyoutAnchor.menu.rotinas.map((r) =>
                renderRotinaItem(r, 0, handleCloseFlyout)
              )}
              {flyoutAnchor.menu.subMenus.map((sub) => (
                <Box key={sub.idMenu} sx={{ mt: 1 }}>
                  <Typography variant="caption" sx={{ px: 1, color: 'text.disabled', fontWeight: 700 }}>
                    {sub.descricao}
                  </Typography>
                  {sub.rotinas.map((r) => renderRotinaItem(r, 1, handleCloseFlyout))}
                </Box>
              ))}
            </List>
          </>
        )}
      </Popover>
    </Box>
  );
};
