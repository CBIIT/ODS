using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Theradex.ODS.Extractor.Helpers.Extensions
{
    public static class CustomerExtensions
    {
        public static int ConvertSecsToMs(this int seconds)
        {
            return Convert.ToInt32(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
        }

        public static string GetDescription(Enum value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();

            var descriptionAttribute =
                enumMember == null
                    ? default(DescriptionAttribute)
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }

        public static string ToBase64Decode(string base64EncodedText)
        {
            if (string.IsNullOrEmpty(base64EncodedText))
            {
                return base64EncodedText;
            }

            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedText);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void RetryOnException(int times, TimeSpan delay, Action operation)
        {
            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    operation();
                    break; // Sucess! Lets exit the loop!
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                        throw;

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }

        public static DataTable jsonStringToTable(this string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }

        public static string jsonToCSV(this string jsonContent, string delimiter)
        {
            var csvConfig = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                ShouldQuote = args => true,
                Delimiter = delimiter
            };

            StringWriter csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString, csvConfig))
            {
                using (var dt = jsonStringToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
            return csvString.ToString();
        }

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff");
        }

        public static string GetUniqueIdentifier()
        {
            DateTime currentDateTime = DateTime.Now;
            Guid uniqueGuid = Guid.NewGuid();

            string shortGuid = uniqueGuid.ToString("N").Substring(0, 4);
            int timestampHash = Math.Abs(currentDateTime.GetHashCode()) % 10000; // Limit to 4 digits

            string shortDateTime = currentDateTime.ToString("yyMMddHHmmssffff");

            string uniqueIdentifier = $"{shortDateTime}{shortGuid}{timestampHash:D4}";

            return uniqueIdentifier;
        }
    }
}
