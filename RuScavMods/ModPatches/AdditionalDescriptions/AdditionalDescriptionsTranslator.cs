using HarmonyLib;
using QoL_Unknown;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuScavMods.ModPatches.AdditionalDescriptions
{
    internal class AdditionalDescriptionsTranslator
    {
        private static readonly Type ItemSetupItemsPatchType = AccessTools.TypeByName("CasualtiesUnknownAdditionalDescriptions.ItemSetupItemsPatch");
        private static readonly Type FluidDescriptionType = AccessTools.TypeByName("CasualtiesUnknownAdditionalDescriptions.FluidDescription");
        private static readonly Type DescriptionLineType = AccessTools.TypeByName("CasualtiesUnknownAdditionalDescriptions.DescriptionLine");

        private static readonly FieldInfo DescriptionsField = AccessTools.Field(ItemSetupItemsPatchType, "Descriptions");
        private static readonly FieldInfo ItemDescriptionsField = AccessTools.Field(ItemSetupItemsPatchType, "ItemDescriptions");

        private static readonly FieldInfo DrinkField = AccessTools.Field(FluidDescriptionType, "drink");
        private static readonly FieldInfo InjectField = AccessTools.Field(FluidDescriptionType, "inject");
        private static readonly FieldInfo ExtraLinesField = AccessTools.Field(FluidDescriptionType, "extraLines");
        private static readonly FieldInfo TextField = AccessTools.Field(DescriptionLineType, "text");

        private static readonly Dictionary<string, FluidTranslation> FluidTranslations = new Dictionary<string, FluidTranslation>();
        private static readonly Dictionary<string, DescriptionTranslation[]> ItemDescriptionTranslations = new Dictionary<string, DescriptionTranslation[]>();

        public static void Translate()
        {
            FillDictionaryTranslation();
            var fluidDescriptions = (IDictionary)DescriptionsField.GetValue(null);
            var itemDescriptions = (IDictionary)ItemDescriptionsField.GetValue(null);
            foreach (DictionaryEntry fluid in fluidDescriptions)
            {
                if (!FluidTranslations.TryGetValue((string)fluid.Key, out var translation))
                    continue;
                object fluidDescription = fluid.Value;
                if (!string.IsNullOrEmpty(translation.Drink)) DrinkField.SetValue(fluidDescription, translation.Drink);
                if (!string.IsNullOrEmpty(translation.Inject)) InjectField.SetValue(fluidDescription, translation.Inject);
                if (translation.ExtraLines != null && translation.ExtraLines.Length != 0)
                {
                    var extraLines = (Array)ExtraLinesField.GetValue(fluidDescription);
                    int count = Math.Min(extraLines.Length, translation.ExtraLines.Length); // На всякий случай
                    for (int i = 0; i < count; i++)
                    {
                        object descriptionLine = extraLines.GetValue(i);
                        TextField.SetValue(descriptionLine, translation.ExtraLines[i].Text);
                    }
                }
            }
            foreach (DictionaryEntry item in itemDescriptions)
            {
                if (!ItemDescriptionTranslations.TryGetValue((string)item.Key, out var translation))
                    continue;
                if (translation.Length != 0 && translation != null)
                {
                    var descriptionLines = (Array)item.Value;
                    int count = Math.Min(descriptionLines.Length, translation.Length); // На всякий случай

                    for (int i = 0; i < count; i++)
                    {
                        object descriptionLine = descriptionLines.GetValue(i);
                        TextField.SetValue(descriptionLine, translation[i].Text);
                    }
                }
            }
        }

        private static void FillDictionaryTranslation()
        {
            FluidTranslations["water"] = new FluidTranslation("Выпить: утоляет жажду на +9, слегка охлаждает.", "Ввод внутрь: никаких полезных эффектов.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["carbonatedwater"] = new FluidTranslation("Выпить: утоляет жажду на +9. Дает небольшой буст к настроению, вызывает отрыжку и немного охлаждает.", "Ввод внутрь: никаких полезных эффектов.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["lrdserum"] = new FluidTranslation("Выпить: вызывает тошноту.", "Ввод внутрь: работает только в Устройстве Локальной Реанимации.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["morphine"] = new FluidTranslation("Выпить: оказывает умеренное обезболивающее действие.", "Ввод внутрь: оказывает сильное обезболивающее действие.", new DescriptionTranslation[]
            {
                new DescriptionTranslation("Риск передозировки начинается с введеных внутрь ~88мл при отсутствии толерантности.")
            });
            FluidTranslations["biochem"] = new FluidTranslation("Выпить: вызывает тошноту, боль. Наносит урон мышцам и вызывает боль. Дизинфцириует.", "Ввод внутрь/применение: дизинфицирует но вызывает тошноту, боль и наносит урон мышцам.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["paincream"] = new FluidTranslation("Выпить: вызывает тошноту и рвоту.", "Применение: сильно дизинфицирует и лечит кожу; боль в конечности уменьшается на величину, равную 15% от разности между текущей болью и 10% от уровня боли в секунду.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["opium"] = new FluidTranslation("Выпить: оказывает слабое обезболивающее действие.", "Ввод внутрь: оказывает сильное обезболивающее действие.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["painkillers"] = new FluidTranslation("Выпить: оказывает сильное обезболивающее действие.", null, new DescriptionTranslation[]
            {
                new DescriptionTranslation("Передозировка составляет ~60мл.")
            });
            FluidTranslations["heroin"] = new FluidTranslation("Выпить: оказывает сильное обезболивающее действие, вызывает тошноту.", "Ввод внутрь: оказывает наисильнейшее обезболивающее действие, вызывает тошноту.", new DescriptionTranslation[]
            {
                new DescriptionTranslation("Риск передозировки начинается с введеных внутрь ~34мл при отсутствии толерантности.")
            });
            FluidTranslations["naloxone"] = new FluidTranslation("Выпить: нет полезных эффектов.", "Ввод внутрь: быстро купирует передозировку опиатами.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["naltrexone"] = new FluidTranslation("Выпить: уменьшает толерантность к опиатам. Ухудшает настроение и может вызвать рвоту.", "Ввод внутрь: никаких полезных эффектов.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["ceftriaxone"] = new FluidTranslation("Выпить: нет полезных эффектов.", "Ввод внутрь: даёт долгий буст к иммунитету. Вызывает боль.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["fentanyl"] = new FluidTranslation("Выпить/Ввод внутрь: серьёзный опиатный эффект; перед применением рекомендуется развести.", null, new DescriptionTranslation[]
            {
                new DescriptionTranslation("Передозировка составляет ~20мл.")
            });
            FluidTranslations["ketchup"] = new FluidTranslation("Выпить: утоляет голод на +4 и жажду на +1.5, увеличивает калории на +0.5. Ухудшает настроение и вызывает тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["milk"] = new FluidTranslation("Выпить: утоляет жажду на +6, увеличивает калории на +2.5. Немного улучшает настроение и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["chloroform"] = new FluidTranslation("Выпить/Ввод внутрь: медленно сводит сознание к нулю.", null, Array.Empty<DescriptionTranslation>());
            FluidTranslations["highgradestimulant"] = new FluidTranslation("Выпить: утоляет жажду на +1. Даёт долгий буст к стамине, сознанию и прилив андреналина.", "Ввод внутрь: более сильный и долгий буст.", new DescriptionTranslation[]
            {
                new DescriptionTranslation("Дозировка составляет 100мл"),
                new DescriptionTranslation("Внимание: эффекты передозировки возникают будучи стимулированным более чем 320 секунд.")
            });
            FluidTranslations["midgradestimulant"] = new FluidTranslation("Выпить: утоляет жажду на +0.5. Вызывает тошноту и ухудшает настроение. Стимулятор не срабатывает.", "Ввод внутрь: оказывает кратковременный мощный стимулирующий эффект. Вызывает тошноту и внутреннее кровотечение.", new DescriptionTranslation[]
            {
                new DescriptionTranslation("Дозировка составляет 50мл."),
                new DescriptionTranslation("Внимание: эффекты передозировки возникают будучи стимулированным более чем 220 секунд.")
            });
            FluidTranslations["lowgradestimulant"] = new FluidTranslation("Выпить/Ввод внутрь: утоляет жажду на +0.5. Оказывает стимулирующий эффект, сопровождающийся тошнотой, жаром и тяжёлым отходняком.", null, new DescriptionTranslation[]
            {
                new DescriptionTranslation("Дозировка составляет 40мл."),
                new DescriptionTranslation("Внимание: эффекты передозировки возникают будучи стимулированным более чем 160 секунд.")
            });
            FluidTranslations["urine"] = new FluidTranslation("Выпить: утоляет жажду на +3.5. Вызывает тошноту, ухудшает настроение и немного охлаждает.", "Ввод внутрь: вызывает сильную тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["icedtea"] = new FluidTranslation("Выпить: утоляет жажду на +8, увеличивает калории на +1. Улучшает настроение, вызывает тошноту и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["soup"] = new FluidTranslation("Выпить: утоляет жажду на +8 и голод на +4, увеличивает калории на +2.5. Улучшает настроение.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["chocolatemilk"] = new FluidTranslation("Выпить: утоляет жажду на +8 и голод на +4. Улучшает настроение и вызывает серьёзную тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["cereal"] = new FluidTranslation("Выпить: утоляет жажду на +8 и голод на +8, увеличивает калории на +5. Улучшает настроение и немного охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["coffee"] = new FluidTranslation("Выпить: уоляет жажду на +8. Дает буст к стамине, энергии и настроению. Вызывает заметную тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["energydrink"] = new FluidTranslation("Выпить: уоляет жажду на +8. Дает буст к стамине, энергии и настроению. Вызывает тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["sportsdrink"] = new FluidTranslation("Выпить: утоляет жажду на +9.5. Дает небольшой буст к энергии и стамине, а также вызывает небольшую тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["oliveoil"] = new FluidTranslation("Выпить: утоляет голод на +8, увеличивает калории на +15. Вызывает тошноту, загрязняет и ухудшает настроение.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["hotsauce"] = new FluidTranslation("Выпить: утоляет жажду на +1 и голод на +1. Вызывает боль и тошноту; ненадолго согревает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["icecream"] = new FluidTranslation("Выпить: утоляет жажду на +6 и голод на +6, увеличивает калории на +12. Улучшает настроение; вызывает лёгкую тошноту и ненадолго охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["yogurt"] = new FluidTranslation("Выпить: утоляет голод на +6 и жажду на +6, увеличивает калории на +2.5 за использования. Немного улучшает настроение, вызывает тошноту и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["mold"] = new FluidTranslation("Выпить: утоляет голод на +0.5. Вызывает тошноту и ухудшение настроение; тебя вырвет если выпить около 25мл.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["powderedmilk"] = new FluidTranslation("Выпить: утоляет голод на +8 и усиливает жажду на -5, повышает калории на +15. Вызывает тошноту и ухудшает настроение.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["radwater"] = new FluidTranslation("Выпить: утоляет жажду на +9 и охлаждает, но вызывает лучевую болезнь.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["mercury"] = new FluidTranslation("Выпить: вызывает тошноту, сгущает кровь и ухудшает настроение. Медленно разрушает мозг.", "Inject: severe sickness and much thicker blood.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["soda"] = new FluidTranslation("Выпить: уоляет жажду на +8. Дает буст к стамине, энергии и настроению. Вызывает тошноту и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["saline"] = new FluidTranslation("Выпить: усиливает жажду, ухудшает настроение и вызывает тошноту.", "Ввод внутрь: восстанавливает но сгущает кровь и усиливает жажду.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["ringersolution"] = new FluidTranslation("Выпить: усиливает жажду, ухудшает настроение и вызывает тошноту.", "Ввод внутрь: восстанавливает но сгущает кровь и усиливает жажду.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["blood"] = new FluidTranslation("Выпить: немного утоляет жажду; вызывает сильную тошноту и ухудшает настроение.", "Ввод внутрь: восстанавливает объём крови.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["redblood"] = new FluidTranslation("Выпить: немного утоляет жажду; вызывает сильную тошноту и ухудшает настроение.", "Ввод внутрь: восстанавливает кровь, но вызывает тошноту, септический шок и ухудшает здоровье мышц.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["alienblood"] = new FluidTranslation("Выпить: немного утоляет жажду; вызывает сильную тошноту и ухудшает настроение.", "Ввод внутрь: немного восстанавливает крови и вызывает тошноту/септический шок.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["groundwater"] = new FluidTranslation("Выпить: утоляет жажду на +8 и слегка охлаждает.", "Без фильтра: 50% шанс вызвать тошноту, немного ухудшает настроение; соломинка с фильтром убирает большинство вреда.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["lumalgae"] = new FluidTranslation("Выпить: утоляет голод на +1.5 и жажду на +6, увеличивает калории на +0.5. Охлаждает и ухудшает настроение.", "Без фильтра: возможна тошнота и рвота; соломинка с фильтром убирает большинство вреда.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["oil"] = new FluidTranslation("Выпить: утоляет голод на +0.5 и жажду на +0.5, увеличивает калории на +10. Вызывает сильную тошноту и ухудшает настроение.", "Ввод внутрь: вызывает сильную тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["sap"] = new FluidTranslation("Выпить: утоляет голод на +4 и жажду на +8, увеличивает калории на +5. Слегка улучшает настроение и вызывает легкую тошноту.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["dirtywater"] = new FluidTranslation("Выпить: утоляет жажду на +6 и слегка охлаждает.", "Без фильта: вызывает тошноту и ухудшает настроение; соломинка с фильтром убирает большинство вреда.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["fat"] = new FluidTranslation("Выпить: утоляет голод на +0.5 и жажду на +0.5, увеличивает калории на +10. Вызывает тошноту и ухудшает настроение.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["soap"] = new FluidTranslation("Выпить: утоляет жажду на 0.2, вызывает тошноту, ухудшает настроение.", "Применение: Слегка дезинфицирует раны и убирает грязь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["producejuice"] = new FluidTranslation("Выпить: утоляет голод на +4 и жажду на +8, увеличивает калории на +2.5. Немного улучшает настроение и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["refinedjuice"] = new FluidTranslation("Выпить: утоляет голод на +10 и жажду на +5, увеличивает калории на +4. Улучшает настроение и охлаждает.", "Ввод внутрь: вызывает тошноту и сгущает кровь.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["alcohol"] = new FluidTranslation("Выпить: утоляет жажду, улучшает настроение, охлаждает и вызывает сильную тошноту.", "Применение: сильное дезинфицирующее средство.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["bleach"] = new FluidTranslation("Выпить: вызывает сильную тошноту, боль, так же возможнен летальный исход.", "Применение: никаких полезных эффектов.", Array.Empty<DescriptionTranslation>());
            FluidTranslations["disinfectant"] = new FluidTranslation("Выпить: сильная тошнота и боль.", "Применение: сильное дизинфекцирующее средство.", Array.Empty<DescriptionTranslation>());

            ItemDescriptionTranslations["neuralbooster"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Первое использование: +10 хп мозгу, излечивает инсульт, +20 к настроению, 10 минут энергии и буст к мобильности навсегда."),
                new DescriptionTranslation("Внимание: использование другого нейробустера или мозгороста во время 20 минутного ББРН сотрёт твою личность."),
                new DescriptionTranslation("Последующие использования: слепота, -20 хп мозга, 0.855Л/мин внутреннего кровотечения, +100 тошнота, +5 к температуре, рвота/рэгдул и хп всех мышц x0.2.")
            };

            ItemDescriptionTranslations["braingrow"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: медленно восстанавливает мозг и уменьшает инсульт, но вызывает тошноту и ухудшает настроение."),
                new DescriptionTranslation("Внимание: применение при ББРН или более 40мл сотрёт твою личность.")
            };

            ItemDescriptionTranslations["mindwipe"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: можно использовать только при депрессии и хуже, или же при серьёзном повреждении мозга/инсульте."),
                new DescriptionTranslation("Выпить: оглушает тебя, восстанавливает мозг и лечит инсульт. Делает тебя П У С Т Ы М; настроение будет заблокировано на 0 и никогда не сможет подняться или упасть. Обнуляет интеллект и заставляет забыть все рецепты."),
                new DescriptionTranslation("Даже 1мл мозгостера сработает на тебя.")
            };

            ItemDescriptionTranslations["antidepressants"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: стабилизирует настроение; каждая доза может вызвать тошноту."),
                new DescriptionTranslation("Внимание: слишком большое количество активного вещества снижает кровяное давление.")
            };

            ItemDescriptionTranslations["sleepingpills"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Drink: causes long-lasting sleepiness (5mL dosage)."),
                new DescriptionTranslation("Warning: Overdose lowers blood pressure and respiration; opiates greatly lower the overdose threshold.")
            };

            ItemDescriptionTranslations["antirad"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: со временем уменьшает лучевую болезнь."),
                new DescriptionTranslation("Внимание: слишком большое количество активного вещества приведёт к тошноте и боли в груди.")
            };

            ItemDescriptionTranslations["procoagulant"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: замедляет внутреннее и внешнее кровотечение, сгущает кровь и снижает инсульт."),
                new DescriptionTranslation("Внимание: повторное использование может привести к чрезмерному сгущению крови.")
            };

            ItemDescriptionTranslations["bloodcoagulant"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: замедляет внутреннее и внешнее кровотечение, сгущает кровь и снижает инсульт."),
                new DescriptionTranslation("Внимание: повторное использование может привести к чрезмерному сгущению крови.")
            };

            ItemDescriptionTranslations["streptokinase"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: сильно разжижает кровь и вызывает тошноту (дозировка ~34мл)."),
                new DescriptionTranslation("Выпить: слабее разжижает кровь и вызывает более сильную тошноту.")
            };

            ItemDescriptionTranslations["antibiotics"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: увеличивает иммунитет и уменьшает септический шок; немного уменьшает настроение (дозировка 20мл).")
            };

            ItemDescriptionTranslations["antivenom"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: уменьшает токсикоз. Если выпить то только вызовет тошноту.")
            };

            ItemDescriptionTranslations["antiserum"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: уменьшает септический шок, увеличивает объём крови, иммунитет и дезинфицирует находящиеся рядом конечности (дозировка 50мл)."),
                new DescriptionTranslation("Выпить: хуже увеличивает иммунитет и вызывает тошноту.")
            };

            ItemDescriptionTranslations["keratinbooster"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить/ввод внутрь: ускоряет регенерацию когтей."),
                new DescriptionTranslation("Внимание: чрезмерное использование во время регенерации может привести к заболеваниям.")
            };

            ItemDescriptionTranslations["amiodarone"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: уменьшает фибрилляцию, но немного повреждает мышцы в груди.")
            };
            ItemDescriptionTranslations["sodiumnitroprusside"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: быстро снижает кровяное давление. Употребление в основном вызывает тошноту.")
            };
            ItemDescriptionTranslations["vasopressin"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: быстро повышает кровяное давление. Употребление оказывает более слабый эффект.")
            };
            ItemDescriptionTranslations["naloxone"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Ввод внутрь: быстро нейтрализует передозировку опиатами (доза ~34мл для шприца, 50мл для сыворотки).")
            };
            ItemDescriptionTranslations["naltrexone"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: нейтрализует опиаты и снижает толерантность к ним; может вызвать рвоту и ухудшение настроение (дозировка 20мл).")
            };
            ItemDescriptionTranslations["tourniquet"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: перекрывает кровообращение к конечности, чтобы остановить кровотечение."),
                new DescriptionTranslation("Внимание: вызывает боль и кислородное голодание; не забудь его убрать.")
            };
            ItemDescriptionTranslations["medicalsuture"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: зашивет раны, останавливает кровотечение и увеличивает здоровье кожи.")
            };
            ItemDescriptionTranslations["splint"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: ускоряет заживление переломов и вывихов; иммобилизирует конечность, снижая её подвижность.")
            };
            ItemDescriptionTranslations["icepack"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: ускоряет заживление мышц, уменьшает боль и охлаждает тело; автоматически восстанавливается пока не используется.")
            };
            ItemDescriptionTranslations["alginate"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: замедляет кровотечение, ускоряет заживление кожи и противодействует инфекции.")
            };
            ItemDescriptionTranslations["manualdefibrillator"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование (грудь): стабилизирует сердечный ритм; быстрее и потребляет меньше энергии чем АВД, но требует INT.")
            };
            ItemDescriptionTranslations["aed"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование (грудь): диагностирует и лечит аритмию с помощью электрошока; медленнее и потребляет больше энергии чем ручной дефибриллятор.")
            };
            ItemDescriptionTranslations["tweezers"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: безболезненно удаляет осколки из конечностей.")
            };
            ItemDescriptionTranslations["boneweldingtool"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: сваривает переломы, значительно ускоряя заживление."),
                new DescriptionTranslation("Внимание: наносит урон конечности и вызывает кровотечение во время сварки.")
            };
            ItemDescriptionTranslations["plasmacutter"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: прижигает раны и разрезает сквозь металлические блоки(?).")
            };
            ItemDescriptionTranslations["wrench"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: более эффективно вправляет вывихи; также может использоваться в качестве молотка в крафте.")
            };
            ItemDescriptionTranslations["chestdrain"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение (грудь): отводит жидкость, уменьшая риск гемоторакса; многоразовый.")
            };
            ItemDescriptionTranslations["woundglue"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: запечатывает раны (тюбик вмещает 80мл, 20мл за использование).")
            };
            ItemDescriptionTranslations["soap"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: убирает грязь и слегка дезинфицирует раны (на основе животного жира).")
            };
            ItemDescriptionTranslations["paincream"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: мгновенно снимает боль и дезинфицирует раны (дозировка 10мл).")
            };
            ItemDescriptionTranslations["bucketofchicken"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +10, увеличивает калории на +2; обладает чрезвычайно долгим сроком хранения.")
            };
            ItemDescriptionTranslations["cake"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +8, увеличивает калории на +1.25; может вызвать тошноту при переедании.")
            };
            ItemDescriptionTranslations["fleshchunk"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +14, увеличивает калории на +1.2.")
            };
            ItemDescriptionTranslations["burger"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +14, увеличивает калории на +1.2.")
            };
            ItemDescriptionTranslations["pizzaslice"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +12, увеличивает калории на +1.2.")
            };
            ItemDescriptionTranslations["steak"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +15, увеличивает калории на +1.6.")
            };
            ItemDescriptionTranslations["pemmican"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +12, увеличивает калории на +1.35.")
            };
            ItemDescriptionTranslations["nutrientbar"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +12.5, увеличивает калории на +0.4.")
            };
            ItemDescriptionTranslations["animalflesh"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +7.5, увеличивает калории на +1; вызывает небольшую тошноту.")
            };
            ItemDescriptionTranslations["blobflesh"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +6, увеличивает калории на +0.6; может вызвать небольшую тошноту.")
            };
            ItemDescriptionTranslations["bread"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +9 и жажду на +2, увеличивает калории на +0.5.")
            };
            ItemDescriptionTranslations["pancake"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +10, увеличивает калории на +1.2.")
            };
            ItemDescriptionTranslations["paprikash"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +12, увеличивает калории на +0.5.")
            };
            ItemDescriptionTranslations["foliagemeal"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +18 и жажду на +2.")
            };
            ItemDescriptionTranslations["cereal"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +6 и усиливает жажду на -2.5, увеличивает калории на +0.45.")
            };
            ItemDescriptionTranslations["dogfood"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +6, увеличивает калории на +0.4.")
            };
            ItemDescriptionTranslations["experimentflesh"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +12.5, увеличивает калории на +1.5; вызывает тошноту.")
            };
            ItemDescriptionTranslations["internalorgans"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +15, увеличивает калории на +3; вызывает ужасную тошноту.")
            };
            ItemDescriptionTranslations["geigefruit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +4 и жажду на +2.5, увеличивает калории на +0.05; может снизить или повысить лучевую болезнь.")
            };
            ItemDescriptionTranslations["banana"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +9 и жажду на +4, увеличивает калории на +0.1.")
            };
            ItemDescriptionTranslations["geofruit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +2.5 и жажду на +5, увеличивает калории на +0.1.")
            };
            ItemDescriptionTranslations["frigiantfruit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +2.5 и жажду на +3.5, увеличивает калории на +0.05; сильно охлаждает тело на -2.5C.")
            };
            ItemDescriptionTranslations["mushball"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет жажду на +7.")
            };
            ItemDescriptionTranslations["mushpear"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод +5 и жажду на +4, увеличивает калории на -2.5; уменьшает массу тела; содержит следы опиатов.")
            };
            ItemDescriptionTranslations["funguschunk"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +10, увеличивает калории на +0.25.")
            };
            ItemDescriptionTranslations["browncap"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод от +7 до +10, меняет калории от -0.5 до +1 (рандомно).")
            };
            ItemDescriptionTranslations["cactusflesh"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +4 и жажду на +4, увеличивает калории на +0.6; вызывает боль.")
            };
            ItemDescriptionTranslations["bloodsac"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +7 и жажду на +7.5, увеличивает калории на +1.")
            };
            ItemDescriptionTranslations["dryfoliage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +6; вызывает небольшую тошноту.")
            };
            ItemDescriptionTranslations["foliage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +5; вызывает тошноту.")
            };
            ItemDescriptionTranslations["candybar"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +6, увеличивает калории на +0.8; может вызвать тошноту при переедании.")
            };
            ItemDescriptionTranslations["stonefruitopen"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +8, увеличивает калории на +2, усиливает жажду на 5.")
            };
            ItemDescriptionTranslations["hardcandy"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +1, увеличивает калории на +0.7.")
            };
            ItemDescriptionTranslations["chips"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +3, увеличивает калории на +0.9.")
            };
            ItemDescriptionTranslations["chocolatebar"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +7, увеличивает калории на +0.8.")
            };
            ItemDescriptionTranslations["cookies"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +3, увеличивает калории на +0.7.")
            };
            ItemDescriptionTranslations["popcorn"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +9, увеличивает калории на +2.")
            };
            ItemDescriptionTranslations["fat"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: немного утоляет голод и вызывает тошноту.")
            };
            ItemDescriptionTranslations["bulbskin"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: импровизированная повязка и анальгетик для конечностей; утоляет жажду на +2. Маслянистая и увлажняющая. Может повлиять на работу мозга.")
            };
            ItemDescriptionTranslations["xalorissponge"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: утоляет голод на +8, увеличивает калории на +0.15. Применение: противодействует инфекции; влияет на иммунитет.")
            };
            ItemDescriptionTranslations["roselight"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение (конечность): выявляет скрытые инфекции; быстро разлагается.")
            };
            ItemDescriptionTranslations["analgesicgauze"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: замедляет кровотечение и уменьшает боль.")
            };
            ItemDescriptionTranslations["bruisekit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: лечит мышцы/кожу и помогает при вывивах. Не влияет на кровотечения.")
            };
            ItemDescriptionTranslations["lrd"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: эффективно лечит травмы; заживляет, замедляет кровотечения, снимает боль и дезинфицирует (требует 25мл сыворотки для У.Л.Р.).")
            };
            ItemDescriptionTranslations["bloodbag"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: восстанавливает 375мл объёма крови; берёт твою кровь при использовании в инвентаре.")
            };
            ItemDescriptionTranslations["bloodbaghuman"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: восстанавливает 375мл объёма крови; берёт твою кровь при использовании в инвентаре.")
            };
            ItemDescriptionTranslations["plasticbandage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: покрывает раны и стабилизирует переломы; никак не влияет на мышцы.")
            };
            ItemDescriptionTranslations["sterilizedbandage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: замедляет кровотечение, стабилизирует переломы и восстанавливает кожу; противодействует инфекции.")
            };
            ItemDescriptionTranslations["bandage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: замедляет кровотечение, стабилизирует переломы и восстанавливает кожу.")
            };
            ItemDescriptionTranslations["adhesivebandage"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: лечит небольшие раны. Уменьшает кровотечение.")
            };
            ItemDescriptionTranslations["rag"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: импровизированная повязка; позволяет вытереться при использовании в инвентаре.")
            };
            ItemDescriptionTranslations["rippeddressing"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: импровизированная повязка на кровоточащие раны.")
            };
            ItemDescriptionTranslations["musharm"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: одноразовый самодельная повязка; может использоваться как листва для крафта.")
            };
            ItemDescriptionTranslations["glowplantfruit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение (конечность): противодействует инфекциям; несъедобен и токсичен.")
            };
            ItemDescriptionTranslations["mushtail"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: мгновенно заставляет заснуть и увеличивает настроение; никакой пищевой ценности.")
            };
            ItemDescriptionTranslations["purpleherb"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Съесть: помогает при тошноте и потере слуха; замедляет внутреннее кровотечение; сильно повышает температуру тела.")
            };
            ItemDescriptionTranslations["lockpickingkit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: открывает контейнеры (требует 10 INT).")
            };
            ItemDescriptionTranslations["blueprint"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: открывает рецепт для крафта и дает опыт к INT.")
            };
            ItemDescriptionTranslations["autopump"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Надето (грудь): поддерживает кровяное давление на приемлимом уровне при отказе сердца.")
            };
            ItemDescriptionTranslations["liquidcentrifuge"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: разделяет смесь на базовые компоненты; разделенная жидкость теряет 1/3 своего изначального объёма. Перезарядка 60 секунд.")
            };
            ItemDescriptionTranslations["filterstraw"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Удерживание: позволяет безопасно пить грунтовые воды и уменьшает воздействие на здоровье других жидкостей.")
            };
            ItemDescriptionTranslations["combatpen"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: мгновенно стабилизирует организм и помогает в боевых действиях (дозировка 100мл).")
            };
            ItemDescriptionTranslations["sleepingpills"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Выпить: вызывает длительную сонливость (дозировка 5мл).")
            };
            ItemDescriptionTranslations["drillrepairkit"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: чинит буровые капсулы.")
            };
            ItemDescriptionTranslations["antisepticmush"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: эффективно лечит инфекции; одноразовый.")
            };
            ItemDescriptionTranslations["carcasssplint"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Применение: импровизированная шина для лечения переломов.")
            };
            ItemDescriptionTranslations["watch"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: отображает время забега и температуру; периодически сообщает информацию о жизненном состоянии.")
            };
            ItemDescriptionTranslations["epda"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: доступ к собранным журналам данных; для работы требуется батарея.")
            };
            ItemDescriptionTranslations["mp3player"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Использование: воиспроизводит сохраненные треки.")
            };
            ItemDescriptionTranslations["scubadivinggear"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Надето: неограниченное дыхание и выносливость под водой; быстрое перемещение в жидкостях и высыхание.")
            };
            ItemDescriptionTranslations["gravbag"] = new DescriptionTranslation[]
            {
                new DescriptionTranslation("Надето: уменьшает вес содержимого."),
                new DescriptionTranslation("Внимание: имеет тенденции ко взрыву когда батарея почти разряжена.")
            };
        }
    }

    internal class FluidTranslation
    {
        public string Drink { get; }
        public string Inject { get; }
        public DescriptionTranslation[] ExtraLines { get; }

        public FluidTranslation(string drink, string inject, DescriptionTranslation[] descriptionLines)
        {
            Drink = drink;
            Inject = inject;
            ExtraLines = descriptionLines;
        }
    }

    internal class DescriptionTranslation
    {
        public string Text { get; }
        public DescriptionTranslation(string text)
        {
            if (text.StartsWith("Внимание:"))
            {
                var coloredText = "<color=#ff4d4d>" + text + "</color>";
                Text = coloredText;
            }
            else
                Text = text;
        }
    }
}
