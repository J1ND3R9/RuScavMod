using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;


namespace RuScavMods.TranslationClasses.Managers
{
    internal class CuCoreTranslationManager
    {
        //private static Dictionary<string, string> Items { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Moodles { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Others { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Logs { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Liquids { get; set; } = new Dictionary<string, string>();
        //private static Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
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
            SendToNative(file.NativeTalks);
        }
        private static void Merge(Dictionary<string, string> source)
        {
            if (source == null) return;
            foreach (var pair in source)
                CuCoreTranslations[pair.Key] = pair.Value;
        }

        private static void SendToNative(Dictionary<string, string> nativeTalks)
        {
            if (nativeTalks == null) return;
            NativeTranslationManager.AddNativeTalks(nativeTalks);
        }
    }
}
