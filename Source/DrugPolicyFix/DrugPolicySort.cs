using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DrugPolicyFix;

public static class DrugPolicySort
{
    public static void SortPolicy(DrugPolicy drugPolicy)
    {
        var list = NonPublicFields.DrugPolicyEntryList(drugPolicy);
        var list2 = new List<DrugPolicyEntry>();
        if (list.Count <= 0)
        {
            return;
        }

        foreach (var drugPolicyEntry in list)
        {
            if (drugPolicyEntry?.drug == null)
            {
                continue;
            }

            checkValues(drugPolicyEntry, drugPolicyEntry.drug, out var drugPolicyEntry2);
            if (drugPolicyEntry2 != null)
            {
                list2.AddDistinct(drugPolicyEntry2);
            }
        }

        if (list2.Count <= 0)
        {
            return;
        }

        var list3 = (from dpe in list2
            orderby dpe.drug.label
            select dpe).ToList();
        NonPublicFields.DrugPolicyEntryList(drugPolicy) = list3;
    }

    public static void SortPolicies()
    {
        var allPolicies = Current.Game.drugPolicyDatabase.AllPolicies;
        var num = 0;
        if (allPolicies.Count > 0)
        {
            foreach (var drugPolicy in allPolicies)
            {
                var list = NonPublicFields.DrugPolicyEntryList(drugPolicy);
                var list2 = new List<DrugPolicyEntry>();
                if (list.Count <= 0)
                {
                    continue;
                }

                foreach (var drugPolicyEntry in list)
                {
                    if (drugPolicyEntry?.drug == null)
                    {
                        continue;
                    }

                    checkValues(drugPolicyEntry, drugPolicyEntry.drug, out var drugPolicyEntry2);
                    if (drugPolicyEntry2 != null)
                    {
                        list2.AddDistinct(drugPolicyEntry2);
                    }
                }

                if (list2.Count <= 0)
                {
                    continue;
                }

                var list3 = (from dpe in list2
                    orderby dpe.drug.label
                    select dpe).ToList();
                NonPublicFields.DrugPolicyEntryList(drugPolicy) = list3;
                num++;
            }
        }

        Log.Message("DrugPolicyFix.Sorted".Translate(num.ToString()));
    }

    private static void checkValues(DrugPolicyEntry dpe, ThingDef drug, out DrugPolicyEntry dpeChecked)
    {
        dpeChecked = dpe;
        if (!DrugPolicyUtility.IsAddictive(drug))
        {
            dpeChecked.allowedForAddiction = false;
        }

        if (!drug.IsPleasureDrug)
        {
            dpeChecked.allowedForJoy = false;
        }
    }
}