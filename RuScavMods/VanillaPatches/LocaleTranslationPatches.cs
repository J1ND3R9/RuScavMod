using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(Locale))]
    [HarmonyPriority(Priority.Last)]
    internal class LocaleTranslationPatches
    {
        [HarmonyPatch(nameof(Locale.LoadLanguage))]
        [HarmonyPostfix]
        private static void LoadLanguagePostfix()
        {
            var currentLang = Locale.currentLang;
            if (currentLang == null) return;
            // AddCustomQuotes();
            NativeTranslationManager.ApplyLocaleTranslation(currentLang);
            NativeTranslationManager.ApplyBuildingTranslation(currentLang);
        }
        private static readonly List<string> FunnyQuotes = new List<string>
        {
            #region "Также попробуйте" рекомендации
            "Также попробуйте Rain World!",
            "Также попробуйте The Binding of Isaac!",
            "Также попробуйте Barotrauma!",
            "Также попробуйте ULTRAKILL!",
            "Также попробуйте Loop Hero!",
            "Также попробуйте Euro Truck Simulator 2!",
            "Также попробуйте Lobotomy Corporation!",
            "Также попробуйте Noita!",
            "Также попробуйте Dead Cells!",
            "Также попробуйте Hollow Knight!",
            "Не пробуйте Dota 2.",
            "Не пробуйте League of Legends.",
            "Не пробуйте Counter-Strike 2.",
            #endregion
            #region Отсылки
            "MANKIND IS DEAD.\nBLOOD IS FUEL.\nHELL IS FULL.",
            "Я не хочу быть жуком.",
            "WHO AM I?!",
            #endregion
            #region Остальное
            "О, тебе нравится Экспи? Тогда жду в личных сообщениях топ 100 фактов о нём.",
            "Может быть по фентанилу?",
            "Сосал?",
            "2+2=7",
            "Класс, прямо как во втором тим фортресс!",
            "Ох, ебать! Обогащенный уран?!",
            "Vore is Hell",
            "Я человек паук. Всем привет, да, я реальный человек паук.",
            "Зверя нет сильней китайца\nСтарый бог сосал нам яйца.",
            #endregion
        };
        private static readonly List<string> SeriousQuotes = new List<string>
        {
            #region Стихи
            "«У Лукоморья дуб зелёный, золотая цепь на дубе том...»",
            "«Я помню чудное мгновенье: передо мной явилась ты, как мимолетное виденье, как гений чистой красоты.»",
            "«Мой дядя самых честных правил, когда не в шутку занемог, он уважать себя заставил и лучше выдумать не мог.»",
            "«Я вас любил: любовь ещё, быть может, в душе моей угасла не совсем; но пусть она вас больше не тревожит; я не хочу печалить вас ничем.»",
            "«Не жалею, не зову, не плачу, все пройдет, как с белых яблонь дым.»",
            "«Ты жива ещё, моя старушка? Жив и я. Привет тебе, привет!»",
            "«Если крикнет рать святая: \"Кинь ты Русь, живи в раю!\". Я скажу: \"Не надо рая, дайте родину мою\"»",
            #endregion
            #region
            "Ты не будешь счастлив.\nТы не будешь счастлив.\nТы не будешь счастлив.\nТы не будешь счастлив.\nТы не будешь счастлив.\nТы не будешь счастлив."
            #endregion
        };
        //private static void AddCustomQuotes()
        //{
        //    Language currentLang = Locale.currentLang;
        //    foreach (var quote in CustomQuotes)
        //        currentLang.pauseQuotes.Add(quote);
        //}

        [HarmonyPatch(nameof(Locale.GetPauseQuote))]
        [HarmonyPostfix]
        private static void GetPauseQuotePostfix(ref string __result)
        {
            const int chance = 5;
            bool isCustomQuote = UnityEngine.Random.Range(0, 101) <= chance;
            if (isCustomQuote)
            {
                if (PluginConfig.EnableFunnyQuotes.Value)
                {
                    bool isSeriousQuote = UnityEngine.Random.Range(0, 2) == 1;
                    if (isSeriousQuote)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, SeriousQuotes.Count);
                        __result = SeriousQuotes[randomIndex];
                    }
                    else
                    {
                        int randomIndex = UnityEngine.Random.Range(0, FunnyQuotes.Count);
                        __result = FunnyQuotes[randomIndex];
                    }
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, SeriousQuotes.Count);
                    __result = SeriousQuotes[randomIndex];
                }
            }
            else
                NativeTranslationManager.ApplyPauseQuoteTranslation(ref __result);
        }
    }
}
