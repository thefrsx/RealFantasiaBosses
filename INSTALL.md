# Instalação — Real Fantasia Bosses (plug and play)

Este módulo pode ser instalado em **qualquer ModernUO** de duas formas. Escolha **uma**.

> Importante: pare o servidor antes de copiar arquivos e suba de novo depois.

---

## Método A — DLL pronta (sem compilar)

O jeito mais rápido. Você só copia a DLL já compilada.

1. Pare o servidor ModernUO.
2. Copie o arquivo:

   `RealFantasiaBosses.dll`

   gerado em:

   `C:\ModernUO\Projects\RealFantasiaBosses\bin\Release\net10.0\win-x64\RealFantasiaBosses.dll`

   para a pasta de assemblies da sua distribuição:

   `...\Distribution\Assemblies\RealFantasiaBosses.dll`

3. Inicie o servidor. Os comandos (`[fxtest`, `[bossload`, `[GMTool`, `[Toolbar`, etc.) e os
   itens/mobs (`[add MonsterBoxItem`, `[add TestBossMob`, ...) já estarão disponíveis.

**Atenção:** a DLL é compilada contra uma versão específica das DLLs do ModernUO
(`Server.dll` / `UOContent.dll`). Se a sua versão do ModernUO for diferente da que gerou esta
DLL, o servidor pode recusar carregar o assembly ou dar erro em runtime. Nesse caso, use o
**Método B**.

---

## Método B — Pasta-fonte (recompilar) — mais portável

Recompila o módulo contra a **sua** versão do ModernUO. É o método recomendado para portar
entre instalações/versões diferentes.

1. Pare o servidor ModernUO.
2. Copie a pasta inteira `RealFantasiaBosses` para dentro de `Projects\` da sua instalação:

   `...\ModernUO\Projects\RealFantasiaBosses\`

   (A pasta já contém o `RealFantasiaBosses.csproj`, que referencia `..\Server\Server.csproj`
   e `..\UOContent\UOContent.csproj` por caminho relativo — por isso basta estar dentro de
   `Projects\`.)

3. Compile, de uma das formas:

   - **Via dotnet (na pasta do projeto):**
     ```
     dotnet build RealFantasiaBosses.csproj -c Release
     ```
   - **Ou via BuildTool do ModernUO** (compila todos os projetos), na raiz do ModernUO:
     ```
     publish.cmd        (Windows)
     ./publish.sh       (Linux/macOS)
     ```
     ou rode o BuildTool/`dotnet build` da solução conforme seu fluxo habitual.

4. A DLL sai em `bin\Release\net10.0\win-x64\RealFantasiaBosses.dll` e o processo de publish do
   ModernUO a coloca em `Distribution\Assemblies\`. Inicie o servidor.

---

## Qual escolher?

| Situação | Método |
|---|---|
| Mesma versão do ModernUO que gerou a DLL | A (DLL pronta) |
| Versão diferente / não tem certeza / quer portabilidade máxima | B (pasta-fonte) |

A regra de ouro: **a DLL precisa casar com a versão do ModernUO**. Quando em dúvida,
recompile (Método B).

---

## Notas
- `TargetFramework`: `net10.0` (precisa do SDK .NET 10 para o Método B).
- O módulo registra comandos e tipos automaticamente ao carregar; não há configuração extra.
- `OwnerFix` mantém a conta de nome `make` como Owner (conveniência de dev). Se não quiser
  esse comportamento, edite ou remova `Command\OwnerFix.cs` antes de compilar (Método B), ou
  simplesmente não use a conta `make`.
