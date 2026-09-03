#!/usr/bin/env node
"use strict";

/**
 * PreToolUse hook (Bash | PowerShell) — Versatus.Net8
 *
 * Aplica em nível técnico as regras de Git do projeto:
 *   - AGENTS.md, Lei 5 ("Estratégia Git e Proteção de Branches")
 *   - specs/03-REGRAS-ANTI-ALUCINACAO.md, REGRA 15
 *   - specs/04-CONTRATO-DA-IA.md, Lei C
 *
 * - Bloqueia (deny) `git push` para/nas branches develop ou main.
 * - Bloqueia (deny) `git merge` para/nas branches develop ou main.
 * - Pede confirmação (ask) para qualquer `git rebase`, já que a regra exige
 *   permissão explícita do usuário a cada uso, não uma proibição permanente.
 *
 * Lê o payload do hook (JSON) via stdin e escreve a decisão em stdout.
 * Ausência de saída = permitir (comportamento padrão do PreToolUse).
 */

const { execSync } = require("child_process");

function readStdin() {
  try {
    return require("fs").readFileSync(0, "utf8");
  } catch {
    return "";
  }
}

function currentBranch() {
  try {
    return execSync("git rev-parse --abbrev-ref HEAD", {
      stdio: ["ignore", "pipe", "ignore"],
    })
      .toString()
      .trim();
  } catch {
    return "";
  }
}

function decide(permissionDecision, permissionDecisionReason) {
  process.stdout.write(
    JSON.stringify({
      hookSpecificOutput: {
        hookEventName: "PreToolUse",
        permissionDecision,
        permissionDecisionReason,
      },
    })
  );
  process.exit(0);
}

const raw = readStdin();
let payload;
try {
  payload = JSON.parse(raw || "{}");
} catch {
  process.exit(0); // payload ilegível: não bloqueia, apenas não age
}

const cmd = (payload.tool_input && payload.tool_input.command) || "";
if (!cmd) process.exit(0);

const lc = cmd.toLowerCase();
// Fronteira de "palavra" tolerante a origin/main, HEAD:develop, etc.
const PROTECTED_BRANCH = /(^|[\s:/])(develop|main)(?=[\s]|$)/;
// "git <subcomando>" só conta como invocação real quando "git" vem logo após um
// separador de comando (início da string, ;, &&, ||, |, quebra de linha) — nunca em
// qualquer lugar do texto. Isso evita falso-positivo quando o próprio comando Bash
// contém, por exemplo, uma mensagem de commit em prosa que menciona "git push" ou
// "develop"/"main" entre aspas (o hook vê o texto bruto do comando inteiro, aspas
// inclusas). Também exige que "merge"/"push"/"rebase" seja o subcomando de verdade,
// não prefixo de outro (git merge-base, git merge-tree, git push... sem sufixo colado).
const CMD_START = "(^|;|&&|\\|\\||\\||\\n)\\s*";
const IS_PUSH = new RegExp(CMD_START + "git\\s+push(?![\\w-])").test(lc);
const IS_MERGE = new RegExp(CMD_START + "git\\s+merge(?![\\w-])").test(lc);
const IS_REBASE = new RegExp(CMD_START + "git\\s+rebase(?![\\w-])").test(lc);
// Flags de recuperação de conflito: nunca iniciam merge/rebase novo, sempre inofensivas.
const IS_RECOVERY_FLAG = /--(abort|continue|quit|skip)\b/.test(lc);

if (IS_PUSH) {
  if (PROTECTED_BRANCH.test(lc)) {
    decide(
      "deny",
      "Bloqueado pela regra do projeto (AGENTS.md Lei 5 / CONTRATO-DA-IA Lei C): " +
        "a IA nunca deve dar `git push` em develop ou main. Peça ao usuário para " +
        "fazer o push manualmente."
    );
  }
  const branch = currentBranch();
  if (branch === "develop" || branch === "main") {
    decide(
      "deny",
      `Bloqueado: \`git push\` desabilitado enquanto a branch atual é '${branch}' ` +
        "(AGENTS.md Lei 5)."
    );
  }
}

if (IS_MERGE && !IS_RECOVERY_FLAG) {
  if (PROTECTED_BRANCH.test(lc)) {
    decide(
      "deny",
      "Bloqueado: merge para develop ou main é exclusivo do usuário (AGENTS.md Lei 5). " +
        "Deixe a branch de recurso aberta e peça aprovação do merge."
    );
  }
  const branch = currentBranch();
  if (branch === "develop" || branch === "main") {
    decide(
      "deny",
      `Bloqueado: \`git merge\` desabilitado enquanto a branch atual é '${branch}' ` +
        "(AGENTS.md Lei 5)."
    );
  }
}

if (IS_REBASE && !IS_RECOVERY_FLAG) {
  decide(
    "ask",
    "Regra do projeto (AGENTS.md Lei 5 / REGRA 15): `git rebase` exige permissão " +
      "explícita do usuário para esta chamada específica."
  );
}

process.exit(0);
