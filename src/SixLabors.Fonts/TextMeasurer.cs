﻿using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SixLabors.Fonts
{
    /// <summary>
    /// Encapulated logic for laying out and measuring text.
    /// </summary>
    public static class TextMeasurer
    {
        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns>The size of the text if it was to be rendered.</returns>
        public static SizeF Measure(string text, Font font, float dpi)
            => TextMeasurerInt.Default.Measure(text, font, dpi);

        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="dpiX">The X dpi.</param>
        /// <param name="dpiY">The Y dpi.</param>
        /// <returns>The size of the text if it was to be rendered.</returns>
        public static SizeF Measure(string text, Font font, float dpiX, float dpiY)
            => TextMeasurerInt.Default.Measure(text, font, dpiX, dpiY);

        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="options">The style.</param>
        /// <returns>The size of the text if it was to be rendered.</returns>
        public static SizeF Measure(string text, RendererOptions options)
            => TextMeasurerInt.Default.Measure(text, options);

        internal static SizeF GetSize(ImmutableArray<GlyphLayout> glyphLayouts, Vector2 dpi)
        {
            if (glyphLayouts.IsEmpty)
            {
                return new SizeF(0, 0);
            }

            return GetBounds(glyphLayouts, dpi).Size;
        }

        internal static RectangleF GetBounds(ImmutableArray<GlyphLayout> glyphLayouts, Vector2 dpi)
        {
            float left = glyphLayouts.Min(x => x.Location.X);
            float right = glyphLayouts.Max(x => x.Location.X + x.Width);

            // location is bottom left of the line
            float top = glyphLayouts.Min(x => x.Location.Y - x.LineHeight);
            float bottom = glyphLayouts.Max(x => x.Location.Y);

            Vector2 topLeft = new Vector2(left, top) * dpi;
            Vector2 bottomRight = new Vector2(right, bottom) * dpi;

            var size = bottomRight - topLeft;
            return new RectangleF(topLeft.X, topLeft.Y, size.X, size.Y);
        }

        internal class TextMeasurerInt
        {
            internal static TextMeasurerInt Default { get; set; } = new TextMeasurerInt();

            private TextLayout layoutEngine;

            internal TextMeasurerInt(TextLayout layoutEngine)
            {
                this.layoutEngine = layoutEngine;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="TextMeasurer"/> class.
            /// </summary>
            internal TextMeasurerInt()
            : this(TextLayout.Default)
            {
            }

            /// <summary>
            /// Measures the text.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="font">The font.</param>
            /// <param name="dpi">The dpi.</param>
            /// <returns>The size of the text if it was to be rendered.</returns>
            internal SizeF Measure(string text, Font font, float dpi)
            {
                return this.Measure(text, new RendererOptions(font, dpi));
            }

            /// <summary>
            /// Measures the text.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="font">The font.</param>
            /// <param name="dpiX">The x dpi.</param>
            /// <param name="dpiY">The y dpi.</param>
            /// <returns>The size of the text if it was to be rendered.</returns>
            internal SizeF Measure(string text, Font font, float dpiX, float dpiY)
            {
                return this.Measure(text, new RendererOptions(font, dpiX, dpiY));
            }

            /// <summary>
            /// Measures the text.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="options">The style.</param>
            /// <returns>The size of the text if it was to be rendered.</returns>
            internal SizeF Measure(string text, RendererOptions options)
            {
                ImmutableArray<GlyphLayout> glyphsToRender = this.layoutEngine.GenerateLayout(text, options);

                return GetSize(glyphsToRender, new Vector2(options.DpiX, options.DpiY));
            }
        }
    }
}