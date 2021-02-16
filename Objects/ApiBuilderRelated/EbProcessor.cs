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

namespace ExpressBase.Objects
{
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

        private Script GetScriptEvaluator()
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

        public ApiScript Execute(ApiGlobals global)
        {
            Script evaluator = GetScriptEvaluator();

            ApiScript script = new ApiScript();

            try
            {
                ScriptState result = evaluator.RunAsync(global).Result;

                if (result.ReturnValue != null)
                {
                    script.Data = JsonConvert.SerializeObject(result.ReturnValue);
                }
                else
                {
                    script.Data = "script executed without any return";
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
    }
}
