import React, { useRef, useLayoutEffect, useState } from 'react';
import { Box, Tooltip, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { useTabs } from '../../context/TabsContext';
import { TabOverflowMenu } from './TabOverflowMenu';

const TAB_MIN_WIDTH = 160;
const TAB_MAX_WIDTH = 220;
const OVERFLOW_BTN_WIDTH = 56;

export const TabBar: React.FC = () => {
  const { abas, abaAtivaId, ativarAba, fecharAba } = useTabs();
  const containerRef = useRef<HTMLDivElement>(null);
  const [visivelCount, setVisivelCount] = useState(abas.length);

  // Recalcula quantas abas cabem sempre que a lista ou o tamanho muda
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
        px: 1,
        pt: 0.5,
        minHeight: 40,
        overflow: 'hidden',
        flexShrink: 0,
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
                gap: 0.5,
                minWidth: TAB_MIN_WIDTH,
                maxWidth: TAB_MAX_WIDTH,
                height: 36,
                px: 1.5,
                mr: 0.5,
                borderRadius: '6px 6px 0 0',
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
                '&:hover': {
                  bgcolor: isAtiva ? 'background.paper' : 'grey.300',
                },
              }}
            >
              {/* Título da aba */}
              <Box
                component="span"
                sx={{
                  flexGrow: 1,
                  fontSize: '0.78rem',
                  fontWeight: isAtiva ? 600 : 400,
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
                  width: 18,
                  height: 18,
                  flexShrink: 0,
                  color: 'text.disabled',
                  '&:hover': { color: 'text.primary', bgcolor: 'action.hover' },
                }}
              >
                <CloseIcon sx={{ fontSize: 12 }} />
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
