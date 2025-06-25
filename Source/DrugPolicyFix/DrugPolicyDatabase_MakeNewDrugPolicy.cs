using HarmonyLib;
using RimWorld;

namespace DrugPolicyFix;

[HarmonyPatch(typeof(DrugPolicyDatabase), nameof(DrugPolicyDatabase.MakeNewDrugPolicy))]
public class DrugPolicyDatabase_MakeNewDrugPolicy
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void Postfix(ref DrugPolicy __result)
    {
        DrugPolicySort.SortPolicy(__result);
    }
}