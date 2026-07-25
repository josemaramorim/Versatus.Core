import React, { useState } from 'react';
import {
  Button,
  Popover,
  Box,
  Typography,
  Grid,
  Paper,
  alpha
} from '@mui/material';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import * as MuiIcons from '@mui/icons-material';
import { useMenu } from '../../context/MenuContext';
import type { ModuloMenuDto } from '../../types/menu';

function getIconComponent(iconName: string | null) {
  if (!iconName) return MuiIcons.Apps;
  const Component = (MuiIcons as Record<string, any>)[iconName];
  return Component || MuiIcons.Apps;
}

// Fallback para gerar cor se corHex for nulo
function getModuloColor(modulo: ModuloMenuDto): string {
  if (modulo.corHex) return modulo.corHex;
  const colors = ['#2065D1', '#10B981', '#8B5CF6', '#06B6D4', '#EF4444', '#F59E0B', '#64748B'];
  return colors[modulo.idModulo % colors.length];
}

export const ModuleSelectorButton: React.FC = () => {
  const { modulos, moduloAtivo, setModuloAtivo } = useMenu();
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);
  const id = open ? 'module-selector-popover' : undefined;

  const currentColor = moduloAtivo ? getModuloColor(moduloAtivo) : '#2065D1';
  const CurrentIcon = moduloAtivo ? getIconComponent(moduloAtivo.iconeMui) : MuiIcons.Apps;

  return (
    <>
      <Button
        aria-describedby={id}
        onClick={handleClick}
        variant="outlined"
        sx={{
          borderRadius: '20px',
          borderColor: alpha(currentColor, 0.4),
          color: 'text.primary',
          textTransform: 'none',
          px: 2,
          py: 0.75,
          bgcolor: alpha(currentColor, 0.06),
          fontWeight: 600,
          '&:hover': {
            bgcolor: alpha(currentColor, 0.12),
            borderColor: currentColor
          }
        }}
        startIcon={
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Box
              sx={{
                width: 8,
                height: 8,
                borderRadius: '50%',
                bgcolor: currentColor,
                flexShrink: 0
              }}
            />
            <CurrentIcon style={{ fontSize: 18, color: currentColor }} />
          </Box>
        }
        endIcon={<KeyboardArrowDownIcon />}
      >
        {moduloAtivo ? moduloAtivo.nome : 'Selecione o Módulo'}
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
              p: 2.5,
              mt: 1,
              width: { xs: 340, sm: 520, md: 640 },
              maxHeight: 520,
              overflowY: 'auto',
              borderRadius: 3,
              border: '1px solid rgba(145, 158, 171, 0.16)'
            }
          }
        }}
      >
        <Typography variant="subtitle2" sx={{ color: 'text.secondary', mb: 2, fontWeight: 700, textTransform: 'uppercase' }}>
          MÓDULOS DO SISTEMA
        </Typography>

        <Grid container spacing={1.5}>
          {modulos.map((m) => {
            const isSelected = moduloAtivo?.idModulo === m.idModulo;
            const color = getModuloColor(m);
            const IconComp = getIconComponent(m.iconeMui);

            return (
              <Grid key={m.idModulo} size={{ xs: 4, sm: 3, md: 3 }}>
                <Paper
                  variant="outlined"
                  onClick={() => {
                    setModuloAtivo(m);
                    handleClose();
                  }}
                  sx={{
                    p: 1.5,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    cursor: 'pointer',
                    borderRadius: 2,
                    borderWidth: isSelected ? '2px' : '1px',
                    borderColor: isSelected ? color : 'rgba(145, 158, 171, 0.2)',
                    bgcolor: isSelected ? alpha(color, 0.08) : 'background.paper',
                    transition: 'all 0.2s ease-in-out',
                    '&:hover': {
                      bgcolor: alpha(color, 0.12),
                      transform: 'translateY(-2px)',
                      boxShadow: `0 4px 12px ${alpha(color, 0.2)}`
                    }
                  }}
                >
                  <Box
                    sx={{
                      width: 40,
                      height: 40,
                      borderRadius: '12px',
                      bgcolor: alpha(color, 0.12),
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      mb: 1
                    }}
                  >
                    <IconComp style={{ color, fontSize: 22 }} />
                  </Box>
                  <Typography
                    variant="caption"
                    sx={{
                      fontWeight: isSelected ? 700 : 500,
                      color: isSelected ? color : 'text.primary',
                      textAlign: 'center',
                      lineHeight: 1.2,
                      display: '-webkit-box',
                      WebkitLineClamp: 2,
                      WebkitBoxOrient: 'vertical',
                      overflow: 'hidden',
                      height: 28
                    }}
                  >
                    {m.nome}
                  </Typography>
                </Paper>
              </Grid>
            );
          })}
        </Grid>
      </Popover>
    </>
  );
};
