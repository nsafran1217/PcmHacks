using System;
using System.Collections.Generic;
using System.Text;

namespace PcmHacking
{ 
    public class HistogramProfile
    {
        public string Name { get; set; }
        public string columnHeaders { get; set; }
        public string rowHeaders { get; set; }
        public string columnParameter { get; set; }
        public string rowParameter { get; set; }
        public string parameter { get; set; }
        public string cellHits { get; set; }
        public string decimalPoints { get; set; }
        //colors not implemented yet

        public HistogramProfile()
        {

        }

        public HistogramProfile(
            string Name,
            string columnHeaders, 
            string rowHeaders, 
            string columnParameter, 
            string rowParameter,
            string parameter,
            string cellHits,
            string decimalPoints)
        {
            this.Name = Name;
            this.columnHeaders = columnHeaders;
            this.rowHeaders = rowHeaders;
            this.columnParameter = columnParameter;
            this.rowParameter = rowParameter;
            this.parameter = parameter;
            this.cellHits = cellHits;
            this.decimalPoints = decimalPoints;
        }

        //public HistogramProfile( //without row headers
        //    string Name,
        //     string columnHeaders,
        //     string columnParameter,
        //     string parameter,
        //     string cellHits,
        //     string decimalPostrings)
        //{
        //    this.Name = Name;
        //    this.columnHeaders = columnHeaders;
        //    this.columnParameter = columnParameter;
        //    this.parameter = parameter;
        //    this.cellHits = cellHits;
        //    this.decimalPostrings = decimalPostrings;
        //}
    }   //
}
