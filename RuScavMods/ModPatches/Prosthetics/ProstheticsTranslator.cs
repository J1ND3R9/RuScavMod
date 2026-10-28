using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RuScavMods.ModPatches.Prosthetics
{
    internal class ProstheticsTranslator
    {
        public static readonly Type SurgeryMachineController = AccessTools.TypeByName("SurgeryMachineController");
        public static Dictionary<string, string> ServitorTranslate = new Dictionary<string, string>()
        {
            { "{0}, state my task. I'm ready to serve.", "{0}, назови мою задачу. Я готов служить." },
            { "Can you take me with you? Please.. I am so lonely.", "Можешь взять меня с собой? Пожалуйста... Я так одинок." },
            { "I am not as functional as I was, I feel myself falling appart", "Я уже не так функционален, как раньше. Чувствую, как разваливаюсь." },
            { "Do you seek to stop pain or life? I can grant mercy if so desired", "Ты хочешь прекратить боль или жизнь? Я могу даровать милосердие, если пожелаешь." },
            { "I offer mercy or prolonged suffering, according to your desire.", "Я предлагаю милосердие или продлённое страдание — по твоему желанию." },

            { "{0} is injured, approach for medical aid.", "{0} ранен. Подойди для медицинской помощи." },
            { "Your body is in need of healing. May I be permitted?", "Твоё тело нуждается в обслуживании. Позволишь?" },
            { "Hmmm... You are a mess...", "Хммм... Ты в ужасном состоянии..." },
            { "I cannot dream. I know only waking nightmare.", "Я не могу видеть сны. Я знаю только явь, что хуже кошмара." },
            { "You are not a fit specimen. Amending.", "Ты негодный образец. Исправляю." },
            { "Analysis, less than optimal.", "Анализ, ниже оптимального." },

            { "{0} is augmented, prosthetic maintenance mode available.", "{0} аугментирован. Доступен режим обслуживания протезов." },

            { "My sole purpose is to serve. I cannot serve. Cannot continue. Error!", "Моё единственное предназначение — служить. Я не могу служить. Не могу продолжать. Ошибка!" },
            { "I need... restoration.", "Мне нужно... восстановление." },
            { "Systems failure. Why does it hurt? Help. Help. Help.", "Отказ систем. Почему это больно? Помогите. Помогите. Помогите." },
            { "Reserves, empty. Purpose, void.", "Резервы, пусты. Предназначение, ничто." },
            { "God why have you forsaken me?", "Боже, почему ты оставил меня?" },
            { "I HAVE NO PURPOSE, NO PURPOSE, NO PURPOSE, NO PURPOSE!", "У МЕНЯ НЕТ ПРЕДНАЗНАЧЕНИЯ, НЕТ ПРЕДНАЗНАЧЕНИЯ, НЕТ ПРЕДНАЗНАЧЕНИЯ, НЕТ ПРЕДНАЗНАЧЕНИЯ!" },

            { "There's a relic capsule in the next region, go deeper and find it.", "На следующем слое есть капсула с реликвией. Спускайся глубже и найди её." }
        };

        private readonly static List<PatchClassProcessor> ProstheticsPatches = new List<PatchClassProcessor>();

        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in ProstheticsPatches)
            {
                try
                {
                    processor.Patch();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Ошибка патча: {ex.Message}");
                }
            }
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.prostheticstranslation");

            ProstheticsPatches.AddRange(new[] {
                new PatchClassProcessor(harmony, typeof(ServitorSayPatch))
            });
        }
    }

    [HarmonyPatch]
    internal static class ServitorSayPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(ProstheticsTranslator.SurgeryMachineController, "Say");
        [HarmonyPrefix]
        static void Prefix(ref string line)
        {
            if (ProstheticsTranslator.ServitorTranslate.TryGetValue(line, out string translatedLine))
                line = translatedLine;
        }
    }
}
