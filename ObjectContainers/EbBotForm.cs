using ExpressBase.Common;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

//namespace ExpressBase.Objects.ObjectContainers
namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [BuilderTypeEnum(BuilderType.WebForm)]
    public class EbBotForm : EbForm
    {
        [OnDeserialized]
        public new void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup("Behavior")]
        [HelpText("Set false if want to render controls like a conversation")]
        public bool RenderAsForm { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup("Data")]
        [HelpText("Name Of database-table Which you want to store Data collected using this Form")]
        public string TableName { get; set; }

        public int FormId { get; set; }//equal to 0 for new mode, any other value indicate edit mode

        public override bool IsReadOnly//to identify a bot form is readonly or not
        {
            get
            {
                foreach (EbControl ctrl in this.Controls)
                {
                    if (!ctrl.IsReadOnly)
                        return false;
                }
                return true;
            }
        }

        public EbBotForm() { }

        public static EbOperations Operations = BFOperations.Instance;

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = "<form id='@name@' class='eb-form'>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html.Replace("@name@", this.Name);
        }

        public override string DiscoverRelatedRefids()
        {
            string refids = "";
            foreach (EbControl control in Controls)
            {
                PropertyInfo[] _props = control.GetType().GetProperties();
                foreach (PropertyInfo _prop in _props)
                {
                    if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        refids+=_prop.GetValue(control, null).ToString()+",";
                }
            }
            return "";
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            foreach (EbControl control in Controls)
            {
                PropertyInfo[] _props = control.GetType().GetProperties();
                foreach (PropertyInfo _prop in _props)
                {
                    if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                    {
                        string _val = _prop.GetValue(control, null).ToString();
                        if (RefidMap.ContainsKey(_val))
                            _prop.SetValue(control, RefidMap[_val],null);
                        else
                            _prop.SetValue(control, "failed-to-update-");
                    }

                }
            }
        }
    }
}
