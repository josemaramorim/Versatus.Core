import React from 'react';
import { Box, Typography } from '@mui/material';
import DashboardCustomizeOutlinedIcon from '@mui/icons-material/DashboardCustomizeOutlined';

export const WelcomeScreen: React.FC = () => (
  <Box
    sx={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100%',
      minHeight: 400,
      gap: 2,
      color: 'text.disabled',
      p: 4,
    }}
  >
    <DashboardCustomizeOutlinedIcon sx={{ fontSize: 64, opacity: 0.3 }} />
    <Typography variant="h6" sx={{ fontWeight: 600, opacity: 0.5 }}>
      Selecione uma opção no menu para começar
    </Typography>
    <Typography variant="body2" sx={{ opacity: 0.4, textAlign: 'center', maxWidth: 360 }}>
      Cada item do menu abre em uma nova aba. Você pode ter várias telas abertas simultaneamente.
    </Typography>
  </Box>
);
