import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  IconButton,
  Avatar,
  Tooltip,
  Chip,
  alpha
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import StarIcon from '@mui/icons-material/Star';
import CloseIcon from '@mui/icons-material/Close';
import { ModuleSelectorButton } from './ModuleSelectorButton';
import { useMenu } from '../../context/MenuContext';

export const TopBar: React.FC = () => {
  const { favoritos, removerFavorito } = useMenu();
  const location = useLocation();
  const navigate = useNavigate();

  return (
    <AppBar
      position="sticky"
      color="inherit"
      elevation={0}
      sx={{
        top: 0,
        zIndex: (theme) => theme.zIndex.drawer + 1,
        backdropFilter: 'blur(6px)',
        bgcolor: 'rgba(255, 255, 255, 0.85)',
        borderBottom: '1px solid rgba(145, 158, 171, 0.12)'
      }}
    >
      <Toolbar variant="dense" sx={{ height: 64, px: { xs: 2, sm: 3 } }}>
        <Typography
          variant="h6"
          component="div"
          sx={{
            mr: 2,
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

        <ModuleSelectorButton />

        {/* Barra de Favoritos rápida na TopBar */}
        {favoritos.length > 0 && (
          <Box
            sx={{
              display: { xs: 'none', md: 'flex' },
              alignItems: 'center',
              gap: 0.75,
              ml: 2,
              maxWidth: '45vw',
              overflowX: 'auto',
              py: 0.5,
              '&::-webkit-scrollbar': { height: 3 },
              '&::-webkit-scrollbar-thumb': { bgcolor: 'rgba(0,0,0,0.1)', borderRadius: 2 }
            }}
          >
            {favoritos.map((fav) => {
              const active = location.pathname === fav.rotaCompleta;
              const chipColor = fav.corHex || '#2065D1';

              return (
                <Tooltip key={fav.idFavorito} title={`${fav.nomeModulo} > ${fav.nomeRotina}`}>
                  <Chip
                    size="small"
                    icon={<StarIcon style={{ fontSize: 13, color: '#F59E0B' }} />}
                    label={fav.nomeRotina}
                    onClick={() => navigate(fav.rotaCompleta)}
                    onDelete={(e) => {
                      e.stopPropagation();
                      removerFavorito(fav.idRotina, fav.nomeRotina);
                    }}
                    deleteIcon={<CloseIcon style={{ fontSize: 12 }} />}
                    sx={{
                      height: 26,
                      fontSize: '0.75rem',
                      fontWeight: active ? 700 : 500,
                      bgcolor: active ? alpha(chipColor, 0.15) : 'action.hover',
                      color: active ? chipColor : 'text.primary',
                      border: active ? `1px solid ${alpha(chipColor, 0.4)}` : '1px solid transparent',
                      cursor: 'pointer',
                      flexShrink: 0,
                      transition: 'all 0.2s ease',
                      '&:hover': {
                        bgcolor: alpha(chipColor, 0.2),
                        transform: 'translateY(-1px)'
                      }
                    }}
                  />
                </Tooltip>
              );
            })}
          </Box>
        )}

        <Box sx={{ flexGrow: 1 }} />

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexShrink: 0 }}>
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
