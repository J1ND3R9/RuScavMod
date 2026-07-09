using HarmonyLib;
using System.Reflection;

namespace RuScavMods.ModPatches.QoLUnknown.SaveMenu
{
    [HarmonyPatch]
    internal class CreateSeedInputTextPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(QoLTranslator.SaveMenuPatcher, "CreateSeedInputText");
        public static void Prefix(ref string value)
        {
            if (value == "Seed")
                value = "Сид";
        }
    }
}