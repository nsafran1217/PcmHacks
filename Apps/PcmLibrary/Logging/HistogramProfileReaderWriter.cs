using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PcmHacking
{
    public class HistogramProfileReaderWriter
    {
        public static void createInitialProfile(string path) //make blank one if needed
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = Environment.NewLine;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                XDocument document = new XDocument();
                XElement top = new XElement("HistogramProfile");
                document.Add(top);

                XElement presets = new XElement("HistogramPresets");
                top.Add(presets);

                //XElement element = new XElement("Preset");
                //
                //element.SetAttributeValue("Name", profile.Name);
                //element.SetAttributeValue("ColumnHeaders", profile.columnHeaders);
                //element.SetAttributeValue("RowHeaders", profile.rowHeaders);
                //element.SetAttributeValue("ColumnParameter", profile.columnParameter);
                //element.SetAttributeValue("RowParameter", profile.rowParameter);
                //element.SetAttributeValue("Parameter", profile.parameter);
                //element.SetAttributeValue("CellHits", profile.cellHits);
                //element.SetAttributeValue("DecimalPoints", profile.decimalPoints);
                //
                //presets.Add(element);

                document.Save(writer);
            }
            }
            public static void writeHistogramProfile(HistogramProfile profile, string path)
        {
            try
            {
                XDocument xml = XDocument.Load(path);
            }
            catch
            {
                createInitialProfile(path);
            }
            finally
            {
                XDocument xml = XDocument.Load(path);

                XElement presets = xml.Root.Element("HistogramPresets");

                XElement element = new XElement("Preset");
                element.SetAttributeValue("Name", profile.Name);
                element.SetAttributeValue("ColumnHeaders", profile.columnHeaders);
                element.SetAttributeValue("RowHeaders", profile.rowHeaders);
                element.SetAttributeValue("ColumnParameter", profile.columnParameter);
                element.SetAttributeValue("RowParameter", profile.rowParameter);
                element.SetAttributeValue("Parameter", profile.parameter);
                element.SetAttributeValue("CellHits", profile.cellHits);
                element.SetAttributeValue("DecimalPoints", profile.decimalPoints);

                presets.Add(element);

                xml.Save(path);
            }
        }
        public static List<HistogramProfile> readHistogramProfiles(string path)
        {
            List<HistogramProfile> histogramProfiles = new List<HistogramProfile>();
            try
            {
                XDocument xml = XDocument.Load(path);

                XElement container = xml.Root.Elements("HistogramPresets").FirstOrDefault();
                XElement test = container;
                if (container != null)
                {
                    foreach (XElement parameterElement in container.Elements("Preset"))
                    {
                        HistogramProfile profile = new HistogramProfile(
                            parameterElement.Attribute("Name").Value,
                            parameterElement.Attribute("ColumnHeaders").Value,
                            parameterElement.Attribute("RowHeaders").Value,
                            parameterElement.Attribute("ColumnParameter").Value,
                            parameterElement.Attribute("RowParameter").Value,
                            parameterElement.Attribute("Parameter").Value,
                            parameterElement.Attribute("CellHits").Value,
                            parameterElement.Attribute("DecimalPoints").Value
                            );

                        histogramProfiles.Add(profile);
                    }
                }
            }
            catch { }
            return histogramProfiles;
        }
        public static void deleteHistogramProfile(string presetName, string path)
        {
            XDocument doc = XDocument.Load(path);
            var q = from node in doc.Descendants("Preset")
                    let attr = node.Attribute("Name")
                    where attr != null && attr.Value == presetName
                    select node;
            q.ToList().ForEach(x => x.Remove());

            doc.Save(path);

        } 

    }
}

