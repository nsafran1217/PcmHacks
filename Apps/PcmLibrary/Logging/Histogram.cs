using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace PcmHacking
{
    public class Histogram
    {
        public float[] columnHeader { get; set; } //eg RPM
        public float[] rowHeader { get; set; } //   eg MAP
        private int decimalPlaces; //rounding for averging values
        private int cellHits; //how many times a cell needs a value before it is populated
        //public int[,] tableDisplayed; //x,y,value //meaning the data that is shown on the gridview
        //public BindingList<HistogramRow[]> tableDisplayed = new BindingList<HistogramRow[]>();
        public DataTable displayedValues = new DataTable("CurrentTableValues");
        public int[] lastChangedCellLocation { get; private set; }
        private DataColumn column;
        private DataRow row;
        private List<float[]> oneDimensionLogData = new List<float[]>(); //float[col, parameter]
        private List<float[]> twoDimensionLogData = new List<float[]>(); //float[col, row, parameter]

        public Histogram()
        {
            this.cellHits = 1;
            this.decimalPlaces = 2;
        }


        public Histogram(int decimalPlaces, int cellHits)
        {
            this.decimalPlaces = decimalPlaces;
            this.cellHits = cellHits;
        }

        public Histogram(float[] columnHeader, float[] rowHeader, int decimalPlaces, int cellHits) //2d constructor
        {
            setHeaders(columnHeader, rowHeader);
            this.decimalPlaces = decimalPlaces;
            this.cellHits = cellHits;
        }
        public Histogram(float[] columnHeader, int decimalPlaces, int cellHits) //1d constructor
        {
            setHeaders(columnHeader);
            this.decimalPlaces = decimalPlaces;
            this.cellHits = cellHits;
        }

        public void setHeaders(float[] columnHeader, float[] rowHeader) //two-d table
        {
            this.columnHeader = columnHeader;
            this.rowHeader = rowHeader;

            for (int i = 0; i < columnHeader.Length; i++) //add columns
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = columnHeader[i].ToString();
                column.ReadOnly = false;
                column.Unique = false;
                displayedValues.Columns.Add(column);
            }

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(displayedValues);
            for (int i = 0; i < rowHeader.Length; i++)
            {
                row = displayedValues.NewRow();
                displayedValues.Rows.Add(row);
            }

        }
        public void setHeaders(float[] columnHeader) //one-d table
        {
            this.columnHeader = columnHeader;

            for (int i = 0; i < columnHeader.Length; i++) //add columns
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = columnHeader[i].ToString();
                column.ReadOnly = false;
                column.Unique = false;
                displayedValues.Columns.Add(column);
            }

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(displayedValues);
            row = displayedValues.NewRow(); //only add 1 row
            displayedValues.Rows.Add(row);

        }

        public void addData(float colData, float rowData, float value) //add data to a 2d histogram
        {
            var nearestColumnValue = columnHeader.OrderBy(x => Math.Abs(colData - x)).First();
            var nearestRowValue = rowHeader.OrderBy(x => Math.Abs(rowData - x)).First();
            twoDimensionLogData.Add(new float[] { nearestColumnValue, nearestRowValue, value }); //40, 1102, .9 //stores into list of float[]. Just store the row and column header it goes under. I dont think i need raw data.

            IEnumerable<float[]> query =
                from row in twoDimensionLogData
                where row[0] == nearestColumnValue && row[1] == nearestRowValue
                select row;
            double valueAverage = 0;
            List<float> matchValues = new List<float>();
            foreach (float[] row in query)
            {
                matchValues.Add(row[2]); //i can move this into a private void for both of theses methods
            }
            if (matchValues.Count >= cellHits) //only do average if enough values land in cell
            {
                valueAverage = Math.Round(matchValues.Average(), decimalPlaces);
                int xVal = Array.IndexOf(columnHeader, nearestColumnValue);
                int yVal = Array.IndexOf(rowHeader, nearestRowValue);
                displayedValues.Rows[yVal][xVal] = valueAverage;
                lastChangedCellLocation = new int[] { xVal, yVal };
            }

        }
        public void addData(float colData, float value) //add data to a 1d histogram
        {
            var nearestColumnValue = columnHeader.OrderBy(x => Math.Abs(colData - x)).First();
            oneDimensionLogData.Add(new float[] { nearestColumnValue, value }); //40, .9 //stores into list of float[]. Just store the column header it goes under. I dont think i need raw data.

            IEnumerable<float[]> query = //find all the values that go in the same cell
                from row in oneDimensionLogData
                where row[0] == nearestColumnValue
                select row;
            double valueAverage = 0;
            List<float> matchValues = new List<float>();
            foreach (float[] row in query)
            {
                matchValues.Add(row[1]); //add all the values that go in the same cell to a list
            }
            if (matchValues.Count >= cellHits) //only do average if enough values land in cell
            {
                valueAverage = Math.Round(matchValues.Average(), decimalPlaces); //average all the values and round to the specified decimal places
                int xVal = Array.IndexOf(columnHeader, nearestColumnValue);
                displayedValues.Rows[0][xVal] = valueAverage;
                lastChangedCellLocation = new int[] { xVal, 0 };
            }

        }

    }
}
