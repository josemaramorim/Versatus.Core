# Diretrizes e Leis do Projeto Versatus.Net8

Este arquivo define as regras e restrições fundamentais que guiam todos os agentes de IA que atuam neste repositório. O não cumprimento destas regras constitui uma quebra de contrato.

---

## 1. Pureza de Domínio (C# Backend)
- Todas as classes na camada `Domain/Entities` devem ser **POCOs puras** (Plain Old CLR Objects).
- **Proibido:** Importar `System.ComponentModel.DataAnnotations` ou usar atributos como `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]` nas entidades do domínio.
- **Obrigatorio:** Toda a configuração e mapeamento de banco de dados deve ser configurada via **Fluent API** na camada de `Infrastructure` (ex: `DbContext.OnModelCreating`).

## 2. Abordagem Spec-First
- Nenhum código deve ser gerado ou alterado sem que a Especificação Funcional correspondente (`docs/spec_f[nome].md` ou na pasta `specs/`) esteja gerada e aprovada pelo usuário.
- A IA não deve "inventar" ou "otimizar" funcionalidades além do que está documentado na Spec e no código legado.
- Nomes de tabelas e colunas são sagrados: mantenha a nomenclatura idêntica ao legado (ex: `EntCliente` em vez de pluralizações como `Clientes`).

## 3. Arquitetura do Frontend (React/TypeScript)
- Toda listagem e formulário CRUD deve herdar e estender a infraestrutura orientada a objetos (`BaseCadastroConfig<T>`).
- **Proibido:** Inserir lógica hardcoded do backend, endpoints ou mapeamentos específicos dentro de componentes ou hooks reutilizáveis como `useCrudListState.ts`.
- **Obrigatorio:** Sobrescrever os métodos `mapBackendToForm` e `mapFormToBackend` no arquivo `[Nome]CadastroConfig.tsx` do formulário quando houver diferenças de enums ou estrutura de dados entre o legado/API e o formulário do React.
- **Obrigatorio:** Organizar as páginas do frontend em subpastas por módulo correspondente ao backend (ex.: `src/pages/[Modulo]/F[Nome]/`), em vez de manter uma estrutura plana diretamente em `src/pages/`. Isso espelha a modularização de negócio do sistema.

## 4. Estratégia Git e Proteção de Branches
- **Proibido:** A IA nunca deve fazer commits ou push diretamente nas branches estáveis `main` ou `develop`.
- **Obrigatorio:** Todo desenvolvimento deve ser isolado em uma branch de recurso (`feat/migrate-[nome]`).
- Os commits devem ser granulares por fase (Spec, Backend, Frontend, Rota) com mensagens padronizadas.
- O merge de `develop` para `main` é estritamente manual e de responsabilidade exclusiva do usuário.

## 5. Validação com Builds
- Após gerar código no Backend C#, o agente deve rodar obrigatoriamente `dotnet build` e verificar se há erros de compilação.
- Após gerar código no Frontend, o agente deve rodar `npm run build` para garantir a integridade das tipagens TypeScript e empacotamento Vite.
