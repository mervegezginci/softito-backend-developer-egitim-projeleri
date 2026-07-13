using PdfSharp.Fonts;
using System;
using System.IO;

namespace seyahat_projesi.Services
{
    public class MyFontResolver : IFontResolver
    {
        public byte[]? GetFont(string faceName)
        {
            string fontFile = "arial.ttf";
            
            if (faceName.Contains("Bold", StringComparison.OrdinalIgnoreCase))
            {
                fontFile = "arialbd.ttf";
            }
            else if (faceName.Contains("Italic", StringComparison.OrdinalIgnoreCase))
            {
                fontFile = "ariali.ttf";
            }

            string fontPath = Path.Combine("C:\\Windows\\Fonts", fontFile);
            
            if (File.Exists(fontPath))
            {
                return File.ReadAllBytes(fontPath);
            }
            
            // Fallback to Arial regular if bold/italic not found
            string fallbackPath = Path.Combine("C:\\Windows\\Fonts", "arial.ttf");
            if (File.Exists(fallbackPath))
            {
                return File.ReadAllBytes(fallbackPath);
            }

            return null;
        }

        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string name = "Arial";
            if (isBold && isItalic) name += "BoldItalic";
            else if (isBold) name += "Bold";
            else if (isItalic) name += "Italic";

            return new FontResolverInfo(name);
        }
    }
}
