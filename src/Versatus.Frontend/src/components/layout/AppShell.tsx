import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box } from '@mui/material';
import { TopBar } from './TopBar';
import { ContextualSidebar } from './ContextualSidebar';

export const AppShell: React.FC = () => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', bgcolor: 'background.default' }}>
      <TopBar />
      <Box sx={{ display: 'flex', flexGrow: 1, position: 'relative' }}>
        <ContextualSidebar />
        <Box
          component="main"
          sx={{
            flexGrow: 1,
            p: 3,
            minWidth: 0,
            overflowX: 'auto',
            bgcolor: 'background.default'
          }}
        >
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
};
