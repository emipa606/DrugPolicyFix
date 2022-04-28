using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DrugPolicyFix
{
    // Token: 0x02000002 RID: 2
    public static class DrugPolicyFix
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
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
                        if (IsDrug(thingDef))
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
                            if (IsDrug(drugPolicyEntry.drug))
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
                        AddNewDrugToPolicy(drugPolicy, thingDef2, drugCategory2);
                    }
                }
            }

            // Log out what was done.
            if (policiesAddedToCount == 0 && policiesRemovedFromCount == 0)
            {
                Log.Message("DrugPolicyFix.DoneNothing".Translate());
            }

            if (policiesAddedToCount > 0)
            {
                Log.Message("DrugPolicyFix.Feedback".Translate(policiesAddedToCount.ToString(), defsAddedCount.ToString()));
            }

            if (policiesRemovedFromCount > 0)
            {
                Log.Message("DrugPolicyFix.Removed".Translate(policiesRemovedFromCount.ToString(), defsRemovedCount.ToString()));
            }

        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000226C File Offset: 0x0000046C
        public static bool IsDrug(ThingDef thingdef)
        {
            var ingestible = thingdef?.ingestible;
            var drugCategory = ingestible?.drugCategory;
            if (ingestible == null || drugCategory == null || drugCategory == DrugCategory.None)
            {
                return false;
            }
            return true;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000022DC File Offset: 0x000004DC
        public static void AddNewDrugToPolicy(DrugPolicy dp, ThingDef newdrug, DrugCategory DC)
        {
            var drugPolicyEntry = new DrugPolicyEntry
            {
                drug = newdrug, allowedForAddiction = false, allowedForJoy = false, allowScheduled = false
            };
            if (dp.label == "SocialDrugs".Translate())
            {
                if (DC == DrugCategory.Social)
                {
                    drugPolicyEntry.allowedForJoy = true;
                }
            }
            else if (dp.label == "Unrestricted".Translate())
            {
                if (newdrug.IsPleasureDrug)
                {
                    drugPolicyEntry.allowedForJoy = true;
                }
            }
            else if (dp.label == "OneDrinkPerDay".Translate() &&
                     (DrugPolicyUtility.IsAlcohol(newdrug) || DrugPolicyUtility.IsSmokey(newdrug)) &&
                     newdrug.IsPleasureDrug)
            {
                drugPolicyEntry.allowedForJoy = true;
            }

            if (DrugPolicyUtility.IsAddictive(newdrug))
            {
                drugPolicyEntry.allowedForAddiction = true;
            }

            var list = NonPublicFields.DrugPolicyEntryList(dp);
            list.AddDistinct(drugPolicyEntry);
            NonPublicFields.DrugPolicyEntryList(dp) = list;
        }
    }
}