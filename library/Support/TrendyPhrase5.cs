using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public class TrendyPhrase5 : TrendyPhraseBase
    {
        public TrendyPhrase5(byte[] data)
            : base(data)
        {
        }

        public TrendyPhrase5(ushort mood, ushort index, ushort word1, ushort word2)
            : base(Pack(mood, index, word1, word2))
        {
        }

        public TrendyPhrase5(TrendyPhrase4 phrase) : base(null)
        {
            // Upgrade a TrendyPhrase4
            // fixme: These 4 values are all wrong and need offsets applied, if not bigger changes
            Data = Pack(phrase.Mood, phrase.Index, phrase.Word1, phrase.Word2);
        }

        public override string Render(string wordFormat)
        {
            return RenderPhrase(Data, wordFormat);
        }

        public static string RenderPhrase(byte[] data, string wordFormat)
        {
            // todo: move me to TrendyPhraseBase, make RenderWord virtual
            if (data == null) throw new ArgumentNullException();
            if (data.Length != 8) throw new ArgumentException();

            ushort mood = BitConverter.ToUInt16(data, 0);
            ushort index = BitConverter.ToUInt16(data, 2);
            ushort word1 = BitConverter.ToUInt16(data, 4);
            ushort word2 = BitConverter.ToUInt16(data, 6);

            // moods higher than 6 seem to say "no answer" but indexes higher than 20 are blank. 
            if (mood >= 7) return "";
            if (index >= 21) return "";
            return string.Format(PHRASES[mood, index],
                string.Format(wordFormat, RenderWord(word1)),
                string.Format(wordFormat, RenderWord(word2))
                );
        }

        public static string RenderWord(ushort word)
        {
            // todo: There's a certain list of banned spoiler words like
            // attacks from BW2 which should be turned into POKÉMON.
            if (word < 652) return WORDS_POKEMON[word];
            if (word < 1212) return WORDS_MOVES[word - 652];
            if (word < 1229) return WORDS_TYPES[word - 1212];
            if (word < 1394) return WORDS_ABILITIES[word - 1229];
            if (word < 1438) return WORDS_TRAINER[word - 1394];
            if (word < 1476) return WORDS_PEOPLE[word - 1438];
            if (word < 1524) return WORDS_GREETINGS[word - 1476];
            if (word < 1627) return WORDS_LIFESTYLE[word - 1524];
            if (word < 1674) return WORDS_FEELINGS[word - 1627];
            if (word < 1706) return WORDS_TERM[word - 1674];
            if (word < 1732) return WORDS_CONNECTION[word - 1706];
            if (word < 1742) return WORDS_ANIMATED[word - 1732];
            if (word < 1803) return WORDS_VOICE[word - 1742];
            if (word == 65535) return ""; // special case for unfilled in word = blank
            return "POKÉMON";
        }

        public TrendyPhrase5 Clone()
        {
            return new TrendyPhrase5(Data);
        }

        #region String tables
        // todo: i18n
        // Special thanks to http://projectpokemon.org/rawdb/black2/msg.php
        // for their string table dumps.

        // a/0/0/2/170 through /176
        private static string[,] PHRASES = new string[,]
        {
            {   // Mood 0: Other /171                
                "Hello!\n{0}!",
                "I am {0}!\nI'm pleased to meet you.",
                "I love {0}!\nI love {1}, too!",
                "My favorite {0} is\n{1}!",
                "What's your favorite\n{0}?",
                "I can do anything for\n{0}!",
                "Is {0}\n{1}?",
                "What do you think of\n{0}?",
                "Do you think {0}\ncan {1}?",
                "{0} is so\n{1}!",
                "{0} bothers me.",
                "After all, it's {0},\nisn't it?",
                "{0} is the real\n{1}!",
                "Did you know that {0}\nis {1}?",
                "{0} is the reason\nfor {1}.",
                "Have you heard of\n{0}?",
                "{0} is actually\n{1}.",
                "Recently, {0}\nseems {1}.",
                "I wonder if\n{0} is yummy...",
                "I never miss {0}.\nIt's part of what I do every day.",
                "",
            },
            {   // Mood 1: Start of battle /173          
                "Please!\n{0}",
                "Go! {0}!",
                "I'll battle with\n{0}!",
                "{0} is\n{1}, right?",
                "{0}, I'm going\nwith {1}!",
                "In comes {0}.",
                "Watch my {0} power\ntake care of {1}!",
                "Now {0}\nbegins!",
                "I'll show you my\n{0} strategy!",
                "I'll shock you with\n{0}!",
                "{0}, I see...\nGo, {1}!",
                "Ta-da!\nHere comes {0}!",
                "I don't think I'll\never lose to {0}!",
                "{0}!\n{1} is here!",
                "Good luck,\n{0}!",
                "Behold my {0}\n{1}!",
                "The power of {0}!\nLet me show you!",
                "You'll choose {0}\nif I choose {1}, right?",
                "I beg you, {0}.\nPlease go with {1}!",
                "May {0} safely\nland on {0}!",
                "",
            },
            {   // Mood 2: Victory /176
                "I win!\n{0}!",
                "I owe my victory\nto {0}!",
                "{0} is strong,\nisn't it?",
                "It's {0}\n{1} after all!",
                "When it comes to {0},\nmy choice is always {1}!",
                "Victory in a\n{0} battle!",
                "Yay, {0}!\n{1}!",
                "Sorry, it's {0}\n{1}.",
                "{0}!\nThank you!",
                "The way I feel now is\n{0}!",
                "{0} sure is\n{1}!",
                "It's all thanks to\n{0}.",
                "{0} is the toughest!",
                "{0}?\nWow, I'm so glad!",
                "{0}?\nThat sounds good!",
                "I have no trouble\ndealing with {0}.",
                "{0} is so much fun.",
                "Huh?\n{0}?!",
                "The power of {0}\nis awesome!",
                "Everyone!\n{0}!",
                "",
            },
            {   // Mood 3: Defeat /170
                "You win...\n{0}!",
                "{0} is\nreally impressive.",
                "Waaah! {0}!",
                "I want to go home with\n{0}...",
                "{0}!\n{1}!",
                "I see {0}\nright in front of me!",
                "{0}?\nI didn't see that coming!",
                "I was confident about\n{0}, too.",
                "You're {0},\naren't you?",
                "{0}!\nCan't be anything else but.",
                "I want to be like {0}!",
                "It might be\n{0} already...",
                "I think {0}\nshould do.",
                "The way I feel now is\n{0}...",
                "{0} won't work!",
                "Nothing beats {0}!",
                "My head's filled with only\n{0} now!",
                "Is it because {0}\nwas lacking?",
                "Isn't {0}\n{1}?",
                "Aww... That's really\n{0}...",
                "",
            },
            {   // Mood 4: Other /175
                "Yo!\nI'm {1}.",
                "Glad to meet you!\nI love {0}!",
                "Do you like {0}?",
                "Let's draw! I want to draw\n{0}!",
                "Let's battle!\nI say {0}!",
                "I'm a {0} Trainer!\nPlease battle me!",
                "Let's have a chat!\nHow about {0}?",
                "Please trade!\nI want a {0}!",
                "Please trade!\nI'm offering {0}!",
                "Want to trade {0}?\nHere's a hint: {1}!",
                "Will you join me\nfor {0}?",
                "Anyone want to\n{0}?",
                "I want to {0}\nwith {1}!",
                "Let's go to {0}\nby {1}!",
                "OK!",
                "{0}?\nI got it!",
                "{0}?\nHold on!",
                "I don't want to\n{0}...",
                "That was fun! I hope we can\n{0} again sometime.",
                "See ya!\n{0}!",
                "",
            },
            {   // Mood 5: Greetings /172 New to GenV. Seems to be locked by default?
                "Glad to meet you!\nI am {0}!",
                "I'm a {0}-loving\n{1} Trainer.",
                "Let's {0} sometime.\nKeep in touch!",
                "{0} is the best!\nI love it!",
                "It's great because it's\n{0}. Don't you agree?",
                "Tell me your favorite\n{0}.",
                "Hi!\nDo you know {0}?",
                "It's very {0}\nand {1}!",
                "Let's {0} soon!",
                "Thank you for taking your time\nwith {0}.",
                "It was so {0}.\nI was moved!",
                "What do you think of\n{0}?",
                "It's {0},\nif you ask me.",
                "It bothers us, doesn't it?\nI'm talking about {0}.",
                "Do you know what\nthey call {0}?",
                "This {0} is\nsurprisingly {1}!",
                "{0} sure is something.\nYou should try it!",
                "Thank you for taking your time\nwith {0}.",
                "{0} is\n{1}, don't you think?",
                "That means {0}.\nThanks!",
                "We should {0} together\nagain. {1}!",
            },
            { // Mood 6: Placeholder to hold a single trendy word
                "{0}",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            },
        };

        private static string[] WORDS_POKEMON = new string[]
        {
            "POKÉMON","BULBASAUR","IVYSAUR","VENUSAUR",           // 0
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
            "SLOWBRO","MAGNEMITE","MAGNETON","FARFETCH'D",
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
            "SHAYMIN","ARCEUS","VICTINI","SNIVY",
            "SERVINE","SERPERIOR","TEPIG","PIGNITE",
            "EMBOAR","OSHAWOTT","DEWOTT","SAMUROTT",
            "PATRAT","WATCHOG","LILLIPUP","HERDIER",
            "STOUTLAND","PURRLOIN","LIEPARD","PANSAGE",
            "SIMISAGE","PANSEAR","SIMISEAR","PANPOUR",
            "SIMIPOUR","MUNNA","MUSHARNA","PIDOVE",
            "TRANQUILL","UNFEZANT","BLITZLE","ZEBSTRIKA",
            "ROGGENROLA","BOLDORE","GIGALITH","WOOBAT",
            "SWOOBAT","DRILBUR","EXCADRILL","AUDINO",
            "TIMBURR","GURDURR","CONKELDURR","TYMPOLE",
            "PALPITOAD","SEISMITOAD","THROH","SAWK",
            "SEWADDLE","SWADLOON","LEAVANNY","VENIPEDE",
            "WHIRLIPEDE","SCOLIPEDE","COTTONEE","WHIMSICOTT",
            "PETILIL","LILLIGANT","BASCULIN","SANDILE",
            "KROKOROK","KROOKODILE","DARUMAKA","DARMANITAN",
            "MARACTUS","DWEBBLE","CRUSTLE","SCRAGGY",
            "SCRAFTY","SIGILYPH","YAMASK","COFAGRIGUS",
            "TIRTOUGA","CARRACOSTA","ARCHEN","ARCHEOPS",
            "TRUBBISH","GARBODOR","ZORUA","ZOROARK",
            "MINCCINO","CINCCINO","GOTHITA","GOTHORITA",
            "GOTHITELLE","SOLOSIS","DUOSION","REUNICLUS",
            "DUCKLETT","SWANNA","VANILLITE","VANILLISH",
            "VANILLUXE","DEERLING","SAWSBUCK","EMOLGA",
            "KARRABLAST","ESCAVALIER","FOONGUS","AMOONGUSS",
            "FRILLISH","JELLICENT","ALOMOMOLA","JOLTIK",
            "GALVANTULA","FERROSEED","FERROTHORN","KLINK",
            "KLANG","KLINKLANG","TYNAMO","EELEKTRIK",
            "EELEKTROSS","ELGYEM","BEHEEYEM","LITWICK",
            "LAMPENT","CHANDELURE","AXEW","FRAXURE",
            "HAXORUS","CUBCHOO","BEARTIC","CRYOGONAL",
            "SHELMET","ACCELGOR","STUNFISK","MIENFOO",
            "MIENSHAO","DRUDDIGON","GOLETT","GOLURK",
            "PAWNIARD","BISHARP","BOUFFALANT","RUFFLET",
            "BRAVIARY","VULLABY","MANDIBUZZ","HEATMOR",
            "DURANT","DEINO","ZWEILOUS","HYDREIGON",
            "LARVESTA","VOLCARONA","COBALION","TERRAKION",
            "VIRIZION","TORNADUS","THUNDURUS","RESHIRAM",
            "ZEKROM","LANDORUS","KYUREM","KELDEO",
            "MELOETTA","GENESECT","EGG","BAD EGG"
        };

        private static string[] WORDS_MOVES = new[]
        {
            "-----","POUND","KARATE CHOP","DOUBLESLAP",         // 652
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
            "DARK VOID","SEED FLARE","OMINOUS WIND","SHADOW FORCE",
            "HONE CLAWS","WIDE GUARD","GUARD SPLIT","POWER SPLIT",
            "WONDER ROOM","PSYSHOCK","VENOSHOCK","AUTOTOMIZE",
            "RAGE POWDER","TELEKINESIS","MAGIC ROOM","SMACK DOWN",
            "STORM THROW","FLAME BURST","SLUDGE WAVE","QUIVER DANCE",
            "HEAVY SLAM","SYNCHRONOISE","ELECTRO BALL","SOAK",
            "FLAME CHARGE","COIL","LOW SWEEP","ACID SPRAY",
            "FOUL PLAY","SIMPLE BEAM","ENTRAINMENT","AFTER YOU",
            "ROUND","ECHOED VOICE","CHIP AWAY","CLEAR SMOG",
            "STORED POWER","QUICK GUARD","ALLY SWITCH","SCALD",
            "SHELL SMASH","HEAL PULSE","HEX","SKY DROP",
            "SHIFT GEAR","CIRCLE THROW","INCINERATE","QUASH",
            "ACROBATICS","REFLECT TYPE","RETALIATE","FINAL GAMBIT",
            "BESTOW","INFERNO","WATER PLEDGE","FIRE PLEDGE",
            "GRASS PLEDGE","VOLT SWITCH","STRUGGLE BUG","BULLDOZE",
            "FROST BREATH","DRAGON TAIL","WORK UP","ELECTROWEB",
            "WILD CHARGE","DRILL RUN","DUAL CHOP","HEART STAMP",
            "HORN LEECH","SACRED SWORD","RAZOR SHELL","HEAT CRASH",
            "LEAF TORNADO","STEAMROLLER","COTTON GUARD","NIGHT DAZE",
            "PSYSTRIKE","TAIL SLAP","HURRICANE","HEAD CHARGE",
            "GEAR GRIND","SEARING SHOT","TECHNO BLAST","RELIC SONG",
            "SECRET SWORD","GLACIATE","BOLT STRIKE","BLUE FLARE",
            "FIERY DANCE","FREEZE SHOCK","ICE BURN","SNARL",
            "ICICLE CRASH","V-CREATE","FUSION FLARE","FUSION BOLT"
        };

        private static string[] WORDS_TYPES = new string[]
        {
            "NORMAL","FIGHTING","FLYING","POISON",              // 1212
            "GROUND","ROCK","BUG","GHOST",
            "STEEL","FIRE","WATER","GRASS",
            "ELECTRIC","PSYCHIC","ICE","DRAGON",
            "DARK"
        };

        private static string[] WORDS_ABILITIES = new string[]
        {
            "-","STENCH","DRIZZLE","SPEED BOOST",               // 1229
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
            "RECKLESS","MULTITYPE","FLOWER GIFT","BAD DREAMS",
            "PICKPOCKET","SHEER FORCE","CONTRARY","UNNERVE",
            "DEFIANT","DEFEATIST","CURSED BODY","HEALER",
            "FRIEND GUARD","WEAK ARMOR","HEAVY METAL","LIGHT METAL",
            "MULTISCALE","TOXIC BOOST","FLARE BOOST","HARVEST",
            "TELEPATHY","MOODY","OVERCOAT","POISON TOUCH",
            "REGENERATOR","BIG PECKS","SAND RUSH","WONDER SKIN",
            "ANALYTIC","ILLUSION","IMPOSTER","INFILTRATOR",
            "MUMMY","MOXIE","JUSTIFIED","RATTLED",
            "MAGIC BOUNCE","SAP SIPPER","PRANKSTER","SAND FORCE",
            "IRON BARBS","ZEN MODE","VICTORY STAR","TURBOBLAZE",
            "TERAVOLT"
        };

        private static string[] WORDS_TRAINER = new string[]
        {
            "MATCH UP","NO. 1","BAD MATCHUP","PREPARATION",     // 1394
            "WINS","NO MATCH","SPIRIT","CRITICAL HIT",
            "ACE CARD","COME ON","NO EFFECT","ATTACK",
            "SURRENDER","COURAGE","TALENT","STRATEGY",
            "MATCH","VICTORY","SENSE","VERSUS",
            "FIGHTS","POWER","CHALLENGE","CHAMPION",
            "STRONG","TAKE IT EASY","FOE","GENIUS",
            "LEGEND","TRAINER","GOOD MATCHUP","BATTLE",
            "EMERGENCY","FIGHT","REVIVE","POINTS",
            "SERIOUS","LOSS","PARTNER","INVINCIBLE",
            "EASY","WEAK","EASY WIN","MOVE"
        };

        private static string[] WORDS_PEOPLE = new string[]
        {
            "OPPONENT","I","YOU","MOTHER",                      // 1438
            "GRANDFATHER","UNCLE","FATHER","BOY",
            "ADULT","BROTHER","SISTER","GRANDMOTHER",
            "AUNT","PARENT","OLD MAN","ME",
            "GIRL","GAL","FAMILY","HER",
            "HIM","YOU","SIBLINGS","KIDS",
            "MR.","MS.","MYSELF","WHO",
            "FRIEND","ALLY","PERSON","BABY",
            "KIDS","I","EVERYONE","RIVAL",
            "I","I"
        };

        private static string[] WORDS_GREETINGS = new string[]
        {
            "こんにちは","HELLO","BONJOUR","CIAO",               // 1476
            "HALLO","HOLA","안녕하세요","HELLO",
            "...","THANKS","NO PROBLEM","NOPE",
            "YES","HERE GOES","LET'S GO","HERE I COME",
            "WELCOME","GREETINGS","WELL DONE","GIVE ME",
            "SORRY","FORGIVE ME","I'M SORRY","GOOD-BYE",
            "THANK YOU","I'VE ARRIVED","PARDON ME","SO SORRY",
            "SEE YA","EXCUSE ME","OK THEN","GO AHEAD",
            "CHEERS","WHAT'S UP?","NO","HI",
            "YEP","YEAH, YEAH","BYE-BYE","MEET YOU",
            "BYE FOR NOW","HELLO?","HI THERE","NO WAY",
            "YAHOO","YO","WELCOME","REGARDS"
        };

        private static string[] WORDS_LIFESTYLE = new string[]
        {
            "IDOL","AUTUMN","TOMORROW","PLAYING",               // 1524
            "ANIME","JOB","SONG","HOME",
            "MOVIE","SWEETS","MONEY","POCKET MONEY",
            "CHIT-CHAT","TALK","BATH","PLAY HOUSE",
            "TOYS","MUSIC","CARDS","SHOPPING",
            "CONVERSATION","SCIENCE","SCHOOL","CAMERA",
            "VIEWING","SPECTATE","ANNIVERSARY","YESTERDAY",
            "TODAY","HABIT","GOURMET","GAME",
            "WORD","STORE","SERVICE","MAGAZINE",
            "WALK","WORK","BICYCLE","GYM",
            "TRAINING","CLASS","LESSONS","HOBBY",
            "INFORMATION","SHOP","SPORTS","DAILY LIFE",
            "TEACHER","SOFTWARE","SONGS","DIET",
            "TOURNAMENT","TREASURE","TRAVEL","BIRTHDAY",
            "DANCE","CHANNEL","FISHING","DATE",
            "LETTER","EVENT","TEST","DEPT. STORE",
            "TELEVISION","TRAIN","PHONE","ITEM",
            "SUMMER","NAME","NEWS","POPULARITY",
            "STUFFED TOY","PARTY","COMPUTER","FLOWERS",
            "SPRING","HERO","NAP","HEROINE",
            "FASHION","WINTER","STUDY","ADVENTURE",
            "BOARD","BALL","BOOK","MACHINE",
            "FESTIVAL","COMICS","STORY","PROMISE",
            "HOLIDAY","FAIRGROUND","DREAM","KINDERGARTEN",
            "PLANS","LIFE","RADIO","CRAZE",
            "VACATION","LOOKS","WORLD"
        };

        private static string[] WORDS_FEELINGS = new string[]
        {
            "BEAUTY","DELIGHT","STRANGENESS","RECOMMEND",       // 1627
            "CLEVERNESS","DISAPPOINTED","COOLNESS","SADNESS",
            "CUTENESS","SIMPLE","ANGER","HEALTHY",
            "REGRET","HAPPINESS","DEPRESSED","INCREDIBLE",
            "LIKES","DISLIKE","BORED","IMPORTANT",
            "ALL RIGHT","ADORE","TOUGHNESS","ENJOYMENT",
            "USELESS","DROOLING","EXCITED","SKILLFUL",
            "NICE","TEARS","HATE","ROFL",
            "HAPPY","ENERGETIC","SURPRISE","SUBTLE",
            "NERVOUS","WANT","SATISFIED","DIFFICULT",
            "RARE","MESSED UP","NO WAY","DANGER",
            "LOVEY-DOVEY","ANTICIPATION","SMILE"
        };

        private static string[] WORDS_TERM = new string[]
        {
            "C-GEAR","PASS POWER","ELEGANT","CUTE",             // 1674
            "COOL","GROUP","GOTCHA","COLLECTION",
            "COMPLETE","SYSTEM","LAUNCHER","NATURE",
            "DOWSING","TOWN MAP","DESIGN","DIGITAL",
            "HALL OF FAME","INSTITUTE","BADGE","BATTLE TEST",
            "BATTLE BOX","PASS ORB","⒆⒇ CENTER","POKÉMON", // todo: Fix PkMn CENTER encoding issue
            "MISSION","MAIL","MESSAGE","ENTREE",
            "UNIQUE","LEVEL","RENTAL","TM"
        };

        private static string[] WORDS_CONNECTION = new string[]
        {
            "PGL","GTS","POKÉMON DW","Wi-Fi",                   // 1706
            "SPIN","GAME SYNC","COLOSSEUM","SUBWAY",
            "GEONET","SINGLE","INFRARED","DOUBLE",
            "CHAT","CONNECTION","FRIEND CODE","TRIPLE",
            "ENTRALINK","BATTLE VIDEOS","VOICE CHAT","POKÉ TRANSFER",
            "MULTI BATTLE","MUSICAL","UNION","TAG LOG",
            "ROTATION","WIRELESS"
        };

        // todo: We need to substitute words in this range with gifs, but only
        // when rendering to the web.
        // Maybe we can nicen their display in ToString() somehow too.
        private static string[] WORDS_ANIMATED = new string[]
        {
            "絵_GOOD DAY!","絵_HELLO!","絵_I LOVE IT!","絵_GOOD LUCK!", // 1732
            "絵_IT'S FUN!","絵_HAPPY!","絵_THANK YOU!","絵_SUPER! ♪",
            "絵_SORRY...","絵_BYE-BYE!"
        };

        private static string[] WORDS_VOICE = new string[]
        {
            "OH WELL","AAH","AHAHA","HUH?",                     // 1742
            "YEAH","URGH","LET ME THINK","HMM",
            "WHOA","ROOOAAR!","WOW","SNICKER",
            "CUTE LAUGH","UNBELIEVABLE","CRIES","OK",
            "AGREE","EH?","BOO-HOO","HEHEHE",
            "HEY","OH, YEAH","OH WOW!","HEEEY",
            "OOPS","OH MY","EEK","YAAAH",
            "GIGGLE","GWAHAHAHA","UGH","HEY!",
            "WEEP","TUT","BLUSH","HEY?",
            "HUH?","SIGH","HAHAHA","AIYEEH",
            "HIYAH","MUHAHAHA","LOL","SNORT",
            "HUMPH","HEY","HE-HE-HE","HEH",
            "HOHOHO","THERE YOU GO","OH, DEAR","ANGRY",
            "MUFUFU","MMM","OK","LALALA",
            "YAY","WAIL","WOW","BOO!",
            "WAHAHA"
        };

        // 1803+ is out of range.

        #endregion

    }
}
