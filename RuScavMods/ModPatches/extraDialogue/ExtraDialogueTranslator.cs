using BepInEx;
using extraDialogue;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RuScavMods.ModPatches.extraDialogue
{
    internal static class ExtraDialogueTranslator
    {
        const bool IsOld = true;
        public static void Translate()
        {
            string translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "extraDialogue.json");
            if (IsOld)
            {
                JObject jobject = JObject.Parse(File.ReadAllText(translatePath));
                Functions.jsonText = jobject;
                Functions.errorString = "no error";
            }
            //var json = File.ReadAllText(translatePath);
            //var data = JsonConvert.DeserializeObject<DialogueData>(json);
            //DialogueManager.Data = data;
        }
    }
}
