using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using ExpressBase.Common.Extensions;
using Newtonsoft.Json;
using ExpressBase.Common;
using ExpressBase.Common.Data;
using System.Linq;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.Objects;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ExpressBase.Objects
{
    public enum DataReaderResult
    {
        Actual = 0,
        Formated = 1
    }

    public class ListOrdered : List<ApiResources>
    {
        public ListOrdered()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }
    }

    public class ApiParams
    {
        public List<Param> Default { set; get; }

        public List<Param> Custom { set; get; }

        public ApiParams()
        {
            Default = new List<Param>();
            Custom = new List<Param>();
        }
    }

    public abstract class EbApiWrapper : EbObject
    {

    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    [BuilderTypeEnum(BuilderType.ApiBuilder)]
    public class EbApi : EbApiWrapper, IEBRootObject
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public ListOrdered Resources { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public ApiParams Request { set; get; }
    }

    public abstract class ApiResources : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [UIproperty]
        [MetaOnly]
        public string Label { set; get; }

        public virtual string Reference { set; get; }

        public object Result { set; get; }

        public virtual List<Param> GetOutParams(List<Param> _param) { return new List<Param>(); }

        public virtual object GetResult() { return this.Result; }

        public virtual EbDataColumn GetColumn(int index, string cname) { return null; }

        public virtual object GetColVal(int index, string cname) { return null; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlReader : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Data Settings")]
        public DataReaderResult ResultType { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlReader' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                     </div>".RemoveCR().DoubleQuoted(); ;
        }

        public override List<Param> GetOutParams(List<Param> _param)
        {
            List<Param> p = new List<Param>();
            foreach (EbDataTable table in (this.Result as EbDataSet).Tables)
            {
                string[] c = _param.Select(item => item.Name).ToArray();
                foreach (EbDataColumn cl in table.Columns)
                {
                    if (c.Contains(cl.ColumnName))
                        p.Add(new Param { Name = cl.ColumnName, Type = cl.Type.ToString(), Value = table.Rows[0][cl.ColumnIndex].ToString() });
                }
            }
            return p;
        }

        public override object GetResult()
        {
            if (this.ResultType == DataReaderResult.Formated)
            {
                JsonTableSet table = new JsonTableSet();
                foreach (EbDataTable t in (this.Result as EbDataSet).Tables)
                {
                    JsonTable jt = new JsonTable { TableName = t.TableName };
                    for (int k = 0; k < t.Rows.Count; k++)
                    {
                        JsonColVal d = new JsonColVal();
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            d.Add(t.Columns[i].ColumnName, t.Rows[k][t.Columns[i].ColumnIndex]);
                        }
                        jt.Rows.Add(d);
                    }
                    table.Tables.Add(jt);
                }
                return table;
            }
            else
                return this.Result;
        }

        public override EbDataColumn GetColumn(int index, string cname)
        {
            return (this.Result as EbDataSet).Tables[index].Columns[cname];
        }

        public override object GetColVal(int index, string cname)
        {
            return (this.Result as EbDataSet).Tables[index].Rows;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlFunc : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iSqlFunction)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlFunc' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override List<Param> GetOutParams(List<Param> _param)
        {
            return new List<Param>();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlWriter : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iDataWriter)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlWriter' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbEmailNode : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iEmailBuilder)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='EmailNode' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbProcessor : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Script { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='Processor' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'></div>
                            <div class='CompVersion'></div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public ApiScript Evaluate(ApiResources _prevres, Dictionary<string, object> GlobalParams)
        {
            string code = this.Script.Code.Trim();
            EbDataSet _ds = null;
            ApiScript script = new ApiScript();

            Script valscript = CSharpScript.Create<dynamic>(code,
                ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core")
                .WithImports("System.Dynamic", "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq")
                .AddReferences(typeof(ExpressBase.Common.EbDataSet).Assembly),
                globalsType: typeof(ApiGlobals));

            if (_prevres != null)
                _ds = _prevres.Result as EbDataSet;
            try
            {
                valscript.Compile();
            }
            catch (Exception e)
            {
                throw new ApiException("Compilation Error: " + e.Message);
            }

            try
            {
                ApiGlobals globals = new ApiGlobals(_ds);

                foreach (KeyValuePair<string, object> kp in GlobalParams)
                {
                    globals["Params"].Add(kp.Key, new NTV
                    {
                        Name = kp.Key,
                        Type = (kp.Value.GetType() == typeof(JObject)) ? EbDbTypes.Object:(EbDbTypes)Enum.Parse(typeof(EbDbTypes), kp.Value.GetType().Name, true),
                        Value = kp.Value
                    });
                }

                script.Data = JsonConvert.SerializeObject(valscript.RunAsync(globals).Result.ReturnValue);
            }
            catch (Exception e)
            {
                throw new Exception("Execution Error: " + e.Message);
            }
            return script;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbConnectApi : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iApi)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='ConnectApi' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override List<Param> GetOutParams(List<Param> _param)
        {
            return new List<Param>();
        }

        public override object GetResult()
        {
            return (this.Result as ApiResponse).Result;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbThirdPartyApi : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Url { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public ApiMethods Method { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<RequestHeader> Headers { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<RequestParam> Parameters { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='ThirdPartyApi' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        private List<Param> GetParams()
        {
            return this.Parameters.Select(i => new Param { Name = i.Name, Type = i.Type.ToString(), Value = i.Value })
                    .ToList();
        }

        public string Execute()
        {
            List<Param> param = this.GetParams();
            var uri = new Uri(this.Url);
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                if (this.Headers != null && this.Headers.Any())
                {
                    foreach (RequestHeader header in this.Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                }

                client.BaseAddress = new Uri(uri.GetLeftPart(System.UriPartial.Authority));
                if (this.Method == ApiMethods.POST)
                {
                    var parameters = param.Select(i => new { prop = i.Name, val = i.Value })
                            .ToDictionary(x => x.prop, x => x.val);
                    response = client.PostAsync(uri.PathAndQuery, new FormUrlEncodedContent(parameters)).Result;
                }
                else if (this.Method == ApiMethods.GET)
                {
                    response = client.GetAsync(uri.PathAndQuery).Result;
                }
            }
            return response.Content.ReadAsStringAsync().Result;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class RequestParam : EbApiWrapper
    {
        //[EnableInBuilder(BuilderType.ApiBuilder)]
        //public string Name { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Value { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public EbDbTypes Type { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public bool UseThisVal { set; get; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class RequestHeader : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Value { set; get; }
    }
}
