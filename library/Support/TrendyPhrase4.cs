﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public class TrendyPhrase4 : TrendyPhraseBase
    {
        public TrendyPhrase4(byte[] data) : base(data)
        {
        }

        public TrendyPhrase4(ushort mood, ushort index, ushort word1, ushort word2)
            : base(Pack(mood, index, word1, word2))
        {
        }

        public override string Render(string wordFormat)
        {
            return RenderPhrase(Data, wordFormat);
        }

        public static string RenderPhrase(byte[] data, string wordFormat)
        {
            if (data == null) throw new ArgumentNullException();
            if (data.Length != 8) throw new ArgumentException();

            ushort mood = BitConverter.ToUInt16(data, 0);
            ushort index = BitConverter.ToUInt16(data, 2);
            ushort word1 = BitConverter.ToUInt16(data, 4);
            ushort word2 = BitConverter.ToUInt16(data, 6);

            if (mood >= 5) return "";
            if (index >= 20) return "";
            return string.Format(PHRASES[mood, index], 
                string.Format(wordFormat, RenderWord(word1)),
                string.Format(wordFormat, RenderWord(word2))
                );
        }

        public static string RenderWord(ushort word)
        {
            if (word < 496) return WORDS_POKEMON[word];
            if (word < 964) return WORDS_MOVES[word - 496];
            if (word < 982) return WORDS_TYPES[word - 964];
            if (word < 1106) return WORDS_ABILITIES[word - 982];
            if (word < 1144) return WORDS_TRAINER[word - 1106];
            if (word < 1182) return WORDS_PEOPLE[word - 1144];
            if (word < 1289) return WORDS_GREETINGS[word - 1182];
            if (word < 1393) return WORDS_LIFESTYLE[word - 1289];
            if (word < 1440) return WORDS_FEELINGS[word - 1393];
            if (word < 1472) return WORDS_TOUGH[word - 1440];
            if (word < 1495) return WORDS_UNION[word - 1472];
            return "";
        }

        public TrendyPhrase4 Clone()
        {
            return new TrendyPhrase4(Data);
        }

        #region String tables
        // Special thanks to http://projectpokemon.org/rawdb/diamond/msg.php
        // for their string table dumps.

        // msg.narc/395 through /399
        private static string[,] PHRASES = new string[,]
        {
            {   // Mood 0: Start of battle                      
                "Please!\n{0}!",                                //  0
                "Go! {0}!",                                     //  1
                "I’ll battle with\n{0}!",                       //  2
                "It’s {0}!",                                    //  3
                "{0}, I’m going\nwith {1}!",                    //  4
                "Look at {0}!",                                 //  5
                "I’ll show you {0}!",                           //  6
                "Now!\n{0}!",                                   //  7
                "I’ll show you my\n{0} strategy!",              //  8
                "I’ll {0}!",                                    //  9
                "I’ll shock you with\n{0}!",                    // 10
                "This is the beginning\nof {0}!",               // 11
                "This battle is\n{0}!",                         // 12
                "I don’t think I’ll\never lose at {0}!",        // 13
                "Team {0} is here!",                            // 14
                "You think you can beat\n{0}?",                 // 15
                "{0}!\n{1} power!",                             // 16
                "This is the {0}\nPokémon!",                    // 17
                "{0} won’t lose!",                              // 18
                "Please {0}!\n{1}!"                             // 19
            },
            {   // Mood 1: Victory
                "I win!\n{0}!",                                 //  0
                "I won!\nI won with {0}!",                      //  1
                "{0} is strong,\nisn’t it?",                    //  2
                "It’s {0}\n{1} after all!",                     //  3
                "{0}, yay!",                                    //  4
                "Yay, {0}!\n{1}!",                              //  5
                "Sorry, it’s {0}\n{1}.",                        //  6
                "{0}!\nThank you!",                             //  7
                "The way I feel now is\n{0}!",                  //  8
                "I wanted people to look at\nmy {0}!",          //  9
                "It’s all thanks to\n{0}.",                     // 10
                "I might have won with\n{0}!",                  // 11
                "I get the happiest with\n{0}!",                // 12
                "{0} secured\nthe victory!",                    // 13
                "This {0}\nwas really good!",                   // 14
                "{0}\nwas fun, wasn’t it?",                     // 15
                "Huh?\n{0}?!",                                  // 16
                "{0} is the toughest!",                         // 17
                "Happy!\n{0} happy!",                           // 18
                "How’s that?!\n{0}!"                            // 19
            },
            {   // Mood 2: Defeat
                "You win...\n{0}",                              //  0
                "{0} was the one\nthing I wanted to avoid...",  //  1
                "Waaah!\n{0}!",                                 //  2
                "I want to go home with\n{0}...",               //  3
                "{0}!\n{1}!",                                   //  4
                "Could it be...?\n{0}...?",                     //  5
                "{0}!\nHow awful!",                             //  6
                "I was confident about\n{0}, too.",             //  7
                "You're {0},\naren’t you?",                     //  8
                "{0}!\nCan’t be anything else but.",            //  9
                "I feel so helplessly angry...\nIt’s {0}!",     // 10
                "{0} makes me sad...",                          // 11
                "I feel sorry for\n{0}!",                       // 12
                "The way I feel now is\n{0}...",                // 13
                "I lost, but I won at\n{0}!",                   // 14
                "I would’ve won if this\nwere {0}...",          // 15
                "My head’s filled with only\n{0} now!",         // 16
                "The way I lost...\nIt’s like {0}...",          // 17
                "Isn’t {0}\n{1}?",                              // 18
                "Aww... That’s really\n{0}..."                  // 19
            },
            {   // Mood 3: Other
                "Hello!\n{0}!",                                 //  0
                "I love {0}!",                                  //  1
                "I love {0}!\nI love {1}, too!",                //  2
                "This {0} is\n{1}, isn’t it?",                  //  3
                "I can do anything for\n{0}!",                  //  4
                "This {0} is\n{1}!",                            //  5
                "{0} is the real\n{1}!",                        //  6
                "It might be {0}...",                           //  7
                "There’s only {0}\nleft!",                      //  8
                "It’s {0}!\nIt’s {1}!",                         //  9
                "I prefer {0}\nafter all!",                     // 10
                "Is {0}\n{1}?",                                 // 11
                "Do you like {0}?",                             // 12
                "What do you think of\n{0}?",                   // 13
                "{0} is so\n{1}!",                              // 14
                "{0} are\n{1}!",                                // 15
                "{0}, right?",                                  // 16
                "Did you know {0}?\nIt’s {1}!",                 // 17
                "Excuse me...\nIt’s {0}!",                      // 18
                "{0}, right?\n{1}!"                             // 19
            },
            {   // Mood 4: Multiplay
                "{0}!\nHello!",                                 //  0
                "Glad to meet you!\nI love {0}!",               //  1
                "I’m a {0} Trainer!\nPlease battle me!",        //  2
                "Please trade!\nI’m offering {0}!",             //  3
                "Please trade!\nI want a {0}!",                 //  4
                "I’ve entered the Union Room.",                 //  5
                "Let’s draw! I want to draw\n{0}!",             //  6
                "I’ve got to go!\n{0}!",                        //  7
                "Please leave me alone...",                     //  8
                "Anyone want to\n{0}?",                         //  9
                "Let’s {0}!",                                   // 10
                "Want to {0}?",                                 // 11
                "I want to {0}!",                               // 12
                "OK!",                                          // 13
                "I don’t want to\n{0}.",                        // 14
                "I’ll go wait at the Colosseum\nnow.",          // 15
                "Please talk to me!",                           // 16
                "Do you know where I am?",                      // 17
                "I want to trade my {0}.\nPlease talk to me.",  // 18
                "I want a {0} battle!\nPlease talk to me!"      // 19
            }
        };

        // 0-495: msg.narc/362 Pokemon
        // 496-963: msg.narc/589 Attacks
        // 964-981: msg.narc/565 Types
        // 982-1105: msg.narc/553 Abilities
        // 1106-1143: msg.narc/388 "Trainer"
        // 1144-1181: msg.narc/389 "People"
        // 1182-1288: msg.narc/390 "Greetings"
        // 1289-1392: msg.narc/391 "Lifestyle"
        // 1393-1439: msg.narc/392 "Feelings"
        // 1440-1471: msg.narc/393 "Tough words"
        // 1472-1494: msg.narc/394 "Union"
        private static string[] WORDS_POKEMON = new string[]
        {
            "-----","BULBASAUR","IVYSAUR","VENUSAUR",           // 0
            "CHARMANDER","CHARMELEON","CHARIZARD","SQUIRTLE",
            "WARTORTLE","BLASTOISE","CATERPIE","METAPOD",
            "BUTTERFREE","WEEDLE","KAKUNA","BEEDRILL",
            "PIDGEY","PIDGEOTTO","PIDGEOT","RATTATA",
            "RATICATE","SPEAROW","FEAROW","EKANS",
            "ARBOK","PIKACHU","RAICHU","SANDSHREW",
            "SANDSLASH","NIDORAN♀","NIDORINA","NIDOQUEEN",
            "NIDORAN♂","NIDORINO","NIDOKING","CLEFAIRY",
            "CLEFABLE","VULPIX","NINETALES","JIGGLYPUFF",
            "WIGGLYTUFF","ZUBAT","GOLBAT","ODDISH",
            "GLOOM","VILEPLUME","PARAS","PARASECT",
            "VENONAT","VENOMOTH","DIGLETT","DUGTRIO",
            "MEOWTH","PERSIAN","PSYDUCK","GOLDUCK",
            "MANKEY","PRIMEAPE","GROWLITHE","ARCANINE",
            "POLIWAG","POLIWHIRL","POLIWRATH","ABRA",
            "KADABRA","ALAKAZAM","MACHOP","MACHOKE",
            "MACHAMP","BELLSPROUT","WEEPINBELL","VICTREEBEL",
            "TENTACOOL","TENTACRUEL","GEODUDE","GRAVELER",
            "GOLEM","PONYTA","RAPIDASH","SLOWPOKE",
            "SLOWBRO","MAGNEMITE","MAGNETON","FARFETCH’D",
            "DODUO","DODRIO","SEEL","DEWGONG",
            "GRIMER","MUK","SHELLDER","CLOYSTER",
            "GASTLY","HAUNTER","GENGAR","ONIX",
            "DROWZEE","HYPNO","KRABBY","KINGLER",
            "VOLTORB","ELECTRODE","EXEGGCUTE","EXEGGUTOR",
            "CUBONE","MAROWAK","HITMONLEE","HITMONCHAN",
            "LICKITUNG","KOFFING","WEEZING","RHYHORN",
            "RHYDON","CHANSEY","TANGELA","KANGASKHAN",
            "HORSEA","SEADRA","GOLDEEN","SEAKING",
            "STARYU","STARMIE","MR. MIME","SCYTHER",
            "JYNX","ELECTABUZZ","MAGMAR","PINSIR",
            "TAUROS","MAGIKARP","GYARADOS","LAPRAS",
            "DITTO","EEVEE","VAPOREON","JOLTEON",
            "FLAREON","PORYGON","OMANYTE","OMASTAR",
            "KABUTO","KABUTOPS","AERODACTYL","SNORLAX",
            "ARTICUNO","ZAPDOS","MOLTRES","DRATINI",
            "DRAGONAIR","DRAGONITE","MEWTWO","MEW",
            "CHIKORITA","BAYLEEF","MEGANIUM","CYNDAQUIL",
            "QUILAVA","TYPHLOSION","TOTODILE","CROCONAW",
            "FERALIGATR","SENTRET","FURRET","HOOTHOOT",
            "NOCTOWL","LEDYBA","LEDIAN","SPINARAK",
            "ARIADOS","CROBAT","CHINCHOU","LANTURN",
            "PICHU","CLEFFA","IGGLYBUFF","TOGEPI",
            "TOGETIC","NATU","XATU","MAREEP",
            "FLAAFFY","AMPHAROS","BELLOSSOM","MARILL",
            "AZUMARILL","SUDOWOODO","POLITOED","HOPPIP",
            "SKIPLOOM","JUMPLUFF","AIPOM","SUNKERN",
            "SUNFLORA","YANMA","WOOPER","QUAGSIRE",
            "ESPEON","UMBREON","MURKROW","SLOWKING",
            "MISDREAVUS","UNOWN","WOBBUFFET","GIRAFARIG",
            "PINECO","FORRETRESS","DUNSPARCE","GLIGAR",
            "STEELIX","SNUBBULL","GRANBULL","QWILFISH",
            "SCIZOR","SHUCKLE","HERACROSS","SNEASEL",
            "TEDDIURSA","URSARING","SLUGMA","MAGCARGO",
            "SWINUB","PILOSWINE","CORSOLA","REMORAID",
            "OCTILLERY","DELIBIRD","MANTINE","SKARMORY",
            "HOUNDOUR","HOUNDOOM","KINGDRA","PHANPY",
            "DONPHAN","PORYGON2","STANTLER","SMEARGLE",
            "TYROGUE","HITMONTOP","SMOOCHUM","ELEKID",
            "MAGBY","MILTANK","BLISSEY","RAIKOU",
            "ENTEI","SUICUNE","LARVITAR","PUPITAR",
            "TYRANITAR","LUGIA","HO-OH","CELEBI",
            "TREECKO","GROVYLE","SCEPTILE","TORCHIC",
            "COMBUSKEN","BLAZIKEN","MUDKIP","MARSHTOMP",
            "SWAMPERT","POOCHYENA","MIGHTYENA","ZIGZAGOON",
            "LINOONE","WURMPLE","SILCOON","BEAUTIFLY",
            "CASCOON","DUSTOX","LOTAD","LOMBRE",
            "LUDICOLO","SEEDOT","NUZLEAF","SHIFTRY",
            "TAILLOW","SWELLOW","WINGULL","PELIPPER",
            "RALTS","KIRLIA","GARDEVOIR","SURSKIT",
            "MASQUERAIN","SHROOMISH","BRELOOM","SLAKOTH",
            "VIGOROTH","SLAKING","NINCADA","NINJASK",
            "SHEDINJA","WHISMUR","LOUDRED","EXPLOUD",
            "MAKUHITA","HARIYAMA","AZURILL","NOSEPASS",
            "SKITTY","DELCATTY","SABLEYE","MAWILE",
            "ARON","LAIRON","AGGRON","MEDITITE",
            "MEDICHAM","ELECTRIKE","MANECTRIC","PLUSLE",
            "MINUN","VOLBEAT","ILLUMISE","ROSELIA",
            "GULPIN","SWALOT","CARVANHA","SHARPEDO",
            "WAILMER","WAILORD","NUMEL","CAMERUPT",
            "TORKOAL","SPOINK","GRUMPIG","SPINDA",
            "TRAPINCH","VIBRAVA","FLYGON","CACNEA",
            "CACTURNE","SWABLU","ALTARIA","ZANGOOSE",
            "SEVIPER","LUNATONE","SOLROCK","BARBOACH",
            "WHISCASH","CORPHISH","CRAWDAUNT","BALTOY",
            "CLAYDOL","LILEEP","CRADILY","ANORITH",
            "ARMALDO","FEEBAS","MILOTIC","CASTFORM",
            "KECLEON","SHUPPET","BANETTE","DUSKULL",
            "DUSCLOPS","TROPIUS","CHIMECHO","ABSOL",
            "WYNAUT","SNORUNT","GLALIE","SPHEAL",
            "SEALEO","WALREIN","CLAMPERL","HUNTAIL",
            "GOREBYSS","RELICANTH","LUVDISC","BAGON",
            "SHELGON","SALAMENCE","BELDUM","METANG",
            "METAGROSS","REGIROCK","REGICE","REGISTEEL",
            "LATIAS","LATIOS","KYOGRE","GROUDON",
            "RAYQUAZA","JIRACHI","DEOXYS","TURTWIG",
            "GROTLE","TORTERRA","CHIMCHAR","MONFERNO",
            "INFERNAPE","PIPLUP","PRINPLUP","EMPOLEON",
            "STARLY","STARAVIA","STARAPTOR","BIDOOF",
            "BIBAREL","KRICKETOT","KRICKETUNE","SHINX",
            "LUXIO","LUXRAY","BUDEW","ROSERADE",
            "CRANIDOS","RAMPARDOS","SHIELDON","BASTIODON",
            "BURMY","WORMADAM","MOTHIM","COMBEE",
            "VESPIQUEN","PACHIRISU","BUIZEL","FLOATZEL",
            "CHERUBI","CHERRIM","SHELLOS","GASTRODON",
            "AMBIPOM","DRIFLOON","DRIFBLIM","BUNEARY",
            "LOPUNNY","MISMAGIUS","HONCHKROW","GLAMEOW",
            "PURUGLY","CHINGLING","STUNKY","SKUNTANK",
            "BRONZOR","BRONZONG","BONSLY","MIME JR.",
            "HAPPINY","CHATOT","SPIRITOMB","GIBLE",
            "GABITE","GARCHOMP","MUNCHLAX","RIOLU",
            "LUCARIO","HIPPOPOTAS","HIPPOWDON","SKORUPI",
            "DRAPION","CROAGUNK","TOXICROAK","CARNIVINE",
            "FINNEON","LUMINEON","MANTYKE","SNOVER",
            "ABOMASNOW","WEAVILE","MAGNEZONE","LICKILICKY",
            "RHYPERIOR","TANGROWTH","ELECTIVIRE","MAGMORTAR",
            "TOGEKISS","YANMEGA","LEAFEON","GLACEON",
            "GLISCOR","MAMOSWINE","PORYGON-Z","GALLADE",
            "PROBOPASS","DUSKNOIR","FROSLASS","ROTOM",
            "UXIE","MESPRIT","AZELF","DIALGA",
            "PALKIA","HEATRAN","REGIGIGAS","GIRATINA",
            "CRESSELIA","PHIONE","MANAPHY","DARKRAI",
            "SHAYMIN","ARCEUS","Egg","Bad Egg"
        };

        private static string[] WORDS_MOVES = new string[]
        {
            "-","POUND","KARATE CHOP","DOUBLESLAP",             // 496
            "COMET PUNCH","MEGA PUNCH","PAY DAY","FIRE PUNCH",
            "ICE PUNCH","THUNDERPUNCH","SCRATCH","VICEGRIP",
            "GUILLOTINE","RAZOR WIND","SWORDS DANCE","CUT",
            "GUST","WING ATTACK","WHIRLWIND","FLY",
            "BIND","SLAM","VINE WHIP","STOMP",
            "DOUBLE KICK","MEGA KICK","JUMP KICK","ROLLING KICK",
            "SAND-ATTACK","HEADBUTT","HORN ATTACK","FURY ATTACK",
            "HORN DRILL","TACKLE","BODY SLAM","WRAP",
            "TAKE DOWN","THRASH","DOUBLE-EDGE","TAIL WHIP",
            "POISON STING","TWINEEDLE","PIN MISSILE","LEER",
            "BITE","GROWL","ROAR","SING",
            "SUPERSONIC","SONICBOOM","DISABLE","ACID",
            "EMBER","FLAMETHROWER","MIST","WATER GUN",
            "HYDRO PUMP","SURF","ICE BEAM","BLIZZARD",
            "PSYBEAM","BUBBLEBEAM","AURORA BEAM","HYPER BEAM",
            "PECK","DRILL PECK","SUBMISSION","LOW KICK",
            "COUNTER","SEISMIC TOSS","STRENGTH","ABSORB",
            "MEGA DRAIN","LEECH SEED","GROWTH","RAZOR LEAF",
            "SOLARBEAM","POISONPOWDER","STUN SPORE","SLEEP POWDER",
            "PETAL DANCE","STRING SHOT","DRAGON RAGE","FIRE SPIN",
            "THUNDERSHOCK","THUNDERBOLT","THUNDER WAVE","THUNDER",
            "ROCK THROW","EARTHQUAKE","FISSURE","DIG",
            "TOXIC","CONFUSION","PSYCHIC","HYPNOSIS",
            "MEDITATE","AGILITY","QUICK ATTACK","RAGE",
            "TELEPORT","NIGHT SHADE","MIMIC","SCREECH",
            "DOUBLE TEAM","RECOVER","HARDEN","MINIMIZE",
            "SMOKESCREEN","CONFUSE RAY","WITHDRAW","DEFENSE CURL",
            "BARRIER","LIGHT SCREEN","HAZE","REFLECT",
            "FOCUS ENERGY","BIDE","METRONOME","MIRROR MOVE",
            "SELFDESTRUCT","EGG BOMB","LICK","SMOG",
            "SLUDGE","BONE CLUB","FIRE BLAST","WATERFALL",
            "CLAMP","SWIFT","SKULL BASH","SPIKE CANNON",
            "CONSTRICT","AMNESIA","KINESIS","SOFTBOILED",
            "HI JUMP KICK","GLARE","DREAM EATER","POISON GAS",
            "BARRAGE","LEECH LIFE","LOVELY KISS","SKY ATTACK",
            "TRANSFORM","BUBBLE","DIZZY PUNCH","SPORE",
            "FLASH","PSYWAVE","SPLASH","ACID ARMOR",
            "CRABHAMMER","EXPLOSION","FURY SWIPES","BONEMERANG",
            "REST","ROCK SLIDE","HYPER FANG","SHARPEN",
            "CONVERSION","TRI ATTACK","SUPER FANG","SLASH",
            "SUBSTITUTE","STRUGGLE","SKETCH","TRIPLE KICK",
            "THIEF","SPIDER WEB","MIND READER","NIGHTMARE",
            "FLAME WHEEL","SNORE","CURSE","FLAIL",
            "CONVERSION 2","AEROBLAST","COTTON SPORE","REVERSAL",
            "SPITE","POWDER SNOW","PROTECT","MACH PUNCH",
            "SCARY FACE","FAINT ATTACK","SWEET KISS","BELLY DRUM",
            "SLUDGE BOMB","MUD-SLAP","OCTAZOOKA","SPIKES",
            "ZAP CANNON","FORESIGHT","DESTINY BOND","PERISH SONG",
            "ICY WIND","DETECT","BONE RUSH","LOCK-ON",
            "OUTRAGE","SANDSTORM","GIGA DRAIN","ENDURE",
            "CHARM","ROLLOUT","FALSE SWIPE","SWAGGER",
            "MILK DRINK","SPARK","FURY CUTTER","STEEL WING",
            "MEAN LOOK","ATTRACT","SLEEP TALK","HEAL BELL",
            "RETURN","PRESENT","FRUSTRATION","SAFEGUARD",
            "PAIN SPLIT","SACRED FIRE","MAGNITUDE","DYNAMICPUNCH",
            "MEGAHORN","DRAGONBREATH","BATON PASS","ENCORE",
            "PURSUIT","RAPID SPIN","SWEET SCENT","IRON TAIL",
            "METAL CLAW","VITAL THROW","MORNING SUN","SYNTHESIS",
            "MOONLIGHT","HIDDEN POWER","CROSS CHOP","TWISTER",
            "RAIN DANCE","SUNNY DAY","CRUNCH","MIRROR COAT",
            "PSYCH UP","EXTREMESPEED","ANCIENTPOWER","SHADOW BALL",
            "FUTURE SIGHT","ROCK SMASH","WHIRLPOOL","BEAT UP",
            "FAKE OUT","UPROAR","STOCKPILE","SPIT UP",
            "SWALLOW","HEAT WAVE","HAIL","TORMENT",
            "FLATTER","WILL-O-WISP","MEMENTO","FACADE",
            "FOCUS PUNCH","SMELLINGSALT","FOLLOW ME","NATURE POWER",
            "CHARGE","TAUNT","HELPING HAND","TRICK",
            "ROLE PLAY","WISH","ASSIST","INGRAIN",
            "SUPERPOWER","MAGIC COAT","RECYCLE","REVENGE",
            "BRICK BREAK","YAWN","KNOCK OFF","ENDEAVOR",
            "ERUPTION","SKILL SWAP","IMPRISON","REFRESH",
            "GRUDGE","SNATCH","SECRET POWER","DIVE",
            "ARM THRUST","CAMOUFLAGE","TAIL GLOW","LUSTER PURGE",
            "MIST BALL","FEATHERDANCE","TEETER DANCE","BLAZE KICK",
            "MUD SPORT","ICE BALL","NEEDLE ARM","SLACK OFF",
            "HYPER VOICE","POISON FANG","CRUSH CLAW","BLAST BURN",
            "HYDRO CANNON","METEOR MASH","ASTONISH","WEATHER BALL",
            "AROMATHERAPY","FAKE TEARS","AIR CUTTER","OVERHEAT",
            "ODOR SLEUTH","ROCK TOMB","SILVER WIND","METAL SOUND",
            "GRASSWHISTLE","TICKLE","COSMIC POWER","WATER SPOUT",
            "SIGNAL BEAM","SHADOW PUNCH","EXTRASENSORY","SKY UPPERCUT",
            "SAND TOMB","SHEER COLD","MUDDY WATER","BULLET SEED",
            "AERIAL ACE","ICICLE SPEAR","IRON DEFENSE","BLOCK",
            "HOWL","DRAGON CLAW","FRENZY PLANT","BULK UP",
            "BOUNCE","MUD SHOT","POISON TAIL","COVET",
            "VOLT TACKLE","MAGICAL LEAF","WATER SPORT","CALM MIND",
            "LEAF BLADE","DRAGON DANCE","ROCK BLAST","SHOCK WAVE",
            "WATER PULSE","DOOM DESIRE","PSYCHO BOOST","ROOST",
            "GRAVITY","MIRACLE EYE","WAKE-UP SLAP","HAMMER ARM",
            "GYRO BALL","HEALING WISH","BRINE","NATURAL GIFT",
            "FEINT","PLUCK","TAILWIND","ACUPRESSURE",
            "METAL BURST","U-TURN","CLOSE COMBAT","PAYBACK",
            "ASSURANCE","EMBARGO","FLING","PSYCHO SHIFT",
            "TRUMP CARD","HEAL BLOCK","WRING OUT","POWER TRICK",
            "GASTRO ACID","LUCKY CHANT","ME FIRST","COPYCAT",
            "POWER SWAP","GUARD SWAP","PUNISHMENT","LAST RESORT",
            "WORRY SEED","SUCKER PUNCH","TOXIC SPIKES","HEART SWAP",
            "AQUA RING","MAGNET RISE","FLARE BLITZ","FORCE PALM",
            "AURA SPHERE","ROCK POLISH","POISON JAB","DARK PULSE",
            "NIGHT SLASH","AQUA TAIL","SEED BOMB","AIR SLASH",
            "X-SCISSOR","BUG BUZZ","DRAGON PULSE","DRAGON RUSH",
            "POWER GEM","DRAIN PUNCH","VACUUM WAVE","FOCUS BLAST",
            "ENERGY BALL","BRAVE BIRD","EARTH POWER","SWITCHEROO",
            "GIGA IMPACT","NASTY PLOT","BULLET PUNCH","AVALANCHE",
            "ICE SHARD","SHADOW CLAW","THUNDER FANG","ICE FANG",
            "FIRE FANG","SHADOW SNEAK","MUD BOMB","PSYCHO CUT",
            "ZEN HEADBUTT","MIRROR SHOT","FLASH CANNON","ROCK CLIMB",
            "DEFOG","TRICK ROOM","DRACO METEOR","DISCHARGE",
            "LAVA PLUME","LEAF STORM","POWER WHIP","ROCK WRECKER",
            "CROSS POISON","GUNK SHOT","IRON HEAD","MAGNET BOMB",
            "STONE EDGE","CAPTIVATE","STEALTH ROCK","GRASS KNOT",
            "CHATTER","JUDGMENT","BUG BITE","CHARGE BEAM",
            "WOOD HAMMER","AQUA JET","ATTACK ORDER","DEFEND ORDER",
            "HEAL ORDER","HEAD SMASH","DOUBLE HIT","ROAR OF TIME",
            "SPACIAL REND","LUNAR DANCE","CRUSH GRIP","MAGMA STORM",
            "DARK VOID","SEED FLARE","OMINOUS WIND","SHADOW FORCE"
        };

        private static string[] WORDS_TYPES = new string[]
        {
            "NORMAL","FIGHTING","FLYING","POISON",              // 964
            "GROUND","ROCK","BUG","GHOST",
            "STEEL","???","FIRE","WATER",
            "GRASS","ELECTRIC","PSYCHIC","ICE",
            "DRAGON","DARK"
        };

        private static string[] WORDS_ABILITIES = new string[]
        {
            "-","STENCH","DRIZZLE","SPEED BOOST",               // 982
            "BATTLE ARMOR","STURDY","DAMP","LIMBER",
            "SAND VEIL","STATIC","VOLT ABSORB","WATER ABSORB",
            "OBLIVIOUS","CLOUD NINE","COMPOUNDEYES","INSOMNIA",
            "COLOR CHANGE","IMMUNITY","FLASH FIRE","SHIELD DUST",
            "OWN TEMPO","SUCTION CUPS","INTIMIDATE","SHADOW TAG",
            "ROUGH SKIN","WONDER GUARD","LEVITATE","EFFECT SPORE",
            "SYNCHRONIZE","CLEAR BODY","NATURAL CURE","LIGHTNINGROD",
            "SERENE GRACE","SWIFT SWIM","CHLOROPHYLL","ILLUMINATE",
            "TRACE","HUGE POWER","POISON POINT","INNER FOCUS",
            "MAGMA ARMOR","WATER VEIL","MAGNET PULL","SOUNDPROOF",
            "RAIN DISH","SAND STREAM","PRESSURE","THICK FAT",
            "EARLY BIRD","FLAME BODY","RUN AWAY","KEEN EYE",
            "HYPER CUTTER","PICKUP","TRUANT","HUSTLE",
            "CUTE CHARM","PLUS","MINUS","FORECAST",
            "STICKY HOLD","SHED SKIN","GUTS","MARVEL SCALE",
            "LIQUID OOZE","OVERGROW","BLAZE","TORRENT",
            "SWARM","ROCK HEAD","DROUGHT","ARENA TRAP",
            "VITAL SPIRIT","WHITE SMOKE","PURE POWER","SHELL ARMOR",
            "AIR LOCK","TANGLED FEET","MOTOR DRIVE","RIVALRY",
            "STEADFAST","SNOW CLOAK","GLUTTONY","ANGER POINT",
            "UNBURDEN","HEATPROOF","SIMPLE","DRY SKIN",
            "DOWNLOAD","IRON FIST","POISON HEAL","ADAPTABILITY",
            "SKILL LINK","HYDRATION","SOLAR POWER","QUICK FEET",
            "NORMALIZE","SNIPER","MAGIC GUARD","NO GUARD",
            "STALL","TECHNICIAN","LEAF GUARD","KLUTZ",
            "MOLD BREAKER","SUPER LUCK","AFTERMATH","ANTICIPATION",
            "FOREWARN","UNAWARE","TINTED LENS","FILTER",
            "SLOW START","SCRAPPY","STORM DRAIN","ICE BODY",
            "SOLID ROCK","SNOW WARNING","HONEY GATHER","FRISK",
            "RECKLESS","MULTITYPE","FLOWER GIFT","BAD DREAMS"
        };

        private static string[] WORDS_TRAINER = new string[]
        {
            "MATCH UP","NO. 1","PREPARATION","WINS",            // 1106
            "NO MATCH","SPIRIT","ACE CARD","COME ON",
            "ATTACK","SURRENDER","COURAGE","TALENT",
            "STRATEGY","MATCH","VICTORY","SENSE",
            "VERSUS","FIGHTS","POWER","CHALLENGE",
            "STRONG","TAKE IT EASY","FOE","GENIUS",
            "LEGEND","BATTLE","FIGHT","REVIVE",
            "POINTS","SERIOUS","LOSS","PARTNER",
            "INVINCIBLE","EASY","WEAK","EASY WIN",
            "MOVE","TRAINER"
        };

        private static string[] WORDS_PEOPLE = new string[]
        {
            "OPPONENT","I","YOU","MOTHER",                      // 1144
            "GRANDFATHER","UNCLE","FATHER","BOY",
            "ADULT","BROTHER","SISTER","GRANDMOTHER",
            "AUNT","PARENT","OLD MAN","ME",
            "GIRL","GAL","FAMILY","HER",
            "HIM","YOU","SIBLINGS","KIDS",
            "MR.","MS.","MYSELF","WHO",
            "FRIEND","ALLY","PERSON","KIDS",
            "I","EVERYONE","RIVAL","I",
            "I","BABY"
        };

        private static string[] WORDS_GREETINGS = new string[]
        {
            "KONNICHIWA","HELLO","BONJOUR","CIAO",              // 1182
            "HALLO","HOLA","OH WELL","AAH",
            "AHAHA","HUH","THANKS","NO PROBLEM",
            "NOPE","YES","HERE GOES","LET’S GO",
            "HERE I COME","YEAH","WELCOME","URGH",
            "LET ME THINK","HMM","WHOA","WROOOAAR!",
            "WOW","SNICKER","CUTE LAUGH","UNBELIEVABLE",
            "CRIES","OK","AGREE","EH?",
            "BOO-HOO","HEHEHE","HEY","OH, YEAH",
            "OH WOW!","HEEEY","GREETINGS","OOPS",
            "WELL DONE","OH MY","EEK","YAAAH",
            "GIGGLE","GIVE ME","GWAHAHAHA","UGH",
            "SORRY","FORGIVE ME","I’M SORRY","HEY!",
            "GOOD-BYE","THANK YOU","I’VE ARRIVED","WEEP",
            "PARDON ME","SO SORRY","SEE YA","EXCUSE ME",
            "OKAY THEN","TUT","BLUSH","GO AHEAD",
            "CHEERS","HEY?","WHAT’S UP?","HUH?",
            "NO","SIGH","HI","YEP",
            "YEAH, YEAH","BYE-BYE","MEET YOU","HAHAHA",
            "AIYEEH","HIYAH","MUHAHAHA","LOL",
            "SNORT","HUMPH","HEY","HE-HE-HE",
            "HEH","HOHOHO","THERE YOU GO","OH, DEAR",
            "BYE FOR NOW","ANGRY","MUFUFU","MMM",
            "HELLO?","HI THERE","NO WAY","YAHOO",
            "YO","WELCOME","OK","REGARDS",
            "LALALA","YAY","WAIL","WOW",
            "BOO!","WAHAHA","..."
        };

        private static string[] WORDS_LIFESTYLE = new string[]
        {
            "IDOL","TOMORROW","PLAYING","ANIME",                // 1289
            "JOB","SONG","HOME","MOVIE",
            "SWEETS","MONEY","POCKET MONEY","CHIT-CHAT",
            "TALK","BATH","PLAY HOUSE","TOYS",
            "MUSIC","CARDS","SHOPPING","CONVERSATION",
            "SCHOOL","CAMERA","VIEWING","SPECTATE",
            "ANNIVERSARY","YESTERDAY","TODAY","HABIT",
            "GROUP","GOURMET","GAME","WORD",
            "COLLECTION","STORE","COMPLETE","SERVICE",
            "MAGAZINE","WALK","WORK","SYSTEM",
            "BICYCLE","TRAINING","CLASS","LESSONS",
            "HOBBY","INFORMATION","SPORTS","DAILY LIFE",
            "TEACHER","SOFTWARE","SONGS","DIET",
            "TOURNAMENT","TREASURE","TRAVEL","BIRTHDAY",
            "DANCE","CHANNEL","FISHING","DATE",
            "LETTER","EVENT","DESIGN","DIGITAL",
            "TEST","DEPT. STORE","TELEVISION","TRAIN",
            "PHONE","ITEM","NAME","NEWS",
            "POPULARITY","STUFFED TOY","PARTY","COMPUTER",
            "FLOWERS","HERO","NAP","HEROINE",
            "FASHION","STUDY","ADVENTURE","BOARD",
            "BALL","BOOK","MACHINE","FESTIVAL",
            "COMICS","MAIL","MESSAGE","STORY",
            "PROMISE","HOLIDAY","DREAM","KINDERGARTEN",
            "PLANS","LIFE","RADIO","CRAZE",
            "VACATION","LOOKS","RENTAL","WORLD"
        };

        private static string[] WORDS_FEELINGS = new string[]
        {
            "BEAUTY","DELIGHT","STRANGENESS","CLEVERNESS",      // 1393
            "DISAPPOINTED","COOLNESS","SADNESS","CUTENESS",
            "ANGER","HEALTHY","REGRET","HAPPINESS",
            "DEPRESSED","INCREDIBLE","LIKES","DISLIKE",
            "BORED","IMPORTANT","ALL RIGHT","ADORE",
            "TOUGHNESS","ENJOYMENT","USELESS","DROOLING",
            "EXCITED","SKILLFUL","TEARS","HATE",
            "ROFL","HAPPY","ENERGETIC","SURPRISE",
            "NERVOUS","WANT","SATISFIED","RARE",
            "MESSED UP","NO WAY","DANGER","LOVEY-DOVEY",
            "ANTICIPATION","SMILE","SUBTLE","RECOMMEND",
            "SIMPLE","NICE","DIFFICULT"
        };

        private static string[] WORDS_TOUGH = new string[]
        {
            "EARTH TONES","IMPLANT","GOLDEN RATIO","OMNIBUS",   // 1440
            "STARBOARD","MONEY RATE","RESOLUTION","CADENZA",
            "EDUCATION","CUBISM","CROSS-STITCH","ARTERY",
            "BONE DENSITY","GOMMAGE","STREAMING","CONDUCTIVITY",
            "COPYRIGHT","TWO-STEP","CONTOUR","NEUTRINO",
            "HOWLING","SPREADSHEET","GMT","IRRITABILITY",
            "FRACTALS","FLAMBE","STOCK PRICES","PH BALANCE",
            "VECTOR","POLYPHENOL","UBIQUITOUS","REM SLEEP"
        };

        private static string[] WORDS_UNION = new string[]
        {
            "SINGLE","DOUBLE","MIX BATTLE","MULTI BATTLE",      // 1472
            "LEVEL 50","LEVEL 100","COLOSSEUM","POKéMON",
            "DRAWING","RECORD","GOTCHA","CHAT",
            "FRIEND CODE","CONNECTION","VOICE CHAT","WI-FI",
            "UNDERGROUND","UNION","POFFIN","CONTEST",
            "BATTLE TOWER","GTS","SECRET BASE"
        };
        #endregion
    }
}
