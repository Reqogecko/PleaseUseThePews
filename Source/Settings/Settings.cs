using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PleaseUseThePews
{
    public class Settings : ModSettings
    {
        public static float GatheringRange = 50f;
        public static float MaxDistanceToRitualSpot = GatheringRange;
        public static int GenRadialTilesToCheck = 3000;

        public static bool GladiatorRitualSpectate = true;
        
        static float previousGatheringRange = GatheringRange;
        static bool RestartPopupWasShown = false;
        
        public override void ExposeData()
        {
            Scribe_Values.Look(ref GatheringRange, "MaxTilesToSearchForSeat", 50f);
            Scribe_Values.Look(ref MaxDistanceToRitualSpot, "MaxTilesToRitualSpot", 50f);
            Scribe_Values.Look(ref GenRadialTilesToCheck, "TotalAmountOfTilesToCheckForSeats", 3000);
            Scribe_Values.Look(ref GladiatorRitualSpectate, "SpectateGladiatorToggle", true);
            base.ExposeData();
        }

        private static FeatureTab selectedTab = FeatureTab.Config;

        private enum FeatureTab
        {
            Config,
            Other,
        }
        
        public static void DoWindowContents(Rect inRect)
        {
            Rect tabRect = new Rect(inRect.x, inRect.y + 32f, inRect.width, 50f);
            Rect contentRect = new Rect(inRect.x, inRect.y + 70f, inRect.width, inRect.height - 70f);

            List<TabRecord> tabs = new List<TabRecord>
            {
                new TabRecord("RES_Config".Translate(), () => selectedTab = FeatureTab.Config, selectedTab == FeatureTab.Config),
                new TabRecord("RES_Other".Translate(), () => selectedTab = FeatureTab.Other, selectedTab == FeatureTab.Other),
            };
            TabDrawer.DrawTabs(tabRect, tabs);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);
            listing.Gap(1f);

            switch (selectedTab)
            {
                case FeatureTab.Config:
                    Text.Font = GameFont.Medium;
                    listing.Label("RES_FeatureTabConfig_Label".Translate());
                    Text.Font = GameFont.Small;
                    listing.GapLine();
                    
                    if (RestartPopupWasShown == false)
                    {
                        RestartPopupWasShown = true;
                        Find.WindowStack.Add(new Dialog_MessageBox("GatherRound_RestartRequired".Translate()));
                    }    
                    
                    // Gather Range
                    GatheringRange = listing.SliderLabeled(
                        "RES_GatheringRange_Label".Translate(GatheringRange.ToString("N0")),
                        GatheringRange,
                        0f,
                        500f,
                        tooltip: "RES_GatheringRange_Tooltip".Translate()
                    );
                    
                    // Add reset button
                    Rect GatheringRange_lastRect = listing.GetRect(24f);
                    Rect GatheringRange_buttonRect = new Rect(GatheringRange_lastRect.x + 12f, GatheringRange_lastRect.y, 80f, 34f);
                    if (Widgets.ButtonText(GatheringRange_buttonRect, "RES_Reset".Translate()))
                    {
                        GatheringRange = 50f;
                    }
                    
                    listing.Gap(6f);
                    listing.Gap(6f);
                    listing.Gap(6f);
                    
                    listing.Label("RES_FeatureTabConfigAdvanced_Label".Translate());
                    Text.Font = GameFont.Small;
                    listing.Label("RES_FeatureTabConfigAdvanced2_Label".Translate());
                    listing.GapLine();
                    listing.Gap(4f);
                    
                    //Radial tiles to check
                    GenRadialTilesToCheck = (int)listing.SliderLabeled(
                        "RES_GenRadialTilesToCheck_Label".Translate(GenRadialTilesToCheck.ToString("N0")),
                        GenRadialTilesToCheck,
                        0,
                        10000,
                        tooltip: "RES_GenRadialTilesToCheck_Tooltip".Translate()
                    );

                    listing.Gap(2f);
                    
                    // Add reset button
                    Rect GenRadialTilesToCheck_lastRect = listing.GetRect(24f);
                    Rect GenRadialTilesToCheck_buttonRect = new Rect(GenRadialTilesToCheck_lastRect.x + 12f, GenRadialTilesToCheck_lastRect.y, 80f, 34f);
                    if (Widgets.ButtonText(GenRadialTilesToCheck_buttonRect, "RES_Reset".Translate()))
                    {
                        GenRadialTilesToCheck = 3000;
                    }
                    break;
                
                case FeatureTab.Other:
                    Text.Font = GameFont.Medium;
                    listing.Label("RES_OtherTab_Label".Translate());
                    Text.Font = GameFont.Small;
                    listing.GapLine();

                   // listing.CheckboxLabeled
                   //   (
                 //       "RES_GladiatorRitualCheckbox_Label".Translate(),
                   //     ref GladiatorRitualSpectate,
                    //    tooltip: "RES_GladiatorRitualCheckbox_Tooltip".Translate()
                    //);
                    
                    break;
            }

            listing.End();
        }
    }
}
