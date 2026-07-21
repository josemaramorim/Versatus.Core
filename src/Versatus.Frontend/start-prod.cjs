const { spawn } = require('child_process');

// Verifica se está em modo de produção ou se o comando de serve foi passado como argumento pelo painel ICP
const isProd = process.env.NODE_ENV === 'production' || 
               process.argv.some(arg => arg.includes('serve') || arg.includes('dist'));

if (isProd) {
  console.log('Iniciando build de produção e inicialização do servidor estático...');
  
  // Executa o build da aplicação
  const build = spawn('npm', ['run', 'build'], { stdio: 'inherit', shell: true });
  build.on('close', (code) => {
    if (code !== 0) {
      console.error('Erro: Falha no build da aplicação (código ' + code + ')');
      process.exit(code);
    }
    
    // Extrai a porta dos argumentos passados pelo painel
    let port = '3000';
    const lIndex = process.argv.indexOf('-l');
    if (lIndex !== -1 && lIndex + 1 < process.argv.length) {
      port = process.argv[lIndex + 1];
    } else {
      // Caso a porta venha como último argumento numérico
      const lastArg = process.argv[process.argv.length - 1];
      if (/^\d+$/.test(lastArg)) {
        port = lastArg;
      }
    }
    
    console.log(`Servindo a pasta dist na porta ${port}...`);
    spawn('npx', ['serve', '-s', 'dist', '-l', port], { stdio: 'inherit', shell: true });
  });
} else {
  console.log('Iniciando servidor de desenvolvimento local (Vite)...');
  spawn('npx', ['vite'], { stdio: 'inherit', shell: true });
}
