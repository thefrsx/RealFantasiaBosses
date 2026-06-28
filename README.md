# Real Fantasia Bosses

Módulo "plug and play" para **ModernUO** que reúne todas as mecânicas custom do shard
**Real Fantasia**: bosses com fases e habilidades aprendíveis, efeitos visuais (FX) de área,
combate por turnos, designer de criaturas in-game (Monster In A Box), ferramentas de GM e
algumas conveniências de desenvolvimento.

O módulo é um projeto C# independente (`RealFantasiaBosses.csproj`) que compila para a DLL
`RealFantasiaBosses.dll`. Como o ModernUO carrega DLLs (não há pasta `Scripts` em runtime
como no ServUO), o módulo é distribuído como **DLL pronta** ou como **pasta-fonte** para
recompilar. Veja `INSTALL.md`.

- **TargetFramework:** `net10.0`
- **Referências de projeto:** `..\Server\Server.csproj`, `..\UOContent\UOContent.csproj`
- **Pacotes:** `ModernUO.Serialization.Annotations` 2.14.2, `ModernUO.Serialization.Generator` 2.14.3
- O `.csproj` não contém nenhum caminho absoluto; é idêntico ao padrão dos outros projetos
  do repositório (ex.: `InteractiveBoss`), portanto pode ser jogado em qualquer `Projects\`.

---

## Sistemas inclusos

### 1. Bosses (`Boss/`)
Framework de bosses com habilidades catalogadas, controlador de encontro, lacaios e gump de
aviso (telegraph) para mecânicas aprendíveis estilo WoW/Outlands.

- `BossController` — orquestra o encontro do boss.
- `RFBoss` / `RFBossTest` — classe base de boss (`BaseCreature`) e boss de teste.
- `RFBossAbility` — definição de uma habilidade de boss.
- `BossCatalog` — catálogo com **33 mecânicas** (Sopro Flamejante, Muralha de Fogo, Nova
  Ardente, Chuva de Meteoros, Ninhada de Aranhas, Firebolt, Investida, Medo, Geiser, Bomba
  Ambulante, Chuva Toxica, Esporos Toxicos, Estalagmites, Agarrar, Arremessar Rocha,
  Explosivo, Barril Explosivo, Rajada de Projeteis, Tempestade de Raios, Explosao Flamejante,
  Tempestade Furiosa, Nevasca, Choque, Teia, Prisao de Gelo, Emboscada, Invocar Lacaios,
  Curar Aliados, Golpe Elemental, Golpe em Area, etc.).
- `ImportedAbilities`..`ImportedAbilities6` — implementações das habilidades importadas.
- `BossAbilities` — habilidades adicionais.
- `BossMinion` — lacaio invocável (`BaseCreature`).
- Gumps: `BossWarningGump`, `BossLoaderGump`, `BossLoadTestGump`.
- Mob de teste: `TestBossMob` (`[add TestBossMob`).

### 2. Efeitos / FX (`Effect/`)
Motor de efeitos de área (telegraphs no chão) reutilizável pelas mecânicas dos bosses.

- `AreaEffectEngine`, `AreaShapes`, `TileFx` — engine e formas (linha, cone, nova, etc.).
- `FireConeEffect`, `FireLineEffect`, `FireNovaEffect`, `MeteorEffect`, `SpiderEggEffect`.
- `SpiderEgg` (`Mobile/`) — ovo de aranha que eclode.

### 3. Combate por turnos (`Combat/`, `Gump/`, `Mobile/`)
Protótipo de combate 1v1 por turnos ao vivo.

- `TurnEncounter` — máquina de estado do encontro por turnos.
- `TurnHudGump`, `TurnActionPickerGump` — HUD e seletor de ação.
- `TurnBasedTestMob` — mob de teste (`[add TurnBasedTestMob`).

### 4. Monster In A Box (`MonsterBox/`)
Designer de criatura in-game via gump.

- `MonsterBoxItem` (`[add MonsterBoxItem`) — item que abre o designer.
- `MonsterBoxMobile` — a criatura gerada.
- `MonsterBoxGump` — interface de edição (nome, hue, body, AI, stats, dano, resistências).

### 5. GM Tools
Duas toolbars de comando de GM, além de utilitários.

- `GMTool/` — toolbar **Ice** (`[GMTool`), com `GMHuePicker` e `GMTravelGump`.
- `JoekuToolbar/` — toolbar **Joeku** (`[Toolbar`), customizável, com `Recover`/`Rec`,
  `AddStairs`, `GMbody`, e itens de staff (`StaffOrb`, `StaffRing`, `GMEthereal`,
  `GMPlateMail`, `GMSash`).

### 6. Utilitários / Comandos de teste
- `craftskin` — abre `CraftSkinGump`.
- `animtest` — testador de animações.
- `fxtest` — menu de teste dos efeitos (`FxTestGump`).
- `OwnerFix` — **conveniência de dev** (sem comando): timer que roda a cada 2s e garante que
  a conta de nome `make` permaneça em `AccessLevel.Owner`. O nome da conta está fixo no
  código (`OwnerFix.cs`); se for usar em outro shard, edite/remova essa classe.

---

## Comandos por categoria

> Os comandos abaixo são registrados via `CommandSystem.Register` / `CommandHandlers.Register`.
> Itens e mobs marcados com `[add ...` são criados pelo comando padrão `[add` do ModernUO.

### Bosses
| Comando | Acesso | Descrição |
|---|---|---|
| `[bossload` | GameMaster | Abre o gump de carregamento de boss (`BossLoaderGump`). |
| `[bossloadtest` | GameMaster | Abre o gump de teste de carregamento (`BossLoadTestGump`). |
| `[add TestBossMob` | (add) | Cria o mob de teste de boss. |
| `[add BossController` / `[add BossMinion` / `[add RFBoss...` | (add) | Cria componentes do encontro. |

### Efeitos / FX
| Comando | Acesso | Uso |
|---|---|---|
| `[firecone` | GameMaster | `firecone <length> [hurt]` |
| `[fireline` | GameMaster | `fireline <tiles> <directions> [hurt]` |
| `[firenova` | GameMaster | `firenova <radius> [hurt]` |
| `[meteor` | GameMaster | `meteor <count> [hurt]` |
| `[spidereggs` | GameMaster | `spidereggs <count> [hatchSeconds]` |
| `[fxtest` | GameMaster | Abre o menu de teste de efeitos. |

### Combate por turnos
| Comando | Acesso | Descrição |
|---|---|---|
| `[add TurnBasedTestMob` | (add) | Cria o mob de combate por turnos. |

### Monster In A Box
| Comando | Acesso | Descrição |
|---|---|---|
| `[add MonsterBoxItem` | (add) | Cria a caixa designer de criatura. |

### GM Tools
| Comando | Acesso | Descrição |
|---|---|---|
| `[GMTool` | Counselor | Toolbar de GM (Ice). |
| `[Toolbar` | Counselor | Toolbar de GM (Joeku), customizável. |
| `[GMbody` | Counselor | Troca de body/forma de staff. |
| `[Recover` / `[Rec` | GameMaster | Recupera itens. |
| `[AddStairs` | GameMaster | Construtor de escadas (addon). |

### Utilitários / teste
| Comando | Acesso | Descrição |
|---|---|---|
| `[craftskin` | GameMaster | Abre o `CraftSkinGump`. |
| `[animtest` | GameMaster | `animtest [start] [end] [intervalSeconds]` |

---

## Observações importantes
- A DLL precisa **casar com a versão do ModernUO** (mesma API de `Server`/`UOContent`). Se as
  versões divergirem, use o **Método B** (pasta-fonte) e recompile. Veja `INSTALL.md`.
- `OwnerFix.cs` é específico do ambiente do autor (conta `make`). Ajuste antes de distribuir
  para terceiros.
