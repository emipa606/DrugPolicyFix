using RimWorld;
using Verse;

namespace DrugPolicyFix;

public class DrugPolicyUtility
{
    public static bool IsAlcohol(ThingDef drug)
    {
        if (drug?.comps == null)
        {
            return false;
        }

        var comps = drug.comps;
        if (comps.Count <= 0)
        {
            return false;
        }

        using var enumerator = comps.GetEnumerator();
        while (enumerator.MoveNext())
        {
            CompProperties_Drug compPropertiesDrug;
            if ((compPropertiesDrug = enumerator.Current as CompProperties_Drug) != null &&
                compPropertiesDrug.chemical == ChemicalDefOf.Alcohol)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsSmokey(ThingDef drug)
    {
        if (drug?.comps == null)
        {
            return false;
        }

        var comps = drug.comps;
        if (comps.Count <= 0)
        {
            return false;
        }

        using var enumerator = comps.GetEnumerator();
        while (enumerator.MoveNext())
        {
            CompProperties_Drug compProperties_Drug;
            if ((compProperties_Drug = enumerator.Current as CompProperties_Drug) != null &&
                compProperties_Drug.chemical == DefDatabase<ChemicalDef>.GetNamed("Smokeleaf"))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAddictive(ThingDef drug)
    {
        return drug.IsAddictiveDrug;
    }
}