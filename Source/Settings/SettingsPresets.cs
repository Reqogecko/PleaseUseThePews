using System;
using System.Linq;
using PleaseUseThePews.RitualPatches;
using RimWorld;
using Verse;

namespace PleaseUseThePews.Settings
{
    public static class SettingsPresets
    {
        public static string[] ReqosPreset = { "Damn Daniel","GladiatorDuel", "AM_SparringMatch" };
        
        public static void ApplyPreset(string[] preset)
        {
            var mods = RitualScanner.ModRitualDictionary
                .Select(kvp => (Mod: kvp.Key, Rituals: kvp.Value.ToList()))
                .Where(x => x.Rituals.Any()).ToList();
            
            foreach (var item in mods)
            {
                // For ritual Index
                for (int i = 0; i < item.Rituals.Count; i++)
                {
                    // For each Defname
                    for (int dn = 0; dn < preset.Length; dn++)
                    {
                        // If name matches the def's
                        if (preset[dn] == item.Rituals[i].defName)
                        {
                            // Disable force enabled
                            
                            SettingsMod.Settings.ForcedUseSeatDefNames.Add(item.Rituals[i].defName);
                            break;
                        }
                        //Else, enable force.
                        SettingsMod.Settings.ForcedUseSeatDefNames.Remove(item.Rituals[i].defName);
                    }
                }
            }
            
        RitualController.RefreshAll();
        }
    }
}