using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.QoLUnknown.SaveMenu
{
    [HarmonyPatch]
    internal static class BuildSaveMenuPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(QoLTranslator.SaveSaveMenuPatcher, "BuildSaveUI");
        private static readonly FieldInfo SlotContainerField = AccessTools.Field(QoLTranslator.SaveSaveMenuPatcher, "slotContainer");
        public static void Postfix()
        {
            Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
            var header = slotContainer.Find("Header");

            if (header != null)
            {
                var text = header.GetComponent<TextMeshProUGUI>();
                text.text = "СЛОТЫ СОХРАНЕНИЙ";
            }
        }
    }
}