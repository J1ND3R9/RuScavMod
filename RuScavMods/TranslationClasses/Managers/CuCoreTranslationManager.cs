using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;


namespace RuScavMods.TranslationClasses.Managers
{
    internal class CuCoreTranslationManager
    {
        public static Dictionary<string, Dictionary<string, string>> CuCoreTranslations { get; } = new Dictionary<string, Dictionary<string, string>>();
        public static void Add(CuCoreTranslationFile file)
        {
            if (file == null) throw new Exception("Файл перевода пустой или не десериализован.");
            Merge("item", file.Items);
            Merge("building", file.Buildings);
            Merge("moodle", file.Moodles);
            Merge("other", file.Others);
            Merge("log", file.Logs);
            Merge("command", file.Commands);
            Merge("option", file.Options);
            Merge("liquid", file.Liquids);
            Merge("title", file.Titles);
            SendTalksToNative(file.NativeTalks);
            SendAlertsToNative(file.NativeAlerts);
            SendMoodlesToNative(file.NativeMoodles);
            SendNoRegexToNative(file.NativeBuildings, NativeTranslationManager.Buildings);
            SendNoRegexToNative(file.NativeWoundViews, NativeTranslationManager.WoundViews);
            SendNoRegexToNative(file.NativeMinigames, NativeTranslationManager.Minigames);
        }
        private static void Merge(string key, Dictionary<string, string> source)
        {
            if (source == null) 
                return;

            if (!CuCoreTranslations.ContainsKey(key))
                CuCoreTranslations[key] = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> pair in source)
                CuCoreTranslations[key][pair.Key] = pair.Value;
        }

        private static void SendTalksToNative(Dictionary<string, string> nativeTalks)
        {
            if (nativeTalks == null) return;
            NativeTranslationManager.AddNativeTalks(nativeTalks);
        }
        private static void SendAlertsToNative(Dictionary<string, string> nativeAlerts)
        {
            if (nativeAlerts == null) return;
            NativeTranslationManager.AddNativeAlerts(nativeAlerts);
        }

        private static void SendMoodlesToNative(Dictionary<string, string> nativeMoodles)
        {
            if (nativeMoodles == null) return;
            NativeTranslationManager.AddNativeMoodles(nativeMoodles);
        }
        private static void SendNoRegexToNative(Dictionary<string, string> source, Dictionary<string, string> target)
        {
            foreach (KeyValuePair<string, string> item in source)
            {
                target[item.Key] = item.Value;
            }
        }
    }
}
