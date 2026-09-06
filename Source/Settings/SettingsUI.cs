using System.Collections.Generic;
using System.Drawing;
using PleaseUseThePews.RitualPatches;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System.Reflection;
using Color = UnityEngine.Color;

namespace PleaseUseThePews.Settings
{
    public static class SettingsUi
    {
        public static float gatheringRange = SettingsConfig.GatheringRange;
        public static int radialTilesToCheck = SettingsConfig.GenRadialTilesToCheck;

        private static Vector2 _scrollPos = Vector2.zero;
        private static readonly Dictionary<string, Vector2> _hScrollPos = new Dictionary<string, Vector2>();

        private static readonly Color SlateHeader = new Color(0.15f, 0.15f, 0.15f);
        private static readonly Color DarkCard = new Color(0.18f, 0.21f, 0.25f, 0.4f);
        private static readonly Color LockoutTint = new Color(1f, 1f, 1f, 0.35f);

        private static FeatureTab selectedTab = FeatureTab.Config;

        private enum FeatureTab
        {
            Config,
            RitualsTab,
        }

        public static void DoWindowContents(Rect inRect, ref string search)
        {
            // vars

            if (RitualScanner.AllValidRituals.Count == 0)
            {
                RitualScanner.Scan();
            }

            var settings = SettingsMod.Settings;

            Rect tabRect = new Rect(inRect.x, inRect.y + 32f, inRect.width, 0f);
            Rect contentRect = new Rect(inRect.x, inRect.y + 40f, inRect.width, inRect.height + 70f);

            List<TabRecord> tabs = new List<TabRecord>
            {
                new TabRecord("RES_Config".Translate(), () => selectedTab = FeatureTab.Config,
                    selectedTab == FeatureTab.Config),
                new TabRecord("RES_Other".Translate(), () => selectedTab = FeatureTab.RitualsTab,
                    selectedTab == FeatureTab.RitualsTab),
            };
            TabDrawer.DrawTabs(tabRect, tabs);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);

            switch (selectedTab)
            {
                case FeatureTab.Config:
                    Text.Font = GameFont.Medium;
                    listing.Label("RES_FeatureTabConfig_Label".Translate());
                    Text.Font = GameFont.Small;
                    listing.GapLine();

                    if (SettingsConfig.RestartPopupWasShown == false)
                    {
                        SettingsConfig.RestartPopupWasShown = true;
                        Find.WindowStack.Add(new Dialog_MessageBox("GatherRound_RestartRequired".Translate()));
                    }

                    // Gather Range
                    gatheringRange = listing.SliderLabeled(
                        "RES_GatheringRange_Label".Translate(gatheringRange.ToString("N0")),
                        gatheringRange,
                        0f,
                        500f,
                        tooltip: "RES_GatheringRange_Tooltip".Translate()
                    );

                    // Add reset button
                    Rect GatheringRange_lastRect = listing.GetRect(24f);
                    Rect GatheringRange_buttonRect = new Rect(GatheringRange_lastRect.x + 12f,
                        GatheringRange_lastRect.y, 80f, 34f);
                    if (Widgets.ButtonText(GatheringRange_buttonRect, "RES_Reset".Translate()))
                    {
                        gatheringRange = 50f;
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
                    radialTilesToCheck = (int)listing.SliderLabeled(
                        "RES_GenRadialTilesToCheck_Label".Translate(radialTilesToCheck.ToString("N0")),
                        radialTilesToCheck,
                        0,
                        10000,
                        tooltip: "RES_GenRadialTilesToCheck_Tooltip".Translate()
                    );

                    listing.Gap(2f);

                    // Add reset button
                    Rect GenRadialTilesToCheck_lastRect = listing.GetRect(24f);
                    Rect GenRadialTilesToCheck_buttonRect = new Rect(GenRadialTilesToCheck_lastRect.x + 12f,
                        GenRadialTilesToCheck_lastRect.y, 80f, 34f);
                    if (Widgets.ButtonText(GenRadialTilesToCheck_buttonRect, "RES_Reset".Translate()))
                    {
                        radialTilesToCheck = 3000;
                    }

                    break;

                // SECOND TAB

                case FeatureTab.RitualsTab:
                    Text.Font = GameFont.Medium;
                    listing.Label("RES_RitualTab_Label".Translate());
                    Text.Font = GameFont.Small;
                    
                    if (RitualScanner.AllValidRituals.Count == 0) RitualScanner.Scan();
                    
                    listing.Label("RES_RitualTabExplanation_Label".Translate());
                    
                    Text.Font = GameFont.Tiny;
                    listing.Label("RES_RitualTabExplanationSearch_Label".Translate());
                    
                    Text.Font = GameFont.Small;
                    search = listing.TextEntry(search);
                    
                    listing.GapLine();
                    
                    Rect mainArea = inRect.BottomPart(0.78f);
                    
                    DrawModList(mainArea, search);

                    GUI.enabled = true;
                    GUI.color = Color.white;
                    
                    static void DrawModList(Rect rect, string filter)
                    {
                        var mods = RitualScanner.ModRitualDictionary
                            .Select(kvp => (Mod: kvp.Key, Rituals: kvp.Value.Where(s => kvp.Key.Name.Contains(filter) || s.label.Contains(filter)).ToList()))
                            .Where(x => x.Rituals.Any()).ToList();

                        Rect viewRect = new Rect(0, 0, rect.width - 20f, mods.Count() * 280f + 30f);
                        Widgets.BeginScrollView(rect, ref _scrollPos, viewRect);

                        float curY = -55f;
                        float cardWidth = viewRect.width * 0.93f;
                        float xPos = (viewRect.width - cardWidth) / 2f;

                        foreach (var item in mods)
                        {
                            Rect headerRect = new Rect(xPos - 20f, curY + 70f, cardWidth, 32f);
                            Widgets.DrawBoxSolid(headerRect, SlateHeader);
                            Widgets.DrawBox(headerRect, 1);

                            Text.Font = GameFont.Medium;
                            Widgets.Label(headerRect.ContractedBy(7f, 2f), item.Mod.Name);
                            Text.Font = GameFont.Small;

                            if (item.Rituals.Count >= 1)
                            {
                                // Toggle All Label
                                if (Widgets.ButtonText(new Rect(headerRect.xMax - 105f, curY + 69f, 120, 36f),
                                        "RES_EnableAllForceSeats_Label".Translate()))
                                {
                                    bool toggleOn = !item.Rituals.Any(s =>
                                        SettingsMod.Settings.ForcedUseSeatDefNames.Contains(s.defName));
                                    foreach (var s in item.Rituals)
                                    {
                                        if (toggleOn) SettingsMod.Settings.ForcedUseSeatDefNames.Add(s.defName);
                                        else SettingsMod.Settings.ForcedUseSeatDefNames.Remove(s.defName);
                                    }

                                    RitualController.RefreshAll();
                                }
                            }

                            curY += 120;

                            // Toggle rituals scroll area
                            Rect scrollArea = new Rect(xPos, curY, cardWidth, 190f);
                            Widgets.DrawBoxSolid(scrollArea, DarkCard);

                            if (!_hScrollPos.TryGetValue(item.Mod.PackageId, out var hPos)) hPos = Vector2.zero;

                            Rect hView = new Rect(0, 0, 80f,(item.Rituals.Count / 6) * 2 * 95f);
                            Widgets.BeginScrollView(scrollArea.ContractedBy(4f), ref hPos, hView);
                            
                            int currentColumn = 0;
                            int ritualIndex = 0;
                            
                            ModContentPack ritualMod = null;
                            
                            for (int i = 0; i < item.Rituals.Count; i++)
                            {
                                // Collumns
                                if (item.Rituals[i].modContentPack != ritualMod)
                                {
                                    ritualMod = item.Rituals[i].modContentPack;
                                    currentColumn = 0;
                                    ritualIndex = 0;
                                }
                                
                                if (ritualIndex >= 6)
                                {
                                    currentColumn += 1;
                                    ritualIndex = 0;
                                }
                                
                                DrawIconCell(new Rect(ritualIndex * 126f, 0 + (125f * currentColumn), 130f, 130f), item.Rituals[i]);
                                
                                ritualIndex += 1;
                            }

                            Widgets.EndScrollView();
                            _hScrollPos[item.Mod.PackageId] = hPos;

                            // Separate from next mod
                            curY += 155f;
                        }
                        
                        Widgets.EndScrollView();
                        
                    }
                    
                    listing.Gap(2f);
                    
                    break;
            }
            
            listing.End();
        }
        
        private static void DrawIconCell(Rect rect, PreceptDef ritual)
        {
            // Ritual Icon
            Texture2D icon = ritual.Icon;
            if (ritual.ritualPatternBase.Icon != null)
            {
                icon = ritual.ritualPatternBase.Icon;
            }
            Widgets.ButtonImage(new Rect(rect.x + 20f, rect.y, 90f, 90f), icon);
            
            var enabled = SettingsMod.Settings.ForcedUseSeatDefNames.Contains(ritual.defName);
            if (enabled) Widgets.DrawHighlight(rect.ContractedBy(2f));

            var label = ritual.label.CapitalizeFirst();
            
            if (Widgets.ButtonText(new Rect(rect.x + 5f, rect.y + 90f, rect.width - 10f, 35f), label))
            {
                if (enabled) SettingsMod.Settings.ForcedUseSeatDefNames.Remove(ritual.defName);
                else SettingsMod.Settings.ForcedUseSeatDefNames.Add(ritual.defName);
                RitualController.RefreshAll();
            }

            TooltipHandler.TipRegion(rect, ritual.LabelCap);
        }
    }
}

