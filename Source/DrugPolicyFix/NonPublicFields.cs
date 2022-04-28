using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DrugPolicyFix;

[StaticConstructorOnStartup]
public static class NonPublicFields
{
    public static readonly AccessTools.FieldRef<DrugPolicy, List<DrugPolicyEntry>> DrugPolicyEntryList =
        AccessTools.FieldRefAccess<DrugPolicy, List<DrugPolicyEntry>>("entriesInt");
}