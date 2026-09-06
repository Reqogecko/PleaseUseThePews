using PleaseUseThePews.RitualPatches;
using RimWorld;
using UnityEngine;
using Verse;

namespace PleaseUseThePews.Settings
{
    public class SettingsMod : Mod
    {
        public static SettingsConfig Settings { get; private set; }
        private string _searchFilter = string.Empty;


        public SettingsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<SettingsConfig>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            SettingsUi.DoWindowContents(inRect, ref _searchFilter);
        }
        
        public override string SettingsCategory() => "RES_PleaseUseThePews_ModTitle".Translate();


        [StaticConstructorOnStartup]
        public static class PleaseUseThePewsInit
        {
            static PleaseUseThePewsInit()
            {
                if (!ModLister.IdeologyInstalled) return;

                RitualScanner.Scan();

                if (!SettingsConfig.InitialSetupDone)
                {
                    SettingsPresets.ApplyPreset(SettingsPresets.ReqosPreset);
                    SettingsConfig.InitialSetupDone = true;
                }
            }
        }
    }
}