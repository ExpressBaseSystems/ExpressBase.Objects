using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
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
                    if (!(control.IsReadOnly || control.IsDisable))
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
            Dictionary<string, string> tbls = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this.TableName))
                throw new FormException("Please enter a valid form table name");
            tbls.Add(this.TableName, "form table");

            foreach (EbControl _control in this.Controls)
            {
                if (_control is EbDynamicCardSet)
                {
                    EbDynamicCardSet ctrl = _control as EbDynamicCardSet;
                    if (string.IsNullOrEmpty(ctrl.DataSourceId))
                        throw new FormException("Set Data Reader for Dynamic Card - " + ctrl.Label ?? ctrl.Name);
                    if (ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for Dynamic Card - " + ctrl.Label ?? ctrl.Name);
                    if (ctrl.CardFields?.Count == 0)
                        throw new FormException("Set Card Fields for Dynamic Card - " + ctrl.Label ?? ctrl.Name);
                    if (string.IsNullOrEmpty(ctrl.TableName))
                        throw new FormException("Please enter a valid Dynamic Card table name");
                }
                else if (_control is EbStaticCardSet)
                {
                    EbStaticCardSet ctrl = _control as EbStaticCardSet;
                    if (ctrl.CardFields?.Count == 0)
                        throw new FormException("Set Card Fields for Static Card - " + ctrl.Label ?? ctrl.Name);
                    if (string.IsNullOrEmpty(ctrl.TableName))
                        throw new FormException("Please enter a valid Static Card table name");
                }
                else if (_control is EbPowerSelect)
                {
                    EbPowerSelect ctrl = _control as EbPowerSelect;
                    if (string.IsNullOrEmpty(ctrl.DataSourceId))
                        throw new FormException("Set Data Reader for " + ctrl.Label);
                    if (ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + ctrl.Label);
                    if (ctrl.RenderAsSimpleSelect && ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + ctrl.Label);
                    if (!ctrl.RenderAsSimpleSelect && (ctrl.DisplayMembers == null || ctrl.DisplayMembers.Count == 0))
                        throw new FormException("Set Display Members for " + ctrl.Label);
                }
            }
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
