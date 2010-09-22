﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ClosedXML.Excel
{
    public class XLWorksheet: IXLWorksheet
    {
        #region Constants

        public const Int32 MaxNumberOfRows = 1048576;
        public const Int32 MaxNumberOfColumns = 16384;

        #endregion

        public XLWorksheet(String sheetName)
        {
            Style = XLWorkbook.DefaultStyle;
            Internals = new XLWorksheetInternals(new Dictionary<IXLAddress, IXLCell>(), new Dictionary<Int32, IXLColumn>(), new Dictionary<Int32, IXLRow>(), new List<String>());
            RowNumber = 1;
            ColumnNumber = 1;
            ColumnLetter = "A";
            PageSetup = new XLPageOptions(XLWorkbook.DefaultPrintOptions);
            this.Name = sheetName;
        }

        public IXLWorksheetInternals Internals { get; private set; }

        #region IXLRange Members

        public Int32 RowNumber { get; private set; }
        public Int32 ColumnNumber { get; private set; }
        public String ColumnLetter { get; private set; }

        public List<IXLColumn> Columns()
        {
            var retVal = new List<IXLColumn>();
            var columnList = new List<Int32>();

            if (Internals.CellsCollection.Count > 0)
                columnList.AddRange(Internals.CellsCollection.Keys.Select(k => k.Column).Distinct());

            if (Internals.ColumnsCollection.Count > 0)
                columnList.AddRange(Internals.ColumnsCollection.Keys.Where(c => !columnList.Contains(c)));

            foreach (var c in columnList)
            {
                retVal.Add(Column(c));
            }

            return retVal;
        }
        public List<IXLColumn> Columns(String columns)
        {
            var retVal = new List<IXLColumn>();
            var columnPairs = columns.Split(',');
            foreach (var pair in columnPairs)
            {
                var columnRange = pair.Split(':');
                var firstColumn = columnRange[0];
                var lastColumn = columnRange[1];
                Int32 tmp;
                if (Int32.TryParse(firstColumn, out tmp))
                    retVal.AddRange(Columns(Int32.Parse(firstColumn), Int32.Parse(lastColumn)));
                else
                    retVal.AddRange(Columns(firstColumn, lastColumn));
            }
            return retVal;
        }
        public List<IXLColumn> Columns(String firstColumn, String lastColumn)
        {
            return Columns(XLAddress.GetColumnNumberFromLetter(firstColumn), XLAddress.GetColumnNumberFromLetter(lastColumn));
        }
        public List<IXLColumn> Columns(Int32 firstColumn, Int32 lastColumn)
        {
            var retVal = new List<IXLColumn>();

            for (var co = firstColumn; co <= lastColumn; co++)
            {
                retVal.Add(Column(co));
            }
            return retVal;
        }

        public List<IXLRow> Rows()
        {
            var retVal = new List<IXLRow>();
            var rowList = new List<Int32>();

            if (Internals.CellsCollection.Count > 0)
                rowList.AddRange(Internals.CellsCollection.Keys.Select(k => k.Row).Distinct());

            if (Internals.ColumnsCollection.Count > 0)
                rowList.AddRange(Internals.ColumnsCollection.Keys.Where(r => !rowList.Contains(r)));

            foreach (var r in rowList)
            {
                retVal.Add(Row(r));
            }

            return retVal;
        }
        public List<IXLRow> Rows(String rows)
        {
            var retVal = new List<IXLRow>();
            var rowPairs = rows.Split(',');
            foreach (var pair in rowPairs)
            {
                var rowRange = pair.Split(':');
                var firstRow = rowRange[0];
                var lastRow = rowRange[1];       
                retVal.AddRange(Rows(Int32.Parse(firstRow), Int32.Parse(lastRow)));
            }
            return retVal;
        }
        public List<IXLRow> Rows(Int32 firstRow, Int32 lastRow)
        {
            var retVal = new List<IXLRow>();

            for (var ro = firstRow; ro <= lastRow; ro++)
            {
                retVal.Add(Row(ro));
            }
            return retVal;
        }
        

        public IEnumerable<IXLCell> Cells()
        {
            return Internals.CellsCollection.Values.AsEnumerable<IXLCell>();
        }

        #endregion

        #region IXLStylized Members

        private IXLStyle style;
        public IXLStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = new XLStyle(this, value);
            }
        }

        public IEnumerable<IXLStyle> Styles
        {
            get 
            {
                UpdatingStyle = true;
                foreach (var c in Internals.CellsCollection.Values)
                {
                    yield return c.Style;
                }
                
                UpdatingStyle = false;
            }
        }

        public Boolean UpdatingStyle { get; set; }

        #endregion

        public IXLRow Row(Int32 row)
        {
            IXLRow xlRow;
            if (Internals.RowsCollection.ContainsKey(row))
            {
                xlRow = Internals.RowsCollection[row];
            }
            else
            {
                var xlRowParameters = new XLRowParameters(this, Style);
                xlRow = new XLRow(row, xlRowParameters);
                Internals.RowsCollection.Add(row, xlRow);
            }

            return xlRow;
        }
        public IXLColumn Column(Int32 column)
        {
            IXLColumn xlColumn;
            if (Internals.ColumnsCollection.ContainsKey(column))
            {
                xlColumn = Internals.ColumnsCollection[column];
            }
            else
            {
                var xlColumnParameters = new XLColumnParameters(this, Style);
                xlColumn = new XLColumn(column, xlColumnParameters);
                Internals.ColumnsCollection.Add(column, xlColumn);
            }

            return xlColumn;
        }
        public IXLColumn Column(String column)
        {
            return Column(XLAddress.GetColumnNumberFromLetter(column));
        }

        #region IXLRange Members

        IXLRange IXLRange.Row(Int32 row)
        {
            var firstCellAddress = new XLAddress(row, 1);
            var lastCellAddress = new XLAddress(row, MaxNumberOfColumns);
            return this.Range(firstCellAddress, lastCellAddress);
        }
        IXLRange IXLRange.Column(int column)
        {
            IXLAddress firstCellAddress = new XLAddress(1, column);
            IXLAddress lastCellAddress = new XLAddress(MaxNumberOfRows, column);
            return this.Range(firstCellAddress, lastCellAddress);
        }
        IXLRange IXLRange.Column(string column)
        {
            IXLAddress firstCellAddress = new XLAddress(1, column);
            IXLAddress lastCellAddress = new XLAddress(MaxNumberOfRows, column);
            return this.Range(firstCellAddress, lastCellAddress);
        }

        #endregion


        public String Name { get; set; }


        public IXLPageSetup PageSetup { get; private set; }


        IXLRangeInternals IXLRange.Internals 
        { 
            get 
            {
                return new XLRangeInternals(Internals.FirstCellAddress, Internals.LastCellAddress, this);
            } 
        }
    }
}
