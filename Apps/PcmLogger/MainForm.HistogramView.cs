using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace PcmHacking
{
    public partial class MainForm
    {
        private const string logfileFilter = "Comma Seperated Values (*.csv)|*.csv|All Files|*.*";
        private Histogram histogram;
        private string parameterName;
        private string colParameterName;
        private string rowParameterName;
        private Color highValueColor;
        private Color lowValueColor;
        private float highValue = 0;
        private float midValue = 0;
        private float lowValue = 0;
        private bool loadedCSV;
        private List<string> columnNames = new List<string>();

        private void addRandomTestData()  //for testing
        {
            for (int i = 0; i < 100; i++)
            {
                Random r = new Random();
                int col = r.Next(100, 8000);
                int row = r.Next(15, 105);
                int data = r.Next(-20, 20);
                Thread.Sleep(1);

                histogram.addData(col, row, data);

            }
        }
        private List<string> getAvaliableParametersFromCurrentProfile()
        {
            List<string> avaliableParameters = new List<string>();

            foreach (var column in this.currentProfile.Columns)
            {
                avaliableParameters.Add(column.ToString());
            }
            return avaliableParameters;
        }
        private bool setupHistogram(List<string> parameters) //open setup dialog, set datagridview datasource, and add rowheader lables //return true if valid histogram is made
        {
            using (var setupDialog = new SetupHistogram(parameters))
            {
                var result = setupDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    histogram = new Histogram();
                    histogram = setupDialog.histogram;
                    parameterName = setupDialog.parameter;
                    rowParameterName = setupDialog.rowParameter;
                    colParameterName = setupDialog.colParameter;
                    highValueColor = setupDialog.highValueColor;
                    lowValueColor = setupDialog.lowValueColor;
                    highValue = setupDialog.highValue;
                    midValue = setupDialog.midValue;
                    lowValue = setupDialog.lowValue;

                    //rowAxisLabel.Text = rowParameterName;
                    columnAxisLabel.Text = colParameterName;
                    parameterLabel.Text = parameterName;

                    histogramDataGridView.DataSource = histogram.displayedValues;

                    if (histogram.rowHeader != null)
                    {
                        for (int i = 0; i < histogram.rowHeader.Length; i++)
                        {
                            this.histogramDataGridView.Rows[i].HeaderCell.Value = histogram.rowHeader[i].ToString();
                        }
                        histogramDataGridView.RowHeadersWidth = 70;
                    }


                    foreach (DataGridViewColumn column in histogramDataGridView.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    histogramDataGridView.AutoResizeColumns();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void populateHistogramFromCSV(List<float[]> data) //load data from previous log into histogram
        {
            //go though column names and get index of columns we need
            int parameterIndex = columnNames.IndexOf(parameterName);
            int colParameterIndex = columnNames.IndexOf(colParameterName);
            loadedCSV = true;
            if (!String.IsNullOrEmpty(rowParameterName)) //2d
            {
                int rowParameterIndex = columnNames.IndexOf(rowParameterName);
                foreach (float[] logRow in data)
                {
                    histogram.addData(logRow[colParameterIndex], logRow[rowParameterIndex], logRow[parameterIndex]);
                }
            }
            else //1d
            {
                foreach (float[] logRow in data)
                {
                    histogram.addData(logRow[colParameterIndex], logRow[parameterIndex]);
                }
            }
            histogramDataGridView.AutoResizeColumns();
        }
        private void updateHistogramFromLogger(Logger logger, IEnumerable<string> rowValues)
        {
            if (histogram == null || loadedCSV) //is histogram setup??
            {
                return;
            }
            else
            {
                float parameter = 0;
                float colParameter = 0;
                float rowParameter = 0;
                IEnumerator<string> rowValueEnumerator = rowValues.GetEnumerator();
                foreach (ParameterGroup group in logger.DpidConfiguration.ParameterGroups)
                {
                    foreach (LogColumn column in group.LogColumns)
                    {
                        rowValueEnumerator.MoveNext();
                        string columnName = column.ToString();
                        if (columnName.Equals(parameterName)) //get parameter
                        {
                            parameter = Convert.ToSingle(rowValueEnumerator.Current);
                        }
                        if (columnName.Equals(colParameterName)) //get col parameter
                        {
                            colParameter = Convert.ToSingle(rowValueEnumerator.Current);
                        }
                        if (columnName.Equals(rowParameterName)) //get row parameter, might be blank
                        {
                            rowParameter = Convert.ToSingle(rowValueEnumerator.Current);
                        }
                    }
                }

                foreach (LogColumn mathColumn in logger.MathValueProcessor.GetMathColumns())
                {
                    rowValueEnumerator.MoveNext();
                    string columnName = mathColumn.ToString();
                    if (columnName.Equals(parameterName)) //get parameter
                    {
                        parameter = Convert.ToSingle(rowValueEnumerator.Current);
                    }
                    if (columnName.Equals(colParameterName)) //get col parameter
                    {
                        colParameter = Convert.ToSingle(rowValueEnumerator.Current);
                    }
                    if (columnName.Equals(rowParameterName)) //get row parameter, might be blank
                    {
                        rowParameter = Convert.ToSingle(rowValueEnumerator.Current);
                    }
                }
                //if (float.IsNaN(parameter)){
                //    parameter = 0;
                //}
                //if (float.IsNaN(colParameter))
                //{
                //    colParameter = 0;
                //}
                //if (float.IsNaN(rowParameter))
                //{
                //    rowParameter = 0;
                //}
                if (String.IsNullOrEmpty(rowParameterName)) //1d
                {
                    histogram.addData(colParameter, parameter);
                }
                else //2d
                {
                    histogram.addData(colParameter, rowParameter, parameter);
                }
            }
        }

        //update shading when data changes. Need to move to WPF form for transparant shading
        private void histogramDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //if (histogram.lastChangedCellLocation != null)
            //{
            //    try
            //    {
            //        histogramDataGridView.CurrentCell = histogramDataGridView.Rows[histogram.lastChangedCellLocation[1]].Cells[histogram.lastChangedCellLocation[0]];
            //    }
            //    catch { }
            //}

            //    if (highValue == 0 && midValue == 0 && lowValue == 0)
            //    {
            //        return;
            //    } else {
            //        if (histogram.lastChangedCellLocation != null)
            //        {
            //            var changedCell = histogramDataGridView.Rows[histogram.lastChangedCellLocation[1]].Cells[histogram.lastChangedCellLocation[0]];
            //            if (String.IsNullOrEmpty(changedCell.Value.ToString()))
            //            {
            //                return;
            //            }
            //            else
            //            {
            //                float newCellValue = Convert.ToSingle(changedCell.Value.ToString());
            //
            //                if (newCellValue >= highValue)
            //                {
            //                    changedCell.Style.BackColor = highValueColor;
            //                }
            //                else if (newCellValue <= lowValue)
            //                {
            //                    changedCell.Style.BackColor = lowValueColor;
            //                }
            //                else
            //                {
            //                    if (newCellValue == midValue)
            //                    {
            //                        changedCell.Style.BackColor = Color.FromName("White");
            //                    }
            //                    else if (newCellValue > midValue)
            //                    {
            //                        
            //                        int alpha = Math.Abs(Convert.ToInt32((newCellValue - midValue) / ((highValue - midValue) / 255)));
            //                        Color newCellColor = Color.FromArgb(alpha, highValueColor);
            //                        changedCell.Style.BackColor = newCellColor;
            //                    }
            //                    else if (newCellValue < midValue)
            //                    {
            //                        int alpha = Math.Abs(Convert.ToInt32((midValue - newCellValue) / ((midValue - lowValue) / 255)));
            //                        Color newCellColor = Color.FromArgb(alpha, lowValueColor);
            //                        changedCell.Style.BackColor = newCellColor;
            //
            //                    }
            //
            //                }
            //            }
            //        }
            //    }
            //    //histogramDataGridView.Rows[histogram.lastChangedCellLocation[1]].Cells[histogram.lastChangedCellLocation[0]].Style.BackColor = highValueColor;
            //
        }
        private void rowAxisLabel_Paint(object sender, PaintEventArgs e)
        {
            Font myFont = new Font("Microsoft Sans Serif,", 16);
            Brush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            //rowParameterName = "test string vertical";
            int height = rowAxisLabel.Height;
            e.Graphics.TranslateTransform(4, height-(height/4));
            e.Graphics.RotateTransform(270);
            e.Graphics.DrawString(rowParameterName, myFont, myBrush, 0, 0);
        }


        private void testAddDataButton_Click(object sender, EventArgs e) //call add random data method
        {
            addRandomTestData();
        }
        private void openPreviousLogButton_Click(object sender, EventArgs e) //open CSV file and load data into histogram !!!!Currently ignores first 2 columns. need to handle those.
        { //TODO:: Add ability to seek through file
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = logfileFilter;
            dialog.Multiselect = false;
            dialog.Title = "Open Previous Log";
            dialog.ValidateNames = true;

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                StreamReader reader;
                try
                {
                    reader = new StreamReader(dialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Could not open file. Is it open?", "Open File Error");
                    return;
                }
                columnNames = new List<string>();
                foreach (string column in reader.ReadLine().Split(','))
                {
                    columnNames.Add(column);
                }
                //string[] firstLine = reader.ReadLine().Split(',');
                //int startColumn = 0;
                //for (int i = 0; i < firstLine.Length; i++) //go through the first line and get the bad columns
                //{
                //    try
                //    {
                //        Convert.ToSingle(firstLine[i]);
                //    }
                //    catch
                //    {
                //        startColumn++; //move forward the start column if its bad
                //        columnNames.RemoveAt(i); //and remove the column header for that column
                //    }
                //}
                columnNames.RemoveAt(0); //remove first 2 column names since they are useless
                columnNames.RemoveAt(0);
                List<float[]> logData = new List<float[]>();
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    float[] logLine = new float[line.Length - 2]; //also subtract 2 from array length so it doesnt include first 2 columns
                    for (int i = 0; i < line.Length - 2; i++) //convert all log lines into float arrays #################Starting at 2 for now. fix later. pcmlogger files first 2 columns arent floats
                    {
                        try
                        {
                            logLine[i] = Convert.ToSingle(line[i + 2]);
                        }
                        catch
                        { //ignore any that arent a valid format. Should probably have an error message so it doesnt cause outliers
                        }
                    }
                    logData.Add(logLine); //add all float arrays to List<>
                }
                if (setupHistogram(columnNames)) //open setup with column names. If valid settings are created, populate histogram
                {
                    populateHistogramFromCSV(logData);
                }
            }
        }

        private void openSetupHistogramButton_Click(object sender, EventArgs e)
        {

            if (setupHistogram(getAvaliableParametersFromCurrentProfile()))
            {
                loadedCSV = false;
            }
        }
    }
}
