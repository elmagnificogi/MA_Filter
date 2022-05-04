using MapAssist.Settings;
using MapAssist.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MapAssist.Helpers
{
    internal sealed class FloatPrecisionConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(double);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit(new Scalar(null, ((double)value).ToString(new CultureInfo("en-US")))); // Otherwise some bug in the yamlconverter won't have the right precisions on doubles
        }
    }

    internal sealed class ItemYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Item);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                if (Enum.TryParse("Class" + scalar.Value.Replace(" ", "").Replace("-", ""), true, out Item itemClass))
                {
                    return itemClass;
                }
                else if (Enum.TryParse(scalar.Value.Replace(" ", "").Replace("-", ""), true, out Item item))
                {
                    return item;
                }
                else
                {
                    throw new Exception($"Failed to parse item: {scalar.Value}");
                }
            }

            return null;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ItemQualityYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(ItemQuality[]);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                var items = new List<string>() { scalar.Value };
                return ParseItemQuality(items);
            }

            if (parser.TryConsume<SequenceStart>(out var _))
            {
                var items = new List<string>();
                while (parser.TryConsume<Scalar>(out var scalarItem))
                {
                    items.Add(scalarItem.Value);
                }

                parser.Consume<SequenceEnd>();
                return ParseItemQuality(items);
            }

            return null;
        }

        private ItemQuality[] ParseItemQuality(List<string> quality)
        {
            return quality.Select(q =>
            {
                ItemQuality parsedQuality;
                var success = Enum.TryParse(q.ToUpper(), true, out parsedQuality);
                return new { success, parsedQuality };
            }).Where(x => x.success).Select(x => x.parsedQuality).ToArray();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ItemTierYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(ItemTier[]);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                var items = new List<string>() { scalar.Value };
                return ParseItemTier(items);
            }

            if (parser.TryConsume<SequenceStart>(out var _))
            {
                var items = new List<string>();
                while (parser.TryConsume<Scalar>(out var scalarItem))
                {
                    items.Add(scalarItem.Value);
                }

                parser.Consume<SequenceEnd>();
                return ParseItemTier(items);
            }

            return null;
        }

        private ItemTier[] ParseItemTier(List<string> quality)
        {
            return quality.Select(q =>
            {
                ItemTier parsedQuality;
                var success = Enum.TryParse(q.ToUpper(), true, out parsedQuality);
                return new { success, parsedQuality };
            }).Where(x => x.success).Select(x => x.parsedQuality).ToArray();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class SkillTreeYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(SkillTree);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                if (Enum.TryParse(scalar.Value.Replace(" ", ""), true, out SkillTree skillTree))
                {
                    return skillTree;
                }
                else
                {
                    throw new Exception($"Failed to parse class tab: {scalar.Value}");
                }
            }

            return null;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class SkillsYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Skill);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                if (Enum.TryParse(scalar.Value.Replace(" ", ""), true, out Skill skill))
                {
                    return skill;
                }
                else
                {
                    throw new Exception($"Failed to parse skill: {scalar.Value}");
                }
            }

            return null;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal static class Helpers
    {
        internal static string GetColorName(Color color)
        {
            if (color.IsNamedColor)
            {
                return color.Name;
            }
            else
            {
                return color.R + ", " + color.G + ", " + color.B;
            }
        }
    }
}
