using BepInEx;
using CasualtyPsychology.Core;
using CasualtyPsychology.Core.Traits;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.MakeshiftProsthetics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.ModPatches.Psychology
{
    internal class PsychologyTranslator
    {
        private readonly static List<PatchClassProcessor> PsychologyPatches = new List<PatchClassProcessor>();

        public static void Translate()
        {
            LoadFile();
            RegisterPatches();
            foreach (PatchClassProcessor processor in PsychologyPatches)
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

        private static void LoadFile()
        {
            MethodInfo method = AccessTools.Method(typeof(PsychologyDialogue), "LoadLocalePayload", new[] { typeof(JObject), typeof(bool), typeof(bool) });
            string translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "psychology.json");
            if (!File.Exists(translatePath)) return;
            string json = File.ReadAllText(translatePath);
            JObject translationObject = JObject.Parse(json);
            var args = new object[] { translationObject, true, false };
            method.Invoke(null, args);
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.psychologytranslation");
            PsychologyPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(PsychologyPatches)),
            });
        }
    }

    [HarmonyPatch]
    class PsychologyPatches
    {
        [HarmonyPatch(typeof(SurvivorDossier), "GetMemoryShortDesc")]
        [HarmonyPostfix]
        static void GetMemoryShortPostfix(ref string __result)
        {
            switch (__result)
            {
                case "Recorded episode":
                    __result = "Записанный эпизод";
                    break;
                case "Cadaver encounter & autonomic shock":
                    __result = "Контакт с трупом и вегетативный шок";
                    break;
                case "Narrow mechanical trap evasion":
                    __result = "Уклонение от узкой механической ловушки";
                    break;
                case "Mechanical trap injury & laceration":
                    __result = "Травма и рваная рана, вызванные механической ловушкой";
                    break;
                case "Beast carcass encounter":
                    __result = "Встреча с тушей зверя";
                    break;
                case "Neutralized subterranean hostile":
                    __result = "ейтрализованный подземный противник";
                    break;
                case "Cadaver destruction impact":
                    __result = "Последствия уничтожения трупа";
                    break;
                case "Survivor death shock":
                    __result = "Шок от смерти выжившего";
                    break;
                case "Beast carcass destruction":
                    __result = "Уничтожение туш животных";
                    break;
                case "Holding cell confinement & sensory isolation":
                    __result = "Содержание в камере содержания и сенсорная изоляция";
                    break;
                case "Caloric intake & nourishment":
                    __result = "Калорийность и питание";
                    break;
                case "Acute musculoskeletal trauma":
                    __result = "Острая травма опорно-двигательного аппарата";
                    break;
                case "Mechanical trap evasion":
                    __result = "Уклонение от механических ловушек";
                    break;
                case "Severe trap mangling":
                    __result = "Сильные повреждения ловушек";
                    break;
                case "Visceral horror encounter":
                    __result = "Жуткая, пробирающая до дрожи встреча";
                    break;
                case "Peaceful survivor interaction":
                    __result = "Мирное взаимодействие выживших";
                    break;
                case "Field fabrication failure":
                    __result = "Ужасная сборка в полевых условиях";
                    break;
                case "Field assembly success":
                    __result = "Успешная сборка в полевых условиях";
                    break;
                case "Item abandonment & loss":
                    __result = "Оставление и утрата предметов";
                    break;
                case "Item recovery":
                    __result = "Восстановление предметов";
                    break;
                case "Subterranean hostile kill":
                    __result = "Убийство врага под землей";
                    break;
                case "Detention wing neural scan":
                    __result = "Нейросканирование блока содержания под стражей";
                    break;
                case "Holding cell sensory isolation":
                    __result = "Сенсорная изоляция в камере содержания";
                    break;
                case "Traumatic survival episode":
                    __result = "Травматический эпизод выживания";
                    break;
                case "Stabilized survival episode":
                    __result = "Стабилизированный эпизод выживания";
                    break;
            }
        }

        [HarmonyPatch(typeof(TraitProfile), nameof(TraitProfile.GetStrengthPrefix))]
        [HarmonyPostfix]
        static void GetStrengthPostfix(ref string __result)
        {
            switch (__result)
            {
                case "Mild":
                    __result = "Мягк.";
                    break;
                case "Moderate":
                    __result = "Умеренн.";
                    break;
                case "Acute":
                    __result = "Остр.";
                    break;
                case "Tenacious":
                    __result = "Крепк.";
                    break;
                case "Severe":
                    __result = "Серьез.";
                    break;
                case "Basal":
                    __result = "Фундамент.";
                    break;
            }
        }
        [HarmonyPatch(typeof(SurvivorDossier), nameof(SurvivorDossier.GetTraitDisplayName))]
        [HarmonyPostfix]
        static void TraitDisplayNamePostfix(TraitType trait, ref string __result)
        {
            switch (trait)
            {
                case TraitType.Resilience:
                    __result = "Устойчивость";
                    break;
                case TraitType.Impulsivity:
                    __result = "Импульсивность";
                    break;
                case TraitType.Aggression:
                    __result = "Агрессивность";
                    break;
                case TraitType.Empathy:
                    __result = "Эмпатичность";
                    break;
                case TraitType.SensationSeeking:
                    __result = "Поиск острых ощущений";
                    break;
                case TraitType.AttachmentTendency:
                    __result = "Склонность к привязанности";
                    break;
                case TraitType.PainSensitivity:
                    __result = "Болевая чувствительность";
                    break;
                case TraitType.MasochisticTendency:
                    __result = "Склонность к мазохизму";
                    break;
                case TraitType.Neuroticism:
                    __result = "Невротизм";
                    break;
                case TraitType.Optimism:
                    __result = "Оптимизм";
                    break;
                case TraitType.Pessimism:
                    __result = "Пессимизм";
                    break;
                case TraitType.Fatalism:
                    __result = "Фатализм";
                    break;
                case TraitType.Cynicism:
                    __result = "Цинизм";
                    break;
                case TraitType.Nihilism:
                    __result = "Нигилизм";
                    break;
                case TraitType.Stoicism:
                    __result = "Стоицизм";
                    break;
            }
        }
    }
}
