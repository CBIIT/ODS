using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Theradex.ODS.Manager.Helpers
{
    public static class UtilityFunctions
    {
        public static DataTable ConvertCsvToDataTable(string data)
        {
            DataTable csvData = new DataTable();
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                MemoryStream stream = new MemoryStream(byteArray);

                using (TextFieldParser csvReader = new TextFieldParser(stream))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();

                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();

                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return csvData;
        }

        public static DataTable ConvertCsvFileToDataTable(string filePath)
        {
            //reading all the lines(rows) from the file.
            string data = File.ReadAllText(filePath);

            return ConvertCsvToDataTable(data);
        }
        public static void SaveDataTableToCsv(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);

            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                //IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                //IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());

                IEnumerable<string> fields = row.ItemArray.Select(field => string.Format("\"{0}\"", field));

                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

    }
}