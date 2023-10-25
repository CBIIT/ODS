using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Theradex.ODS.Extractor.Helpers.Extensions
{
    public static class StringFunctions
    {


        public static bool IsSimpleRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.Ratio(text, text2) >= 70;
        }

        public static bool IsPartialRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.PartialRatio(text, text2) >= 70;
        }

        public static bool IsTokenSortRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.TokenSortRatio(text, text2) >= 100;
        }

        public static bool IsPartialTokenSortRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.PartialTokenSortRatio(text, text2) >= 100;
        }

        public static bool IsTokenSetRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.TokenSetRatio(text, text2) >= 100;
        }

        public static bool IsPartialTokenSetRatio(this string text, string text2, int ratio)
        {
            return FuzzySharp.Fuzz.PartialTokenSetRatio(text, text2) >= ratio;
        }

        public static bool IsWeightedRatio(this string text, string text2)
        {
            return FuzzySharp.Fuzz.WeightedRatio(text, text2) >= 90;
        }

        public static string GetNumbers(this string text)
        {
            return new string(text.Where(char.IsDigit).ToArray());
        }
        public static string ToXML<T>(this T obj)
        {
            try
            {
                // Remove Declaration
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (var stream = new StringWriter())

                using (var writer = XmlWriter.Create(stream, settings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj);
                    return stream.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static T FromXML<T>(this string xml)
        {
            T instance = default(T);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var stringreader = new StringReader(xml))
                {
                    instance = (T)xmlSerializer.Deserialize(stringreader);
                }

                return instance;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsNull(this Guid? value)
        {
            return (value == null);
        }

        public static bool IsNull(this int? value)
        {
            return (value == null);
        }

        public static bool IsNull(this decimal? value)
        {
            return (value == null);
        }

        public static bool IsNull<T>(this T value) where T : class
        {
            return (value == null);
        }

        public static bool NotNull<T>(this T value) where T : class
        {
            return (!value.IsNull<T>());
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool NotNullAndNotEmpty(this string value)
        {
            return (!value.IsNullOrEmpty());
        }

        public static bool NotNullAndNotEmpty(this ICollection value)
        {
            return (value.NotNull<ICollection>() && (value.Count > 0));
        }

        public static bool IsNullOrEmpty(this ICollection value)
        {
            return (!value.NotNullAndNotEmpty());
        }

        public static bool AreEqual(this string value1, string value2)
        {
            return (value1 ?? string.Empty).Trim().Equals((value2 ?? string.Empty).Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool NotEqual(this string value1, string value2)
        {
            return !(value1 ?? string.Empty).Trim().Equals((value2 ?? string.Empty).Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsString(this string value, string searchString)
        {
            if (value.IsNullOrEmpty())
                return false;

            return value.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        public static bool IsCharLength(this string text, int maxCharLength)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            return text.Length <= maxCharLength;
        }

        public static bool IsDouble(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            double value;
            return double.TryParse(text, out value);
        }

        public static bool IsDecimal(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            decimal value;
            return decimal.TryParse(text, out value);
        }

        public static bool IsInt(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            int value;
            return int.TryParse(text, out value);
        }

        public static bool IsLong(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            long value;
            return long.TryParse(text, out value);
        }

        public static bool IsDateTime(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            DateTime value;
            return DateTime.TryParse(text, out value);
        }

        public static bool IsDateExact(this string text, string format)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            DateTime value;
            return DateTime.TryParseExact(text, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out value);
        }

        public static string GetDateExact(this string text, string format)
        {
            return DateTime.ParseExact(text, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).ToString();
        }

        public static DateTime GetDateTimeExact(this string text, string format)
        {
            return DateTime.ParseExact(text, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }

        public static bool IsActiveFlag(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return (text.AreEqual("true") || text.AreEqual("t") || text.AreEqual("false") || text.AreEqual("f") || text.AreEqual("0") || text.AreEqual("1")
                || text.AreEqual("y") || text.AreEqual("n") || text.AreEqual("yes") || text.AreEqual("no"));
        }

        public static bool IsActive(this string text)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return (text.AreEqual("true") || text.AreEqual("t") || text.AreEqual("1") || text.AreEqual("y") || text.AreEqual("yes"));
        }

        public static bool FindString(this string text, string searchString)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string ReplaceAll(this string seed, string[] strings, string replacementString)
        {
            if (string.IsNullOrWhiteSpace(seed))
                return string.Empty;

            return strings.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementString));
        }
    }
}