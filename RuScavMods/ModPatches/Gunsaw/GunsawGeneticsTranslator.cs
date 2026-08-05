using BepInEx;
using GunsawGenetics;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.Gunsaw
{
    public class GunsawTranslationDef
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

    }
    internal static class GunsawGeneticsTranslator
    {
        private readonly static List<PatchClassProcessor> GGPatches = new List<PatchClassProcessor>();
        public static Dictionary<string, string> Translations;
        private static readonly FieldInfo StringsField = AccessTools.Field(typeof(GunsawGenetics.Localization), "Strings");

        public static TMP_FontAsset _russianFont;

        //private static readonly Dictionary<string, AbilityEntry[]> AbilityEntriesTranslation = new Dictionary<string, AbilityEntry[]>
        //{
        //    "Velvet", 
        //};
        public static void Translate()
        {
            if (Translations == null)
                FillTranslatedStrings();
            FindFont();
            RegisterPatches();
            foreach (PatchClassProcessor processor in GGPatches)
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
            ApplyTranslations();
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.gunsawgeneticstranslate");
            GGPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(LoadLanguagePatch)),
                new PatchClassProcessor(harmony, typeof(FindFontPatch)),
                new PatchClassProcessor(harmony, typeof(ApplyFontPatch))
            });
        }

        public static void ApplyTranslations()
        {
            Dictionary<string, string> strings = (Dictionary<string, string>)StringsField.GetValue(null);
            if (strings == null)
                return;

            foreach (KeyValuePair<string, string> pair in Translations)
            {
                strings[pair.Key] = pair.Value;
            }
            AddTextEffectPresets();
            ApplySettingsTranslations();
        }
        private static void FillTranslatedStrings()
        {
            string translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "gunsaw.json");
            if (!File.Exists(translatePath)) return;
            string json = File.ReadAllText(translatePath);
            JObject jObject = JObject.Parse(json);
            Translations = jObject["translations"]?.ToObject<List<GunsawTranslationDef>>()?.ToDictionary(x => x.Key, x => x.Value) ?? throw new Exception("Секция translations не найдена или пуста.");
        }

        private static void AddTextEffectPresets()
        {
            var textEffectPresets = TextEffectPresets.All;

            textEffectPresets["маскировку"] = TextEffectPresets.All["cloaking"];
            textEffectPresets["Обожание"] = TextEffectPresets.All["Idolatry"];
            textEffectPresets["Высокомерие"] = TextEffectPresets.All["Arrogance"];

            textEffectPresets["гордыню"] = TextEffectPresets.All["Prideful"];

            textEffectPresets["малой паутины"] = TextEffectPresets.All["small web"];
            textEffectPresets["длинной паутины"] = TextEffectPresets.All["long web"];
            textEffectPresets["Психопатия"] = TextEffectPresets.All["psychopathy"];
            textEffectPresets["каннибала"] = TextEffectPresets.All["cannibal"];

            textEffectPresets["Питание плотью"] = TextEffectPresets.All["Feeding"];
            textEffectPresets["Плотоядный"] = TextEffectPresets.All["Carnivore"];

            textEffectPresets["рассинхрона"] = TextEffectPresets.All["DESYNC"];

            textEffectPresets["Левитация"] = TextEffectPresets.All["Levitation"];
            textEffectPresets["Реформация"] = TextEffectPresets.All["Reformation"];
            textEffectPresets["Обмен сознанием"] = TextEffectPresets.All["Mindswap"];
            textEffectPresets["Опустошение"] = TextEffectPresets.All["Hollow"];
        }

        private static void ApplySettingsTranslations()
        {
            Language currentLang = Locale.currentLang;
            if (((currentLang != null) ? currentLang.other : null) == null)
            {
                return;
            }
            Locale.currentLang.other["gamesetgunsaw_abilitykeybind"] = "Кнопка активного умения";
            Locale.currentLang.other["gamesetgunsaw_abilitykeybinddsc"] = "Универсальная кнопка для активации способностей видов.";
            Locale.currentLang.other["gamesetgunsaw_disableexpmodifiers"] = "Отключить модификаторы опыта";
            Locale.currentLang.other["gamesetgunsaw_disableexpmodifiersdsc"] = "Отключает все изменения скорости получения опыта для каждого вида. Представители каждого вида будут получать STR, RES и INT с обычной скоростью, а отображение модификаторов будет скрыто.";
            Locale.currentLang.other["gamesetgunsaw_mercenary_levitation"] = "Наёмник: Левитация";
            Locale.currentLang.other["gamesetgunsaw_mercenary_levitationdsc"] = "Key used to grab objects with Mercenary's Levitation chip.";
            Locale.currentLang.other["gamesetgunsaw_mercenary_reformation"] = "Наёмник: Реформация";
            Locale.currentLang.other["gamesetgunsaw_mercenary_reformationdsc"] = "Key used to Reform during Mercenary's death sequence.";
            Locale.currentLang.other["gamesetgunsaw_recolorenabled"] = "Смена цвета в меню осмотра";
            Locale.currentLang.other["gamesetgunsaw_recolorenableddsc"] = "Включает цветовую схему в меню осмотра для каждого вида.";
            Locale.currentLang.other["gamesetgunsaw_texteffects"] = "Эффект текстов";
            Locale.currentLang.other["gamesetgunsaw_texteffectsdsc"] = "Включает анимированные цвета или анимации для {Ключевых} статусных текстов. Если эта функция отключена, ключевые слова будут подчеркиваться.";
            Locale.currentLang.other["gamesetgunsaw_tendrilmode"] = "Режим щупалец";
            Locale.currentLang.other["gamesetgunsaw_tendrilmodedsc"] = "Включает показ конечностей-щупалец Вельвет в меню осмотра.";
            Locale.currentLang.other["gamesetgunsaw_language"] = "Язык мода";
            Locale.currentLang.other["gamesetgunsaw_languagedsc"] = "Лучше всего это не трогать и оставить RMT, т.к. я че, зря переводил что-ли? Выбор русского подгрузит другой недописанный файл.";
            Locale.currentLang.other["gamesetgunsaw_languageenglish"] = "RMT";
            Locale.currentLang.other["gamesetgunsaw_languagerussian"] = "Русский";
            Locale.currentLang.other["gamesetgunsaw_languagechinese"] = "Упрощённый китайский";

            for (int i = 1; i <= 9; i++)
            {
                Locale.currentLang.other[string.Format("gamesetgunsaw_spriteswitch_slot{0}", i)] = string.Format("Смена спрайта: Слот {0}", i);
                Locale.currentLang.other[string.Format("gamesetgunsaw_spriteswitch_slot{0}dsc", i)] = string.Format("Клавиша для превью спрайта в слоте {0} (дебаг/тест).", i);
            }
            Locale.currentLang.other["gamesetgunsaw_unlock_experiment"] = "Разблокировать Эксперимента";
            Locale.currentLang.other["gamesetgunsaw_unlock_experimentdsc"] = "Вручную открывает Эксперимента без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_orange"] = "Разблокировать Оранжа";
            Locale.currentLang.other["gamesetgunsaw_unlock_orangedsc"] = "Вручную открывает Оранжа без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_milky"] = "Разблокировать Милки";
            Locale.currentLang.other["gamesetgunsaw_unlock_milkydsc"] = "Вручную открывает Милки без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_leapy"] = "Разблокировать Липи";
            Locale.currentLang.other["gamesetgunsaw_unlock_leapydsc"] = "Вручную открывает Липи без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_chompy"] = "Разблокировать Чомпи";
            Locale.currentLang.other["gamesetgunsaw_unlock_chompydsc"] = "Вручную открывает Чомпи без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_roza"] = "Разблокировать Розу";
            Locale.currentLang.other["gamesetgunsaw_unlock_rozadsc"] = "Вручную открывает Розу без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_voyager"] = "Разблокировать Вояджера";
            Locale.currentLang.other["gamesetgunsaw_unlock_voyagerdsc"] = "Вручную открывает Вояджера без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_baron"] = "Разблокировать Барона";
            Locale.currentLang.other["gamesetgunsaw_unlock_barondsc"] = "Вручную открывает Барона без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_chik"] = "Разблокировать Чики";
            Locale.currentLang.other["gamesetgunsaw_unlock_chikdsc"] = "Вручную открывает Чики без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_crystal"] = "Разблокировать Кристалл";
            Locale.currentLang.other["gamesetgunsaw_unlock_crystaldsc"] = "Вручную открывает Кристалл без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_velvet"] = "Разблокировать Вельвет";
            Locale.currentLang.other["gamesetgunsaw_unlock_velvetdsc"] = "Вручную открывает Вельвет без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_icke"] = "Разблокировать Ики";
            Locale.currentLang.other["gamesetgunsaw_unlock_ickedsc"] = "Вручную открывает Ики без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_shelly"] = "Разблокировать Шелли";
            Locale.currentLang.other["gamesetgunsaw_unlock_shellydsc"] = "Вручную открывает Шелли без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_dune"] = "Разблокировать Дюну";
            Locale.currentLang.other["gamesetgunsaw_unlock_dunedsc"] = "Вручную открывает Дюну без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_albino"] = "Разблокировать Альбиноса";
            Locale.currentLang.other["gamesetgunsaw_unlock_albinodsc"] = "Вручную открывает Альбиноса без выполнения условий для разблокировки";

            Locale.currentLang.other["gamesetgunsaw_unlock_mercenary"] = "Разблокировать Наёмника";
            Locale.currentLang.other["gamesetgunsaw_unlock_mercenarydsc"] = "Вручную открывает Наёмника без выполнения условий для разблокировки";
        }

        public static void FindFont()
        {
            if (GunsawGeneticsTranslator._russianFont == null)
            {
                GunsawGeneticsTranslator._russianFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                    .FirstOrDefault(f => f.name == "Retro GamingPix");
            }
        }
    }

    [HarmonyPatch(typeof(GunsawGenetics.Localization))]
    internal static class LoadLanguagePatch
    {
        [HarmonyPatch(nameof(GunsawGenetics.Localization.LoadLanguage))]
        [HarmonyPostfix]
        static void Postfix()
        {
            GunsawGeneticsTranslator.ApplyTranslations();
        }
    }

    [HarmonyPatch(typeof(GunsawGenetics.SpeciesPickerUI))]
    internal static class FindFontPatch
    {
        [HarmonyPatch("FindFont")]
        [HarmonyPostfix]
        static void Postfix(ref TMP_FontAsset ___gameFont)
        {
            GunsawGeneticsTranslator.FindFont();
            if (GunsawGeneticsTranslator._russianFont != null)
                ___gameFont = GunsawGeneticsTranslator._russianFont;

            Plugin.Log.LogInfo("Подменён шрифт для Gunsaw Genetics SpeciesPickerUI");
        }
    }

    [HarmonyPatch(typeof(GunsawGenetics.SpeciesInfoPanel))]
    internal static class ApplyFontPatch
    {
        [HarmonyPatch("ApplyFont")]
        [HarmonyPostfix]
        static void Postfix(TextMeshProUGUI tmp, bool isHeader)
        {
            GunsawGeneticsTranslator.FindFont();

            if (GunsawGeneticsTranslator._russianFont != null && isHeader)
                tmp.font = GunsawGeneticsTranslator._russianFont;

            Plugin.Log.LogInfo("Подменён шрифт для Gunsaw Genetics ApplyFont");
        }
    }
}
