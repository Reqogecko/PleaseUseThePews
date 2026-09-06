using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PleaseUseThePews.RitualPatches
{
    public static class RitualScanner
    {
        public static readonly List<PreceptDef> AllValidRituals = new List<PreceptDef>();

        public static readonly Dictionary<ModContentPack, List<PreceptDef>> 
            ModRitualDictionary = new Dictionary<ModContentPack, List<PreceptDef>>();

        public static void Scan()
        {
            AllValidRituals.Clear();
            ModRitualDictionary.Clear();
            var allDefs = DefDatabase<PreceptDef>.AllDefsListForReading;
            for (int i = 0; i < allDefs.Count; i++)
            {
                var def = allDefs[i];
                var mod = def.modContentPack;
                
                if (def.preceptClass != typeof(Precept_Ritual) || def.ritualPatternBase == null) continue;
                
                AllValidRituals.Add(def);

                if (mod == null) continue;
                if (!ModRitualDictionary.TryGetValue(mod, out var list))
                {
                    list = new List<PreceptDef>();
                    ModRitualDictionary[mod] = list;
                }
                list.Add(def);
            }
        }

        public static bool IsUsingSeats(this RitualPatternDef def) =>
            def.ritualBehavior.stages[1].defaultDuty == DutyDefOf.Spectate;
    }
}