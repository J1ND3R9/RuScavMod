using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.VanillaPatches
{
    [HarmonyPatch(typeof(WoundView))]
    [HarmonyPriority(Priority.Last)]
    internal class WoundViewTranslationPatches
    {
        [HarmonyPatch("UpdateView")]
        [HarmonyPostfix]
        private static void UpdateViewPostfix(WoundView __instance)
        {
            if (__instance == null) return;
            __instance.specialUseText.text = NativeTranslationManager.ApplyWoundViewTranslation(__instance.specialUseText.text, ref __instance.specialUseTip.tipName, ref __instance.specialUseTip.tipDesc);
        }
    }
}
