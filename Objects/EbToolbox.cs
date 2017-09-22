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

            var _jsResult = CSharpToJs.GenerateJs<EbControl>(_builderType, typeArray);

            this.AllMetas = _jsResult.Meta;
            this.AllControlls = _jsResult.JsObjects;
            this.html = _jsResult.ToolBoxHtml;
            this.TypeRegister = _jsResult.TypeRegister;
            this.JsonToJsObjectFuncs = _jsResult.JsonToJsObjectFuncs;
            this.EbObjectTypes = _jsResult.EbObjectTypes;
        }

        public string getHead()
        {
            return this.EbObjectTypes + this.AllControlls + this.AllMetas + this.TypeRegister;
        }
    }
}
