using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using ExpressBase.Common.Extensions;
using Newtonsoft.Json;
using ExpressBase.Common;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.Objects;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ExpressBase.CoreBase.Globals;
using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json.Linq;
using ExpressBase.Common.Helpers;
using ServiceStack.RabbitMq;
using ServiceStack;
using ExpressBase.Common.ServiceClients;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbProcessor : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public EvaluatorVersion EvaluatorVersion { get; set; }

        [JsonIgnore]
        public EbSciptEvaluator evaluator = new EbSciptEvaluator
        {
            OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
        };

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

        private Script GetScriptEvaluatorV1()
        {
            string code = this.Script.Code.Trim();

            Script evaluator = CSharpScript.Create<dynamic>(
                               code,
                               ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core")
                               .WithImports("System.Dynamic", "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq")
                               .AddReferences(typeof(EbDataSet).Assembly),
                               globalsType: typeof(ApiGlobals));

            try
            {
                evaluator.Compile();
            }
            catch (Exception ex)
            {
                throw new Exception("script evaluator compilation error, " + ex.Message);
            }

            return evaluator;
        }

        public object ExecuteScript(EbApi Api)
        {
            ApiGlobalParent global;

            if (this.EvaluatorVersion == EvaluatorVersion.Version_1)
                global = new ApiGlobals(Api.GlobalParams);
            else
                global = new ApiGlobalsCoreBase(Api.GlobalParams);

            global.ResourceValueByIndexHandler += (index) =>
            {
                object resourceValue = Api.Resources[index].Result;

                if (resourceValue != null && resourceValue is string converted)
                {
                    if (converted.StartsWith("{") && converted.EndsWith("}") || converted.StartsWith("[") && converted.EndsWith("]"))
                    {
                        string formated = converted.Replace(@"\", string.Empty);
                        return JObject.Parse(formated);
                    }
                }
                return resourceValue;
            };

            global.ResourceValueByNameHandler += (name) =>
            {
                int index = Api.Resources.GetIndex(name);

                object resourceValue = Api.Resources[index].Result;

                if (resourceValue != null && resourceValue is string converted)
                {
                    if (converted.StartsWith("{") && converted.EndsWith("}") || converted.StartsWith("[") && converted.EndsWith("]"))
                    {
                        string formated = converted.Replace(@"\", string.Empty);
                        return JObject.Parse(formated);
                    }
                }
                return resourceValue;
            };

            global.GoToByIndexHandler += (index) =>
            {
                Api.Step = index;
                Api.Resources[index].Result = EbApiHelper.GetResult(Api.Resources[index], Api);
            };

            global.GoToByNameHandler += (name) =>
            {
                int index = Api.Resources.GetIndex(name);
                Api.Step = index;
                Api.Resources[index].Result = EbApiHelper.GetResult(Api.Resources[index], Api);
            };

            global.ExitResultHandler += (obj) =>
            {
                ApiScript script = new ApiScript
                {
                    Data = JsonConvert.SerializeObject(obj),
                };
                Api.ApiResponse.Result = script;
                Api.Step = Api.Resources.Count - 1;
            };

            ApiResources lastResource = Api.Step == 0 ? null : Api.Resources[Api.Step - 1];

            if (this.EvaluatorVersion == EvaluatorVersion.Version_1 && lastResource?.Result is EbDataSet dataSet)
            {
                (global as ApiGlobals).Tables = dataSet.Tables;
            }

            ApiScript result;

            try
            {
                result = this.Execute(global);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ApiScript Execute(ApiGlobalParent global)
        {
            ApiScript script = new ApiScript();

            try
            {
                if (this.EvaluatorVersion == EvaluatorVersion.Version_1)
                {
                    Script evaluator = GetScriptEvaluatorV1();

                    ScriptState result = evaluator.RunAsync(global as ApiGlobals).Result;

                    if (result.ReturnValue != null)
                    {
                        script.Data = JsonConvert.SerializeObject(result.ReturnValue);
                    }
                    else
                    {
                        script.Data = "script executed without any return";
                    }
                }
                else
                {
                    evaluator.Context = global as ApiGlobalsCoreBase;
                    evaluator.EvaluateVariable += EvaluateVariable;
                    object o = evaluator.Execute(this.Script.Code.Trim());
                    script.Data = JsonConvert.SerializeObject(o);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ExplicitExitException)
                    throw e.InnerException;
                else
                    throw new Exception("Execution Error: " + e.Message);
            }
            return script;
        }

        private static void EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            if (e.This is JObject jObj)
            {
                e.Value = jObj[e.Name];
            }
            else if (e.This is CoreBase.Globals.NTVDict p)
            {
                e.Value = p.TryGetMember(e.Name);
            }
        }
    }
}
