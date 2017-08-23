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

        public string XXXX { get; set; }

        EbToolbox() { }

        public EbToolbox(BuilderType _builderType)
        {
            var typeArray = this.GetType().GetTypeInfo().Assembly.GetTypes();

            var _jsResult = CSharpToJs.GenerateJs<EbControl>(_builderType, typeArray);

            this.AllMetas = _jsResult.Meta;
            this.AllControlls = _jsResult.JsObjects;
            this.html = _jsResult.ToolBoxHtml;
            this.TypeRegister = _jsResult.TypeRegister;
            this.XXXX = _jsResult.XXX;
        }

        public string getHead()
        {
            return this.AllControlls + this.AllMetas + this.TypeRegister;
        }
    }
}
