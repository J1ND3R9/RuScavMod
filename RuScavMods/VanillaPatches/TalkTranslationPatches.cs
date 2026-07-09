using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(Talker))]
    [HarmonyPriority(Priority.Last)]
    internal class TalkTranslationPatches
    {
        [HarmonyPatch(nameof(Talker.Talk), new[] { typeof(string), typeof(Limb), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        private static void TalkPrefix(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            NativeTranslationManager.ApplyTalkTranslation(ref text);
        }
    }
}
