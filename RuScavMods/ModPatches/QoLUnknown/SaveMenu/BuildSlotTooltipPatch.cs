using HarmonyLib;
using System.Reflection;

namespace RuScavMods.ModPatches.QoLUnknown.SaveMenu
{
    [HarmonyPatch]
    internal static class BuildSlotTooltipPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(QoLTranslator.SaveMenuPatcher, "BuildSlotTooltip");


        [HarmonyPostfix]
        static void Postfix(ref string tipName, ref string tipDesc)
        {
            tipName = "Продолжить";
            tipDesc = tipDesc
                .Replace("Resume your", "Продолжить")
                .Replace("m expedition with Experiment #", "м экспедицию Эксперимента #")
                .Replace("Seed", "Сид")
                .Replace("In-game time", "Время в игре")
                .Replace("Unknown", "[УДАЛЕНО]")
                .Replace("No expedition data in Slot", "Нет экспедиции в слоте");
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
//    //     ▓▒                                                     
//    //    ░░░░░░░░░                                               
//    //    ▓░  ░ ░░░░░▒             ▒▒▒▒▒▒░▒░   omg!!! hi expie!!!!!! what are u doing here?!??!?????
//    //    ▒░░ ░░ ░ ░ ░░░░   ▒▒▒░░░░░░░ ░░░░░                      
//    //    ▒░ ░ ░░░░░░ ░░░░░░ ░ ░ ░ ░ ░ ░ ░░    its not new layer, silly!!!! go back!!!!!!!                
//    //    ▓░░░▒▒▒▒▒▒░ ░ ░ ░ ░░░ ░ ░ ░ ░ ░░░                    
//    //     ░▒    ▒█▓░░ ░ ░ ░ ░▒▒▒▒░░ ░░░░░                     
//    //     ▓▒   ███▓░░░ ░░░░▒▓░  █▓░░░░░                      
//    //   ▓░░█▓▓████▓░ ░░ ░░▓▒   ░██▒░░░                        
//    //   ▓░░█▓██░ ▒▒░░ ░ ░░░ ██████░▒▒▒▒                          
//    //    ▒░▒░▓▓░ ▒▒ ░░ ░ ░▒ ▓█▓▒█▓░ ░▒                      
//    //     ▓▒▓▓▒▒░░░ ░ ░░░░▒     █░░▒▒                            
//    //       ▒▒░░ ░ ░ ░ ░ ░ ░▒▓█▓░▓                           
//    //         ▓░░ ░░░ ░ ░ ░░░█                         
//    //       ░▒▒▓░░▒▒░░░░░░ ░░░░▓                                 
//    //  ▓▒░░▒░ ░░▒▓▒░▒▒▒▒░░▓▒░ ░ ░▒▒                              
//    // ▒░░░ ░ ░ ░░░▒▒▒▒▒▒▒░░░ ░ ░ ░░░▒▓▓▒░                     
//    // ▓░ ░░ ░░░░░░▒▓░░░░░ ░ ░░░░░ ░ ░ ░ ░▒                       
//    //▒░░░ ░░ ░░▒▒░░▒▓░░░ ░ ░ ░░▓░░ ░░ ░░░░▒▓▒▒▒                  
//    //    ▓ ░ ░░▒▓▒░░▒▒░░░ ░░ ░ ▓▒░░ ░░░▒▓▒▒░░▒       ░▒  ▒       
//    //             ▒░ ░░ ░░ ░ ░░▒█▒░░ ░░▒▓░░ ░▒▓▒░▒▒              
//    //             ░░░ ░ ▒░░ ░ ░▒▓░▒▒▒░░░░ ░░ ░░ ▒░              
//    //              ▒░░ ░▓▒░░ ░ ░█░ ░ ░░ ░░ ░░ ░░     ░▒          
//    //              ▒░ ░░▓▓░░░░░ ░▓▓░░ ░░ ░░░░▓                   
//    //            ░▒░░░░▒  ░░ ░░░▓▒░▒  ░░░░░░░░░▒░        
//    //             ░ ░                                                                                                                                                                                                      
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

//            public static void CreateSeedInputTextPrefix(ref string value)
//            {
//                if (value == "Seed")
//                    value = "Сид";
//            }
//            public static void RefreshSlotsPostfix()
//            {
//                Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
//                if (slotContainer == null) return;

//                for (int i = 1; i < slotContainer.childCount; i++)
//                {
//                    var info = slotContainer.GetChild(i)
//                        ?.Find("Content")
//                        ?.Find("Info");

//                    if (info == null) continue;

//                    TextMeshProUGUI textMesh = info.GetComponent<TextMeshProUGUI>();

//                    textMesh.text = textMesh.text
//                        .Replace("Slot", "Слот")
//                        .Replace("Empty", "Пустой")
//                        .Replace("m Deep", "м Глубина")
//                        .Replace("m deep", "м глубина")
//                        .Replace("Corrupt", "Повреждено")
//                        .Replace("run time", "время забега");
//                }
//            }
//            public static void BuildSaveUIPostfix()
//            {
//                Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
//                var header = slotContainer.Find("Header");

//                if (header != null)
//                {
//                    var text = header.GetComponent<TextMeshProUGUI>();
//                    text.text = "СЛОТЫ СОХРАНЕНИЙ";
//                }
//            }
//            public static void BuildSlotTooltipPostfix(ref string tipName, ref string tipDesc)
//            {
//                tipName = "Продолжить";
//                tipDesc = tipDesc
//                    .Replace("Resume your", "Продолжить")
//                    .Replace("m expedition with Experiment #", "м экспедицию Эксперимента #")
//                    .Replace("Seed", "Сид")
//                    .Replace("In-game time", "Время в игре")
//                    .Replace("Unknown", "[УДАЛЕНО]")
//                    .Replace("No expedition data in Slot", "Нет экспедиции в слоте");
//            }
//        }
//    }
//}