using HarmonyLib;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.CUCoreLib;
using RuScavMods.ModPatches.Gunsaw;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.ModPatches.MilkableMilkies
{
    internal class MilkableMilkiesTranslator
    {

        private readonly static List<PatchClassProcessor> MMPatches = new List<PatchClassProcessor>();
        public static readonly Type MilkableMilkiesPluginType = AccessTools.TypeByName("MilkableMilkies.MilkableMilkiesPlugin");
        public static readonly Type MilkLiquidsType = AccessTools.TypeByName("MilkableMilkies.MilkLiquids");

        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in MMPatches)
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
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.milkablemilkiestranslate");
            MMPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(MilkableMilkiesPluginApplyHoverHintsPatch)),
                new PatchClassProcessor(harmony, typeof(MilkableMilkiesPluginInjectLocalePatch)),
            });
        }
    }

    [HarmonyPatch]
    internal static class MilkableMilkiesPluginApplyHoverHintsPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(MilkableMilkiesTranslator.MilkableMilkiesPluginType, "ApplyHoverHints");

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string s && s.Contains("Milk the Milky"))
                {
                    instruction.operand = "] Подоить Милки (держи емкость в руках)";
                }
                yield return instruction;
            }
        }
    }

    [HarmonyPatch]
    internal static class MilkableMilkiesPluginInjectLocalePatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(MilkableMilkiesTranslator.MilkLiquidsType, "InjectLocale");

        [HarmonyPostfix]
        static void Postfix()
        {
            Language currentLang = Locale.currentLang;
            if (currentLang == null) return;

            currentLang.other["milkiestmilk"] = "Сгущённое молочко Милки";
            currentLang.other["milkiestmilkdsc"] = "Молоко совершенно иного рода. Прямо из источников! Непастеризованное, чуть теплое. Оно ценнее почти всего, что у тебя с собой. Милки предпочел бы, чтобы ты никому не говорил, где ты его достал.";
            currentLang.other["wrongmilkiestmilk"] = "Спущённое молочко Милки";
            currentLang.other["wrongmilkiestmilkdsc"] = "Молоко совершенно иного рода. Прямо из источников! Непастеризованное, чуть теплое и более густое, чем обычно. Милки предпочел бы, чтобы ты никому не говорил, где ты его достал. Милки настойчиво просил тебя остаться.";
        }
    }
}
