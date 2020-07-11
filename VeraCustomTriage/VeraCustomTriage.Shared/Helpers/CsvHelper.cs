using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VeraCustomTriage.Shared.Helpers
{
    public static class CsvHelper
    {
        public static string ToCsv<T>(IEnumerable<T> objectlist)
        {
            Type t = typeof(T);
            PropertyInfo[] fields = t.GetProperties();
            string header = string.Join(",", fields.Select(f => $"\"{f.Name}\"").ToArray());
            var csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
                csvdata.AppendLine(ToCsvFields(fields, o));

            return csvdata.ToString();
        }

        public static string ToCsvFields(PropertyInfo[] fields, object o) =>
            string.Join(",", fields.Select(x => $"\"{x.GetValue(o)}\"").ToArray());
    }
}
