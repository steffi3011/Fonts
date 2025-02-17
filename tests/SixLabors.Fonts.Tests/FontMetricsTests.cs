// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using SixLabors.Fonts.Unicode;
using Xunit;

namespace SixLabors.Fonts.Tests
{
    public class FontMetricsTests
    {
        [Fact]
        public void FontMetricsMatchesReference()
        {
            // Compared to FontForge metrics.
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.OpenSansFile);
            Font font = family.CreateFont(12);

            Assert.Equal(2048, font.FontMetrics.UnitsPerEm);
            Assert.Equal(2189, font.FontMetrics.Ascender);
            Assert.Equal(-600, font.FontMetrics.Descender);
            Assert.Equal(0, font.FontMetrics.LineGap);
            Assert.Equal(2789, font.FontMetrics.LineHeight);
            Assert.Equal(2470, font.FontMetrics.AdvanceWidthMax);
            Assert.Equal(font.FontMetrics.LineHeight, font.FontMetrics.AdvanceHeightMax);

            Assert.False(font.IsBold);
            Assert.False(font.IsItalic);
        }

        [Fact]
        public void FontMetricsVerticalFontMatchesReference()
        {
            // Compared to EveryFonts TTFDump metrics
            // https://everythingfonts.com/ttfdump
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.NotoSansSCThinFile);
            Font font = family.CreateFont(12);

            Assert.Equal(1000, font.FontMetrics.UnitsPerEm);
            Assert.Equal(806, font.FontMetrics.Ascender);
            Assert.Equal(-256, font.FontMetrics.Descender);
            Assert.Equal(90, font.FontMetrics.LineGap);
            Assert.Equal(1152, font.FontMetrics.LineHeight);
            Assert.Equal(1000, font.FontMetrics.AdvanceWidthMax);
            Assert.Equal(1000, font.FontMetrics.AdvanceHeightMax);

            Assert.False(font.IsBold);
            Assert.False(font.IsItalic);
        }

        [Fact]
        public void GlyphMetricsMatchesReference()
        {
            // Compared to FontForge metrics
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.OpenSansFile);
            Font font = family.CreateFont(12);

            var codePoint = new CodePoint('A');
            GlyphMetrics glyphMetrics = font.FontMetrics.GetGlyphMetrics(codePoint, ColorFontSupport.None).First();
            GlyphMetrics glyphMetrics1 = font.GetGlyphs(codePoint, ColorFontSupport.None).First().GlyphMetrics;

            Assert.Equal(glyphMetrics, glyphMetrics1);

            Assert.Equal(codePoint, glyphMetrics.CodePoint);
            Assert.Equal(font.FontMetrics.UnitsPerEm, glyphMetrics.UnitsPerEm);
            Assert.Equal(glyphMetrics.UnitsPerEm * 72F, glyphMetrics.ScaleFactor);
            Assert.Equal(1295, glyphMetrics.AdvanceWidth);
            Assert.Equal(2789, glyphMetrics.AdvanceHeight);
            Assert.Equal(1293, glyphMetrics.Width);
            Assert.Equal(1468, glyphMetrics.Height);
            Assert.Equal(0, glyphMetrics.LeftSideBearing);
            Assert.Equal(721, glyphMetrics.TopSideBearing);
            Assert.Equal(GlyphType.Standard, glyphMetrics.GlyphType);
        }

        [Fact]
        public void GlyphMetricsMatchesReference_WithWoff1format()
        {
            // Compared to EveryFonts TTFDump metrics
            // https://everythingfonts.com/ttfdump
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.OpenSansFileWoff1);
            Font font = family.CreateFont(12);

            var codePoint = new CodePoint('A');
            GlyphMetrics glyphMetrics = font.FontMetrics.GetGlyphMetrics(codePoint, ColorFontSupport.None).First();
            GlyphMetrics glyphMetrics1 = font.GetGlyphs(codePoint, ColorFontSupport.None).First().GlyphMetrics;

            Assert.Equal(glyphMetrics, glyphMetrics1);

            Assert.Equal(codePoint, glyphMetrics.CodePoint);
            Assert.Equal(font.FontMetrics.UnitsPerEm, glyphMetrics.UnitsPerEm);
            Assert.Equal(glyphMetrics.UnitsPerEm * 72F, glyphMetrics.ScaleFactor);
            Assert.Equal(1295, glyphMetrics.AdvanceWidth);
            Assert.Equal(2789, glyphMetrics.AdvanceHeight);
            Assert.Equal(1293, glyphMetrics.Width);
            Assert.Equal(1468, glyphMetrics.Height);
            Assert.Equal(0, glyphMetrics.LeftSideBearing);
            Assert.Equal(721, glyphMetrics.TopSideBearing);
            Assert.Equal(GlyphType.Standard, glyphMetrics.GlyphType);
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void GlyphMetricsMatchesReference_WithWoff2format()
        {
            // Compared to EveryFonts TTFDump metrics
            // https://everythingfonts.com/ttfdump
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.OpenSansFileWoff2);
            Font font = family.CreateFont(12);

            var codePoint = new CodePoint('A');
            GlyphMetrics glyphMetrics = font.FontMetrics.GetGlyphMetrics(codePoint, ColorFontSupport.None).First();
            GlyphMetrics glyphMetrics1 = font.GetGlyphs(codePoint, ColorFontSupport.None).First().GlyphMetrics;

            Assert.Equal(glyphMetrics, glyphMetrics1);

            Assert.Equal(codePoint, glyphMetrics.CodePoint);
            Assert.Equal(font.FontMetrics.UnitsPerEm, glyphMetrics.UnitsPerEm);
            Assert.Equal(glyphMetrics.UnitsPerEm * 72F, glyphMetrics.ScaleFactor);
            Assert.Equal(1295, glyphMetrics.AdvanceWidth);
            Assert.Equal(2789, glyphMetrics.AdvanceHeight);
            Assert.Equal(1293, glyphMetrics.Width);
            Assert.Equal(1468, glyphMetrics.Height);
            Assert.Equal(0, glyphMetrics.LeftSideBearing);
            Assert.Equal(721, glyphMetrics.TopSideBearing);
            Assert.Equal(GlyphType.Standard, glyphMetrics.GlyphType);
        }
#endif

        [Fact]
        public void GlyphMetricsVerticalMatchesReference()
        {
            // Compared to EveryFonts TTFDump metrics
            // https://everythingfonts.com/ttfdump
            var collection = new FontCollection();
            FontFamily family = collection.Add(TestFonts.NotoSansSCThinFile);
            Font font = family.CreateFont(12);

            var codePoint = new CodePoint('A');
            GlyphMetrics glyphMetrics = font.FontMetrics.GetGlyphMetrics(codePoint, ColorFontSupport.None).First();
            GlyphMetrics glyphMetrics1 = font.GetGlyphs(codePoint, ColorFontSupport.None).First().GlyphMetrics;

            Assert.Equal(glyphMetrics, glyphMetrics1);

            // Position 0.
            Assert.Equal(codePoint, glyphMetrics.CodePoint);
            Assert.Equal(font.FontMetrics.UnitsPerEm, glyphMetrics.UnitsPerEm);
            Assert.Equal(glyphMetrics.UnitsPerEm * 72F, glyphMetrics.ScaleFactor);
            Assert.Equal(364, glyphMetrics.AdvanceWidth);
            Assert.Equal(1000, glyphMetrics.AdvanceHeight);
            Assert.Equal(265, glyphMetrics.Width);
            Assert.Equal(666, glyphMetrics.Height);
            Assert.Equal(33, glyphMetrics.LeftSideBearing);
            Assert.Equal(134, glyphMetrics.TopSideBearing);
            Assert.Equal(GlyphType.Fallback, glyphMetrics.GlyphType);
        }
    }
}
