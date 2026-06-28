# Real Fantasia — ModernUO Content Module

> A gameplay content module for **[ModernUO](https://github.com/modernuo/ModernUO)**, a modern C#/.NET
> server emulator for *Ultima Online*. It adds a data-driven **boss mechanics framework**, a
> **turn-based combat prototype**, an **in-game creature designer**, and a suite of in-game tools —
> all loaded as a single hot-swappable `.dll`, with **zero changes to the engine**.

<p>
  <img alt=".NET 10" src="https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white">
  <img alt="C#" src="https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white">
  <img alt="ModernUO" src="https://img.shields.io/badge/ModernUO-module-8A2BE2">
  <img alt="Status" src="https://img.shields.io/badge/status-active%20WIP-orange">
</p>

---

## Why this project

I build and run a private *Ultima Online* shard ("Real Fantasia") on the ModernUO server. Rather than
forking the engine, I designed everything in this repo as a **self-contained module**: a separate C#
project that compiles to one DLL and drops into the server's `Projects/` (or `Assemblies/`) folder.
The server discovers the new commands, items, and creatures automatically at startup.

The interesting part for an engineer isn't the game — it's the constraints:

- **No engine edits.** Every feature is built on the public `Server` / `UOContent` API. The turn-based
  combat system, for example, runs entirely on existing primitives (`Mobile.Frozen`, combat timers,
  vetoable movement events) instead of patching the real-time combat loop.
- **Server-authoritative, thin client.** The client only knows how to *draw*. All game logic, state,
  and timing live on the server, which streams UI ("gump") definitions and effect packets to the client.
- **Soft-realtime.** Boss telegraphs, damage-over-time pools, and phase transitions are driven by
  300 ms timer ticks across many concurrent encounters — so the code has to be cheap per tick and
  resilient to entities dying or disconnecting mid-effect.

## Highlights (what to look at)

| Area | What it demonstrates | Entry points |
|---|---|---|
| **Boss mechanics framework** | Data-driven design, the strategy pattern, an attachable controller over a 300 ms tick loop, telegraph/phase state | `Boss/BossController.cs`, `Boss/RFBossAbility.cs`, `Boss/BossCatalog.cs` |
| **Turn-based combat** | A finite state machine layered on a real-time engine using only public primitives | `Combat/TurnEncounter.cs` |
| **Area-effect (FX) engine** | Reusable geometry (line / cone / nova / ring), decoupled from the abilities that use it | `Effect/AreaEffectEngine.cs`, `Effect/AreaShapes.cs` |
| **In-game tooling** | Building interactive UIs and a CRUD-style editor at runtime | `MonsterBox/`, `Gump/BossLoaderGump.cs` |

### Boss mechanics framework
A `BossController` can be **attached to any creature at runtime** through an in-game admin UI
(`[bossload`) — no recompile, no restart. It runs a tick loop that:

- gates abilities behind **HP-percentage phases** (a mechanic might unlock only below 50% HP),
- **telegraphs** the next attack (an on-ground pulsing shape, colored by element, plus a warning
  banner and the boss glowing/shouting) for a configurable window before it fires,
- only acts **while genuinely in combat** (a valid `Combatant` or someone on the `Aggressors` list —
  it won't nuke a player just for walking past), and
- captures its target at the start of the telegraph so the effect survives the AI dropping aggro.

Each ability is a self-describing object (`RFBossAbility`: cooldown, damage, telegraph text/seconds,
unlock threshold) with a virtual `PaintTelegraph` hook, so a new mechanic is mostly *data*.

### Turn-based combat prototype
`TurnEncounter` is a finite state machine (your turn / enemy turn) that delivers BG-style turn pacing
**without modifying the engine's real-time combat at all**. It does this by composing existing
primitives: `Mobile.Frozen` to lock casting/movement/swings, the engine's combat-timing stamps, and a
*vetoable* movement event to enforce a per-turn movement budget (which also kills "kiting"). Actions
(attack, potion, scroll, defend, flee) each resolve before the turn passes.

### Area-effect engine
The shapes (`AreaShapes`) and the renderer/damage pass (`AreaEffectEngine`, `TileFx`) are deliberately
separate from the abilities that consume them, so fire, ice, poison, and energy variants reuse the same
geometry and only differ in hue and payload.

## Tech stack

- **Language / runtime:** C# 12, .NET 10
- **Platform:** ModernUO server API (`Server.csproj`, `UOContent.csproj` project references)
- **Serialization:** `ModernUO.Serialization.Generator` (source generators emit save/load code from
  `[SerializableField]` attributes — no hand-written persistence)
- **Build:** `dotnet build` / the ModernUO `publish` toolchain

## Project structure

```
RealFantasiaBosses/
├─ Boss/         # Boss framework: controller, ability model, catalog, minions
├─ Combat/       # Turn-based combat state machine
├─ Effect/       # Reusable area-effect engine + shapes (FX)
├─ Gump/         # In-game UI: boss loader, FX test menu, HUDs, editors
├─ Command/      # [ console commands that register the features
├─ Mobile/       # Custom creatures (test bosses, hatching spider egg)
├─ MonsterBox/   # "Monster In A Box" — in-game creature designer
├─ GMTool/       # Game-master command toolbar (adapted, see Attribution)
└─ JoekuToolbar/ # Customizable GM toolbar (adapted, see Attribution)
```

~60 source files. The core authored systems (boss framework, turn combat, FX engine, creature
designer, gumps) are the parts to review; the two GM toolbars are ports — see **Attribution** below.

## Getting started

> This is a **module**, not a standalone app. It references `..\Server` and `..\UOContent` by relative
> path, so it builds *inside* a ModernUO checkout. You need a working ModernUO server first.

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download) and clone
   [ModernUO](https://github.com/modernuo/ModernUO).
2. Drop this folder into the ModernUO tree:
   ```
   ModernUO/Projects/RealFantasiaBosses/
   ```
3. Build the module:
   ```bash
   dotnet build RealFantasiaBosses.csproj -c Release
   ```
   The output DLL is placed in `Distribution/Assemblies/` by the ModernUO publish step.
4. Start the server and try a feature in-game, e.g.:
   - `[bossload` — attach boss mechanics to any creature
   - `[add MonsterBoxItem` — open the in-game creature designer
   - `[fxtest` — preview the area-effect engine
   - `[add TurnBasedTestMob` — fight the turn-based prototype

Full install notes (including shipping a prebuilt DLL) are in [`INSTALL.md`](INSTALL.md).

## Demo

> 📸 *GIFs of the boss telegraphs, phase transitions, and turn-based HUD go here.* The visual feedback
> (on-ground telegraph shapes, lingering fire, phase-shift particle bursts) is a core part of the
> design and is best shown in motion.

## What I focused on

- **Designing for extension over modification** — new bosses and mechanics are added as data/catalog
  entries, not new control flow.
- **Working within a large existing codebase and its API** instead of forking it.
- **Event-driven and timer-driven systems** that stay correct when entities die, disconnect, or lose
  target mid-effect.
- **Building runtime UI and editor tooling** so designers can iterate in-game without a rebuild.

## Attribution

This is a personal/portfolio project on top of the open-source **ModernUO** server. In the spirit of
honest credit:

- **GM toolbars** (`GMTool/`, `JoekuToolbar/`) are ports/adaptations of well-known community scripts
  (Ice's GM Tool and Joeku's Toolbar) updated from ServUO to the ModernUO API. The framework is theirs;
  I did the port.
- A number of boss abilities (`Boss/ImportedAbilities*.cs`) **reimplement the logic** of mechanics from
  the community "Custom Abilities 3.0" pack on top of *my* `RFBossAbility` framework — the framework and
  integration are mine; the original mechanic ideas are credited to that pack.
- The boss framework, turn-based combat system, FX engine, creature designer, and the gump UIs are my
  own work.

## License

My original work in this repo (boss framework, turn-based combat, FX engine, creature designer, gumps,
commands) is released under the **MIT License**. The adapted community scripts (`GMTool/`,
`JoekuToolbar/`, `Boss/ImportedAbilities*.cs`) are **not** relicensed and retain their original authors'
terms, and ModernUO is licensed separately in its own repository. See [`LICENSE`](LICENSE) for the full
breakdown.
