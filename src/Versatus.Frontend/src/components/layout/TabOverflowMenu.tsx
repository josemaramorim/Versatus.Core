import React, { useState } from 'react';
import { Box, Menu, MenuItem, Badge, Tooltip } from '@mui/material';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import type { TabItem } from '../../types/tabs';
import { useTabs } from '../../context/TabsContext';

interface Props {
  abasOcultas: TabItem[];
}

export const TabOverflowMenu: React.FC<Props> = ({ abasOcultas }) => {
  const { ativarAba } = useTabs();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleOpen = (e: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(e.currentTarget);
  };
  const handleClose = () => setAnchorEl(null);

  const handleSelecionar = (id: string) => {
    ativarAba(id);
    handleClose();
  };

  return (
    <>
      <Tooltip title="Abas ocultas">
        <Box
          onClick={handleOpen}
          sx={{
            display: 'flex',
            alignItems: 'center',
            gap: 0.25,
            ml: 'auto',
            mr: 0.5,
            px: 1,
            py: 0.5,
            height: 28,
            borderRadius: 2,
            bgcolor: 'warning.main',
            color: 'warning.contrastText',
            cursor: 'pointer',
            fontSize: '0.72rem',
            fontWeight: 700,
            flexShrink: 0,
            alignSelf: 'center',
            userSelect: 'none',
            '&:hover': { bgcolor: 'warning.dark' },
            transition: 'background-color 0.15s ease',
          }}
        >
          <Badge badgeContent={abasOcultas.length} color="warning" sx={{ '& .MuiBadge-badge': { display: 'none' } }} />
          +{abasOcultas.length}
          <KeyboardArrowDownIcon sx={{ fontSize: 14 }} />
        </Box>
      </Tooltip>

      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
        slotProps={{ paper: { sx: { minWidth: 220, mt: 0.5 } } }}
      >
        {abasOcultas.map(aba => (
          <MenuItem
            key={aba.id}
            onClick={() => handleSelecionar(aba.id)}
            sx={{ fontSize: '0.85rem', py: 0.75 }}
          >
            <Box sx={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
              {aba.titulo}
              {aba.isDirty && (
                <Box component="span" sx={{ ml: 0.75, color: 'warning.main', fontSize: '0.7rem' }}>● não salvo</Box>
              )}
            </Box>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};
