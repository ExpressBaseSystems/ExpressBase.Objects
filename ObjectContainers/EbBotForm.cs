using ExpressBase.Common;
using ExpressBase.Common.Constants;
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
    [BuilderTypeEnum(BuilderType.BotForm)]
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
        [PropertyGroup(PGConstants.DATA)]
        [HelpText("Name Of database-table Which you want to store Data collected using this Form")]
        public override string TableName { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Web Form")]
        public string WebFormRefId { set; get; }
        
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

        public override string GetHtml4Bot()
        {
            string html = string.Empty;

			foreach (EbControl c in this.Controls)
				if (c.ObjType == "TextBox")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "Date")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "CheckBoxGroup")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "Numeric")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "BooleanSelect")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "FileUploader")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "PowerSelect")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "SimpleSelect")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "RadioGroup")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "DynamicCardSet")
				{
					html += c.GetHtml4Bot();
				}
				else if (c.ObjType == "StaticCardSet")
				{
					html += c.GetHtml4Bot();
				}
				else
				{
					html += c.GetHtml4Bot();
					//html += c.GetHtml();
				}

            return html.Replace("@name@", this.Name);
        }

		
		public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();
            foreach (EbControl control in Controls)
            {
                PropertyInfo[] _props = control.GetType().GetProperties();
                foreach (PropertyInfo _prop in _props)
                {
                    if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        refids.Add(_prop.GetValue(control, null).ToString());
                }
            }
            return refids;
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
