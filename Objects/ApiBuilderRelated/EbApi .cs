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
        public string Script { get; set; }

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

        public ApiScript Evaluate(ApiResources _prevres)
        {
            string code = this.Script.B2S().Trim();
            ApiScript script = new ApiScript();
            Script valscript = CSharpScript.Create<dynamic>(code,
                   ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core")
                   .WithImports("System.Dynamic", "System", "System.Collections.Generic",
                   "System.Diagnostics", "System.Linq")
                   , globalsType: typeof(ApiGlobals));
            EbDataSet _ds = _prevres.Result as EbDataSet;
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
    public class EbThirdPartyApi: ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Url { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public ApiMethods Method { set; get; }

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
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class RequestParam
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Name { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public object Value { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public bool UseThisVal { set; get; }
    }
}
