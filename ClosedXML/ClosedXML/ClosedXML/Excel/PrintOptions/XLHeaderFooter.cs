﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClosedXML.Excel
{
    public class XLHeaderFooter: IXLHeaderFooter
    {
        public XLHeaderFooter()
        {
            Left = new XLHFItem();
            Right = new XLHFItem();
            Center = new XLHFItem();
        }
        public IXLHFItem Left { get; private set; }
        public IXLHFItem Center { get; private set; }
        public IXLHFItem Right { get; private set; }

        public String GetText(XLHFOccurrence occurrence)
        {
            var retVal = String.Empty;
            var leftText = Left.GetText(occurrence);
            var centerText = Center.GetText(occurrence);
            var rightText = Right.GetText(occurrence);
            retVal += leftText.Length > 0 ? "&L" + leftText : String.Empty;
            retVal += centerText.Length > 0 ? "&C" + centerText : String.Empty;
            retVal += rightText.Length > 0 ? "&R" + rightText : String.Empty;
            if (retVal.Length > 255)
                throw new ArgumentOutOfRangeException("Headers and Footers cannot be longer than 255 characters (including style markups)");
            return retVal;
        }
    }
}
