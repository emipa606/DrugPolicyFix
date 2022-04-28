using System.Reflection;
using HarmonyLib;
using Verse;

namespace DrugPolicyFix;

[StaticConstructorOnStartup]
public static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.Rimworld.DrugPolicyFix").PatchAll(Assembly.GetExecutingAssembly());
    }
}