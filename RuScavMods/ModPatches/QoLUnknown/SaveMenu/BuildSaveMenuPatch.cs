using HarmonyLib;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.QoLUnknown.SaveMenu
{
    [HarmonyPatch]
    internal static class BuildSaveMenuPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(QoLTranslator.SaveSaveMenuPatcher, "BuildSaveUI");
        private static readonly FieldInfo SlotContainerField = AccessTools.Field(QoLTranslator.SaveSaveMenuPatcher, "slotContainer");
        [HarmonyPostfix]
        public static void Postfix()
        {
            Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
            Transform header = slotContainer.Find("Header");

            if (header != null)
            {
                TextMeshProUGUI text = header.GetComponent<TextMeshProUGUI>();
                text.text = "СЛОТЫ СОХРАНЕНИЙ";
            }
        }
    }
}