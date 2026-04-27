# MOD-08 — Nota Fiscal Eletrônica (NFe)

> **Fase:** 3 | **Prioridade:** ALTA | **Status:** Pausado - Estrutura inicial criada (2026-04-27), aguardando conclusão dos módulos iniciais (MOD-01 a MOD-07) conforme ordem de dependências.

## Visão Geral
Emissão e controle de NF-e (modelo 55), NF-Ce e eventos relacionados
(cancelamento, CCe - Carta de Correção, inutilização).

## Inventário

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `NFEGen.cs` | **253 KB** | **Central** | Gerador de XML da NF-e (ENORME) |
| `NFeDocumento.cs` | 60 KB | Entidade | Documento NF-e |
| `NFeDocumentoHistorico.cs` | 9 KB | Relacionamento | Histórico da NF-e |
| `NFeDocumentoImage.cs` | 3 KB | Relacionamento | Imagem (DANFE) |
| `NFeEventoBase.cs` | 7 KB | Base | Base de eventos da NF-e |
| `NFeInutilizacao.cs` | 5 KB | Entidade | Inutilização de série |
| `NFeLote.cs` | 2 KB | Entidade | Lote de NF-e |
| `NFeSequencialEvento.cs` | 8 KB | Entidade | Sequencial de eventos |
| `NFeToken.cs` | 11 KB | Entidade | Token de autenticação SEFAZ |
| `NFeTokenFilial.cs` | 6 KB | Relacionamento | Token por filial |
| `NFeUtil.cs` | 1 KB | Utilitário | Utilitários |
| `CCe/` | Pasta | Subpasta | Carta de Correção Eletrônica |
| `Cancelamento NF-e/` | Pasta | Subpasta | Cancelamento de NF-e |
| `Layouts/` | Pasta | Subpasta | Layouts XML (schemas) |

## Regras Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-08-001 — NFEGen.cs é monolítico
Com 253 KB, este arquivo gera o XML completo da NF-e.
**Não tentar migrar de uma vez.** Dividir por seções do XML (Ide, Emit, Dest, Det, Total...).

### RN-08-002 — Schemas de XML
Os layouts e schemas XSD da NF-e são fornecidos pela SEFAZ e mudam com versões.
Manter a pasta `Layouts/` e garantir que o novo gerador use os mesmos schemas.

### RN-08-003 — Comunicação com SEFAZ
O legado usa serviços Web Service SOAP da SEFAZ.
No novo sistema, avaliar uso de biblioteca especializada (ex: `NFe.io SDK` ou similar).

### RN-08-004 — Certificado Digital
A assinatura da NF-e usa certificado digital A1 ou A3.
`NFeToken` e `CertificadoDigital` (MOD-02) são críticos.

## Checklist MOD-08
- [ ] `NFEGen.cs` analisado e dividido por seções
- [ ] NFeDocumento implementado
- [ ] Geração de XML implementada (seção por seção)
- [ ] Comunicação SEFAZ implementada
- [ ] Cancelamento e CCe implementados
- [ ] Testes com SEFAZ ambiente homologação

---

## Roteiro de Tarefas — MOD-08 (NFe)

Este roteiro descreve a implementação de MOD-08. A migração deve ser feita em etapas pequenas, especialmente para `NFEGen.cs` e para os layouts XML de SEFAZ.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.NFe**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.NFe`
- Adicione referências a `Versatus.Framework`, `Versatus.AcessoGlobal`, `Versatus.GestaoTributo`, `Versatus.Faturamento`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/nfe-project`
- Commit: `setup: Create Versatus.NFe project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Documentos/`, `Eventos/`, `Tokens/`, `Layouts/`, `Util/`
- Branch: `setup/nfe-structure`
- Commit: `setup: Create NFe folder structure`

**Tarefa 1.3 — Criar DbContext base**
- Crie `Infrastructure/NFeDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/nfe-dbcontext-base`
- Commit: `setup: Create NFeDbContext base`

### Fase 2: Análise Crítica do Legado

**Tarefa 2.1 — Analisar `NFEGen.cs`**
- Leia todo o arquivo `NFEGen.cs` (253 KB)
- Divida o gerador em seções do XML: Ide, Emit, Dest, Det, Total, Transporte, Cobr, Pag, InfAdic
- Documente dependências de `CertificadoDigital`, `NFeToken`, `Layouts` e `SEFAZ`
- Branch: `analysis/nfegen`
- Commit: `docs: Analyze legacy NFEGen.cs for sectioned migration`

**Tarefa 2.2 — Analisar layouts XML e schemas**
- Levante os arquivos em `Layouts/` e identifique os schemas XSD usados
- Documente versões do XML e requisitos de homologação SEFAZ
- Branch: `analysis/nfe-layouts`
- Commit: `docs: Analyze NFe XML layouts and schema versions`

### Fase 3: Documento NF-e e Tokens

**Tarefa 3.1 — Implementar NFeDocumento**
- Crie `Domain/Documentos/NFeDocumento.cs`, `NFeDocumentoHistorico.cs`, `NFeDocumentoImage.cs`
- Use coleções genéricas para relacionamentos e evite classes `*Lista`
- Branch: `feat/nfe-documento-entity`
- Commit: `feat: Implement NFeDocumento entity`

**Tarefa 3.2 — Implementar tokens SEFAZ**
- Crie `Domain/Tokens/NFeToken.cs`, `NFeTokenFilial.cs`
- Garanta que o token suporte configuração por filial
- Branch: `feat/nfe-token-entity`
- Commit: `feat: Implement NFe SEFAZ token entities`

**Tarefa 3.3 — Configurar NFe no DbContext**
- Atualize `NFeDbContext` com os DbSet de documento e token
- Branch: `feat/nfe-dbcontext`
- Commit: `feat: Configure NFe entities in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Geração de XML por Seção

**Tarefa 4.1 — Implementar gerador de Ide/Emit/Dest**
- Crie classes de geração para as seções `Ide`, `Emit`, `Dest`
- Consolide o XML em um serviço de alto nível que orquestra cada seção
- Branch: `feat/nfe-xml-sections-1`
- Commit: `feat: Implement NFe XML generator sections Ide/Emit/Dest`

**Tarefa 4.2 — Implementar gerador de Det/Total/Transporte**
- Crie classes de geração para `Det`, `Total`, `Transporte`
- Valide cada seção isoladamente antes de montar o documento completo
- Branch: `feat/nfe-xml-sections-2`
- Commit: `feat: Implement NFe XML generator sections Det/Total/Transporte`

**Tarefa 4.3 — Implementar gerador de Pag/InfAdic**
- Crie classes de geração para `Pag`, `InfAdic` e demais campos finais
- Branch: `feat/nfe-xml-sections-3`
- Commit: `feat: Implement NFe XML generator sections Pag/InfAdic`

**Tarefa 4.4 — Testar geração de XML seccionada**
- Adicione testes unitários que validem cada seção separadamente
- Branch: `test/nfe-xml-sections`
- Commit: `test: Add NFe XML section tests`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Comunicação com SEFAZ

**Tarefa 5.1 — Implementar envio de NF-e**
- Crie serviço de comunicação com SEFAZ usando Web Service SOAP ou SDK especializado
- Garanta suporte a ambiente homologação e produção
- Branch: `feat/nfe-sefaz-communication`
- Commit: `feat: Implement NFe SEFAZ communication`

**Tarefa 5.2 — Implementar cancelamento e eventos**
- Crie `Domain/Eventos/NFeEventoBase.cs`, `NFeInutilizacao.cs`, `NFeSequencialEvento.cs`
- Implemente fluxos de cancelamento, CCe e inutilização
- Branch: `feat/nfe-events`
- Commit: `feat: Implement NFe event entities and flows`

**Tarefa 5.3 — Testar comunicação e eventos**
- Adicione testes de integração com ambiente SEFAZ de homologação
- Branch: `test/nfe-sefaz`
- Commit: `test: Add NFe SEFAZ communication tests`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Certificado Digital e Autenticação

**Tarefa 6.1 — Integrar Certificado Digital**
- Garanta que o módulo utilize o certificado A1/A3 via `CertificadoDigital` do MOD-02
- Crie verificações de validade e assinatura do XML
- Branch: `feat/nfe-certificate`
- Commit: `feat: Integrate digital certificate support for NFe`

**Tarefa 6.2 — Testar assinatura de XML**
- Adicione testes que validem a assinatura digital e a emissão do XML assinado
- Branch: `test/nfe-signature`
- Commit: `test: Add NFe XML signature tests`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Repositórios e Migrations

**Tarefa 7.1 — Criar repositórios chave**
- Crie `Infrastructure/Repositorios/INFeDocumentoRepository.cs`, `INFeTokenRepository.cs`, `INFeEventoRepository.cs`
- Branch: `feat/nfe-repositories`
- Commit: `feat: Implement NFe repository interfaces`

**Tarefa 7.2 — Criar migrations EF Core**
- Crie a primeira migration para `NFeDbContext`
- Valide todos os relacionamentos e tabelas
- Branch: `setup/nfe-migrations`
- Commit: `setup: Create initial EF Core migrations for NFe`

**Tarefa 7.3 — Registrar DI**
- Crie método de extensão para registrar DbContext, serviços e repositórios
- Branch: `setup/nfe-di`
- Commit: `setup: Configure dependency injection for NFe`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Testes de Paridade e Homologação

**Tarefa 8.1 — Testar paridade XML**
- Crie testes que comparem o XML gerado com exemplos do legado
- Branch: `test/nfe-parity`
- Commit: `test: Add NFe XML parity tests`

**Tarefa 8.2 — Testar ciclo completo de emissão**
- Adicione testes de ponta a ponta para emissão, cancelamento e CCe
- Branch: `test/nfe-e2e`
- Commit: `test: Add NFe end-to-end tests`
- Marque no checklist: ✅ Fase 8 completa
