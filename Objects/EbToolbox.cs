using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common;
using ExpressBase.Common.Objects;

namespace ExpressBase.Objects
{
    public class EbToolbox
    {
        public string AllControlls { get; set; }

        public string AllMetas { get; set; }

        public string html { get; set; }

        public string TypeRegister { get; set; }

        public string JsonToJsObjectFuncs { get; set; }

        public string EbObjectTypes { get; set; }

        EbToolbox() { }

        public EbToolbox(BuilderType _builderType)
        {
            var typeArray = this.GetType().GetTypeInfo().Assembly.GetTypes();

            //var _jsResult = CSharpToJs.GenerateJs<EbControl>(_builderType, typeArray);
            var _c2js = new Context2Js(typeArray, _builderType, typeof(EbControl));
            System.Console.WriteLine("-------------> Time take for Context2Js: " + _c2js.MilliSeconds.ToString());

            this.AllMetas = _c2js.AllMetas;
            this.AllControlls = _c2js.JsObjects;
            this.html = _c2js.ToolBoxHtml;
            this.TypeRegister = _c2js.TypeRegister;
            this.JsonToJsObjectFuncs = _c2js.JsonToJsObjectFuncs;
            this.EbObjectTypes = _c2js.EbObjectTypes;
        }

        public string getHead()
        {
            return this.EbObjectTypes + this.AllControlls + this.AllMetas + this.TypeRegister;
        }
    }
}
