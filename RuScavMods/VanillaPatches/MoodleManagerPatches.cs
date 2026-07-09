using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(MoodleManager))]
    [HarmonyPriority(Priority.Last)]
    internal class MoodleManagerPatches
    {
        [HarmonyPatch(nameof(MoodleManager.AddMoodle))]
        [HarmonyPrefix]
        private static void AddMoodlePrefix(ref string name, ref string desc)
        {
            if (string.IsNullOrEmpty(name)) return;
            NativeTranslationManager.ApplyMoodleTranslation(ref name, ref desc);
        }
    }
}
