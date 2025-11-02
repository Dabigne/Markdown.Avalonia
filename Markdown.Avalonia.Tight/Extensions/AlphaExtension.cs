using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Markdown.Avalonia.Extensions
{
    public class AlphaExtension : MarkupExtension
    {
        private readonly string _brushName;
        private readonly float _alpha;

        // ReSharper disable once IntroduceOptionalParameters.Global
        public AlphaExtension(string colorKey) : this(colorKey, 1f) { }

        public AlphaExtension(string colorKey, float alpha)
        {
            _brushName = colorKey;
            _alpha = alpha;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var dyExt = new DynamicResourceExtension(_brushName);

            var brush = dyExt.ProvideValue(serviceProvider);

            return new MultiBinding()
            {
                Bindings = new[] { brush },
                Converter = new AlphaConverter(_alpha)
            };
        }

        private class AlphaConverter : IMultiValueConverter
        {
            public float Alpha { get; }

            public AlphaConverter(float alpha)
            {
                Alpha = alpha;
            }

            public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            {
                Color c;
                switch (values[0])
                {
                    case ISolidColorBrush b:
                        c = b.Color;
                        break;
                    case Color col:
                        c = col;
                        break;
                    default:
                        return values[0];
                }

                return new SolidColorBrush(
                            Color.FromArgb(
                                (byte)(c.A / 255f * Alpha * 255f),
                                c.R, c.G, c.B));
            }
        }
    }
}
