﻿using System.Collections.Generic;
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
            var num = 0;
            var num2 = 0;
            var allPolicies = Current.Game.drugPolicyDatabase.AllPolicies;
            if (allPolicies.Count > 0)
            {
                foreach (var drugPolicy in allPolicies)
                {
                    var list = new List<ThingDef>();
                    if (allDefsListForReading.Count > 0)
                    {
                        foreach (var thingDef in allDefsListForReading)
                        {
                            if (!IsDrug(thingDef, out _))
                            {
                                continue;
                            }

                            var b = false;
                            var list2 = NonPublicFields.DrugPolicyEntryList(drugPolicy);
                            if (list2.Count > 0)
                            {
                                foreach (var drugPolicyEntry in list2)
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
                                list.AddDistinct(thingDef);
                            }
                        }
                    }

                    if (list.Count <= 0)
                    {
                        continue;
                    }

                    num++;
                    foreach (var thingDef2 in list)
                    {
                        num2++;
                        var drugCategory2 = thingDef2.ingestible.drugCategory;
                        AddNewDrugToPolicy(drugPolicy, thingDef2, drugCategory2);
                    }
                }
            }

            string text = "DrugPolicyFix.DoneNothing".Translate();
            if (num > 0)
            {
                text = "DrugPolicyFix.Feedback".Translate(num.ToString(), num2.ToString());
            }

            Log.Message(text);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000226C File Offset: 0x0000046C
        public static bool IsDrug(ThingDef thingdef, out DrugCategory DC)
        {
            DC = DrugCategory.None;
            if (thingdef?.ingestible == null)
            {
                return false;
            }

            var ingestible = thingdef.ingestible;
            var drugCategory = ingestible != null ? new DrugCategory?(ingestible.drugCategory) : null;
            var drugCategory2 = DrugCategory.None;
            var b = !((drugCategory.GetValueOrDefault() == drugCategory2) & (drugCategory != null));

            if (!b)
            {
                return false;
            }

            DC = thingdef.ingestible.drugCategory;
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