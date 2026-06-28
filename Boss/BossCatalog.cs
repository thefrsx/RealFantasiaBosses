using System;
using System.Collections.Generic;

namespace Server.Custom.Bosses
{
    // Definição de uma entrada do catálogo. Além de criar a habilidade, descreve seus PARÂMETROS configuráveis
    // (forma: tiles/raio/qtd) e os defaults comuns (aviso/dano/cooldown), pra o gump carregador editar como o [fxtest.
    public sealed class BossAbilityDef
    {
        public string Key;
        public string Display;
        public string Category;
        public (string Label, int Default)[] Shape = Array.Empty<(string, int)>(); // 0-2 params de forma
        public int Telegraph = 5;       // tempo de disparo (s)
        public int DamageDefault = 20;  // dano fixo
        public int CooldownDefault = 10;
        public int UnlockDefault = 100; // % de vida em que desbloqueia (100 = desde o início)
        // Build(forma[], telegraph, dano, cooldown, vidaPct) -> habilidade configurada.
        public Func<int[], int, int, int, int, RFBossAbility> Build;
    }

    // Catálogo central. Adicionar mecânica/magia = adicionar uma entrada aqui.
    public static class BossCatalog
    {
        public static readonly List<BossAbilityDef> All = new()
        {
            new BossAbilityDef
            {
                Key = "cone", Display = "Sopro Flamejante", Category = "FX",
                Shape = new[] { ("Tiles", 6) }, Telegraph = 5, DamageDefault = 16, CooldownDefault = 8,
                Build = (s, tel, dmg, cd, unlock) => new FireConeAbility
                {
                    Name = "Sopro Flamejante", WarningText = "Saiam da frente!",
                    TelegraphSeconds = tel, Length = s[0], Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "line", Display = "Muralha de Fogo", Category = "FX",
                Shape = new[] { ("Tiles", 6), ("Dir", 4) }, Telegraph = 6, DamageDefault = 18, CooldownDefault = 10,
                UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new FireLineAbility
                {
                    Name = "Muralha de Fogo", WarningText = "Todos cairao!",
                    TelegraphSeconds = tel, Tiles = s[0], Dirs = s[1], Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "nova", Display = "Nova Ardente", Category = "FX",
                Shape = new[] { ("Raio", 6) }, Telegraph = 6, DamageDefault = 20, CooldownDefault = 12,
                UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new FireNovaAbility
                {
                    Name = "Nova Ardente", WarningText = "Afastem-se do centro!",
                    TelegraphSeconds = tel, Radius = s[0], Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "meteor", Display = "Chuva de Meteoros", Category = "FX",
                Shape = new[] { ("Qtd", 6) }, Telegraph = 8, DamageDefault = 26, CooldownDefault = 10,
                UnlockDefault = 25,
                Build = (s, tel, dmg, cd, unlock) => new MeteorAbility
                {
                    Name = "Chuva de Meteoros", WarningText = "Olhem para cima!",
                    TelegraphSeconds = tel, Count = s[0], Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "eggs", Display = "Ninhada de Aranhas", Category = "FX",
                Shape = new[] { ("Qtd", 4) }, Telegraph = 5, DamageDefault = 0, CooldownDefault = 16,
                UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new SpiderEggAbility
                {
                    Name = "Ninhada de Aranhas", WarningText = "Destruam os ovos!",
                    TelegraphSeconds = tel, Count = s[0], Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Garimpadas do Custom Abilities 3.0 (lote 1) ----
            new BossAbilityDef
            {
                Key = "firebolt", Display = "Firebolt", Category = "Mag",
                Telegraph = 2, DamageDefault = 14, CooldownDefault = 6, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new FireboltAbility
                {
                    Name = "Firebolt", WarningText = "Esquive do projetil!",
                    TelegraphSeconds = tel, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "charge", Display = "Investida", Category = "Mov",
                Telegraph = 3, DamageDefault = 18, CooldownDefault = 12, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new ChargeAbility
                {
                    Name = "Investida", WarningText = "Ele vai avancar!",
                    TelegraphSeconds = tel, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "fear", Display = "Medo", Category = "CC",
                Shape = new[] { ("Raio", 8) }, Telegraph = 3, DamageDefault = 0, CooldownDefault = 20, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new FearAbility
                {
                    Name = "Medo", WarningText = "Nao entre em panico!",
                    TelegraphSeconds = tel, Range = s.Length > 0 ? s[0] : 8, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Lote 2: telegrafias de chao ----
            new BossAbilityDef
            {
                Key = "geyser", Display = "Geiser", Category = "Chao",
                Telegraph = 3, DamageDefault = 22, CooldownDefault = 10, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new GeyserAbility
                {
                    Name = "Geiser", WarningText = "Saia do redemoinho!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "walkingbomb", Display = "Bomba Ambulante", Category = "Chao",
                Telegraph = 5, DamageDefault = 24, CooldownDefault = 20, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new WalkingBombAbility
                {
                    Name = "Bomba Ambulante", WarningText = "Afaste-se dos outros!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "toxicrain", Display = "Chuva Toxica", Category = "Chao",
                Telegraph = 2, DamageDefault = 18, CooldownDefault = 15, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new ToxicRainAbility
                {
                    Name = "Chuva Toxica", WarningText = "Saia da nuvem acida!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "toxicspores", Display = "Esporos Toxicos", Category = "Chao",
                Shape = new[] { ("Raio", 5) }, Telegraph = 3, DamageDefault = 26, CooldownDefault = 30, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new ToxicSporesAbility
                {
                    Name = "Esporos Toxicos", WarningText = "Saia do circulo!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 5, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "impale", Display = "Estalagmites", Category = "Chao",
                Shape = new[] { ("Raio", 5) }, Telegraph = 2, DamageDefault = 24, CooldownDefault = 25, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new ImpaleAoeAbility
                {
                    Name = "Estalagmites", WarningText = "Pule de lado!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 5, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Lote 3: puxar / arremesso ----
            new BossAbilityDef
            {
                Key = "grapple", Display = "Agarrar", Category = "CC",
                Telegraph = 2, DamageDefault = 8, CooldownDefault = 20, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new GrappleAbility
                {
                    Name = "Agarrar", WarningText = "Ele vai te puxar!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "boulder", Display = "Arremessar Rocha", Category = "Arrem",
                Telegraph = 2, DamageDefault = 16, CooldownDefault = 6, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new ThrowBoulderAbility
                {
                    Name = "Arremessar Rocha", WarningText = "Saia de baixo!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "explosives", Display = "Explosivo", Category = "Arrem",
                Telegraph = 2, DamageDefault = 20, CooldownDefault = 6, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new ThrowExplosivesAbility
                {
                    Name = "Explosivo", WarningText = "Explosivo chegando!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "timedbomb", Display = "Barril Explosivo", Category = "Arrem",
                Telegraph = 3, DamageDefault = 36, CooldownDefault = 16, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new ThrowTimedExplosivesAbility
                {
                    Name = "Barril Explosivo", WarningText = "Corra do barril!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "barrage", Display = "Rajada de Projeteis", Category = "Mag",
                Telegraph = 2, DamageDefault = 16, CooldownDefault = 15, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new BarrageOfBoltsAbility
                {
                    Name = "Rajada de Projeteis", WarningText = "Rajada incoming!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Lote 4: tempestades / AoE ----
            new BossAbilityDef
            {
                Key = "thunderstorm", Display = "Tempestade de Raios", Category = "Tempt",
                Shape = new[] { ("Raio", 8) }, Telegraph = 3, DamageDefault = 14, CooldownDefault = 8, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new ThunderstormAbility
                {
                    Name = "Tempestade de Raios", WarningText = "Relampagos!",
                    TelegraphSeconds = tel, Range = s.Length > 0 ? s[0] : 8, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "flameburst", Display = "Explosao Flamejante", Category = "AoE",
                Shape = new[] { ("Raio", 5) }, Telegraph = 3, DamageDefault = 30, CooldownDefault = 20, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new FlameBurstAoeAbility
                {
                    Name = "Explosao Flamejante", WarningText = "Afaste-se do boss!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 5, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "tempest", Display = "Tempestade Furiosa", Category = "Tempt",
                Shape = new[] { ("Raio", 6) }, Telegraph = 4, DamageDefault = 12, CooldownDefault = 40, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new RagingTempestAbility
                {
                    Name = "Tempestade Furiosa", WarningText = "A tempestade vai durar!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 6, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "blizzard", Display = "Nevasca", Category = "Tempt",
                Shape = new[] { ("Raio", 5) }, Telegraph = 3, DamageDefault = 16, CooldownDefault = 15, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new BlizzardAbility
                {
                    Name = "Nevasca", WarningText = "Saia da nevasca!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 5, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Lote 5: CC / invocacao / cura ----
            new BossAbilityDef
            {
                Key = "zap", Display = "Choque", Category = "CC",
                Telegraph = 2, DamageDefault = 12, CooldownDefault = 12, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new ZapAbility
                {
                    Name = "Choque", WarningText = "Choque eletrico!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "snare", Display = "Teia", Category = "CC",
                Telegraph = 2, DamageDefault = 12, CooldownDefault = 12, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new SnareAbility
                {
                    Name = "Teia", WarningText = "Vai te prender!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "iceprison", Display = "Prisao de Gelo", Category = "CC",
                Telegraph = 2, DamageDefault = 0, CooldownDefault = 30, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new IcePrisonAbility
                {
                    Name = "Prisao de Gelo", WarningText = "Congelado!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "ambush", Display = "Emboscada", Category = "Mov",
                Telegraph = 2, DamageDefault = 22, CooldownDefault = 30, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new AmbushAbility
                {
                    Name = "Emboscada", WarningText = "Ele sumiu!",
                    TelegraphSeconds = tel, Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "summon", Display = "Invocar Lacaios", Category = "Inv",
                Shape = new[] { ("Qtd", 2), ("Body", 50) }, Telegraph = 4, DamageDefault = 0, CooldownDefault = 30, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new SummonMinionAbility
                {
                    Name = "Invocar Lacaios", WarningText = "Reforcos chegando!",
                    TelegraphSeconds = tel, Count = s.Length > 0 ? s[0] : 2, MinionBody = s.Length > 1 ? s[1] : 50,
                    Damage = dmg, Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "healallies", Display = "Curar Aliados", Category = "Cura",
                Shape = new[] { ("Raio", 8) }, Telegraph = 3, DamageDefault = 30, CooldownDefault = 12, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new HealAlliesAbility
                {
                    Name = "Curar Aliados", WarningText = "Ele vai se curar!",
                    TelegraphSeconds = tel, Range = s.Length > 0 ? s[0] : 8, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },

            // ---- Lote 6: FlameStrike elemental (Elem: 1 Fogo,2 Gelo,3 Veneno,4 Energia,5 Agua,6 Vapor,7 Necro,8 Sagrado,9 Sangue) ----
            new BossAbilityDef
            {
                Key = "fs_targeted", Display = "Golpe Elemental", Category = "Elem",
                Shape = new[] { ("Elem", 1) }, Telegraph = 2, DamageDefault = 16, CooldownDefault = 12, UnlockDefault = 100,
                Build = (s, tel, dmg, cd, unlock) => new FlameStrikeTargetedAbility
                {
                    Name = "Golpe Elemental", WarningText = "Golpe chegando!",
                    TelegraphSeconds = tel, Elem = s.Length > 0 ? s[0] : 1, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "fs_aoe", Display = "Golpe em Area", Category = "Elem",
                Shape = new[] { ("Raio", 5), ("Elem", 1) }, Telegraph = 3, DamageDefault = 16, CooldownDefault = 12, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new FlameStrikeAoeAbility
                {
                    Name = "Golpe em Area", WarningText = "Afaste-se do boss!",
                    TelegraphSeconds = tel, Radius = s.Length > 0 ? s[0] : 5, Elem = s.Length > 1 ? s[1] : 1, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "fs_line", Display = "Golpe em Linha", Category = "Elem",
                Shape = new[] { ("Tiles", 5), ("Elem", 1) }, Telegraph = 3, DamageDefault = 18, CooldownDefault = 10, UnlockDefault = 75,
                Build = (s, tel, dmg, cd, unlock) => new FlameStrikeLineAbility
                {
                    Name = "Golpe em Linha", WarningText = "Saia da linha!",
                    TelegraphSeconds = tel, Length = s.Length > 0 ? s[0] : 5, Elem = s.Length > 1 ? s[1] : 1, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "fs_cone", Display = "Golpe em Cone", Category = "Elem",
                Shape = new[] { ("Tiles", 5), ("Elem", 1) }, Telegraph = 3, DamageDefault = 18, CooldownDefault = 12, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new FlameStrikeConeAbility
                {
                    Name = "Golpe em Cone", WarningText = "Saia da frente!",
                    TelegraphSeconds = tel, Length = s.Length > 0 ? s[0] : 5, Elem = s.Length > 1 ? s[1] : 1, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            },
            new BossAbilityDef
            {
                Key = "fs_forked", Display = "Golpe Triplice", Category = "Elem",
                Shape = new[] { ("Tiles", 6), ("Elem", 1) }, Telegraph = 4, DamageDefault = 18, CooldownDefault = 30, UnlockDefault = 50,
                Build = (s, tel, dmg, cd, unlock) => new ForkedFlameStrikeAbility
                {
                    Name = "Golpe Triplice", WarningText = "Tres jatos!",
                    TelegraphSeconds = tel, Length = s.Length > 0 ? s[0] : 6, Elem = s.Length > 1 ? s[1] : 1, Damage = dmg,
                    Cooldown = TimeSpan.FromSeconds(cd), UnlockAtPercent = unlock
                }
            }
        };
    }
}
