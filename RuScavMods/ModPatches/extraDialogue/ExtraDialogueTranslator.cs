using BepInEx;
using extraDialogues;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace RuScavMods.ModPatches.extraDialogue
{
    internal static class ExtraDialogueTranslator
    {
        public static void Translate()
        {
            var translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "extraDialogue.json");
            var json = File.ReadAllText(translatePath);
            var data = JsonConvert.DeserializeObject<DialogueData>(json);
            DialogueManager.Data = data;
        }
    }
}
