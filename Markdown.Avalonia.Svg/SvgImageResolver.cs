using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Media;
using Avalonia.Svg;
using Markdown.Avalonia.Utils;
using Svg.Model.Services;

namespace Markdown.Avalonia.Svg
{
    internal class SvgImageResolver : IImageResolver
    {
        private static readonly AvaloniaSvgAssetLoader _svgAssetLoader = new();
        
        public async Task<IImage?> Load(Stream stream)
        {
            var task = Task.Run(() =>
            {
                if (!IsSvgFile(stream))
                    return null;
                
                var document = SvgService.Open(stream);
                var picture = document is { } ? SvgService.ToModel(document, _svgAssetLoader, out _, out _) : default;
                var svgsrc = new SvgSource { Picture = picture };
                return (IImage)new VectorImage() { Source = svgsrc };
            });

            return await task;
        }

        private static bool IsSvgFile(Stream fileStream)
        {
            try
            {
                int firstChr = fileStream.ReadByte();
                if (firstChr != ('<' & 0xFF))
                    return false;

                fileStream.Seek(0, SeekOrigin.Begin);
                using var xmlReader = XmlReader.Create(fileStream);
                return xmlReader.MoveToContent() == XmlNodeType.Element &&
                       "svg".Equals(xmlReader.Name, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
            finally
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
