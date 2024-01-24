using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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


        public static T GetValueOrDefault<T>(this DbDataReader dr,
                                                          string name)
        {
            object value = dr[name];
            if (DBNull.Value == value) return default(T);
            return (T)value;
        }

        public static T GetValueOrDefault<T>(this DbDataReader dr,
                                                            int index)
        {
            if (dr.IsDBNull(index)) return default(T);
            return (T)dr[index];
        }

        public static bool IsDBNull(this DbDataReader dr, string name)
        {
            return dr.IsDBNull(dr.GetOrdinal(name));
        }
        public static T GetValueOrDefault<T>(this DbDataReader reader, int ordinal, T defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetFieldValue<T>(ordinal);
        }

    }

}