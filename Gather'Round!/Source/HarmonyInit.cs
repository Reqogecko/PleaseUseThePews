using Verse;

[StaticConstructorOnStartup]
public class MinorChangesModStartup {
    static MinorChangesModStartup() {
        var harmony = new HarmonyLib.Harmony("net.reqo.RimWorld.MinorChanges");
        harmony.PatchAll();
    }
}