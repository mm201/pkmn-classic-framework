using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PkmnFoundations.Data;
using System.Data.SQLite;
using System.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;
using System.Threading;
using System.Globalization;
using System.IO;

namespace VeekunImport
{
    class Program
    {
        const int GENERATIONS = 6;

        static void Main(string[] args)
        {
            string veekunFilename;
            ConnectionStringSettings css;

            if (args.Length < 1) veekunFilename = "pokedex.sqlite";
            else veekunFilename = args[0];

            if (veekunFilename.Contains(';')) throw new NotSupportedException("The character ; in filenames is not supported.");
            if (veekunFilename.Contains('?'))
            {
                Console.WriteLine("Usage: VeekunImport [filename [connectionString providerName]]");
                Console.WriteLine("filename: Filename of Veekun sqlite database. Default: pokedex.sqlite");
                Console.WriteLine("connectionString: pkmnFoundations connection string. Default: from app.config");
                Console.WriteLine("providerName: .NET classname of connection provider. Default: from app.config");
                return;
            }

            if (args.Length < 3)
                css = ConfigurationManager.ConnectionStrings["pkmnFoundationsConnectionString"];
            else
                css = new ConnectionStringSettings("", args[1], args[2]);

            Database db = Database.CreateInstance(css);

            using (SQLiteConnection connVeekun = new SQLiteConnection("Data Source=" + veekunFilename + "; Version=3"))
            {
                connVeekun.Open();

                // General logic:
                // 1. run a query against veekun, populate a DataTable.
                // 2. foreach DataRow, instance a class from the PkmnFoundations.Pokedex namespace.
                // 3. call Database.AddToPokedex on that object.

                // todo: We need a way to rebuild specific tables

                // pkmncf_pokedex_pokemon_families
                // Obtain the families map. We will use it in a few places.
                Dictionary<int, int> familyMap = new Dictionary<int, int>();
                List<int[]> familyList = new List<int[]>();
                ProcessTSV("families_map.txt", 1, row =>
                    {
                        // todo: allow for comments by stopping consumption of fields once one fails to parse.
                        int[] fieldsInt = row.Fields.Select(s => Convert.ToInt32(s)).ToArray();
                        familyList.Add(fieldsInt);
                        foreach (int x in fieldsInt)
                        {
                            // lineNumber is one-based, unlike familyList index.
                            // species is the key, family ID is the value.
                            // If any species is repeated in family_map.txt, this ought to fail.
                            familyMap.Add(x, row.ValidLineNumber + 1);
                        }
                    });

                // families from families.txt override default behaviour.
                List<Family> overrideFamilies = new List<Family>();
                ProcessTSV("families.txt", 7, row =>
                    {
                        string[] fields = row.Fields;
                        overrideFamilies.Add(new Family(null,
                            Convert.ToInt32(fields[0]),
                            Convert.ToInt32(fields[1]),
                            Convert.ToInt32(fields[2]),
                            Convert.ToInt32(fields[3]),
                            Convert.ToInt32(fields[4]),
                            Convert.ToInt32(fields[5]),
                            Convert.ToByte(fields[6])
                            ));
                    });
                // already sorted
                //someFamilies.Sort((f, other) => f.ID.CompareTo(other.ID));

                int familyCount = familyList.Count;
                int nextOverrideIndex = 0;
                for (int familyId = 1; familyId <= familyCount; familyId++)
                {
                    int basicSpeciesId = familyList[familyId - 1][0];
                    Family f;
                    if (nextOverrideIndex < overrideFamilies.Count && overrideFamilies[nextOverrideIndex].ID == familyId)
                    {
                        // An override exists in the table; use it.
                        f = overrideFamilies[nextOverrideIndex];
                        nextOverrideIndex++;
                    }
                    else
                    {
                        // No override exists so go with default:
                        // Basic male/female are the same species which is the one listed first.
                        // There is no baby species nor incense
                        // (Non-incense babies are considered basic in this system anyway.)
                        // Gender ratio comes from the basic species.
                        byte genderRatio = (byte)Convert.ToInt32(connVeekun.ExecuteScalar("SELECT gender_rate FROM pokemon_species WHERE id = @id", new SQLiteParameter("@id", basicSpeciesId)));
                        f = new Family(null, familyId, basicSpeciesId, basicSpeciesId, 0, 0, 0, genderRatio);
                    }
                    db.PokedexInsertFamily(f);
                    string basic = (f.BasicMaleID != f.BasicFemaleID) ?
                        String.Format(" {0}/{1}", f.BasicMaleID, f.BasicFemaleID) :
                        String.Format(" {0}", f.BasicMaleID);
                    string baby;
                    if (f.BabyMaleID != f.BabyFemaleID)
                        baby = String.Format(" {0}/{1} incense {2}", f.BabyMaleID, f.BabyFemaleID, f.IncenseID);
                    else
                        baby = (f.BabyMaleID == 0) ? "" :
                            String.Format(" {0} incense {1}", f.BabyMaleID, f.IncenseID);
                    string gender = (f.GenderRatio == 255) ? "genderless" :
                        String.Format("{0}% female", (float)f.GenderRatio * 12.5f);
                    Console.WriteLine("Inserted family {0}{1}{2} {3}", f.ID, basic, baby, gender);
                }

                // pkmncf_pokedex_pokemon
                SQLiteDataReader rdPokemon = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT " +
                    "pokemon_species.id, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT pokemon_species_names.name FROM pokemon_species_names WHERE pokemon_species_names.pokemon_species_id = pokemon_species.id AND local_language_id = 3) AS name_ko, " +
                    "growth_rate_id, gender_rate, " +
                    "(SELECT egg_group_id FROM pokemon_egg_groups WHERE species_id = pokemon_species.id ORDER BY egg_group_id LIMIT 1) AS egg_group_1, " +
                    "(SELECT egg_group_id FROM pokemon_egg_groups WHERE species_id = pokemon_species.id ORDER BY egg_group_id LIMIT 1, 1) AS egg_group_2, " +
                    "hatch_counter, has_gender_differences " +
                    "FROM pokemon_species " +
                    "ORDER BY pokemon_species.id");

                while (rdPokemon.Read())
                {
                    int id = Convert.ToInt32(rdPokemon["id"]);
                    byte growth_rate_id = Convert.ToByte(rdPokemon["growth_rate_id"]);
                    int gender_rate = Convert.ToInt32(rdPokemon["gender_rate"]);
                    byte egg_group_1 = Convert.ToByte(rdPokemon["egg_group_1"]);
                    byte egg_group_2 = 0;
                    if (!(rdPokemon["egg_group_2"] is DBNull)) egg_group_2 = Convert.ToByte(rdPokemon["egg_group_2"]);
                    int hatch_counter = Convert.ToInt32(rdPokemon["hatch_counter"]);
                    byte has_gender_differences = Convert.ToByte(rdPokemon["has_gender_differences"]);

                    if (id == 255) has_gender_differences = 0; // Torchic doesn't have gender differences.

                    // todo: Family ID
                    Species s = new Species(null, id,
                        familyMap[id],
                        GetLocalizedString(rdPokemon, "name_"),
                        (GrowthRates)growth_rate_id,
                        (byte)gender_rate,
                        (EggGroups)egg_group_1,
                        (EggGroups)egg_group_2,
                        (byte)hatch_counter,
                        has_gender_differences != 0
                        );

                    db.PokedexInsertSpecies(s);
                    Console.WriteLine("Inserted {0} {1} {2} {3} {4} {5}",
                        s.NationalDex, s.Name.ToString(), s.GrowthRate, s.GenderRatio, s.EggSteps, s.GenderVariations);
                }
                rdPokemon.Close();

                // pkmncf_pokedex_pokemon_forms
                Dictionary<int, byte> formValueMap = new Dictionary<int, byte>();
                ProcessTSV("form_values.txt", 3, row =>
                    {
                        string[] fields = row.Fields;
                        formValueMap.Add(Convert.ToInt32(fields[1]), Convert.ToByte(fields[2]));
                    });

                SQLiteDataReader rdForms = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, " +
                    "(SELECT species_id FROM pokemon WHERE id = pokemon_id) AS NationalDex, " +
                    "form_order - 1 AS FormValue, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT pokemon_form_names.form_name FROM pokemon_form_names WHERE pokemon_form_names.pokemon_form_id = pokemon_forms.id AND local_language_id = 3) AS name_ko, " +
                    "form_identifier, " +
                    "(SELECT height FROM pokemon WHERE id = pokemon_id) AS height, " +
                    "(SELECT weight FROM pokemon WHERE id = pokemon_id) AS weight, " +
                    "(SELECT base_experience FROM pokemon WHERE id = pokemon_id) AS experience " +
                    "FROM pokemon_forms");

                while (rdForms.Read())
                {
                    int id = Convert.ToInt32(rdForms["id"]);
                    int NationalDex = Convert.ToInt32(rdForms["NationalDex"]);
                    byte FormValue = Convert.ToByte(rdForms["FormValue"]);
                    string form_identifier = rdForms["form_identifier"].ToString();
                    int height = Convert.ToInt32(rdForms["height"]);
                    int weight = Convert.ToInt32(rdForms["weight"]);
                    int experience = Convert.ToInt32(rdForms["experience"]);

                    if (formValueMap.ContainsKey(id)) FormValue = formValueMap[id];

                    Form f = new Form(null, id,
                        NationalDex, FormValue,
                        GetLocalizedString(rdForms, "name_"),
                        form_identifier,
                        height,
                        weight,
                        experience
                        );

                    db.PokedexInsertForm(f);
                    Console.WriteLine("Inserted {0} {1} {2} {3} {4} {5:0.0}m {6:0.0}kg {7}",
                        f.ID, f.SpeciesID, f.Value, f.Name.ToString(),
                        f.Suffix, f.Height * 0.1f, f.Weight * 0.1f, f.Experience);
                }
                rdForms.Close();

                // pkmncf_pokedex_pokemon_form_stats
                for (int generation = 1; generation <= GENERATIONS; generation++)
                {
                    string filename = String.Format("form_stats{0}.txt", generation);
                    ProcessTSV(filename, 15, row =>
                        {
                            int[] fieldsInt = row.Fields.Select(s => Convert.ToInt32(s)).ToArray();
                            FormStats f = new FormStats(null, fieldsInt[0], (Generations)generation, fieldsInt[1], fieldsInt[2],
                                new IntStatValues(fieldsInt[3],
                                    fieldsInt[4],
                                    fieldsInt[5],
                                    fieldsInt[6],
                                    fieldsInt[7],
                                    fieldsInt[8]),
                                new ByteStatValues((byte)fieldsInt[9],
                                    (byte)fieldsInt[10],
                                    (byte)fieldsInt[11],
                                    (byte)fieldsInt[12],
                                    (byte)fieldsInt[13],
                                    (byte)fieldsInt[14])
                                );

                            db.PokedexInsertFormStats(f);
                            Console.WriteLine("Inserted stats for {0} gen {1}", f.FormID, (int)f.MinGeneration);
                        });
                }

                // todo: pkmncf_pokedex_pokemon_evolutions

                // pkmncf_pokedex_types
                SQLiteDataReader rdTypes = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, damage_class_id, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT name FROM type_names WHERE type_names.type_id = types.id AND local_language_id = 3) AS name_ko " +
                    "FROM types ORDER BY id");
                // http://stackoverflow.com/questions/27156585/failed-to-enable-constraints-without-dataadapter

                while (rdTypes.Read())
                {
                    int id = Convert.ToInt32(rdTypes["id"]);

                    PkmnFoundations.Pokedex.Type t = new PkmnFoundations.Pokedex.Type(null,
                        id,
                        GetLocalizedString(rdTypes, "name_"),
                        GetDamageClass(rdTypes));

                    db.PokedexInsertType(t);
                    Console.WriteLine("Inserted type {0} {1} {2}", t.ID, t.Name.ToString(), t.DamageClass);
                }
                rdTypes.Close();

                // pkmncf_pokedex_moves
                SQLiteDataReader rdMoves = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, type_id, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT name FROM move_names WHERE move_names.move_id = moves.id AND local_language_id = 3) AS name_ko, " +
                    "damage_class_id, power, pp, accuracy, priority, target_id FROM moves");

                while (rdMoves.Read())
                {
                    int id = Convert.ToInt32(rdMoves["id"]);
                    int type_id = Convert.ToInt32(rdMoves["type_id"]);
                    int damage_class_id = Convert.ToInt32(rdMoves["damage_class_id"]);
                    short power = DatabaseExtender.Cast<short ?>(rdMoves["power"]) ?? 0;
                    short pp = DatabaseExtender.Cast<short?>(rdMoves["pp"]) ?? 0;
                    short accuracy = DatabaseExtender.Cast<short?>(rdMoves["accuracy"]) ?? 0;
                    short priority = Convert.ToInt16(rdMoves["priority"]);
                    int target_id = Convert.ToInt32(rdMoves["target_id"]);

                    Move m = new Move(null,
                        id,
                        type_id,
                        GetLocalizedString(rdMoves, "name_"),
                        (DamageClass)damage_class_id,
                        power,
                        pp,
                        accuracy,
                        priority,
                        (BattleTargets)target_id
                        );

                    db.PokedexInsertMove(m);
                    Console.WriteLine("Inserted move {0} {1}", m.ID, m.Name.ToString());
                }

                // pkmncf_pokedex_abilities
                SQLiteDataReader rdAbilities = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT name FROM ability_names WHERE ability_names.ability_id = abilities.id AND local_language_id = 3) AS name_ko " +
                    "FROM abilities WHERE is_main_series = 1");

                while (rdAbilities.Read())
                {
                    int id = Convert.ToInt32(rdAbilities["id"]);
                    Ability a = new Ability(null,
                        id, GetLocalizedString(rdAbilities, "name_")
                        );

                    db.PokedexInsertAbility(a);
                    Console.WriteLine("Inserted ability {0} {1}", a.Value, a.Name.ToString());
                }
                rdAbilities.Close();

                // pkmncf_pokedex_items
                Dictionary<int, ItemLoading> items = new Dictionary<int, ItemLoading>();

                for (int generation = 3; generation <= GENERATIONS; generation++)
                {
                    string filename = String.Format("items{0}.txt", generation);
                    ProcessTSV(filename, 3, row =>
                        {
                            string[] fields = row.Fields;
                            int id, thisGenId;
                            string name = fields[1];
                            if (!Int32.TryParse(fields[0], out thisGenId) ||
                                !Int32.TryParse(fields[2], out id))
                            {
                                Console.WriteLine("{0} line {1} has bad format, skipped.", filename, row.LineNumber);
                                return;
                            }
                            ItemLoading theItem = null;
                            if (!items.ContainsKey(id))
                            {
                                theItem = new ItemLoading(id, name);
                                items.Add(id, theItem);
                            }
                            theItem = theItem ?? items[id];
                            theItem.Name = name; // prefer newer names where available
                            theItem.SetGenerationValue(thisGenId, generation);
                        });
                }

                // lookup Veekun ID number against some generation or another.
                Dictionary<int, ItemLoading> items3 = items
                    .Where(i => i.Value.Value3 != null && i.Value.NameLocalized == null)
                    .ToDictionary(i => (int)i.Value.Value3, i => i.Value);
                Dictionary<int, ItemLoading> items4 = items
                    .Where(i => i.Value.Value4 != null && i.Value.NameLocalized == null)
                    .ToDictionary(i => (int)i.Value.Value4, i => i.Value);
                Dictionary<int, ItemLoading> items5 = items
                    .Where(i => i.Value.Value5 != null && i.Value.NameLocalized == null)
                    .ToDictionary(i => (int)i.Value.Value5, i => i.Value);
                Dictionary<int, ItemLoading> items6 = items
                    .Where(i => i.Value.Value6 != null && i.Value.NameLocalized == null)
                    .ToDictionary(i => (int)i.Value.Value6, i => i.Value);

                // veekun has incorrect Gen3 indices for Helix/Dome fossil.
                // (Or they disagree with Bulbapedia which is my primary source)
                SQLiteDataReader rdItems = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, cost, " +
                    "(SELECT game_index FROM item_game_indices WHERE item_id = items.id AND generation_id = 3 AND NOT item_id IN (101, 102)) AS value3, " +
                    "(SELECT game_index FROM item_game_indices WHERE item_id = items.id AND generation_id = 4) AS value4, " +
                    "(SELECT game_index FROM item_game_indices WHERE item_id = items.id AND generation_id = 5) AS value5, " +
                    "(SELECT game_index FROM item_game_indices WHERE item_id = items.id AND generation_id = 6) AS value6, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT name FROM item_names WHERE item_names.item_id = items.id AND local_language_id = 3) AS name_ko " +
                    "FROM items");

                Dictionary<int, ItemLoading>[] itemsGeneration = new Dictionary<int, ItemLoading>[] { null, null, items3, items4, items5, items6 };

                while (rdItems.Read())
                {
                    List<ItemLoading> toProcess = new List<ItemLoading>(4);

                    for (int generation = 3; generation <= GENERATIONS; generation++)
                    {
                        string col = String.Format("value{0}", generation);
                        if (!(rdItems[col] is DBNull))
                        {
                            Dictionary<int, ItemLoading> dict = itemsGeneration[generation - 1];
                            int value = Convert.ToInt32(rdItems[col]);
                            if (dict.ContainsKey(value))
                            {
                                ItemLoading il = dict[value];
                                il.NameLocalized = GetLocalizedString(rdItems, "name_");
                                il.Price = Convert.ToInt32(rdItems["cost"]);
                            }
                        }
                    }
                }
                rdItems.Close();

                foreach (ItemLoading il in items.Values)
                {
                    LocalizedString name = il.NameLocalized;
                    if (name == null)
                    {
                        name = new LocalizedString() { { "EN", il.Name } };
                        Console.WriteLine("Veekun database missing item {0} {1}. Non-English translations will be missing.", il.ID, name);
                    }
                    Item i = new Item(null, il.ID, il.Value3, il.Value4, il.Value5, il.Value6, il.Price, name);
                    db.PokedexInsertItem(i);
                    Console.WriteLine("Inserted item {0} {1}", i.ID, i.Name.ToString());
                }

                // pkmncf_pokedex_ribbons
                Dictionary<int, RibbonLoading> ribbons = new Dictionary<int, RibbonLoading>();
                if (!ProcessTSV("ribbons.txt", 3, row =>
                    {
                        string[] fields = row.Fields;
                        int id = Convert.ToInt32(fields[0]);
                        ribbons.Add(id, new RibbonLoading(id, fields[1], fields[2]));
                    }))
                    Console.WriteLine("ribbons.txt not found. Not inserting any ribbons.");
                else
                {
                    for (int generation = 3; generation <= GENERATIONS; generation++)
                    {
                        string filename = String.Format("ribbon_positions{0}.txt", generation);
                        ProcessTSV(filename, 2, row =>
                            {
                                string[] fields = row.Fields;
                                int id = Convert.ToInt32(fields[0]);
                                int position = Convert.ToInt32(fields[1]);
                                if (!ribbons.ContainsKey(id))
                                {
                                    Console.WriteLine("Error: Ribbon ID {0} contained in {1} but not in ribbons.txt!", id, filename);
                                    return;
                                }
                                RibbonLoading r = ribbons[id];
                                r.SetGenerationPosition(position, generation);
                            });
                    }

                    foreach (KeyValuePair<int, RibbonLoading> pair in ribbons)
                    {
                        RibbonLoading r = pair.Value;
                        db.PokedexInsertRibbon(new Ribbon(null, r.ID,
                            new LocalizedString() { { "EN", r.Name } },
                            new LocalizedString() { { "EN", r.Description } },
                            r.Position3, r.Position4, r.Position5, r.Position6,
                            null, null, null, null
                            ));
                        Console.WriteLine("Inserted ribbon {0} {1}", r.ID, r.Name);
                    }
                }

                // pkmncf_pokedex_regions
                ProcessTSV("regions.txt", 2, row =>
                {
                    string[] fields = row.Fields;
                    int id = Convert.ToInt32(fields[0]);
                    Region r = new Region(null, id, new LocalizedString() { { "EN", fields[1] } });
                    db.PokedexInsertRegion(r);
                    Console.WriteLine("Inserted region {0} {1}", r.ID, r.Name);
                });

                // pkmncf_pokedex_locations
                SQLiteDataReader rdLocations = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, region_id, " +
                    "(SELECT game_index FROM location_game_indices WHERE location_id = locations.id AND generation_id = 3) AS value3, " +
                    "(SELECT game_index FROM location_game_indices WHERE location_id = locations.id AND generation_id = 4) AS value4, " +
                    "(SELECT game_index FROM location_game_indices WHERE location_id = locations.id AND generation_id = 5) AS value5, " +
                    "(SELECT game_index FROM location_game_indices WHERE location_id = locations.id AND generation_id = 6) AS value6, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 1) AS name_ja, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 9) AS name_en, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 5) AS name_fr, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 8) AS name_it, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 6) AS name_de, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 7) AS name_es, " +
                    "(SELECT name FROM location_names WHERE location_names.location_id = locations.id AND local_language_id = 3) AS name_ko " +
                    "FROM locations WHERE id NOT IN (522, 523, 524)");
                // 522, 523, and 524 are duplicates of Kanto routes 19, 20, and 21.

                // todo: gba/colo/xd encounter location values
                
                while (rdLocations.Read())
                {
                    Location l = new Location(null, 
                        Convert.ToInt32(rdLocations["id"]),
                        VeekunLocationToRegion(rdLocations),
                        (int ?)DatabaseExtender.Cast<long?>(rdLocations["value3"]),
                        null,
                        null,
                        (int?)DatabaseExtender.Cast<long?>(rdLocations["value4"]),
                        (int?)DatabaseExtender.Cast<long?>(rdLocations["value5"]),
                        (int?)DatabaseExtender.Cast<long?>(rdLocations["value6"]),
                        GetLocalizedString(rdLocations, "name_")
                        );

                    db.PokedexInsertLocation(l);
                    Console.WriteLine("Inserted location {0} {1}", l.ID, l.Name);
                }
                rdLocations.Close();

                connVeekun.Close();
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static string[] LANGS = { "JA", "EN", "FR", "IT", "DE", "ES", "KO" };
        private static LocalizedString GetLocalizedString(IDataReader reader, string column)
        {
            LocalizedString result = new LocalizedString();
            foreach (string lang in LANGS)
            {
                string col = column + lang.ToLowerInvariant();
                if (reader[col] is DBNull) continue;
                result.Add(lang, (string)reader[col]);
            }
            return result;
        }

        private static DamageClass GetDamageClass(IDataReader reader)
        {
            if (reader["damage_class_id"] is DBNull) return DamageClass.None;
            return (DamageClass)(Convert.ToInt32(reader["damage_class_id"]) - 1);
        }

        private static bool ProcessTSV(string filename, int requiredFields, Action<TsvRow> action)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("File {0} not found, skipped.", filename);
                return false;
            }
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string line;
                int lineNumber = 0;
                int validLineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = line.Split('\t');
                    if (fields.Length < requiredFields)
                    {
                        Console.WriteLine("File {0} line {1} has too few fields, skipped.", filename, lineNumber);
                        lineNumber++;
                        continue;
                    }
                    action(new TsvRow(lineNumber, validLineNumber, fields));
                    lineNumber++;
                    validLineNumber++;
                }
                fs.Close();
            }
            return true;
        }

        private static int VeekunLocationToRegion(SQLiteDataReader reader)
        {
            int id = Convert.ToInt32(reader["id"]);
            long? region = DatabaseExtender.Cast<long?>(reader["region_id"]);

            if (region == null) // not set, usually event
            {
                if (id == 256) return 2; // kanto
                if (id == 257) return 3; // johto
                if (id == 258) return 4; // hoenn
                if (id == 259) return 7; // sinnoh
                if (id == 260) return 5; // "distant land" (only used for orre?)
                if (id == 261) return 7; // traveling man (happini?)
                if (id == 262) return 7; // riley
                if (id == 263) return 7; // cynthia
                if (id == 342) return 3; // mr. pokemon
                if (id == 343) return 3; // primo
                return 1;
            }
            if (region == 1) // kanto and seviis
            {
                if (id >= 491 && id <= 497) return 6; // seven island unown chambers
                if (id >= 500 && id <= 521) return 6; // sevii islands
                if (id >= 526 && id <= 529) return 6; // sevii islands
                return 2;
            }
            if (region == 2) return 3; // johto
            if (region == 3) return 4; // hoenn
            if (region == 4) return 7; // sinnoh
            if (region == 5) return 8; // unova
            if (region == 6) return 9; // kalos

            return (int)region + 3; // for now, assume veekun won't skip more regions.
        }

    }

    internal class ItemLoading
    {
        public ItemLoading(int id, string name)
        {
            ID = id;
            Name = name;
            Value3 = Value4 = Value5 = Value6 = null;
            NameLocalized = null;
        }

        public int ID;
        public string Name;
        public LocalizedString NameLocalized;
        public int? Value3, Value4, Value5, Value6;
        public int Price;

        public void SetGenerationValue(int ? value, int generation)
        {
            switch (generation)
            {
                case 3:
                    Value3 = value;
                    break;
                case 4:
                    Value4 = value;
                    break;
                case 5:
                    Value5 = value;
                    break;
                case 6:
                    Value6 = value;
                    break;
            }
        }
    }

    internal class RibbonLoading
    {
        public RibbonLoading(int id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
            Position3 = Position4 = Position5 = Position6 = null;
        }

        public int ID;
        public string Name;
        public string Description;
        public int? Position3, Position4, Position5, Position6;

        public void SetGenerationPosition(int? position, int generation)
        {
            switch (generation)
            {
                case 3:
                    Position3 = position;
                    break;
                case 4:
                    Position4 = position;
                    break;
                case 5:
                    Position5 = position;
                    break;
                case 6:
                    Position6 = position;
                    break;
            }
        }
    }

    internal class TsvRow
    {
        public TsvRow(int lineNumber, int validLineNumber, string[] fields)
        {
            LineNumber = lineNumber;
            ValidLineNumber = validLineNumber;
            Fields = fields;
        }

        public int LineNumber;
        public int ValidLineNumber;
        public string[] Fields;
    }
}
