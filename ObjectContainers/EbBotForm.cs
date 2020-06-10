using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using ServiceStack;
using ServiceStack.Redis;
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
        public EbBotForm()
        {
            this.Notifications = new List<EbFormNotification>();
        }

        [OnDeserialized]
        public new void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup("Behavior")]
        [HelpText("Set false if want to render controls like a conversation")]
        public bool RenderAsForm { get; set; }


        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public bool HaveInputControls
        {
            get
            {
                bool res = false;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    EbControl control = this.Controls[i];
                    if (!control.IsDisable)
                    {
                        res = true;
                        break;
                    }
                }
                return res;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        public string IconPicker { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.DATA)]
        [HelpText("Name Of database-table Which you want to store Data collected using this Form")]
        public override string TableName { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Web Form")]
        public string WebFormRefId { set; get; }

        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbFormNotification> Notifications { get; set; }

        public override bool IsReadOnly//to identify a bot form is readonly or not
        {
            get
            {
                foreach (EbControl ctrl in this.Controls)
                {
                    if (!ctrl.IsDisable)
                        return false;
                }
                return true;
            }
        }
        
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
            {
                //if (c.ObjType == "TextBox")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "Date")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "CheckBoxGroup")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "Numeric")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "BooleanSelect")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "FileUploader")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "PowerSelect")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "SimpleSelect")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "RadioGroup")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "DynamicCardSet")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else if (c.ObjType == "StaticCardSet")
                //{
                //	html += c.GetHtml4Bot();
                //}
                //else
                //{
                //	html += c.GetHtml4Bot();
                //	//html += c.GetHtml();
                //}
                html += c.GetHtml4Bot();
            }

            return html.Replace("@name@", this.Name);
        }

        public override void BeforeSave(IServiceClient serviceClient, IRedisClient redis)
        {
            BeforeSaveHelper.BeforeSave_BotForm(this, serviceClient, redis);
        }

        public override List<string> DiscoverRelatedRefids()
        {
            return EbFormHelper.DiscoverRelatedRefids(this);
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            EbFormHelper.ReplaceRefid(this, RefidMap);
        }
    }
}
