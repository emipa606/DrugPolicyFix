using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DrugPolicyFix;

public static class DrugPolicySort
{
    public static void SortPolicy(DrugPolicy DP)
    {
        var list = NonPublicFields.DrugPolicyEntryList(DP);
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

            CheckValues(DP, drugPolicyEntry, drugPolicyEntry.drug, out var drugPolicyEntry2);
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
        NonPublicFields.DrugPolicyEntryList(DP) = list3;
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

                    CheckValues(drugPolicy, drugPolicyEntry, drugPolicyEntry.drug, out var drugPolicyEntry2);
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

    public static void CheckValues(DrugPolicy DP, DrugPolicyEntry DPE, ThingDef drug,
        out DrugPolicyEntry DPEChecked)
    {
        DPEChecked = DPE;
        if (!DrugPolicyUtility.IsAddictive(drug))
        {
            DPEChecked.allowedForAddiction = false;
        }

        if (!drug.IsPleasureDrug)
        {
            DPEChecked.allowedForJoy = false;
        }
    }
}