using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExpressBase.Objects.Attributes;
using ExpressBase.Common;

namespace ExpressBase.Objects
{
    public class EbToolbox
    {
        public string AllControlls { get; set; }

        public string AllMetas { get; set; }

        public string html { get; set; }

        public string TypeRegister { get; set; }

        public string XXXX { get; set; }

        EbToolbox() { }

        public EbToolbox(BuilderType _builderType)
        {
            string _toolsHtml = string.Empty;

            string _metaStr = "AllMetas = {";

            string _controlsStr = "var EbObjects = {};";

            string xxx = @"
function Proc(jsonObj, rootContainerObj)
{
    ProcRecur(jsonObj.Controls, rootContainerObj.Controls);
};

function ProcRecur(src_controls, dest_controls)
{
    $.each(src_controls.$values, function(i, control) {
        if (!control.IsContainer)
            dest_controls.Append(ObjectFactory(control));
        else
            ProcRecur(control.Controls, dest_controls.Controls);
    });
};
";

            string _typeInfos = "function ObjectFactory(jsonObj) { ";

            var types = this.GetType().GetTypeInfo().Assembly.GetTypes();

            foreach (var tool in types)
            {
                if (tool.GetTypeInfo().IsSubclassOf(typeof(EbControl)))
                {
                    if (tool.GetTypeInfo().IsDefined(typeof(EnableInBuilder))
                         && tool.GetTypeInfo().GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                    {
                        if (!tool.GetTypeInfo().IsDefined(typeof(HideInToolBox)))
                            _toolsHtml += GetToolHtml(tool.Name.Substring(2));

                        var toolObj = Activator.CreateInstance(tool) as EbControl;
                        _typeInfos += string.Format("if (jsonObj['$type'].includes('{0}')) return new EbObjects.{1}Obj(jsonObj.EbSid, jsonObj); ", toolObj.GetType().FullName, toolObj.GetType().Name);
                        toolObj.GetJsObject(_builderType, ref _metaStr, ref _controlsStr);
                    }
                }
            }

            _metaStr += "}";

            _controlsStr += "";

            this.AllMetas = _metaStr;

            this.AllControlls = _controlsStr;

            this.html = _toolsHtml;

            this.TypeRegister = _typeInfos + " };";

            this.XXXX = xxx;
        }

        public string getHead()
        {
            return this.AllControlls + this.AllMetas + this.TypeRegister;
        }

        private static string GetToolHtml(string tool_name)
        {
            return @"<div eb-type='@toolName' class='tool'>
                            @toolName
                    </div>".Replace("@toolName", tool_name);
        }
    }

    public class Meta
    {
        public string name { get; set; }

        public string group { get; set; }

        public PropertyEditorType editor { get; set; }

        public string[] options { get; set; }

        public bool IsUIproperty { get; set; }

        public string helpText { get; set; }
    }
}
