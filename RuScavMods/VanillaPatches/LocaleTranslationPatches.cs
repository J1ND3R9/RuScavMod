using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
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
            AddCustomQuotes();
            var currentLang = Locale.currentLang;
            if (currentLang == null) return;
            NativeTranslationManager.ApplyLocaleTranslation(currentLang);
            if (Plugin.IsCUCoreLibInstalled)
                CuCoreTranslationManager.ApplyLocaleTranslation(currentLang);
        }
        private static readonly List<string> CustomQuotes = new List<string>
            {
                //"Упал - вставай\nВстал - упай\nУпай - чокопай\nНе суди, не гуди, не будай\nВада буди дабуди дабудай.",
                //"В камине в 6 утра,",
                //"фотография твоя...",
                //"Чтобы поставить на паузу, скажите \"Бурмалда\".",
                //"Экспи не будет счастлив, смирись.",
                //"Это не Малевелон-Крик...",
                //"nah, i'd win.",
                //"ZA WARUDO!!!! TOKI WO TOMARE!!",
                //"Послушай меня, я не хочу быть твоим другом. Я хочу быть твоей тенью.",
                //"Мой любимый исполнитель - \"LAZZY2WICE\". Его треки отличаются агрессией, тяжелыми гитарными риффами и глубоким, мрачным басом. Мой любимый трек \"забракованный\".",
                //"Моя любимая книга - \"Божественная комедия\" Данте Алигьери. Она рассказывает о путешествии через Ад, Чистилище и Рай, и я нахожу её очень вдохновляющей.",
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
                #region Стихи
                "«У Лукоморья дуб зелёный, золотая цепь на дубе том...»",
                "«Я помню чудное мгновенье: передо мной явилась ты, как мимолетное виденье, как гений чистой красоты.»",
                "«Мой дядя самых честных правил, когда не в шутку занемог, он уважать себя заставил и лучше выдумать не мог.»",
                "«Я вас любил: любовь ещё, быть может, в душе моей угасла не совсем; но пусть она вас больше не тревожит; я не хочу печалить вас ничем.»",
                "«Не жалею, не зову, не плачу, все пройдет, как с белых яблонь дым.»",
                "«Ты жива ещё, моя старушка? Жив и я. Привет тебе, привет!»",
                "«Если крикнет рать святая: \"Кинь ты Русь, живи в раю!\". Я скажу: \"Не надо рая, дайте родину мою\"»",
                #endregion
                #region Остальное
                "О, тебе нравится Экспи? Тогда жду в личных сообщениях топ 100 фактов о нём.",
                "Может быть по фентанилу?",
                "Сосал?",
                #endregion
            };
        private static void AddCustomQuotes()
        {
            Language currentLang = Locale.currentLang;
            foreach (var quote in CustomQuotes)
                currentLang.pauseQuotes.Add(quote);
        }

        [HarmonyPatch(nameof(Locale.GetPauseQuote))]
        [HarmonyPostfix]
        private static void GetPauseQuotePostfix(ref string __result)
        {
            NativeTranslationManager.ApplyPauseQuoteTranslation(ref __result);
        }
    }
}
