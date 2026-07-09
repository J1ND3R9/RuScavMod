using BepInEx;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using QoL_Unknown;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RuScavMods.TranslationClasses.Definitions.NativeTranslationFile;

namespace RuScavMods.ModPatches.QoLUnknown.Settings
{
    internal static class ModOptionsTranslator
    {
        //private static readonly AccessTools.FieldRef<ModOptionDefinition, string> IdRef = AccessTools.FieldRefAccess<ModOptionDefinition, string>("Id");
        private static readonly AccessTools.FieldRef<ModOptionDefinition, string> LabelRef = AccessTools.FieldRefAccess<ModOptionDefinition, string>("Label");
        private static readonly AccessTools.FieldRef<ModOptionDefinition, string> DescriptionRef = AccessTools.FieldRefAccess<ModOptionDefinition, string>("Description");

        private static readonly Dictionary<string, ModOptionDefinition> DefinitionsById = (Dictionary<string, ModOptionDefinition>)AccessTools.Field(typeof(ModOptionsMenu), "DefinitionsById").GetValue(null);
        private static readonly List<ModCustomSettingDefinition> CustomDefinitions = (List<ModCustomSettingDefinition>)AccessTools.Field(typeof(ModOptionsMenu), "CustomDefinitions").GetValue(null);
        private static Dictionary<string, NameDescription> Modules = new Dictionary<string, NameDescription>();
        
        public static void TranslateSettings()
        {
            RegisterModules();
            foreach (var pair in Modules)
            {
                if (DefinitionsById.TryGetValue(pair.Key, out var def))
                {
                    LabelRef(def) = pair.Value.Name;
                    DescriptionRef(def) = pair.Value.Description;
                }
                var moduleDef = QoLModuleManager.Modules.FirstOrDefault(q => q.ID == pair.Key) ?? null;
                if (moduleDef != null)
                {
                    moduleDef.Name = pair.Value.Name;
                    moduleDef.Description = pair.Value.Description;
                }
                var customDef = CustomDefinitions.FirstOrDefault(c => c.Id == pair.Key) ?? null;
                if (customDef != null)
                {
                    customDef.Label = pair.Value.Name;
                    customDef.Description = pair.Value.Description;
                }
            }
        }


        static void RegisterModules()
        {
            Modules.Clear();
            var modulesFile = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "org.bepinex.plugins.qol_unknown.json");
            if (File.Exists(modulesFile))
            {
                var text = File.ReadAllText(modulesFile);
                JObject obj = JObject.Parse(text);
                Dictionary<string, NameDescription> parsed = obj["modules"].ToObject<Dictionary<string, NameDescription>>();
                if (parsed != null || parsed.Count > 0)
                    Modules = parsed;
            }
        }
    }
}
