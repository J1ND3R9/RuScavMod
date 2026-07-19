using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.AdditionalDescriptions;
using RuScavMods.ModPatches.CUCoreLib;
using RuScavMods.ModPatches.CUTarkovMedicine;
using RuScavMods.ModPatches.extraDialogue;
using RuScavMods.ModPatches.Gunsaw;
using RuScavMods.ModPatches.KrokoshaMP;
using RuScavMods.ModPatches.QoLUnknown;
using RuScavMods.ModPatches.RshCCL;
using RuScavMods.Patches;
using RuScavMods.TranslationClasses.Definitions;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

//     ▓▒                                                     
//    ░░░░░░░░░                                               
//    ▓░  ░ ░░░░░▒             ▒▒▒▒▒▒░▒░   чзх!!!!!!!! экспи!!!! что ты тут делаешь!??!?!
//    ▒░░ ░░ ░ ░ ░░░░   ▒▒▒░░░░░░░ ░░░░░                      
//    ▒░ ░ ░░░░░░ ░░░░░░ ░ ░ ░ ░ ░ ░ ░░    глупенький зверь, это не новый слой!!!!!!! уходи из моего кода!!!!!!!                
//    ▓░░░▒▒▒▒▒▒░ ░ ░ ░ ░░░ ░ ░ ░ ░ ░░░                    
//     ░▒    ▒█▓░░ ░ ░ ░ ░▒▒▒▒░░ ░░░░░                     
//     ▓▒   ███▓░░░ ░░░░▒▓░  █▓░░░░░                      
//   ▓░░█▓▓████▓░ ░░ ░░▓▒   ░██▒░░░                        
//   ▓░░█▓██░ ▒▒░░ ░ ░░░ ██████░▒▒▒▒                          
//    ▒░▒░▓▓░ ▒▒ ░░ ░ ░▒ ▓█▓▒█▓░ ░▒                      
//     ▓▒▓▓▒▒░░░ ░ ░░░░▒     █░░▒▒                            
//       ▒▒░░ ░ ░ ░ ░ ░ ░▒▓█▓░▓                           
//         ▓░░ ░░░ ░ ░ ░░░█                         
//       ░▒▒▓░░▒▒░░░░░░ ░░░░▓                                 
//  ▓▒░░▒░ ░░▒▓▒░▒▒▒▒░░▓▒░ ░ ░▒▒                              
// ▒░░░ ░ ░ ░░░▒▒▒▒▒▒▒░░░ ░ ░ ░░░▒▓▓▒░                     
// ▓░ ░░ ░░░░░░▒▓░░░░░ ░ ░░░░░ ░ ░ ░ ░▒                       
//▒░░░ ░░ ░░▒▒░░▒▓░░░ ░ ░ ░░▓░░ ░░ ░░░░▒▓▒▒▒                  
//    ▓ ░ ░░▒▓▒░░▒▒░░░ ░░ ░ ▓▒░░ ░░░▒▓▒▒░░▒       ░▒  ▒       
//             ▒░ ░░ ░░ ░ ░░▒█▒░░ ░░▒▓░░ ░▒▓▒░▒▒              
//             ░░░ ░ ▒░░ ░ ░▒▓░▒▒▒░░░░ ░░ ░░ ▒░              
//              ▒░░ ░▓▒░░ ░ ░█░ ░ ░░ ░░ ░░ ░░     ░▒          
//              ▒░ ░░▓▓░░░░░ ░▓▓░░ ░░ ░░░░▓                   
//            ░▒░░░░▒  ░░ ░░░▓▒░▒  ░░░░░░░░░▒░        
//             ░ ░   

namespace RuScavMods
{
    [BepInPlugin("zzz_com.Floydba.ruscavmods", "RuScavMod", "2.1.1.3")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private readonly static List<PatchClassProcessor> VanillaPatches = new List<PatchClassProcessor>();
        private readonly static Dictionary<string, string> BridgeCandidates = new Dictionary<string, string>();
        public static bool RshCCLInstalled = File.Exists(Path.Combine(Paths.PluginPath, "RshLib.dll")) && AccessTools.TypeByName("RshLib.RshItemAdapter") != null; // У автора мода интересный случай: он удаляет себя из PluginInfos и, затем, пытается вернуть патчем. Но патч не срабатывает, поэтому он просто делитит себя, bruh.
        internal sealed class ModTranslator
        {
            public string Guid { get; set; }
            public string Name { get; set; }
            public Action Translate { get; set; }
        }

        private static readonly List<ModTranslator> Translators = new List<ModTranslator>()
        {
            new ModTranslator()
            {
                Guid = "KrokoshaCasualtiesMP",
                Name = "Krokosha_MP_CU",
                Translate = KrokoshaMPTranslator.Translate
            },
            new ModTranslator()
            {
                Guid = "org.bepinex.plugins.qol_unknown",
                Name = "QoL: Unknown",
                Translate = QoLTranslator.Translate
            },
            new ModTranslator()
            {
                Guid = "noah.casualtiesunknown.additionaldescriptions",
                Name = "AdditionalDescriptions",
                Translate = AdditionalDescriptionsTranslator.Translate
            },
            new ModTranslator()
            {
                Guid = "com.yourname.cu.tarkovmedicalmod",
                Name = "Tarkov Medical",
                Translate = CUTarkovMedicine.Translate
            },
            new ModTranslator()
            {
                Guid = "GunsawGenetics",
                Name = "Gunsaw Genetics",
                Translate = GunsawGeneticsTranslator.Translate
            },
            new ModTranslator()
            {
                Guid = "furr.extraDialogue",
                Name = "Extra Dialogue",
                Translate = ExtraDialogueTranslator.Translate
            },
            new ModTranslator()
            {
                Guid = "net.cucorelib",
                Name = "CUCore",
                Translate = CUCoreTranslator.Translate
            }
        };
        private void Awake()
        {
            Plugin.Log = base.Logger;
            LoadTranslationFiles();
            ApplyVanillaPatches();
            ApplyModTranslations();
        }

        private void LoadTranslationFiles()
        {
            var pluginsInfo = Chainloader.PluginInfos;
            var pathTranslations = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations");
            foreach (var plugin in pluginsInfo)
            {
                var pluginId = plugin.Key;
                var translationFile = Path.Combine(pathTranslations, $"{pluginId}.json");
                if (File.Exists(translationFile))
                {
                    try
                    {
                        string json = File.ReadAllText(translationFile);
                        TranslationType? type = GetJsonType(json) ?? throw new Exception("GetJsonType вернул null!");
                        if (type == TranslationType.Native)
                            LoadNativeTranslation(json);
                        else if (type == TranslationType.CuCore)
                            LoadCuCoreTranslation(json);
                        else
                            continue;
                    }
                    catch (Exception ex)
                    {
                        Log.LogError($"Произошла ошибка при загрузке перевода {plugin.Value.Metadata.Name}: {ex.Message}");
                    }
                }
            }
        }
        private void RegisterVanillaPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.vanillapatches");
            VanillaPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(AlertTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(GetStringTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(ItemTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(LocaleTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(MoodleManagerPatches)),
                new PatchClassProcessor(harmony, typeof(TalkTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(TutorialTranslationPatches))
            });
        }
        private void LoadNativeTranslation(string json)
        {
            var translationData = JsonConvert.DeserializeObject<NativeTranslationFile>(json) ?? throw new Exception("Файл не был десериализован.");
            NativeTranslationManager.Add(translationData);
        }
        private void LoadCuCoreTranslation(string json)
        {
            var translationData = JsonConvert.DeserializeObject<CuCoreTranslationFile>(json) ?? throw new Exception("Файл не был десериализован.");
            CuCoreTranslationManager.Add(translationData);
        }

        private TranslationType? GetJsonType(string json)
        {
            JObject obj = JObject.Parse(json);
            string type = obj["type"]?.ToString();
            if (type == null)
                return null;
            switch (type)
            {
                case "native":
                    return TranslationType.Native;
                case "custom":
                    return TranslationType.Custom;
                case "cucore":
                    return TranslationType.CuCore;
                default:
                    return TranslationType.Custom;
            }
        }
        private void ApplyVanillaPatches()
        {
            RegisterVanillaPatches();
            foreach (var processor in VanillaPatches)
            {
                try
                {
                    processor.Patch();
                    Log.LogInfo($"Патчим {processor}");

                }
                catch (Exception ex)
                {
                    Log.LogError($"Ошибка патча {processor}: {ex.Message}");
                }
            }
        }


        private void ApplyModTranslations()
        {
            foreach (var translator in Translators)
            {
                if (!Chainloader.PluginInfos.ContainsKey(translator.Guid))
                    continue;

                try
                {
                    translator.Translate();
                    Log.LogInfo($"Переводим {translator.Name}");
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Ошибка при переводе {translator.Name}: {ex}");
                }
            }
            if (RshCCLInstalled)
            {
                try
                {
                    RshCCLTranslator.Translate();
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
            }
        }
    }
}