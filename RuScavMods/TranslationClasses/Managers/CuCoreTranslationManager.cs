using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;


namespace RuScavMods.TranslationClasses.Managers
{
    internal class CuCoreTranslationManager
    {
        public static Dictionary<string, string> CuCoreTranslations { get; } = new Dictionary<string, string>();
        public static void Add(CuCoreTranslationFile file)
        {
            if (file == null) throw new Exception("Файл перевода пустой или не десериализован.");
            Merge(file.Items);
            Merge(file.Buildings);
            Merge(file.Moodles);
            Merge(file.Others);
            Merge(file.Logs);
            Merge(file.Commands);
            Merge(file.Options);
            Merge(file.Liquids);
            Merge(file.Titles);
            SendTalksToNative(file.NativeTalks);
            SendAlertsToNative(file.NativeAlerts);
            SendNoRegexToNative(file.NativeBuildings, NativeTranslationManager.Buildings);
        }
        private static void Merge(Dictionary<string, string> source)
        {
            if (source == null) return;
            foreach (var pair in source)
                CuCoreTranslations[pair.Key] = pair.Value;
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
        private static void SendNoRegexToNative(Dictionary<string, string> source, Dictionary<string, string> target)
        {
            foreach (var item in source)
            {
                target[item.Key] = item.Value;
            }
        }
    }
}
