using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder()]
    public class EbLabels : EbControlUI
	{

        public EbLabels()
        {

            this.LabelCollection = new List<EbLabel>();
        }

        [HideInPropertyGrid]
        public override string Label { get => base.Label; set => base.Label = value; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => true; }

		[OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbLabel> LabelCollection { get; set; }

        public override string DesignHtml4Bot
        {
			get => @"
    <div class='msg-cont'>
      <div class='bot-icon'></div>
      <div class='msg-cont-bot'>
         <div class='msg-wraper-bot'>
            @Label@
            <div class='msg-time'>4:21pm</div>
         </div>
      </div>
   </div>".Replace("@Label@", this.Label);
			set => base.DesignHtml4Bot = value;
		}

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
	let label=new EbObjects.EbLabel('lbl_'+Date.now().toString(36));
	label.Label = 'First label';
    this.LabelCollection.$values.push(label);
};";
			
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-question'></i>"; } set { } }


        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-question'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {
            return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }

        public override string GetBareHtml()
        {
            string html = "<div id='@name@' name='@name@' type='Labels'>";
            foreach (EbLabel ec in this.LabelCollection)
            {
                html += ec.GetHtml();
            }
            html += "</div>";
            return html
.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='Labels' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
       @barehtml@
    </div>
"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name);
            return EbCtrlHTML;
        }
    }
}
