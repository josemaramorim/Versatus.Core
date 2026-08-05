# Estudo Estratégico & Arquitetural: Inovação com IA, UX e Governança no Versatus ERP

Este documento consolida as análises de arquitetura, experiência do usuário (UX), governança gerencial e aplicações práticas de Inteligência Artificial para o **Versatus ERP (.NET 10 + React)**, focando nos módulos **Financeiro**, **Global (Cadastros Base)** e **Estoque/Produtos**.

---

## 📌 Índice Geral

1. [Visão Geral & Pilares de IA no ERP](#1-visão-geral--pilares-de-ia-no-erp)
2. [Módulo Financeiro: IA, Liquidação & Rateio Inteligente](#2-módulo-financeiro-ia-liquidação--rateio-inteligente)
3. [Módulo Global: Governança, Entidades & Busca Semântica](#3-módulo-global-governança-entidades--busca-semântica)
4. [Módulo Estoque: Cadastro Automático por Código de Barras (EAN/GTIN)](#4-módulo-estoque-cadastro-automático-por-código-de-barras-eangtin)
5. [Arquitetura Técnica de Integração (.NET 10 + React + CQRS)](#5-arquitetura-técnica-de-integração-net-10--react--cqrs)

---

## 1. Visão Geral & Pilares de IA no ERP

No contexto do Versatus ERP, o uso de Inteligência Artificial deve ser **pragmático, orientado ao ROI e focado na redução de trabalho braçal e no suporte à tomada de decisão estratégica**.

```
  ┌─────────────────────────────────────────────────────────────────────────────────┐
  │ 1. IA Generativa & Copiloto (LLM / Semantic Kernel)                             │
  │    Assistente visual, preenchimento inteligente e resumos executivos em texto   │
  └─────────────────────────────────────────────────────────────────────────────────┘
                                           │
  ┌─────────────────────────────────────────────────────────────────────────────────┐
  │ 2. IA Preditiva & Machine Learning (Time-Series / ML.NET)                       │
  │    Previsão de fluxo de caixa real, score de inadimplência e recomendação de crédito│
  └─────────────────────────────────────────────────────────────────────────────────┘
                                           │
  ┌─────────────────────────────────────────────────────────────────────────────────┐
  │ 3. Visão Computacional & OCR (Document Intelligence)                            │
  │    Leitura e reconciliação automática de comprovantes PIX, boletos e extratos   │
  └─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Módulo Financeiro: IA, Liquidação & Rateio Inteligente

### 2.1 Reconciliação Bancária & OCR de Comprovantes
- **Desafio:** Digitação manual e cruzamento demorado de extratos (OFX/PDF) e comprovantes de PIX/TED.
- **Solução com IA:**
  - **OCR de Comprovantes:** Extração automática de valor, data, pagador/beneficiário e código de autenticação via OCR.
  - **Matching Semântico (Fuzzy Matching):** Reconciliação inteligente onde variações de razão social (ex.: *"João da Silva ME"* vs *"Silva Serviços"*) são associadas com cálculo de confiança (% match) e baixa em 1 clique.

### 2.2 Fluxo de Caixa Preditivo Ajustado por Comportamento
- **Desafio:** Fluxos de caixa tradicionais apenas somam títulos pela data teórica de vencimento.
- **Solução com IA:**
  - Algoritmo de séries temporais (ML.NET) que projeta o **Fluxo de Caixa Real Ajustado**, considerando histórico de atraso médio por cliente e sazonalidade.
  - **Simulação de Cenários "What-If":** Permite perguntas como: *"Se anteciparmos R$ 50 mil em títulos com 2% de desconto, qual será nosso saldo em 15 dias?"*.

### 2.3 UX de Liquidação & Rateio por Centro de Custo / Projeto
- **Presets de Rateio:** Salvar modelos padrão (ex.: *"Rateio Matriz 40/30/30"*) para aplicação instantânea.
- **Smart Defaults por Fornecedor:** Ao selecionar um fornecedor (ex.: distribuidora de energia), o sistema sugere automaticamente o centro de custo *"Instalações e Utilidades"*.
- **Ajuste Automático de Arredondamento:** Correção automática de dízimas na divisão de valores (ex.: ajustar o R$ 0,01 residual na maior parcela).
- **Liquidação Parcial Proporcional:** Ao pagar um valor parcial de um título rateado, oferece botões de cálculo proporcional ou por prioridade de centro de custo.
- **Alertas de Orçado vs. Realizado:** Notificação visual em tempo real para o gestor durante a aprovação/liquidação se o valor ultrapassar o orçamento aprovado do Projeto.

---

## 3. Módulo Global: Governança, Entidades & Busca Semântica

### 3.1 Cadastro Inteligente & Auto-Preenchimento (`FEntidade`)
- **Auto-Complete por CNPJ:** Ao digitar o CNPJ, consulta APIs públicas/oficiais e preenche automaticamente Razão Social, Nome Fantasia, CNAE, Inscrição Estadual e Endereço.
- **Deduplicação por Similaridade Semântica:** Alerta preventivo ao tentar cadastrar entidades com nomes ou documentos parecidos (ex.: *"Entidade similar encontrada: ID 302 - Confiança 96%"*).

### 3.2 Recomendador de Limite de Crédito
- Cruzamento de dados de porte empresarial, capital social e histórico de compras de entidades similares para sugerir um **Limite de Crédito Inicial Seguro** e **Condição de Pagamento Recomendada**.

### 3.3 Busca Semântica Universal no ERP (Global Smart Search)
- Permite ao usuário buscar informações digitando em linguagem natural (ex.: *"Quais fornecedores de embalagens de SP possuem pagamento em 30 dias?"*) e abrindo as telas do ERP pré-filtradas.

---

## 4. Módulo Estoque: Cadastro Automático por Código de Barras (EAN/GTIN)

### 4.1 Consulta Integrada por EAN-13 / GTIN-13
Ao bipar ou digitar um código de barras no cadastro de produtos (`FProduto`):

```
   ┌────────────────────────────────────────────────────────┐
   │ 1. O operador bipa o código de barras (EAN-13)         │
   └────────────────────────────────────────────────────────┐
                               │
                               ▼ (< 1 segundo)
   ┌────────────────────────────────────────────────────────┐
   │ 2. Consulta APIs (Cosmos / GS1 / CCG SEFAZ) + IA       │
   └────────────────────────────────────────────────────────┐
                               │
                               ▼
   ┌────────────────────────────────────────────────────────┐
   │ 3. Preenchimento Automático:                           │
   │    • Descrição Comercial Padronizada                   │
   │    • NCM e CEST Tributários                            │
   │    • Marca e Fabricante                                │
   │    • Unidade de Medida (UN, CX, KG)                    │
   │    • Imagem / Foto Oficial                             │
   └────────────────────────────────────────────────────────┘
```

### 4.2 Enriquecimento com IA
- **Padronização de Nomes:** Converte descrições poluídas de nota de entrada (ex.: `L COND MOCA LT 395G`) no padrão limpo do ERP (*Leite Condensado Moça Lata 395g*).
- **Sugestão de Tributação:** Sugere a regra fiscal de saída (CFOP, alíquotas de ICMS/PIS/COFINS) com base no NCM retornado.

---

## 5. Arquitetura Técnica de Integração (.NET 10 + React + CQRS)

Para manter o cumprimento das diretrizes de arquitetura do repositório (`AGENTS.md`):

```
                            ┌────────────────────────────────────────┐
                            │   Frontend React (MUI Copilot UI)      │
                            └────────────────────────────────────────┘
                                                │
                                                ▼ HTTP REST / SignalR
                            ┌────────────────────────────────────────┐
                            │      WebAPI (.NET 10 Controller)       │
                            └────────────────────────────────────────┘
                                                │
                                                ▼ Interface (DI)
                            ┌────────────────────────────────────────┐
                            │   Versatus.Framework.AI (Infra)        │
                            │   - OpenAiService / DocumentAI         │
                            │   - SemanticKernel / ML.NET Predictor  │
                            └────────────────────────────────────────┘
                                     │                      │
                   ┌─────────────────┘                      └─────────────────┐
                   ▼                                                          ▼
     ┌───────────────────────────┐                              ┌───────────────────────────┐
     │   Provedor LLM / OCR      │                              │  Banco de Dados Leitura   │
     │ (Azure OpenAI / DeepSeek) │                              │    CQRS ReadConnection    │
     └───────────────────────────┘                              └───────────────────────────┘
```

### Princípios Invioláveis de Implementação:
1. **Desacoplamento por Interfaces:** Toda a lógica de IA reside em projetos de infraestrutura de serviços (`Versatus.Framework.AI`), injetados via DI (`IAiDocumentProcessor`, `IAiCreditScorer`).
2. **CQRS Leve na Leitura:** Consultas analíticas para a IA utilizam sempre a `ReadConnection` (sem tracking) para preservar o desempenho das operações de escrita.
3. **Validação Humana (Human-in-the-Loop):** A IA gera sugestões de ação na interface gráfica do React. A gravação final no banco de dados depende sempre da aprovação explícita do usuário.
