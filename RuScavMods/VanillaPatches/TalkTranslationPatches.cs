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
                    ["imn"] = "рука",
                    ["rod"] = "руки",
                    ["dat"] = "руке",
                    ["vin"] = "руку",
                    ["tvr"] = "рукой",
                    ["prd"] = "руке",
                },
                ["нога"] = new Dictionary<string, string>
                {
                    ["imn"] = "нога",
                    ["rod"] = "ноги",
                    ["dat"] = "ноге",
                    ["vin"] = "ногу",
                    ["tvr"] = "ногой",
                    ["prd"] = "ноге",
                },
                ["кисть"] = new Dictionary<string, string>
                {
                    ["imn"] = "кисть",
                    ["rod"] = "кисти",
                    ["dat"] = "кисти",
                    ["vin"] = "кисть",
                    ["tvr"] = "кистью",
                    ["prd"] = "кисти",
                },
                ["лапа"] = new Dictionary<string, string>
                {
                    ["imn"] = "лапа",
                    ["rod"] = "лапы",
                    ["dat"] = "лапе",
                    ["vin"] = "лапу",
                    ["tvr"] = "лапой",
                    ["prd"] = "лапе",
                },
                ["грудь"] = new Dictionary<string, string>
                {
                    ["imn"] = "грудь",
                    ["rod"] = "груди",
                    ["dat"] = "груди",
                    ["vin"] = "грудь",
                    ["tvr"] = "грудью",
                    ["prd"] = "груди",
                },
                ["голова"] = new Dictionary<string, string>
                {
                    ["imn"] = "голова",
                    ["rod"] = "головы",
                    ["dat"] = "голове",
                    ["vin"] = "голову",
                    ["tvr"] = "головой",
                    ["prd"] = "голове",
                },
                ["живот"] = new Dictionary<string, string>
                {
                    ["imn"] = "живот",
                    ["rod"] = "живота",
                    ["dat"] = "животу",
                    ["vin"] = "живот",
                    ["tvr"] = "животом",
                    ["prd"] = "животе",
                },
                ["хвост"] = new Dictionary<string, string>
                {
                    ["imn"] = "хвост",
                    ["rod"] = "хвоста",
                    ["dat"] = "хвосту",
                    ["vin"] = "хвост",
                    ["tvr"] = "хвостом",
                    ["prd"] = "хвосте",
                },
            };

            static readonly Dictionary<string, string> KeyReplaces = new Dictionary<string, string>
            {
                { "оя голова", "голова" },
                { "оя грудь", "грудь" },
                { "ой живот", "живот" },
                { "оя рука", "рука" },
                { "оя лапа", "лапа" },
                { "оя нога", "нога" }
            };
            static readonly Dictionary<string, string> FemininePossessive = new Dictionary<string, string>
            {
                ["tvr"] = "моей",
                ["imn"] = "моя",
                ["rod"] = "моей",
                ["dat"] = "моей",
                ["vin"] = "мою",
                ["prd"] = "моей"
            };
            static readonly Dictionary<string, string> MasculinePossessive = new Dictionary<string, string>
            {
                ["imn"] = "мой",
                ["tvr"] = "моим",
                ["rod"] = "моего",
                ["dat"] = "моему",
                ["vin"] = "мой",
                ["prd"] = "моём"
            };

            public static string Get(string shortName, string caseCode)
            {
                string key = shortName.ToLower();

                if (KeyReplaces.ContainsKey(key))
                {
                    key = KeyReplaces[key];
                }

                if (string.IsNullOrEmpty(caseCode)) caseCode = "imn";

                if (Table.TryGetValue(key, out Dictionary<string, string> forms) && forms.TryGetValue(caseCode, out string word))
                {
                    return word;
                }

                return shortName;
            }

            public static string Agree(string shortName, string caseCode, string possession)
            {
                string key = shortName.ToLower();

                if (KeyReplaces.ContainsKey(key))
                {
                    key = KeyReplaces[key];
                }

                if (key != "живот" && key != "хвост") return possession;

                if (string.IsNullOrEmpty(caseCode)) caseCode = "imn";

                if (MasculinePossessive.TryGetValue(caseCode, out string newPosession))
                    return newPosession;

                return possession;
            }
        }

        static readonly Regex LimbTagRegex = new Regex(@"(?:(моей|моя)\s+)?<limb:(?:(\w+))?>", RegexOptions.Compiled);

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
                    string possessive = m.Groups[1].Success ? m.Groups[1].Value : null;
                    string caseCode = m.Groups[2].Success ? m.Groups[2].Value : "";

                    string declined = Decline(limb, caseCode);

                    if (possessive != null)
                    {
                        string agreed = AgreePossessive(limb, caseCode, possessive);
                        return agreed + " " + declined;
                    }
                    return declined;
                }));
            }

            lines = newLines;
        }

        private static string AgreePossessive(Limb limb, string caseCode, string originalPossessive) => LimbDeclensions.Agree(limb.shortName, caseCode, originalPossessive);

        private static string Decline(Limb limb, string caseCode) => LimbDeclensions.Get(limb.shortName, caseCode);
    }
}
