using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PcmHacking
{
    public partial class SetupHistogram : Form
    {
        public Histogram histogram;
        public Color highValueColor;
        public Color lowValueColor;
        public float highValue = 0;
        public float midValue = 0;
        public float lowValue = 0;
        public string colParameter;
        public string rowParameter;
        public string parameter;
        private List<string> parameters;
        private List<string> colParameters;
        private List<string> rowParameters;

        public SetupHistogram(List<string> avaliableParameters)
        {

            parameters = avaliableParameters.ToList();
            colParameters = avaliableParameters.ToList();
            rowParameters = avaliableParameters.ToList();
            rowParameters.Add(""); //make sure a blank one is avaliable for the row
            InitializeComponent();
            parameterComboBox.DataSource = this.parameters;
            columnAxisParameterComboBox.DataSource = this.colParameters;
            rowAxisParameterComboBox.DataSource = this.rowParameters;
        }

        private void highValueColorButton_Click(object sender, EventArgs e)
        {
            if (highValueColorDialog.ShowDialog() == DialogResult.OK)
            {
                highValueColorButton.BackColor = highValueColorDialog.Color;
            }
        }

        private void lowValueColorButton_Click(object sender, EventArgs e)
        {
            if (lowValueColorDialog.ShowDialog() == DialogResult.OK)
            {
                lowValueColorButton.BackColor = lowValueColorDialog.Color;
            }

        }

        private void oKButton_Click(object sender, EventArgs e)
        {
            #region create Histogram object
            float[] rowHeaders = null;
            float[] columnHeaders = null;
            int decimalPoints = 2;
            int cellHits = 1;
            highValueColor = highValueColorDialog.Color;
            lowValueColor = lowValueColorDialog.Color;
            parameter = this.parameterComboBox.GetItemText(this.parameterComboBox.SelectedItem);
            colParameter = this.columnAxisParameterComboBox.GetItemText(this.columnAxisParameterComboBox.SelectedItem);
            rowParameter = this.rowAxisParameterComboBox.GetItemText(this.rowAxisParameterComboBox.SelectedItem);
            try
            {
                columnHeaders = columnAxisValuesTextBox.Text.Split(',').Select(n => float.Parse(n, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
                if (!String.IsNullOrEmpty(rowAxisValuesTextBox.Text))
                {
                    rowHeaders = rowAxisValuesTextBox.Text.Split(',').Select(n => float.Parse(n, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
                }
            }
            catch
            {
                MessageBox.Show("Check Column Axis Values and Row Axis Values. Make sure they are comma sperated and no other characters are present", "Axis Values Error");
                return;
            }
            if (!String.IsNullOrEmpty(cellHitsTextBox.Text))
            {
                try
                {
                    cellHits = Convert.ToInt32(cellHitsTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Check Cell Hits Text Box. Non whole number detected", "Cell Hits Error");
                    return;
                }
            }
            if (!String.IsNullOrEmpty(numOfDecimalsTextBox.Text))
            {
                try
                {
                    decimalPoints = Convert.ToInt32(numOfDecimalsTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Check Decimals Text Box. Non whole number detected", "Decimal Points Error");
                    return;
                }
            }
            if (String.IsNullOrEmpty(colParameter) || String.IsNullOrEmpty(parameter))
            {
                MessageBox.Show("Your Parameter or Column Axis parameter was not valid.", "Parameter Error");
                return;
            }
            if (rowHeaders != null && !String.IsNullOrEmpty(rowParameter)) //2d histogram
            {
                histogram = new Histogram(columnHeaders, rowHeaders, decimalPoints, cellHits);

            }
            else //1d histogram
            {
                histogram = new Histogram(columnHeaders, decimalPoints, cellHits);
            }
            #endregion

            if (!String.IsNullOrEmpty(highValueTextBox.Text) && !String.IsNullOrEmpty(midValueTextBox.Text) && !String.IsNullOrEmpty(lowValueTextBox.Text))
            {
                try
                {
                    highValue = Convert.ToSingle(highValueTextBox.Text);
                    midValue = Convert.ToSingle(midValueTextBox.Text);
                    lowValue = Convert.ToSingle(lowValueTextBox.Text);
                    if (highValue < midValue || lowValue > midValue) //fix this validation its broken
                    {
                        highValue = 0;
                        midValue = 0;
                        lowValue = 0;
                        MessageBox.Show("Check Shading Values. Valid values not found", "Shading Value Error");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Check Shading Values. Valid value not found", "Shading Value Error");
                    return;
                }
            }
            else
            {
                highValue = 0;
                midValue = 0;
                lowValue = 0;
            }


            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
