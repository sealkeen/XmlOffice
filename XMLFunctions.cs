// StackOverflow open-source license (copy-pasted from there)

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XMLOffice
{
    public static class XMLFunctions
    {
        public static List<Tuple<string, string>> GetXMlTagsAndValues(string xml)
        {
            var xmlList = new List<Tuple<string, string>>();

            var doc = XDocument.Parse(xml);

            foreach (var element in doc.Descendants())
            {
                // we don't care about the parent tags
                if (element.Descendants().Count() > 0)
                {
                    continue;
                }

                //var path = element.AncestorsAndSelf().Select(e => e.Name.LocalName).Reverse();
                var xPath = element.Name;//;string.Join("/", path);

                xmlList.Add(Tuple.Create(xPath.LocalName, element.Value));
            }

            return xmlList;
        }

        public static System.Data.DataTable CreateDataTableFromXmlFile(string xmlFilePath)
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            string input = File.ReadAllText(xmlFilePath);

            var xmlTagsAndValues = GetXMlTagsAndValues(input);
            var columnList = new List<string>();

            foreach (var xml in xmlTagsAndValues)
            {
                if (!columnList.Contains(xml.Item1))
                {
                    var name = (xml.Item1);

                    columnList.Add(name);
                    Dt.Columns.Add(name, typeof(string));
                }
            }

            int rowID = 0;
            DataRow dtrow = Dt.NewRow();
            foreach (var xml in xmlTagsAndValues)
            {

                var columnList2 = new Dictionary<string, string>();
                if (!columnList2.Keys.Contains(xml.Item1)) {
                    dtrow[xml.Item1] = xml.Item2;
                    columnList2.Add(xml.Item1, xml.Item2);
                }
                else {   // Here we are using the same column but appending the next value
                    dtrow[xml.Item1] = columnList2[xml.Item1] + "," + xml.Item2;

                    columnList2[xml.Item1] = columnList2[xml.Item1] + "," + xml.Item2;
                }
                if (rowID + 1 == columnList.Count)
                {
                    Dt.Rows.Add(dtrow);
                    dtrow = Dt.NewRow();
                    rowID = 0; continue;
                }
                rowID++;
            }

            return Dt;
        }
    }
}
