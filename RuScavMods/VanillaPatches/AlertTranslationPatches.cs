using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(PlayerCamera))]
    [HarmonyPriority(Priority.Last)]
    internal class AlertTranslationPatches
    {
        [HarmonyPatch(nameof(PlayerCamera.DoAlert))]
        [HarmonyPrefix]
        private static void DoAlertPrefix(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            NativeTranslationManager.ApplyTalkTranslation(ref text);
        }
    }
}
