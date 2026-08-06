import React, { useRef, useLayoutEffect, useState } from 'react';
import { Box, Tooltip, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import BusinessIcon from '@mui/icons-material/Business';
import TuneIcon from '@mui/icons-material/Tune';
import CreditCardIcon from '@mui/icons-material/CreditCard';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import { useTabs } from '../../context/TabsContext';
import { TabOverflowMenu } from './TabOverflowMenu';

const TAB_MIN_WIDTH = 150;
const TAB_MAX_WIDTH = 220;
const OVERFLOW_BTN_WIDTH = 64;

function getTabIcon(rota: string) {
  if (rota.includes('entidade')) {
    return <BusinessIcon sx={{ fontSize: 16, color: 'primary.main', mr: 0.75, flexShrink: 0 }} />;
  }
  if (rota.includes('parametro')) {
    return <TuneIcon sx={{ fontSize: 16, color: 'primary.main', mr: 0.75, flexShrink: 0 }} />;
  }
  if (rota.includes('condicao')) {
    return <CreditCardIcon sx={{ fontSize: 16, color: 'primary.main', mr: 0.75, flexShrink: 0 }} />;
  }
  return <InsertDriveFileIcon sx={{ fontSize: 16, color: 'primary.main', mr: 0.75, flexShrink: 0 }} />;
}

export const TabBar: React.FC = () => {
  const { abas, abaAtivaId, ativarAba, fecharAba } = useTabs();
  const containerRef = useRef<HTMLDivElement>(null);
  const [visivelCount, setVisivelCount] = useState(abas.length);

  useLayoutEffect(() => {
    const calcular = () => {
      if (!containerRef.current) return;
      const larguraTotal = containerRef.current.offsetWidth - OVERFLOW_BTN_WIDTH;
      const cabem = Math.max(1, Math.floor(larguraTotal / TAB_MIN_WIDTH));
      setVisivelCount(Math.min(cabem, abas.length));
    };
    calcular();
    const observer = new ResizeObserver(calcular);
    if (containerRef.current) observer.observe(containerRef.current);
    return () => observer.disconnect();
  }, [abas.length]);

  if (abas.length === 0) return null;

  const abasVisiveis = abas.slice(0, visivelCount);
  const abasOcultas = abas.slice(visivelCount);

  const handleFechar = (e: React.MouseEvent, id: string) => {
    e.stopPropagation();
    fecharAba(id);
  };

  return (
    <Box
      ref={containerRef}
      sx={{
        display: 'flex',
        alignItems: 'flex-end',
        bgcolor: 'grey.100',
        borderBottom: '1px solid',
        borderColor: 'divider',
        px: 1.5,
        pt: 0.75,
        minHeight: 42,
        overflow: 'hidden',
        flexShrink: 0,
        width: '100%',
      }}
    >
      {/* Abas visíveis */}
      {abasVisiveis.map(aba => {
        const isAtiva = aba.id === abaAtivaId;
        return (
          <Tooltip key={aba.id} title={aba.titulo} enterDelay={600}>
            <Box
              onClick={() => ativarAba(aba.id)}
              sx={{
                display: 'flex',
                alignItems: 'center',
                minWidth: TAB_MIN_WIDTH,
                maxWidth: TAB_MAX_WIDTH,
                height: 36,
                px: 1.5,
                mr: 0.75,
                borderRadius: '8px 8px 0 0',
                border: '1px solid',
                borderBottom: 'none',
                cursor: 'pointer',
                userSelect: 'none',
                flexShrink: 0,
                transition: 'all 0.15s ease',
                bgcolor: isAtiva ? 'background.paper' : 'grey.200',
                borderColor: isAtiva ? 'divider' : 'transparent',
                borderBottomWidth: isAtiva ? 2 : 0,
                borderBottomColor: isAtiva ? 'primary.main' : 'transparent',
                boxShadow: isAtiva ? '0 -2px 6px rgba(0,0,0,0.04)' : 'none',
                '&:hover': {
                  bgcolor: isAtiva ? 'background.paper' : 'grey.300',
                },
              }}
            >
              {/* Ícone da aba */}
              {getTabIcon(aba.rota)}

              {/* Título da aba */}
              <Box
                component="span"
                sx={{
                  flexGrow: 1,
                  fontSize: '0.8rem',
                  fontWeight: isAtiva ? 700 : 500,
                  color: isAtiva ? 'text.primary' : 'text.secondary',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap',
                }}
              >
                {aba.titulo}
                {aba.isDirty && (
                  <Box component="span" sx={{ ml: 0.5, color: 'warning.main' }}>●</Box>
                )}
              </Box>

              {/* Botão fechar */}
              <IconButton
                size="small"
                onClick={(e) => handleFechar(e, aba.id)}
                sx={{
                  width: 20,
                  height: 20,
                  ml: 0.5,
                  flexShrink: 0,
                  color: 'text.disabled',
                  '&:hover': { color: 'text.primary', bgcolor: 'action.hover' },
                }}
              >
                <CloseIcon sx={{ fontSize: 13 }} />
              </IconButton>
            </Box>
          </Tooltip>
        );
      })}

      {/* Botão de overflow +N */}
      {abasOcultas.length > 0 && (
        <TabOverflowMenu abasOcultas={abasOcultas} />
      )}
    </Box>
  );
};
