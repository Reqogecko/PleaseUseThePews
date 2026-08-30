using System;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using GatherRound;

[HarmonyPatch]
static class GatheringsUtilityPatch
{
    private static float CustomGatheringRange = Settings.GatheringRange;

    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(
            typeof(GatheringsUtility),
            nameof(GatheringsUtility.InGatheringArea));

        foreach (var type in typeof(GatheringsUtility)
                     .GetNestedTypes(AccessTools.all))
        foreach (var method in type.GetMethods(AccessTools.all))
            if (method.Name.StartsWith("InGatheringArea"))
                yield return method;
    }

    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            if (code.opcode == OpCodes.Ldc_R4 &&
                code.operand is float f &&
                Math.Abs(f - 18f) < 0.001f)
                code.operand = CustomGatheringRange;

            yield return code;
        }
    }
}