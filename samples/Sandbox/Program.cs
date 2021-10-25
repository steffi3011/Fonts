// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using SixLabors.Fonts;

namespace Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string fontFile = @"me_quran_volt_newmet.ttf";
            ushort upem = 1250;
            Font font = new FontCollection().Add(fontFile).CreateFont(upem);
            var renderer = new ColorGlyphRenderer();
            string testStr = "للَّهِ";

            TextRenderer.RenderTextTo(renderer, testStr, new RendererOptions(font)
            {
                ApplyKerning = true
            });

            Console.WriteLine($"Glyphs: {renderer.GlyphKeys.Count}");
            foreach (GlyphRendererParameters glyphRendererParameters in renderer.GlyphKeys)
            {
                Console.WriteLine($"GlyphIndex: {glyphRendererParameters.GlyphIndex}");
            }

            foreach (FontRectangle glyphRect in renderer.GlyphRects)
            {
                Console.WriteLine(glyphRect);
            }

            Console.WriteLine("done");
        }
    }
}
