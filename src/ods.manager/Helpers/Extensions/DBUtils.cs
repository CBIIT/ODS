using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Theradex.ODS.Manager.Helpers.Extensions
{
    public static class DBUtils
    {
        public static T FromDB<T>(object value)
        {
            return value == DBNull.Value ? default(T) : (T)value;
        }

        public static object ToDB<T>(T value)
        {
            return value == null ? (object)DBNull.Value : value;
        }
    }

}