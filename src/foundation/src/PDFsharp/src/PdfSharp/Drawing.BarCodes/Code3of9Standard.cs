// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

using System;

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Implementation of the Code 3 of 9 bar code.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class Code3of9Standard : ThickThinBarCode
    {
        /// <summary>
        /// Initializes a new instance of Standard3of9.
        /// </summary>
        public Code3of9Standard()
            : base("", XSize.Empty, CodeDirection.LeftToRight)
        { }

        /// <summary>
        /// Initializes a new instance of Standard3of9.
        /// </summary>
        public Code3of9Standard(string code)
            : base(code, XSize.Empty, CodeDirection.LeftToRight)
        { }

        /// <summary>
        /// Initializes a new instance of Standard3of9.
        /// </summary>
        public Code3of9Standard(string code, XSize size)
            : base(code, size, CodeDirection.LeftToRight)
        { }

        /// <summary>
        /// Initializes a new instance of Standard3of9.
        /// </summary>
        public Code3of9Standard(string code, XSize size, CodeDirection direction)
            : base(code, size, direction)
        { }

        /// <summary>
        /// Returns an array of size 9 that represents the thick (true) and thin (false) lines and spaces
        /// representing the specified digit.
        /// </summary>
        /// <param name="ch">The character to represent.</param>
        static bool[] ThickThinLines(char ch)
        {
            return Lines["0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*".IndexOf(ch, StringComparison.Ordinal)];
        }

        static readonly bool[][] Lines =
        {
            // '0'
            new bool[] {false, false, false, true, true, false, true, false, false},
            // '1'
            new bool[] {true, false, false, true, false, false, false, false, true},
            // '2'
            new bool[] {false, false, true, true, false, false, false, false, true},
            // '3'
            new bool[] {true, false, true, true, false, false, false, false, false},
            // '4'
            new bool[] {false, false, false, true, true, false, false, false, true},
            // '5'
            new bool[] {true, false, false, true, true, false, false, false, false},
            // '6'
            new bool[] {false, false, true, true, true, false, false, false, false},
            // '7'
            new bool[] {false, false, false, true, false, false, true, false, true},
            // '8'
            new bool[] {true, false, false, true, false, false, true, false, false},
            // '9'
            new bool[] {false, false, true, true, false, false, true, false, false},
            // 'A'
            new bool[] {true, false, false, false, false, true, false, false, true},
            // 'B'
            new bool[] {false, false, true, false, false, true, false, false, true},
            // 'C'
            new bool[] {true, false, true, false, false, true, false, false, false},
            // 'D'
            new bool[] {false, false, false, false, true, true, false, false, true},
            // 'E'
            new bool[] {true, false, false, false, true, true, false, false, false},
            // 'F'
            new bool[] {false, false, true, false, true, true, false, false, false},
            // 'G'
            new bool[] {false, false, false, false, false, true, true, false, true},
            // 'H'
            new bool[] {true, false, false, false, false, true, true, false, false},
            // 'I'
            new bool[] {false, false, true, false, false, true, true, false, false},
            // 'J'
            new bool[] {false, false, false, false, true, true, true, false, false},
            // 'K'
            new bool[] {true, false, false, false, false, false, false, true, true},
            // 'L'
            new bool[] {false, false, true, false, false, false, false, true, true},
            // 'M'
            new bool[] {true, false, true, false, false, false, false, true, false},
            // 'N'
            new bool[] {false, false, false, false, true, false, false, true, true},
            // 'O'
            new bool[] {true, false, false, false, true, false, false, true, false},
            // 'P':
            new bool[] {false, false, true, false, true, false, false, true, false},
            // 'Q'
            new bool[] {false, false, false, false, false, false, true, true, true},
            // 'R'
            new bool[] {true, false, false, false, false, false, true, true, false},
            // 'S'
            new bool[] {false, false, true, false, false, false, true, true, false},
            // 'T'
            new bool[] {false, false, false, false, true, false, true, true, false},
            // 'U'
            new bool[] {true, true, false, false, false, false, false, false, true},
            // 'V'
            new bool[] {false, true, true, false, false, false, false, false, true},
            // 'W'
            new bool[] {true, true, true, false, false, false, false, false, false},
            // 'X'
            new bool[] {false, true, false, false, true, false, false, false, true},
            // 'Y'
            new bool[] {true, true, false, false, true, false, false, false, false},
            // 'Z'
            new bool[] {false, true, true, false, true, false, false, false, false},
            // '-'
            new bool[] {false, true, false, false, false, false, true, false, true},
            // '.'
            new bool[] {true, true, false, false, false, false, true, false, false},
            // ' '
            new bool[] {false, true, true, false, false, false, true, false, false},
            // '$'
            new bool[] {false, true, false, true, false, true, false, false, false},
            // '/'
            new bool[] {false, true, false, true, false, false, false, true, false},
            // '+'
            new bool[] {false, true, false, false, false, true, false, true, false},
            // '%'
            new bool[] {false, false, false, true, false, true, false, true, false},
            // '*'
            new bool[] {false, true, false, false, true, false, true, false, false},
        };


        /// <summary>
        /// Calculates the thick and thin line widths,
        /// taking into account the required rendering size.
        /// </summary>
        internal override void CalcThinBarWidth(BarCodeRenderInfo info)
        {
            /*
             * The total width is the sum of the following parts:
             * Starting lines      = 3 * thick + 7 * thin
             *  +
             * Code Representation = (3 * thick + 7 * thin) * code.Length
             *  +
             * Stopping lines      =  3 * thick + 6 * thin
             * 
             * with r = relation ( = thick / thin), this results in
             * 
             * Total width = (13 + 6 * r + (3 * r + 7) * code.Length) * thin
             */
            double thinLineAmount = 13 + 6 * WideNarrowRatio + (3 * WideNarrowRatio + 7) * Text.Length;
            info.ThinBarWidth = Size.Width / thinLineAmount;
        }

        /// <summary>
        /// Checks the code to be convertible into a standard 3 of 9 bar code.
        /// </summary>
        /// <param name="text">The code to be checked.</param>
        protected override void CheckCode(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length == 0)
                throw new ArgumentException(BcgSR.Invalid3Of9Code(text));

            foreach (char ch in text)
            {
                if ("0123456789ABCDEFGHIJKLMNOP'QRSTUVWXYZ-. $/+%*".IndexOf(ch, StringComparison.Ordinal) < 0)
                    throw new ArgumentException(BcgSR.Invalid3Of9Code(text));
            }
        }

        /// <summary>
        /// Renders the bar code.
        /// </summary>
        protected internal override void Render(XGraphics gfx, XBrush brush, XFont? font, XPoint position)
        {
            XGraphicsState state = gfx.Save();

            BarCodeRenderInfo info = new BarCodeRenderInfo(gfx, brush, font, position);
            InitRendering(info);
            info.CurrPosInString = 0;
            //info.CurrPos = Center - Size / 2;
            info.CurrPos = position - CalcDistance(AnchorType.TopLeft, Anchor, Size);

            if (TurboBit)
                RenderTurboBit(info, true);
            RenderStart(info);
            while (info.CurrPosInString < Text.Length)
            {
                RenderNextChar(info);
                RenderGap(info, false);
            }
            RenderStop(info);
            if (TurboBit)
                RenderTurboBit(info, false);
            if (TextLocation != TextLocation.None)
                RenderText(info);

            gfx.Restore(state);
        }

        void RenderNextChar(BarCodeRenderInfo info)
        {
            RenderChar(info, Text[info.CurrPosInString]);
            ++info.CurrPosInString;
        }

        void RenderChar(BarCodeRenderInfo info, char ch)
        {
            bool[] thickThinLines = ThickThinLines(ch);
            int idx = 0;
            while (idx < 9)
            {
                RenderBar(info, thickThinLines[idx]);
                if (idx < 8)
                    RenderGap(info, thickThinLines[idx + 1]);
                idx += 2;
            }
        }

        void RenderStart(BarCodeRenderInfo info)
        {
            RenderChar(info, '*');
            RenderGap(info, false);
        }

        void RenderStop(BarCodeRenderInfo info)
        {
            RenderChar(info, '*');
        }
    }
}
