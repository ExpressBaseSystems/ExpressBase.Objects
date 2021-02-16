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

        public string ToolsHtml { get; set; }

        public object UserControlsHtml { get; }

        public string TypeRegister { get; set; }

        public string JsonToJsObjectFuncs { get; set; }

        public string EbObjectTypes { get; set; }

        public string EbOnChangeUIfns { get; set; }

        public string QCtrlsNames { get; set; }

        public string ACtrlsNames { get; set; }

        EbToolbox() { }

        public void SetSuveyControlsRoles()
        {
            QCtrlsNames = "eb_QCtrlsNames = [";
            ACtrlsNames = "eb_ACtrlsNames = [";

            Type[] typeArray = typeof(EbDashBoardWraper).GetTypeInfo().Assembly.GetTypes();



            foreach (Type tool in typeArray)
            {
                if ((tool.IsDefined(typeof(EnableInBuilder)) && tool.GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(BuilderType.SurveyControl)))
                {
                    try
                    {
                        if (tool.IsDefined(typeof(SurveyBuilderRoles)) && (tool.GetCustomAttribute<SurveyBuilderRoles>().Roles.Contains(SurveyRoles.AnswerControl)))
                        {
                            ACtrlsNames += "'" + tool.GetTypeInfo().Name + "', ";
                        }
                        if (tool.IsDefined(typeof(SurveyBuilderRoles)) && (tool.GetCustomAttribute<SurveyBuilderRoles>().Roles.Contains(SurveyRoles.QuestionControl)))
                        {
                            QCtrlsNames += "'" + tool.GetTypeInfo().Name + "', ";
                        }
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine("Exception: " + ee.ToString());
                        throw new Exception(ee.Message);
                    }
                }
            }
            QCtrlsNames = QCtrlsNames.TrimEnd(',') + "]";
            ACtrlsNames = ACtrlsNames.TrimEnd(',') + "]";

        }

        public EbToolbox(BuilderType _builderType)
        {
            Type[] _typeArray = (this.GetType().GetTypeInfo().Assembly.GetTypes());
            Type[] typeArray = new Type[_typeArray.Length + 1];
            _typeArray.CopyTo(typeArray, 0);
            typeArray[_typeArray.Length] = typeof(EbValidator);

            //var _jsResult = CSharpToJs.GenerateJs<EbControl>(_builderType, typeArray);
            Context2Js _c2js = new Context2Js(typeArray, _builderType, typeof(EbObject));
            System.Console.WriteLine("-------------> Time take for Context2Js: " + _c2js.MilliSeconds.ToString());

            this.AllMetas = _c2js.AllMetas;
            this.AllControlls = _c2js.JsObjects;
            this.ToolsHtml = _c2js.ToolsHtml;
            this.TypeRegister = _c2js.TypeRegister;
            this.JsonToJsObjectFuncs = _c2js.JsonToJsObjectFuncs;
            this.EbObjectTypes = _c2js.EbObjectTypes;
            this.EbOnChangeUIfns = _c2js.EbOnChangeUIfns;
        }

        public string getHead()
        {
            return this.EbObjectTypes + this.AllControlls + this.AllMetas + this.TypeRegister;
        }

        private string getUserControlsHtml()
        {


            return string.Empty;
        }
    }
}
