using System;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using PleaseUseThePews;
using HarmonyLib;

[HarmonyPatch(typeof(SpectatorCellFinder))]
[HarmonyPatch(nameof(SpectatorCellFinder.TryFindSpectatorCellFor))]
static class SpectatorDistancePatch
{
    private static float MaxDistanceToPartySpot = Settings.MaxDistanceToRitualSpot;
    private static int NewMaxNum = Settings.GenRadialTilesToCheck;
    
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            if (code.opcode == OpCodes.Ldc_R4 &&
                code.operand is float f &&
                Math.Abs(f - 210.25f) < 0.001f)
                code.operand = MaxDistanceToPartySpot * MaxDistanceToPartySpot;
            if (code.opcode == OpCodes.Ldc_I4 &&
                code.operand is int i &&
                i == 1000)
                code.operand = NewMaxNum;

            yield return code;
        }
    }
}
