using System.Collections.Generic;
using Verse;
using PleaseUseThePews.Settings;
using RimWorld;
using Verse.AI;

namespace PleaseUseThePews.RitualPatches
{
    public static class RitualController
    {
        public static void RefreshAll()
        {
            var settings = SettingsMod.Settings;
            foreach (var ritual in RitualScanner.AllValidRituals)
            {
                bool active = !settings.ForcedUseSeatDefNames.Contains(ritual.ritualPatternBase.ritualBehavior.defName);
                if (active) ApplyForceSeat(ritual.ritualPatternBase.ritualBehavior);
            } 
        }
        
        public static void BatchUpdateAll(bool enable)
        {
            var settings = SettingsMod.Settings;
            settings.ForcedUseSeatDefNames.Clear();

            if (!enable) return;
            foreach (var ritual in RitualScanner.AllValidRituals)
                settings.ForcedUseSeatDefNames.Add(ritual.defName);
        }
        
        private static void ApplyForceSeat(RitualBehaviorDef def)
        {
            foreach (var stage in def.stages)
            {
                stage.defaultDuty = DutyDefOf.Spectate;
            }
        }
    }
}