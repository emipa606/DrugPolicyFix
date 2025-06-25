using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DrugPolicyFix;

public static class DrugPolicyFix
{
    public static void CorrectPolicies()
    {
        var allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
        var policiesAddedToCount = 0;
        var defsAddedCount = 0;
        var policiesRemovedFromCount = 0;
        var defsRemovedCount = 0;
        var allPolicies = Current.Game.drugPolicyDatabase.AllPolicies;
        if (allPolicies.Count > 0)
        {
            // Collect together all known ThingDefs that are categorized as drugs
            var allDrugDefs = new List<ThingDef>();
            if (allDefsListForReading.Count > 0)
            {
                foreach (var thingDef in allDefsListForReading)
                {
                    if (isDrug(thingDef))
                    {
                        allDrugDefs.AddDistinct(thingDef);
                    }
                }
            }

            foreach (var drugPolicy in allPolicies)
            {
                // Remove any non-drugs that might have found their way into existing policies (e.g. through use of the
                // Cherry Picker mod)
                var existingDrugPolicyEntries = NonPublicFields.DrugPolicyEntryList(drugPolicy);
                var filteredDrugPolicyEntries = new List<DrugPolicyEntry>();
                if (existingDrugPolicyEntries.Count > 0)
                {
                    foreach (var drugPolicyEntry in existingDrugPolicyEntries)
                    {
                        if (isDrug(drugPolicyEntry.drug))
                        {
                            filteredDrugPolicyEntries.AddDistinct(drugPolicyEntry);
                        }
                    }
                }

                if (existingDrugPolicyEntries.Count > filteredDrugPolicyEntries.Count)
                {
                    defsRemovedCount = existingDrugPolicyEntries.Count - filteredDrugPolicyEntries.Count;
                    policiesRemovedFromCount++;
                    NonPublicFields.DrugPolicyEntryList(drugPolicy) = filteredDrugPolicyEntries;
                }

                // Find new drugs that aren't already in the policy to add
                var drugDefsToAdd = new List<ThingDef>();
                foreach (var thingDef in allDrugDefs)
                {
                    var b = false;

                    if (filteredDrugPolicyEntries.Count > 0)
                    {
                        foreach (var drugPolicyEntry in filteredDrugPolicyEntries)
                        {
                            if (thingDef != drugPolicyEntry.drug)
                            {
                                continue;
                            }

                            b = true;
                            break;
                        }
                    }

                    if (!b)
                    {
                        drugDefsToAdd.AddDistinct(thingDef);
                    }
                }

                if (drugDefsToAdd.Count <= 0)
                {
                    continue;
                }

                policiesAddedToCount++;

                // Create drug policy entries from the ThingDef and add them
                foreach (var thingDef2 in drugDefsToAdd)
                {
                    defsAddedCount++;
                    var drugCategory2 = thingDef2.ingestible.drugCategory;
                    addNewDrugToPolicy(drugPolicy, thingDef2, drugCategory2);
                }
            }
        }

        switch (policiesAddedToCount)
        {
            // Log out what was done.
            case 0 when policiesRemovedFromCount == 0:
                Log.Message("DrugPolicyFix.DoneNothing".Translate());
                break;
            case > 0:
                Log.Message("DrugPolicyFix.Feedback".Translate(policiesAddedToCount.ToString(),
                    defsAddedCount.ToString()));
                break;
        }

        if (policiesRemovedFromCount > 0)
        {
            Log.Message("DrugPolicyFix.Removed".Translate(policiesRemovedFromCount.ToString(),
                defsRemovedCount.ToString()));
        }
    }

    private static bool isDrug(ThingDef thingDef)
    {
        var ingestible = thingDef?.ingestible;
        var drugCategory = ingestible?.drugCategory;
        return ingestible != null && drugCategory != DrugCategory.None;
    }

    private static void addNewDrugToPolicy(DrugPolicy drugPolicy, ThingDef newDrug, DrugCategory drugCategory)
    {
        var drugPolicyEntry = new DrugPolicyEntry
        {
            drug = newDrug, allowedForAddiction = false, allowedForJoy = false, allowScheduled = false
        };
        if (drugPolicy.label == "SocialDrugs".Translate())
        {
            if (drugCategory == DrugCategory.Social)
            {
                drugPolicyEntry.allowedForJoy = true;
            }
        }
        else if (drugPolicy.label == "Unrestricted".Translate())
        {
            if (newDrug.IsPleasureDrug)
            {
                drugPolicyEntry.allowedForJoy = true;
            }
        }
        else if (drugPolicy.label == "OneDrinkPerDay".Translate() &&
                 (DrugPolicyUtility.IsAlcohol(newDrug) || DrugPolicyUtility.IsSmokey(newDrug)) &&
                 newDrug.IsPleasureDrug)
        {
            drugPolicyEntry.allowedForJoy = true;
        }

        if (DrugPolicyUtility.IsAddictive(newDrug))
        {
            drugPolicyEntry.allowedForAddiction = true;
        }

        var list = NonPublicFields.DrugPolicyEntryList(drugPolicy);
        list.AddDistinct(drugPolicyEntry);
        NonPublicFields.DrugPolicyEntryList(drugPolicy) = list;
    }
}