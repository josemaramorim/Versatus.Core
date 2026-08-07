import React from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  IconButton,
  Avatar,
  Tooltip,
  alpha
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import { Sun, Moon } from 'lucide-react';
import { ModuleSelectorButton } from './ModuleSelectorButton';
import { FavoritesSelectorButton } from './FavoritesSelectorButton';
import { useThemeMode } from '../../context/ThemeContext';

export const TopBar: React.FC = () => {
  const { mode, toggleTheme } = useThemeMode();

  return (
    <AppBar
      position="sticky"
      color="inherit"
      elevation={0}
      sx={{
        top: 0,
        zIndex: (theme) => theme.zIndex.drawer + 1,
        backdropFilter: 'blur(6px)',
        bgcolor: (theme) => alpha(theme.palette.background.paper, 0.85),
        borderBottom: '1px solid',
        borderColor: 'divider'
      }}
    >
      <Toolbar variant="dense" sx={{ height: 64, px: { xs: 2, sm: 3 } }}>
        <Typography
          variant="h6"
          component="div"
          sx={{
            mr: 2.5,
            fontWeight: 800,
            letterSpacing: '-0.5px',
            color: 'primary.main',
            display: 'flex',
            alignItems: 'center',
            gap: 1,
            flexShrink: 0
          }}
        >
          VERSATUS{' '}
          <Typography
            component="span"
            variant="caption"
            sx={{ bgcolor: 'primary.main', color: '#fff', px: 0.8, py: 0.2, borderRadius: 1, fontWeight: 700 }}
          >
            ERP
          </Typography>
        </Typography>

        {/* Botões seletores de Módulo e Favoritos */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <ModuleSelectorButton />
          <FavoritesSelectorButton />
        </Box>

        <Box sx={{ flexGrow: 1 }} />

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexShrink: 0 }}>
          {/* Seletor discreto de Tema Claro / Escuro (IconButton puro com Sol/Lua) */}
          <Tooltip title={mode === 'dark' ? 'Alternar para tema claro' : 'Alternar para tema escuro'}>
            <IconButton
              size="small"
              onClick={toggleTheme}
              sx={{
                color: mode === 'dark' ? 'warning.light' : 'text.secondary',
                transition: 'transform 0.2s ease',
                '&:hover': { transform: 'scale(1.1)' }
              }}
            >
              {mode === 'dark' ? <Sun size={20} /> : <Moon size={20} />}
            </IconButton>
          </Tooltip>

          <Tooltip title="Busca (⌘K — Em breve)">
            <IconButton size="small" sx={{ color: 'text.secondary' }}>
              <SearchIcon />
            </IconButton>
          </Tooltip>

          <Tooltip title="Notificações">
            <IconButton size="small" sx={{ color: 'text.secondary' }}>
              <NotificationsNoneIcon />
            </IconButton>
          </Tooltip>

          <Avatar
            alt="Usuário Demonstrador"
            sx={{
              width: 34,
              height: 34,
              ml: 1,
              bgcolor: 'primary.main',
              fontSize: 14,
              fontWeight: 700,
              cursor: 'pointer'
            }}
          >
            US
          </Avatar>
        </Box>
      </Toolbar>
    </AppBar>
  );
};
