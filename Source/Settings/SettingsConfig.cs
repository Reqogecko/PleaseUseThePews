using System.Collections.Generic;
using Verse;

namespace PleaseUseThePews.Settings
{
    public class SettingsConfig : ModSettings
    {
        public static float GatheringRange = 50f;
        public static int GenRadialTilesToCheck = 1500;

        public HashSet<string> ForcedUseSeatDefNames = new HashSet<string>();

        public static bool InitialSetupDone = false;
        public static bool RestartPopupWasShown = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref GatheringRange, "MaxTilesToSearchForSeat", 50f);
            Scribe_Values.Look(ref GenRadialTilesToCheck, "TotalAmountOfTilesToCheckForSeats", 3000);
            
            Scribe_Values.Look(ref InitialSetupDone, "InitialSetupDone", false);
            Scribe_Collections.Look(ref ForcedUseSeatDefNames, "ForcedUseSeatDefNames", LookMode.Value);
            
            if (ForcedUseSeatDefNames == null)
            {
                ForcedUseSeatDefNames = new HashSet<string>();
            }
            
            base.ExposeData();
        }
    }
}