using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(Item))]
    [HarmonyPriority(Priority.Last)]
    internal class ItemTranslationPatches
    {
        [HarmonyPatch(nameof(Item.SetupItems))]
        [HarmonyPostfix]
        private static void SetupItemsPostfix()
        {
            if (Item.GlobalItems == null) return;
            if (Item.GlobalItems.Count == 0) return;
            NativeTranslationManager.ApplyItemTranslation(Item.GlobalItems);
        }
    }
}
