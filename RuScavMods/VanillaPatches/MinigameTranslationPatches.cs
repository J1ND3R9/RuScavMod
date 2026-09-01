using HarmonyLib;
using RuScavMods.TranslationClasses.Definitions;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.VanillaPatches
{
    [HarmonyPatch(typeof(MinigameBase))]
    internal class MinigameTranslationPatches
    {
        [HarmonyPatch(nameof(MinigameBase.StartMinigame))]
        [HarmonyPostfix]
        private static void StartMinigamePostfix(MinigameBase __instance)
        {
            if (__instance == null) return;
            __instance.guideText.text = NativeTranslationManager.ApplyMinigameTranslation(__instance.guideText.text);
        }
    }
}
