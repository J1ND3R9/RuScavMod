using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.QoLUnknown;
using RuScavMods.ModPatches.QoLUnknown.SaveMenu;
using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace RuScavMods.ModPatches.CUTarkovMedicine
{
    internal static class CUTarkovMedicine
    {
        public static readonly Type I18n = AccessTools.TypeByName("CUTarkovMedicalMod.Framework.I18n");
        private readonly static List<PatchClassProcessor> CUTarkovMedicinePatches = new List<PatchClassProcessor>();
        // public static Dictionary<string, string> TarkovMedicineDefinitions = new Dictionary<string, string>();
        public static void Translate()
        {
            RegisterPatches();
            // FillDictionary();
            foreach (var processor in CUTarkovMedicinePatches)
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
        }
        private static void RegisterPatches()
        {
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.cutarkovmedicinetranslate");
            CUTarkovMedicinePatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(I18nPatch)),
            });
        }
        //private static void FillDictionary()
        //{
        //    TarkovMedicineDefinitions.Clear();
        //    var translationFile = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "CUTarkovMedicalMod.json");
        //    if (File.Exists(translationFile))
        //    {
        //        var text = File.ReadAllText(translationFile);
        //        var translationData = JsonConvert.DeserializeObject<Dictionary<string, string>>(text) ?? throw new Exception("Файл не был десериализован.");
        //        TarkovMedicineDefinitions = translationData;
        //    }
        //}
    }

    [HarmonyPatch]
    internal static class I18nPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(CUTarkovMedicine.I18n, "GetTranslationFilePath");
        [HarmonyPostfix]
        static void Postfix(ref string __result)
        {
            __result = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "CUTarkovMedicalMod.json");
        }
    }
}
