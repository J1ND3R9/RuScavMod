using HarmonyLib;
using QoL_Unknown;
using RuScavMods.ModPatches.QoLUnknown.Camera;
using RuScavMods.ModPatches.QoLUnknown.SaveMenu;
using RuScavMods.ModPatches.QoLUnknown.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.QoLUnknown
{

    internal static class QoLTranslator
    {
        private readonly static List<PatchClassProcessor> QoLPatches = new List<PatchClassProcessor>();
        public static readonly Type SaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveMenuPatcher");
        public static readonly Type SaveSaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveSaveMenuPatcher");
        public static void Translate()
        {
            RegisterPatches();
            foreach (var processor in QoLPatches)
            {
                try
                {
                    processor.Patch();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Ошибка патча: {ex.Message}");
                }
            }
            TranslateEnumerables();
        }

        private static void RegisterPatches()
        {
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.qolunknowntranslate");
            QoLPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(BuildSlotTooltipPatch)),
                new PatchClassProcessor(harmony, typeof(CreateSeedInputPatch)),
                new PatchClassProcessor(harmony, typeof(CreateSeedInputTextPatch)),
                new PatchClassProcessor(harmony, typeof(RefreshSlotsPatch)),
                new PatchClassProcessor(harmony, typeof(BuildSaveMenuPatch)),
                new PatchClassProcessor(harmony, typeof(PlayerCameraPatches)),
            });
        }
        private static void TranslateEnumerables()
        {
            ModOptionsTranslator.TranslateSettings();
        }
    }
}



//using HarmonyLib;
//using System;
//using System.Collections;
//using System.Reflection;
//using TMPro;
//using UnityEngine;

//namespace RuScavMods.ReflectPatches
//{
                                                                                                                                                                                                   
//    internal static class QoLTranslator
//    {
//        private static readonly Type ModOptionsMenu = AccessTools.TypeByName("QoL_Unknown.ModOptionsMenu");
//        private static readonly Type ModOptionDefinition = AccessTools.TypeByName("QoL_Unknown.ModOptionDefinition");
//        private static readonly Type ModCustomSettingDefinition = AccessTools.TypeByName("QoL_Unknown.ModCustomSettingDefinition");
//        private static readonly Type QoLModuleManager = AccessTools.TypeByName("QoL_Unknown.QoLModuleManager");
//        private static readonly Type QoLModuleDef = AccessTools.TypeByName("QoL_Unknown.QoLModuleDef");

//        private static readonly IDictionary DefinitionsById = (IDictionary)AccessTools.Field(ModOptionsMenu, "DefinitionsById").GetValue(null);
//        private static readonly IList CustomDefinitions = (IList)AccessTools.Field(ModOptionsMenu, "CustomDefinitions").GetValue(null);
//        private static readonly IList Modules = (IList)AccessTools.Field(QoLModuleManager, "Modules").GetValue(null);

//        private static readonly FieldInfo ModOptionDefinitionLabel = AccessTools.Field(ModOptionDefinition, "Label");
//        private static readonly FieldInfo ModOptionDefinitionDescription = AccessTools.Field(ModOptionDefinition, "Description");
//        private static readonly FieldInfo ModCustomSettingDefinitionId = AccessTools.Field(ModCustomSettingDefinition, "Id");
//        private static readonly FieldInfo ModCustomSettingDefinitionLabel = AccessTools.Field(ModCustomSettingDefinition, "Label");
//        private static readonly FieldInfo ModCustomSettingDefinitionDescription = AccessTools.Field(ModCustomSettingDefinition, "Description");
//        private static readonly FieldInfo QoLModuleDefID = AccessTools.Field(QoLModuleDef, "ID");
//        private static readonly FieldInfo QoLModuleDefName = AccessTools.Field(QoLModuleDef, "Name");
//        private static readonly FieldInfo QoLModuleDefDescription = AccessTools.Field(QoLModuleDef, "Description");

//        public static void Translate()
//        {
//            var log = Plugin.Log;
//            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.qolunknowntranslate");
//            try
//            {
//                TranslateDictionary(DefinitionsById, ModOptionDefinitionLabel, ModOptionDefinitionDescription);
//                TranslateList(CustomDefinitions, ModCustomSettingDefinitionId, ModCustomSettingDefinitionLabel, ModCustomSettingDefinitionDescription);
//                TranslateList(Modules, QoLModuleDefID, QoLModuleDefName, QoLModuleDefDescription);
//                TranslateOther(harmony);
//                log.LogInfo("Перевод QoL_Unknown загружен!");
//            }
//            catch (Exception ex)
//            {
//                log.LogError($"Ошибка перевода QoL_Unknown: " + ex.Message);
//                return;
//            }
//        }
//        private static void TranslateOther(Harmony harmony)
//        {
//            var saveSaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveSaveMenuPatcher");
//            var saveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveMenuPatcher");
//            var inventorySortButtonPatcher = AccessTools.TypeByName("QoL_Unknown.InventorySortButtonPatcher");
//            if (saveSaveMenuPatcher == null || saveMenuPatcher == null || inventorySortButtonPatcher == null) return;

//            var refreshSlotsOriginal = AccessTools.Method(saveSaveMenuPatcher, "RefreshSlots");
//            var buildSaveUIOriginal = AccessTools.Method(saveSaveMenuPatcher, "BuildSaveUI");
//            var buildSlotTooltipOriginal = AccessTools.Method(saveMenuPatcher, "BuildSlotTooltip");
//            var createSeedInputOriginal = AccessTools.Method(saveMenuPatcher, "CreateSeedInput");
//            var createSeedInputTextOriginal = AccessTools.Method(saveMenuPatcher, "CreateSeedInputText");
//            var PlayerCameraStartOriginal = AccessTools.Method(typeof(PlayerCamera), "Start");

//            harmony.Patch(
//                refreshSlotsOriginal,
//                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.RefreshSlotsPostfix)));
//            harmony.Patch(
//                buildSaveUIOriginal,
//                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.BuildSaveUIPostfix)));
//            harmony.Patch(
//                buildSlotTooltipOriginal,
//                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.BuildSlotTooltipPostfix)));
//            harmony.Patch(
//                createSeedInputOriginal,
//                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.CreateSeedInputPostfix)));
//            harmony.Patch(
//                createSeedInputTextOriginal,
//                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.CreateSeedInputTextPrefix)));
//            harmony.Patch(
//                PlayerCameraStartOriginal,
//                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.PlayerCameraStartPostfix)));

//        }

//        private static void TranslateDictionary(
//            IDictionary dict,
//            FieldInfo labelField,
//            FieldInfo descField
//            )
//        {
//            foreach (var pair in TranslationManager.Modules)
//            {
//                if (!dict.Contains(pair.Key))
//                    continue;
//                var item = dict[pair.Key];

//                labelField.SetValue(item, pair.Value.Name);
//                descField.SetValue(item, pair.Value.Description);
//            }
//        }

//        private static void TranslateList(
//            IList list,
//            FieldInfo idField,
//            FieldInfo labelField,
//            FieldInfo descField
//            )
//        {
//            foreach (var item in list)
//            {
//                var id = (string)idField.GetValue(item);

//                if (!TranslationManager.Modules.TryGetValue(id, out var translation))
//                    continue;

//                labelField.SetValue(item, translation.Name);
//                descField.SetValue(item, translation.Description);
//            }
//        }

//        private static class Patches
//        {
//            private static readonly Type SaveSaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveSaveMenuPatcher");
//            private static readonly Type SaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveMenuPatcher");

//            private static readonly FieldInfo SlotContainerField = AccessTools.Field(SaveSaveMenuPatcher, "slotContainer");

//            public static void PlayerCameraStartPostfix(PlayerCamera __instance)
//            {
//                Transform sort = __instance.radialMenu.Find("sort");
//                if (sort == null) return;

//                var tooltip = sort.GetComponent<UITooltip>();

//                tooltip.tipName = "Сортировать";
//                tooltip.tipDesc = "Сортирует твой интвентарь для минимизации веса";
//            }


//    }
//}