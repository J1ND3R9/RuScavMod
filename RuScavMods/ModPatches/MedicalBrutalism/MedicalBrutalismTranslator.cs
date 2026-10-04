using Alexx__s_medicals;
using HarmonyLib;
using MedicalBrutalismMod;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.CUCoreLib;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.ModPatches.MedicalBrutalism
{
    internal class MedicalBrutalismTranslator
    {
        private readonly static List<PatchClassProcessor> MBPatches = new List<PatchClassProcessor>();
        public readonly static Dictionary<string, string> MBTranslates = new Dictionary<string, string>
        {
            { "Challenges", "Челленджи" },
            { "Changelog", "Обновления" },
            { "Bestiary", "Бестиарий" },
            { "Nexus Mods Page", "Страница мода" },
            { "Config", "Конфиг" },
            { "Run Records", "Забеги" },
            { "< Back", "Назад" },

            { "BESTIARY", "БЕСТИАРИЙ" },
            { "No moodle data available.", "Нет информации о мудле." },
            { "(No description available.)", "(Нет описания)" },

            { "CHANGELOG", "ОБНОВЛЕНИЯ" },
            { "No changelog entries found.", "Нет информации об обновлениях." },

            { "CHALLENGES", "ЧЕЛЛЕНДЖИ" },
            { "Pick a challenge to begin a run with. ", "Выбери челлендж, с которого начнешь забег. " },
            { "No challenges registered.", "Нет зарегистрированных челленджей." },
            { "Speedrun", "Спидран" },
            { "Submit", "Отправка" },

            { "RUN RECORDS", "ЗАБЕГИ" },
            { "Clear", "Очистить" },
            { "No death records yet.\nPlay a run and die, and it will appear here.", "Нет записей.\nНачни забег и умри и запись будет тут!" },

            { "Bandage", "Бинт" },
            { "Coagulant", "Коагулянт" },
            { "Water", "Вода" },
            { "Food", "Еда" },
            { "Bruise kit", "Набор для лечения синяков" },

            { "Corpse", "Труп" },
            { "Unconscious survivor", "Выживший без сознания" },

            { "Dyatlov Pass <frostbite.png>", "Перевал Дятлова <frostbite.png>" },
            { "Your life pod crashes on the\ncold side of Grey Planet,\nhorribly injuring you on impact. Already frostbitten\nat the extremities\nand barely conscious,\nevery step costs you blood and sanity.\nDifficulty:\n<star.png><star.png><star.png><star.png><srat.png>", "Твоя капсула разбилась на\nхолодной стороне Серой планеты,\nужасно искалечив тебя. С обмороженными\nконечностями и на грани потери сознания,\nкаждый шаг стоит тебе крови и рассудка.\nСложность:\n<star.png><star.png><star.png><star.png><srat.png>" },
            { "Decayed Winter <systemicinfection2.png>", "Увядающая Зима <systemicinfection2.png>" },
            { "For experimental purposes, you were sent to one\nof Grey Planet's poles.\nThe first breath you took felt horrible,\nlike something had gotten into your lungs.\nScavengers here are more aggressive than normal.\nDifficulty:\n<star.png><star.png><srat.png><srat.png><srat.png>", "В рамках эксперимента тебя отправили на\nодин из полюсов Серой планеты.\nПервый же вдох оказался ужасным,\nсловно что-то попало тебе в лёгкие.\nМестные падальщики агрессивнее обычного.\nСложность:\n<star.png><star.png><srat.png><srat.png><srat.png>" },

            { "Removed", "Удалено" },
            { "Failing ({0})", "Отказ ({0})" },

            { "Internal parasites ({0})", "Внутренние паразиты ({0})" },
            { "There are parasites inside your intestines.", "Внутри твоих кишок паразиты." },
            { "Remove parasites", "Удалить паразитов" },
            { "Bowel", "Кишечник" },

            { "This kidney has been surgically removed.", "Эта почка была хирургически удалена." },
            { "The kidney is failing and needs treatment.", "Почка отказывает и нуждается в лечении." },
            { "Remove kidney", "Удалить почку" },
            { "Left kidney", "Левая почка" },
            { "Right kidney", "Правая почка" },

            { "Right Kidney", "Правая почка" },
            { "Left Kidney", "Левая почка" },

            { "The liver is badly damaged and needs to be cut clean.", "Печень сильно повреждена и требует иссечения." },
            { "Cut off infected area", "Отрезать заражённую область" },
            { "Disfunction", "Дисфункция" },
            { "The liver is struggling.", "Печень с трудом справляется." },
            { "Liver", "Печень" },

            { "Bilateral pneumothorax", "Двусторонний пневмоторакс" },
            { "Both lungs are punctured. Breathing is nearly impossible.", "Оба лёгких пробиты. Дышать почти невозможно." },
            { "Stitch lung", "Зашить лёгкое" },
            { "Pneumothorax", "Пневмоторакс" },
            { "One lung is punctured. Breathing is difficult.", "Одно лёгкое пробито. Дышать трудно." },
            { "Hemothorax ({0}%)", "Гемоторакс ({0}%)" },
            { "Blood is pooling around the lungs.", "Кровь скапливается вокруг лёгких." },
            { "Drain hemothorax", "Дренировать гемоторакс" },
            { "Pulmonary embolism", "Тромбоэмболия лёгочной артерии" },
            { "A clot is lodged in the lungs.", "Тромб застрял в лёгких." },
            { "Respiratory arrest", "Остановка дыхания" },
            { "You are not breathing.", "Ты не дышишь." },
            { "Lung", "Лёгкое" },
            { "Lungs", "Лёгкие" },

            { "Unknown", "Неизвестно" },

            { "Cardiac arrest", "Остановка сердца" },
            { "Asystole, your heart is not beating anymore.", "Асистолия, твоё сердце больше не бьётся." },
            { "V-Fibrillation", "Фибрилляция желудочков" },
            { "Life threatening, chaotic heart rhythm. Asystole is soon to follow.", "Угрожающий жизни хаотичный ритм сердца. Вскоре последует асистолия." },
            { "V-Tachycardia", "Желудочковая тахикардия" },
            { "Severe irregular heartbeat. Can be defibrillated.", "Тяжёлый нерегулярный ритм сердца. Поддаётся дефибрилляции." },
            { "Arrhythmia", "Аритмия" },
            { "Abnormal heart rhythm. Can be defibrillated.", "Аномальный ритм сердца. Поддаётся дефибрилляции." },
            { "Infarction ({0}%)", "Инфаркт ({0}%)" },
            { "Blood clot blocked off heart's major artery causing it to necrose.", "Тромб перекрыл крупную артерию сердца, вызывая его некроз." },
            { "Tamponade ({0}%)", "Тампонада ({0}%)" },
            { "Blood is compressing the heart.", "Кровь сдавливает сердце." },
            { "Drain tamponade", "Дренировать тампонаду" },
            { "Heart", "Сердце" },

            { "THORAX SURGERY", "ХИРУРГИЯ ГРУДНОЙ КЛЕТКИ" },
            { "ABDOMEN SURGERY", "ХИРУРГИЯ БРЮШНОЙ ОБЛАСТИ" },

            { "Click an organ to inspect it.", "Нажми на орган, чтобы осмотреть его" },

            { "Select an organ.", "Выбери орган." },
            { "Click on organ to view its details.", "Нажми на орган, чтобы просмотреть его детали." },

            { "─── GLOBAL ───", "─── ГЛОБАЛЬНОЕ ───" },
            { "Transgender mode\n(support for <color=#abe7ff>n<color=#ffc4f0>y<color=white>a</color>r</color>u</color>'s transition)\n<color=grey>(requires makeshift prosthetics)</color>", "Режим трансгендера\n(поддержка перехода <color=#abe7ff>n<color=#ffc4f0>y<color=white>a</color>r</color>u</color>)\n<color=grey>(требуется makeshift prosthetics)</color>" },

            { "─── LIMB EFFECTS ───", "─── ЭФФЕКТЫ КОНЕЧНОСТЕЙ ───" },
            { "Arterial bleeding", "Артериальное кровотечение" },
            { "Bruises", "Ушибы" },
            { "Lacerations", "Рваные раны" },
            { "Frostbite", "Обморожения" },
            { "Parasites", "Паразиты" },
            { "Burning & grease", "Ожоги и жир" },
            { "Crooked bones", "Искривлённые кости" },

            { "─── ORGAN SYSTEMS ───", "─── СИСТЕМА ОРГАНОВ ───" },
            { "Blood iron", "Железо в крови" },
            { "Contusions", "Контузия" },
            { "Acid–base balance (pH)", "Кислотно-щёлочный баланс (pH)" },
            { "DNA damage", "Повреждение ДНК" },
            { "Liver failure", "Отказ печени" },
            { "Kidney failure", "Отказ почки" },
            { "Heart attack (infarction)", "Сердечный приступ (инфаркт)" },
            { "Heart tamponade", "Тампонада сердца" },
            { "Toxin accumulation", "Накопление токсинов" },
            { "Systemic infection", "Системная инфекция" },
            { "Hemolytic reaction", "Гемолитическая реакция" },

            { "─── MISC CONDITIONS ───", "─── ДОП. СОСТОЯНИЯ ───" },
            { "Back pain", "Боль в спине" },
            { "Insomnia", "Инсомния" },
            { "Lazarus syndrome", "Синдром Лазаря" },
            { "Cold & flu", "Простуда и грипп" },
            { "Epilepsy / seizures", "Эпилепсия / судороги" },
            { "Spinal rupture", "Разрыв позвоночника" },

            { "─── MISC FEATURES ───", "─── ДОП. ФУНКЦИИ ───" },
            { "Wounded traders", "Раненные трейдеры" },

            { "Disable any mod features you don't want.", "Отключи любую функцию мода которая тебе не нужна." }
        };

        private readonly static Dictionary<string, (string, string)> MBFallbacksTranslated = new Dictionary<string, (string, string)>
        {
            { "artery", ("Артериальное кровотечение", "Артерия перерезана. Рана будет кровоточить, пока её как следует не закрыть.") },
            { "backpain", ("Боль в спине", "Постоянная боль в пояснице, вызванная перенапряжением или плохим сном.") },
            { "lowiron", ("Дефицит железа", "Низкий уровень железа в крови. Утомляемость, слабость и риск потери сознания.") },
            { "highiron", ("Гемохроматоз", "Избыток железа в крови. Боль в суставах и повреждение органов.") },
            { "acidosis", ("Ацидоз", "Кровь слишком кислая. Утомляемость, тяжёлое дыхание и повреждение органов.") },
            { "alkalosis", ("Алкалоз", "Кровь слишком щелочная. Мышечные подёргивания, судороги и спутанность сознания.") },
            { "bruise", ("Ушиб", "Ушибленная конечность. Больно двигать, мышцы ослаблены.") },
            { "contusion1", ("Лёгкая контузия", "Лёгкая травма мозга. Шум в ушах и периодическая дезориентация.") },
            { "contusion2", ("Контузия", "Умеренная травма мозга. Частая дезориентация и головные боли.") },
            { "contusion3", ("Разжижение мозга", "Катастрофическое повреждение мозга. Высокий риск инсульта и потеря моторного контроля.") },
            { "dnadamage", ("Повреждение ДНК", "Повреждённая ДНК. Иммунная система и регенерация нарушены.") },
            { "fewer", ("Простуда", "Ты простудился. Чихание, жар и слабость.") },
            { "frostbite", ("Обморожение", "Обморожение конечности. Онемение, боль и медленный некроз.") },
            { "hemolytic", ("Гемолитическая реакция", "Твоё тело атакует собственные клетки крови.") },
            { "insomnia", ("Бессонница", "Ты не можешь спать. Мысли несутся, а тело отказывается отдыхать.") },
            { "laceration", ("Рваная рана", "Глубокая рана. Кровоточит и склонна к заражению.") },
            { "lazarus", ("Синдром Лазаря", "По всем признакам ты должен быть мёртв. Но ты не мёртв.") },
            { "liverfailure1", ("Дисфункция печени", "Твоя печень отказывает. Тошнота, боль и слабость.") },
            { "liverfailure2", ("Печёночная недостаточность", "Тяжёлая печёночная недостаточность. Желтизна, истощение, усиливающаяся слабость.") },
            { "liverfailure3", ("Терминальная печёночная недостаточность", "Твои органы отключаются. Смерть близка.") },
            { "otitis", ("Отит", "Инфекция уха. Боль, выделения и приглушённый слух.") },
            { "parasites", ("Паразиты", "Паразиты пожирают конечность. Больно и разрушительно.") },
            { "pneumothorax", ("Пневмоторакс", "Спавшееся лёгкое. Дышать больно и трудно.") },
            { "bilateralpneumothorax", ("Двусторонний пневмоторакс", "Оба лёгких спавшиеся. Дышать почти невозможно.") },
            { "spinalrupture", ("Разрыв спинного мозга", "Парализован ниже пояса. Возможно, ты больше никогда не пойдёшь.") },
            { "systemicinfection1", ("Системная инфекция I", "Инфекция распространяется по телу.") },
            { "systemicinfection2", ("Системная инфекция II", "Системная инфекция укоренилась. Вялость и жар.") },
            { "systemicinfection3", ("Системная инфекция III", "Системная инфекция ухудшилась. Что-то не так внутри.") },
            { "systemicinfection4", ("Системная инфекция IV", "Критическая стадия. Конец кажется близким.") },
            { "systemicinfection5", ("Терминальная инфекция", "Терминальная инфекция. Твоё тело отключается.") },
            { "tamponade", ("Тампонада сердца", "Кровь сдавливает сердце. Сердечный выброс снижен.") },
            { "hypovisc", ("Гиповязкость", "Аномально жидкая кровь. Плохая доставка кислорода.") },
            { "hypervisc", ("Гипервязкость", "Аномально густая кровь. Риск эмболии.") },
            { "feeling", ("Предчувствие", "Ты ощущаешь странное присутствие рядом. Кто-то нуждается в помощи.") },
            { "i4s", ("I4S-DS", "Мощный экспериментальный препарат. Регенерация и защита сознания.") },
            { "relieved", ("Облегчение", "Спокойствие и глубокое облегчение. Ты желаешь, чтобы это никогда не кончалось.") },
            { "augmentin", ("Аугментин", "Антибиотик. Останавливает прогресс системной инфекции.") },
            { "calamity", ("КАРА", "Состояние чистой ярости. Боль ушла, но твоё тело разрывается изнутри.") },
            { "epilepsy", ("Судороги", "В настоящее время испытываешь судороги. Потеря моторного контроля.") },
            { "infarction", ("Инфаркт миокарда", "Сердечный приступ в процессе. Тромб блокирует коронарную артерию.") },
            { "condition1", ("Так себе состояние", "Твоё состояние немного тревожно.") },
            { "condition2", ("Тревожное состояние", "Некоторые жизненные показатели отклонены от нормы.") },
            { "condition3", ("Плохое состояние", "Нужна немедленная медицинская помощь.") },
            { "condition4", ("Критическое состояние", "Ты рискуешь умереть.") }
        };
        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in MBPatches)
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
            TranslateChallenges();
            TranslateFallbackMoodles();
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.medicalbrutralismtranslation");

            MBPatches.AddRange(new[] {
                new PatchClassProcessor(harmony, typeof(MBCreateButtonPatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateTextPatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateInlineRowPatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateInlineTextBlockPatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateChallengeCheckboxPatch)),
                new PatchClassProcessor(harmony, typeof(MBRegisterChallengePatch)),
                new PatchClassProcessor(harmony, typeof(MBWoundedReqPatch)),
                new PatchClassProcessor(harmony, typeof(MBUpdateTooltipPatch)),
                new PatchClassProcessor(harmony, typeof(MBSelfSurgerySystemPatch)),
                new PatchClassProcessor(harmony, typeof(MBSafeGetMoodlePatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateConfigHeaderPatch)),
                new PatchClassProcessor(harmony, typeof(MBCreateConfigRowPatch)),
            });
        }

        private static void TranslateChallenges()
        {
            foreach (var c in Challenges.AllChallenges)
            {
                if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(c.title, out string translatedTitle))
                    c.title = translatedTitle;

                if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(c.description, out string translatedDesc))
                    c.description = translatedDesc;
            }
        }

        private static void TranslateFallbackMoodles()
        {
            var field = AccessTools.Field(typeof(ModMenuWindow), "ModMoodleFallbackList");
            var fallbacks = (ValueTuple<string, string, string, string>[])field.GetValue(null);

            for (int i = 0; i < fallbacks.Length; i++)
            {
                if (MBFallbacksTranslated.TryGetValue(fallbacks[i].Item2, out (string, string) nameDesc))
                {
                    fallbacks[i] = (
                        fallbacks[i].Item1,
                        fallbacks[i].Item2,
                        nameDesc.Item1,
                        nameDesc.Item2
                    );
                }

                //else if (Locale.currentLang.main.TryGetValue(fallbacks[i].Item2, out string itemName))
                //{
                //    fallbacks[i] = (
                //        fallbacks[i].Item1,
                //        fallbacks[i].Item2,
                //        itemName,
                //        ""
                //    );
                //}
            }
        }
    }
    [HarmonyPatch]
    internal static class MBCreateButtonPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateButton");

        [HarmonyPrefix]
        private static void Prefix(ref string label)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(label, out string translated))
                label = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBCreateTextPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateText");

        [HarmonyPrefix]
        private static void Prefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBCreateInlineRowPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateInlineRow");

        [HarmonyPrefix]
        private static void Prefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBCreateInlineTextBlockPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateInlineTextBlock");

        [HarmonyPrefix]
        private static void Prefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;

            text = text
                .Replace("Best completion", "Лучшее выполнение")
                .Replace("Best death", "Лучшая смерть");
        }
    }

    [HarmonyPatch]
    internal static class MBCreateConfigHeaderPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateConfigHeader");

        [HarmonyPrefix]
        private static void Prefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBCreateConfigRowPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateConfigRow");

        [HarmonyPrefix]
        private static void Prefix(ref string label)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(label, out string translated))
                label = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBCreateChallengeCheckboxPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(ModMenuWindow), "CreateChallengeCheckbox");

        [HarmonyPrefix]
        private static void Prefix(ref string label)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(label, out string translated))
                label = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBRegisterChallengePatch
    {
        [HarmonyPatch(typeof(Challenges), nameof(Challenges.RegisterChallenge))]
        [HarmonyPrefix]
        private static void Prefix(Challenges.Challenge c)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(c.title, out string translatedTitle))
                c.title = translatedTitle;

            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(c.description, out string translatedDesc))
                c.description = translatedDesc;
        }
    }

    [HarmonyPatch]
    internal static class MBWoundedReqPatch
    {
        [HarmonyPatch(typeof(WoundedRequirement), nameof(WoundedRequirement.GetDisplayName))]
        [HarmonyPostfix]
        private static void Postfix(ref string __result)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(__result, out string translated))
                __result = translated;
        }
    }

    [HarmonyPatch]
    internal static class MBUpdateTooltipPatch
    {
        [HarmonyPatch(typeof(WoundedTraderSystem), "UpdateTooltip")]
        [HarmonyPostfix]
        private static void Postfix(WoundedTrader wt)
        {
            if (wt.trader == null)
                return;

            BuildingEntity trader = wt.trader.GetComponent<BuildingEntity>();

            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(trader.fullName, out string traderName))
                trader.fullName = traderName;

            trader.description = trader.description
                .Replace("This person has succumbed to their wounds. They are still slightly warm.", "Это существо скончалось от полученных ран. Тело ещё слегка тёплое.")
                .Replace("Unconscious. Needs immediate assistance.", "Без сознания. Требуется неотложная помощь.");

            wt.originalFullName = "Спасённый выживший";
            wt.originalDescription = "Счастливчик. Вы спасли ему жизнь. К счастью, он дышит.";
        }
    }

    [HarmonyPatch]
    internal static class MBSelfSurgerySystemPatch
    {
        private static readonly Type T = AccessTools.Inner(typeof(SelfSurgerySystem), "OrganCondition");
        private static readonly FieldInfo LabelField = AccessTools.Field(T, "label");
        private static readonly FieldInfo DescriptionField = AccessTools.Field(T, "description");
        private static readonly FieldInfo ActionLabelField = AccessTools.Field(T, "actionLabel");

        [HarmonyPatch(typeof(SelfSurgerySystem), "GetOrganDisplayName")]
        [HarmonyPostfix]
        private static void GetOrganDisplayNamePostfix(ref string __result)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(__result, out string translated))
                __result = translated;
        }

        //[HarmonyPatch(typeof(SelfSurgerySystem), "AddTissueDamageCondition")]
        //[HarmonyPostfix]
        //private static void AddTissueDamageConditionPostfix(System.Collections.IList list, ref string organName)
        //{
        //    if (list == null) return;

        //    foreach (object cond in list)
        //    {
        //        if (cond == null) continue;

        //        SetLocalized(cond, LabelField);
        //        SetLocalized(cond, DescriptionField);
        //        SetLocalized(cond, ActionLabelField);
        //    }
        //}

        [HarmonyPatch(typeof(SelfSurgerySystem), "AddHeader")]
        [HarmonyPrefix]
        private static void AddHeaderPrefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;
        }

        [HarmonyPatch(typeof(SelfSurgerySystem), "MakeLabel")]
        [HarmonyPrefix]
        private static void MakeLabelPrefix(ref string text)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(text, out string translated))
                text = translated;
        }

        [HarmonyPatch(typeof(SelfSurgerySystem), "AddTissueDamageCondition")]
        [HarmonyPrefix]
        private static void AddTissueDamageConditionPrefix(ref string organName)
        {
            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(organName, out string translated))
                organName = translated;
        }

        [HarmonyPatch(typeof(SelfSurgerySystem), "GetConditions")]
        [HarmonyPostfix]
        private static void GetConditionsPostfix(System.Collections.IList __result)
        {
            if (__result == null) return;

            foreach (object cond in __result)
            {
                if (cond == null) continue;

                SetLocalized(cond, LabelField);
                SetLocalized(cond, DescriptionField);
                SetLocalized(cond, ActionLabelField);
            }
        }

        private static void SetLocalized(object cond, FieldInfo field)
        {
            if (!(field.GetValue(cond) is string s)) return;

            if (MedicalBrutalismTranslator.MBTranslates.TryGetValue(s, out string translated))
            {
                field.SetValue(cond, translated);
            }
        }
    }

    [HarmonyPatch]
    internal static class MBSafeGetMoodlePatch
    {
        [HarmonyPatch(typeof(ModMenuWindow), "SafeGetMoodle")]
        [HarmonyPostfix]
        private static void Postfix(ref string __result, string key)
        {
            Plugin.Log.LogInfo($"{key} = {__result}");
        }
    }
}
