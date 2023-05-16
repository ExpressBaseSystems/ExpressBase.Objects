using ExpressBase.Common.Data;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.Helpers
{
    public class SqlHelper
    {
        public static List<Param> GetSqlParams(string sql, int obj_type)
        {
            //sql = Base64Decode(sql);
            if (obj_type == EbObjectTypes.DataWriter || obj_type == EbObjectTypes.DataReader)
            {
                return GetSqlParams(sql);
            }
            else if (!string.IsNullOrEmpty(sql) && obj_type == EbObjectTypes.SqlFunction)
            {
                List<Param> param = new List<Param>();
                Regex r = new Regex(@"(\w+)(\s+|)\(.*?\)");
                Regex r1 = new Regex(@"\(.*?\)");
                //string _func = r.Match(sql.Replace("\n", "").Replace("\r", "").Replace("\t", "")).Groups[1].Value;
                string _params = r.Match(sql.Replace("\n", "").Replace("\r", "").Replace("\t", "")).Groups[0].Value;
                string[] _arguments = r1.Match(_params).Groups[0].Value.Replace("(", "").Replace(")", "").Split(",");

                foreach (string _arg in _arguments)
                {
                    if (!string.IsNullOrEmpty(_arg))
                    {
                        param.Add(new Param
                        {
                            Name = _arg.Split(" ")[0],
                        });
                    }
                }
                return param;
            }
            else
                return null;
        }

        public static List<Param> GetSqlParams(string sql)
        {
            List<Param> param = new List<Param>();
            sql = Regex.Replace(sql, @"'([^']*)'", string.Empty);
            //Regex r = new Regex(@"\:\w+|\@\w+g");
            Regex r = new Regex(@"((?<=:(?<!::))\w+|(?<=@(?<!::))\w+)");

            foreach (Match match in r.Matches(sql))
            {
                if (param.Find(e => e.Name == match.Value) == null)
                    param.Add(new Param { Name = match.Value });
            }
            return param;
        }

        public static bool ContainsParameter(string Query, string Parameter)
        {
            Regex r = new Regex(@"((?<=:(?<!::))|(?<=@(?<!::)))" + Parameter + @"\b");
            return r.IsMatch(Query);
        }

        public static string RenameParameter(string Query, string Parameter, string NewParameter)
        {
            Regex r = new Regex(@"((?<=:(?<!::))|(?<=@(?<!::)))" + Parameter + @"\b");
            return r.Replace(Query, NewParameter);
        }

        //replace @ : also
        public static string ReplaceParamByValue(string Query, string ParamName, string Value)
        {
            Regex r = new Regex(@"((:(?<!::))|(@(?<!::)))" + ParamName + @"\b");
            return r.Replace(Query, Value);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
