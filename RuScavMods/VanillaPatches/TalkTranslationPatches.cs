using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(Talker))]
    [HarmonyPriority(Priority.Last)]
    internal class TalkTranslationPatches
    {
        static class LimbDeclensions
        {
            static readonly Dictionary<string, Dictionary<string, string>> Table = new Dictionary<string, Dictionary<string, string>>
            {
                ["рука"] = new Dictionary<string, string>
                {
                    ["rod"] = "руки",
                    ["dat"] = "руке",
                    ["vin"] = "руку",
                    ["tvr"] = "рукой",
                    ["prd"] = "руке",
                },
                ["нога"] = new Dictionary<string, string>
                {
                    ["rod"] = "ноги",
                    ["dat"] = "ноге",
                    ["vin"] = "ногу",
                    ["tvr"] = "ногой",
                    ["prd"] = "ноге",
                },
                ["кисть"] = new Dictionary<string, string>
                {
                    ["rod"] = "кисти",
                    ["dat"] = "кисти",
                    ["vin"] = "кисть",
                    ["tvr"] = "кистью",
                    ["prd"] = "кисти",
                },
                ["лапа"] = new Dictionary<string, string>
                {
                    ["rod"] = "лапы",
                    ["dat"] = "лапе",
                    ["vin"] = "лапу",
                    ["tvr"] = "лапой",
                    ["prd"] = "лапе",
                },
                ["грудь"] = new Dictionary<string, string>
                {
                    ["rod"] = "груди",
                    ["dat"] = "груди",
                    ["vin"] = "грудь",
                    ["tvr"] = "грудью",
                    ["prd"] = "груди",
                },
                ["голова"] = new Dictionary<string, string>
                {
                    ["rod"] = "головы",
                    ["dat"] = "голове",
                    ["vin"] = "голову",
                    ["tvr"] = "головой",
                    ["prd"] = "голове",
                }
            };

            public static string Get(string shortName, string caseCode)
            {
                string key = shortName.ToLower();
                if (Table.TryGetValue(key, out Dictionary<string, string> forms) && forms.TryGetValue(caseCode, out string word))
                {
                    return word;
                }
                return shortName;
            }
        }

        static readonly Regex LimbTagRegex = new Regex(@"<limb:(?:(\w+))?>", RegexOptions.Compiled);


        [HarmonyPatch(nameof(Talker.Talk), new[] { typeof(string), typeof(Limb), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        private static void TalkPrefix(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            NativeTranslationManager.ApplyTalkTranslation(ref text);
        }

        [HarmonyPatch(nameof(Talker.Talk), new[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        private static void TalkLimbPrefix(ref List<string> lines, Limb limb)
        {
            if (limb == null || lines == null || lines.Count == 0) return;

            bool anyTag = false;
            foreach (string line in lines)
            {
                if (LimbTagRegex.IsMatch(line))
                {
                    anyTag = true;
                    break;
                }
            }
            if (!anyTag) return;

            List<string> newLines = new List<string>(lines.Count);

            foreach (string line in lines)
            {
                newLines.Add(LimbTagRegex.Replace(line, m =>
                {
                    string caseCode = m.Groups[1].Success ? m.Groups[1].Value : "";
                    return Decline(limb, caseCode);
                }));
            }

            lines = newLines;
        }

        private static string Decline(Limb limb, string caseCode) => LimbDeclensions.Get(limb.shortName, caseCode);
    }
}
