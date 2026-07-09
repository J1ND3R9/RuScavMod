using HarmonyLib;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.QoLUnknown.SaveMenu
{
    [HarmonyPatch]
    internal static class RefreshSlotsPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(QoLTranslator.SaveMenuPatcher, "CreateSeedInput");
        private static readonly FieldInfo SlotContainerField = AccessTools.Field(QoLTranslator.SaveSaveMenuPatcher, "slotContainer");
        static void Postfix()
        {
            Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
            if (slotContainer == null) return;

            for (int i = 1; i < slotContainer.childCount; i++)
            {
                var info = slotContainer.GetChild(i)
                    ?.Find("Content")
                    ?.Find("Info");

                if (info == null) continue;

                TextMeshProUGUI textMesh = info.GetComponent<TextMeshProUGUI>();

                textMesh.text = textMesh.text
                    .Replace("Slot", "Слот")
                    .Replace("Empty", "Пустой")
                    .Replace("m Deep", "м Глубина")
                    .Replace("m deep", "м глубина")
                    .Replace("Corrupt", "Повреждено")
                    .Replace("run time", "время забега");
            }
        }
    }
}