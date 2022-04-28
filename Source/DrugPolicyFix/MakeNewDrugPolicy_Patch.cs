using HarmonyLib;
using RimWorld;

namespace DrugPolicyFix;

[HarmonyPatch(typeof(DrugPolicyDatabase), "MakeNewDrugPolicy")]
public class MakeNewDrugPolicy_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void Postfix(ref DrugPolicy __result)
    {
        DrugPolicySort.SortPolicy(__result);
    }
}