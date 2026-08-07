import React from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Chip,
  Avatar,
  Divider,
  Stack,
  Paper,
  LinearProgress,
  alpha,
  useTheme
} from '@mui/material';
import {
  Building2,
  ShieldCheck,
  UserCheck,
  Server,
  Database,
  KeyRound,
  Activity,
  CheckCircle2,
  Cpu,
  HardDrive
} from 'lucide-react';

export const DashboardScreen: React.FC = () => {
  const theme = useTheme();

  return (
    <Box
      sx={{
        p: { xs: 2, sm: 3, md: 4 },
        minHeight: '100%',
        bgcolor: 'background.default',
        display: 'flex',
        flexDirection: 'column',
        gap: 3
      }}
    >
      {/* ── 1. CABEÇALHO DO DASHBOARD ───────────────────────────────────────── */}
      <Paper
        elevation={0}
        sx={{
          p: 3,
          borderRadius: 3,
          background: `linear-gradient(135deg, ${alpha(theme.palette.primary.main, 0.05)} 0%, ${alpha(
            theme.palette.primary.dark,
            0.12
          )} 100%)`,
          border: '1px solid',
          borderColor: alpha(theme.palette.primary.main, 0.15),
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          alignItems: { xs: 'flex-start', sm: 'center' },
          justifyContent: 'space-between',
          gap: 2
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Avatar
            sx={{
              width: 54,
              height: 54,
              bgcolor: 'primary.main',
              boxShadow: `0 4px 14px ${alpha(theme.palette.primary.main, 0.35)}`
            }}
          >
            <Activity size={28} color="#fff" />
          </Avatar>
          <Box>
            <Typography variant="h5" sx={{ fontWeight: 800, color: 'text.primary', letterSpacing: '-0.5px' }}>
              Painel de Controle Versatus
            </Typography>
            <Typography variant="body2" sx={{ color: 'text.secondary', fontWeight: 500 }}>
              Visão geral do ambiente, status de licenciamento e saúde da infraestrutura ERP
            </Typography>
          </Box>
        </Box>

        <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
          <Chip
            icon={<CheckCircle2 size={16} style={{ color: theme.palette.success.main }} />}
            label="Sistema Operacional & Regular"
            color="success"
            variant="outlined"
            sx={{ fontWeight: 600, bgcolor: alpha(theme.palette.success.main, 0.08), borderRadius: 2 }}
          />
          <Chip
            label=".NET 10 WebAPI"
            color="primary"
            size="small"
            sx={{ fontWeight: 700, borderRadius: 1.5 }}
          />
        </Stack>
      </Paper>

      {/* ── 2. CARDS PRINCIPAIS: EMPRESA, LICENÇA E USUÁRIO ─────────────────── */}
      <Grid container spacing={3}>
        {/* Card 1: Licenciamento & Contrato */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card
            elevation={0}
            sx={{
              height: '100%',
              borderRadius: 3,
              border: '1px solid',
              borderColor: 'divider',
              transition: 'all 0.2s ease',
              '&:hover': {
                boxShadow: `0 8px 24px ${alpha(theme.palette.common.black, 0.06)}`,
                borderColor: alpha(theme.palette.primary.main, 0.3)
              }
            }}
          >
            <CardContent sx={{ p: 3, display: 'flex', flexDirection: 'column', height: '100%', gap: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Typography variant="subtitle2" sx={{ fontWeight: 700, color: 'text.secondary', textTransform: 'uppercase', fontSize: '0.75rem' }}>
                  Licenciamento ERP
                </Typography>
                <Avatar sx={{ width: 36, height: 36, bgcolor: alpha(theme.palette.success.main, 0.1), color: 'success.main' }}>
                  <ShieldCheck size={20} />
                </Avatar>
              </Box>

              <Box>
                <Typography variant="h6" sx={{ fontWeight: 800, color: 'text.primary' }}>
                  Versatus Enterprise .NET 10
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', mt: 0.5 }}>
                  Licença Anual de Software
                </Typography>
              </Box>

              <Divider sx={{ my: 0.5 }} />

              <Stack spacing={1.5}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Status do Licenciamento:
                  </Typography>
                  <Chip label="🟢 Ativa & Regular" size="small" color="success" sx={{ fontWeight: 700, height: 22 }} />
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Validade da Licença:
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: 'text.primary' }}>
                    21/09/2026 (45 dias restantes)
                  </Typography>
                </Box>

                <Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                    <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                      Sessões / Limite Usuários:
                    </Typography>
                    <Typography variant="caption" sx={{ fontWeight: 700, color: 'primary.main' }}>
                      8 de 10 Ativos (80%)
                    </Typography>
                  </Box>
                  <LinearProgress variant="determinate" value={80} color="primary" sx={{ height: 6, borderRadius: 3 }} />
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>

        {/* Card 2: Empresa & Filial Conectada */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card
            elevation={0}
            sx={{
              height: '100%',
              borderRadius: 3,
              border: '1px solid',
              borderColor: 'divider',
              transition: 'all 0.2s ease',
              '&:hover': {
                boxShadow: `0 8px 24px ${alpha(theme.palette.common.black, 0.06)}`,
                borderColor: alpha(theme.palette.primary.main, 0.3)
              }
            }}
          >
            <CardContent sx={{ p: 3, display: 'flex', flexDirection: 'column', height: '100%', gap: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Typography variant="subtitle2" sx={{ fontWeight: 700, color: 'text.secondary', textTransform: 'uppercase', fontSize: '0.75rem' }}>
                  Empresa & Filial
                </Typography>
                <Avatar sx={{ width: 36, height: 36, bgcolor: alpha(theme.palette.primary.main, 0.1), color: 'primary.main' }}>
                  <Building2 size={20} />
                </Avatar>
              </Box>

              <Box>
                <Typography variant="h6" sx={{ fontWeight: 800, color: 'text.primary', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  Versatus Tecnologia Ltda.
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', mt: 0.5 }}>
                  CNPJ: 12.345.678/0001-90
                </Typography>
              </Box>

              <Divider sx={{ my: 0.5 }} />

              <Stack spacing={1.5}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Filial Conectada:
                  </Typography>
                  <Chip label="01 — Matriz Principal" size="small" color="primary" variant="outlined" sx={{ fontWeight: 700, height: 22 }} />
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Regime Tributário:
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: 'text.primary' }}>
                    Lucro Presumido
                  </Typography>
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Certificado Digital A1:
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: 'success.main' }}>
                    Válido até 15/12/2026
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>

        {/* Card 3: Usuário Logado & Sessão */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card
            elevation={0}
            sx={{
              height: '100%',
              borderRadius: 3,
              border: '1px solid',
              borderColor: 'divider',
              transition: 'all 0.2s ease',
              '&:hover': {
                boxShadow: `0 8px 24px ${alpha(theme.palette.common.black, 0.06)}`,
                borderColor: alpha(theme.palette.primary.main, 0.3)
              }
            }}
          >
            <CardContent sx={{ p: 3, display: 'flex', flexDirection: 'column', height: '100%', gap: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Typography variant="subtitle2" sx={{ fontWeight: 700, color: 'text.secondary', textTransform: 'uppercase', fontSize: '0.75rem' }}>
                  Sessão do Usuário
                </Typography>
                <Avatar sx={{ width: 36, height: 36, bgcolor: alpha(theme.palette.info.main, 0.1), color: 'info.main' }}>
                  <UserCheck size={20} />
                </Avatar>
              </Box>

              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                <Avatar sx={{ width: 44, height: 44, bgcolor: 'primary.main', fontWeight: 700 }}>
                  US
                </Avatar>
                <Box sx={{ minWidth: 0 }}>
                  <Typography variant="h6" sx={{ fontWeight: 800, color: 'text.primary', fontSize: '1rem', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    Admin Versatus
                  </Typography>
                  <Typography variant="caption" sx={{ color: 'text.secondary', display: 'block' }}>
                    Login: <strong>admin</strong> | Perfil: <strong>Administrador</strong>
                  </Typography>
                </Box>
              </Box>

              <Divider sx={{ my: 0.5 }} />

              <Stack spacing={1.5}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Status da Conta:
                  </Typography>
                  <Chip label="Ativo & Autenticado" size="small" color="success" sx={{ fontWeight: 700, height: 22 }} />
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    IP de Conexão:
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: 'text.primary', fontFamily: 'monospace' }}>
                    127.0.0.1 (Local)
                  </Typography>
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600 }}>
                    Início da Sessão:
                  </Typography>
                  <Typography variant="caption" sx={{ fontWeight: 700, color: 'text.primary' }}>
                    Hoje às 16:37
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* ── 3. SEÇÃO DE DIAGNÓSTICO & SAÚDE DO SISTEMA (CQRS / API / DB) ───── */}
      <Box>
        <Typography variant="subtitle1" sx={{ fontWeight: 800, color: 'text.primary', mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
          <Cpu size={20} color={theme.palette.primary.main} />
          Diagnóstico de Infraestrutura & Banco de Dados (CQRS Split)
        </Typography>

        <Grid container spacing={2.5}>
          {/* Item 1: WebAPI .NET 10 */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Paper
              elevation={0}
              sx={{
                p: 2.5,
                borderRadius: 2.5,
                border: '1px solid',
                borderColor: 'divider',
                bgcolor: 'background.paper',
                display: 'flex',
                alignItems: 'center',
                gap: 2
              }}
            >
              <Avatar sx={{ bgcolor: alpha(theme.palette.primary.main, 0.1), color: 'primary.main', width: 42, height: 42 }}>
                <Server size={22} />
              </Avatar>
              <Box sx={{ minWidth: 0 }}>
                <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600, display: 'block' }}>
                  Backend WebAPI
                </Typography>
                <Typography variant="subtitle2" sx={{ fontWeight: 800, color: 'success.main' }}>
                  .NET 10 (200 OK)
                </Typography>
                <Typography variant="caption" sx={{ color: 'text.disabled', fontSize: '0.7rem' }}>
                  Latência: 14ms
                </Typography>
              </Box>
            </Paper>
          </Grid>

          {/* Item 2: Conexão de Escrita (Write DB) */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Paper
              elevation={0}
              sx={{
                p: 2.5,
                borderRadius: 2.5,
                border: '1px solid',
                borderColor: 'divider',
                bgcolor: 'background.paper',
                display: 'flex',
                alignItems: 'center',
                gap: 2
              }}
            >
              <Avatar sx={{ bgcolor: alpha(theme.palette.info.main, 0.1), color: 'info.main', width: 42, height: 42 }}>
                <Database size={22} />
              </Avatar>
              <Box sx={{ minWidth: 0 }}>
                <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600, display: 'block' }}>
                  Conexão Mutações (Write)
                </Typography>
                <Typography variant="subtitle2" sx={{ fontWeight: 800, color: 'info.main' }}>
                  WriteConnection OK
                </Typography>
                <Typography variant="caption" sx={{ color: 'text.disabled', fontSize: '0.7rem' }}>
                  Transações & Locks
                </Typography>
              </Box>
            </Paper>
          </Grid>

          {/* Item 3: Conexão de Leitura (Read DB Split) */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Paper
              elevation={0}
              sx={{
                p: 2.5,
                borderRadius: 2.5,
                border: '1px solid',
                borderColor: 'divider',
                bgcolor: 'background.paper',
                display: 'flex',
                alignItems: 'center',
                gap: 2
              }}
            >
              <Avatar sx={{ bgcolor: alpha(theme.palette.success.main, 0.1), color: 'success.main', width: 42, height: 42 }}>
                <HardDrive size={22} />
              </Avatar>
              <Box sx={{ minWidth: 0 }}>
                <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600, display: 'block' }}>
                  Conexão Leitura (Read Split)
                </Typography>
                <Typography variant="subtitle2" sx={{ fontWeight: 800, color: 'success.main' }}>
                  ReadConnection OK
                </Typography>
                <Typography variant="caption" sx={{ color: 'text.disabled', fontSize: '0.7rem' }}>
                  No-Tracking (Fast GET)
                </Typography>
              </Box>
            </Paper>
          </Grid>

          {/* Item 4: SQL Server 2008 Compatibility */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Paper
              elevation={0}
              sx={{
                p: 2.5,
                borderRadius: 2.5,
                border: '1px solid',
                borderColor: 'divider',
                bgcolor: 'background.paper',
                display: 'flex',
                alignItems: 'center',
                gap: 2
              }}
            >
              <Avatar sx={{ bgcolor: alpha(theme.palette.warning.main, 0.1), color: 'warning.main', width: 42, height: 42 }}>
                <KeyRound size={22} />
              </Avatar>
              <Box sx={{ minWidth: 0 }}>
                <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600, display: 'block' }}>
                  Banco de Dados Legado
                </Typography>
                <Typography variant="subtitle2" sx={{ fontWeight: 800, color: 'warning.dark' }}>
                  SQL Server 2008
                </Typography>
                <Typography variant="caption" sx={{ color: 'text.disabled', fontSize: '0.7rem' }}>
                  Paginação em Memória
                </Typography>
              </Box>
            </Paper>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );
};
