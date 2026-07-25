import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Button,
  Popover,
  Box,
  Typography,
  Paper,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Tooltip,
  Badge,
  alpha
} from '@mui/material';
import StarIcon from '@mui/icons-material/Star';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import CloseIcon from '@mui/icons-material/Close';
import { useMenu } from '../../context/MenuContext';

export const FavoritesSelectorButton: React.FC = () => {
  const { favoritos, removerFavorito } = useMenu();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);
  const id = open ? 'favorites-selector-popover' : undefined;

  const favColor = '#F59E0B';

  return (
    <>
      <Button
        aria-describedby={id}
        onClick={handleClick}
        variant="outlined"
        sx={{
          borderRadius: '20px',
          borderColor: alpha(favColor, 0.4),
          color: 'text.primary',
          textTransform: 'none',
          px: 2,
          py: 0.75,
          bgcolor: alpha(favColor, 0.08),
          fontWeight: 600,
          '&:hover': {
            bgcolor: alpha(favColor, 0.15),
            borderColor: favColor
          }
        }}
        startIcon={
          <Badge badgeContent={favoritos.length} color="warning" max={99} sx={{ '& .MuiBadge-badge': { fontSize: '0.65rem', height: 16, minWidth: 16 } }}>
            <StarIcon style={{ fontSize: 18, color: favColor }} />
          </Badge>
        }
        endIcon={<KeyboardArrowDownIcon />}
      >
        Favoritos
      </Button>

      <Popover
        id={id}
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left'
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left'
        }}
        slotProps={{
          paper: {
            elevation: 8,
            sx: {
              p: 2,
              mt: 1,
              width: { xs: 300, sm: 380 },
              maxHeight: 440,
              overflowY: 'auto',
              borderRadius: 3,
              border: '1px solid rgba(145, 158, 171, 0.16)'
            }
          }
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5, px: 1 }}>
          <Typography variant="subtitle2" sx={{ color: 'text.secondary', fontWeight: 700, textTransform: 'uppercase' }}>
            MEUS FAVORITOS ({favoritos.length})
          </Typography>
        </Box>

        {favoritos.length === 0 ? (
          <Box sx={{ py: 4, textAlign: 'center', px: 2 }}>
            <StarIcon style={{ fontSize: 40, color: alpha(favColor, 0.3) }} />
            <Typography variant="body2" sx={{ color: 'text.secondary', mt: 1, fontWeight: 500 }}>
              Nenhum favorito adicionado.
            </Typography>
            <Typography variant="caption" sx={{ color: 'text.disabled', display: 'block', mt: 0.5 }}>
              Passe o mouse nas telas no menu lateral e clique na estrela ⭐ para favoritar.
            </Typography>
          </Box>
        ) : (
          <List disablePadding sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
            {favoritos.map((fav) => {
              const moduloCor = fav.corHex || '#2065D1';

              return (
                <Paper
                  key={fav.idFavorito}
                  variant="outlined"
                  sx={{
                    borderRadius: 2,
                    border: '1px solid rgba(145, 158, 171, 0.12)',
                    transition: 'all 0.2s',
                    '&:hover': {
                      bgcolor: alpha(moduloCor, 0.06),
                      borderColor: alpha(moduloCor, 0.3)
                    }
                  }}
                >
                  <ListItemButton
                    onClick={() => {
                      navigate(fav.rotaCompleta);
                      handleClose();
                    }}
                    sx={{
                      py: 1,
                      px: 1.5,
                      borderRadius: 2,
                      display: 'flex',
                      alignItems: 'center',
                      gap: 1.5
                    }}
                  >
                    <Box
                      sx={{
                        width: 10,
                        height: 10,
                        borderRadius: '50%',
                        bgcolor: moduloCor,
                        flexShrink: 0
                      }}
                    />

                    <ListItemText
                      primary={fav.nomeRotina}
                      secondary={`${fav.nomeModulo}`}
                      slotProps={{
                        primary: { variant: 'body2', sx: { fontWeight: 600, fontSize: '0.85rem' } },
                        secondary: { variant: 'caption', sx: { color: 'text.disabled', fontSize: '0.725rem' } }
                      }}
                    />

                    <Tooltip title="Remover dos favoritos">
                      <IconButton
                        size="small"
                        onClick={(e) => {
                          e.stopPropagation();
                          removerFavorito(fav.idRotina, fav.nomeRotina);
                        }}
                        sx={{
                          p: 0.5,
                          color: 'text.disabled',
                          '&:hover': { color: 'error.main', bgcolor: alpha('#EF4444', 0.1) }
                        }}
                      >
                        <CloseIcon style={{ fontSize: 16 }} />
                      </IconButton>
                    </Tooltip>
                  </ListItemButton>
                </Paper>
              );
            })}
          </List>
        )}
      </Popover>
    </>
  );
};
