using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
	

	[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
	public class EbBluePrint : EbControlUI, IEbSpecialContainer
	{
		public EbBluePrint(){}




		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public override string ToolIconHtml { get { return "<i class='fa fa-building'></i>"; } set { } }

		public override string ToolNameAlias { get { return "Blueprint"; } set { } }

		public override string ToolHelpText { get { return "Blueprint"; } set { } }

		public override string UIchangeFns
		{
			get
			{
				return @"EbBluePrint = {
                
            }";
			}
		}


		[EnableInBuilder(BuilderType.WebForm)]
		public string TableName { get; set; }

		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript VisibleExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool IsDisable { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public int BlueprintId  { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

		//objects to store BP details

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public string Blueprint_UniqueID { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public string BP_ImageRefid { get; set; }

		//[EnableInBuilder(BuilderType.WebForm)]
		//[HideInPropertyGrid]
		//public string Form_RefID { get; set; }

		//[EnableInBuilder(BuilderType.WebForm)]
		//[HideInPropertyGrid]
		//public string Form_DataID { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public List<AreaMarked> AreaMarkedList { get; set; }


		//objects to store BP Area details



		public override string GetBareHtml()
		{


			return @" 
 <div id='@ebsid@'  > 
        <div class='ebimg-cont' style='width:100%; text-align:center;'>
            <img id='@name@_imgID' class='dpctrl-img' src='@src@'  style='opacity:0.5; max-width:100%; max-height:@maxheight@px;' alt='@alt@'>
        </div>
        <div class='edit_bp_btn-cont' style='width: 100%;position: absolute;bottom: 0;background: transparent; ;'>
                <div id='edit_buleprintbtn' class='edit_bp@ebsid@' style='height: 20px;width: 70px;display: inline-block;text-align: center;border: 1px solid #ccc;border-radius: 20px;font-size: 12px;padding: 1px;background-color: #eee;cursor: pointer;'>
                <i class='fa fa-cog' aria-hidden='true'></i> Config
            </div>
        </div>
</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@src@", "/images/image.png")
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@alt@ ", "Display Picture");
//.Replace("@maxheight@", this.MaxHeight > 0 ? this.MaxHeight.ToString() : "200");

					
		}

		public override string GetDesignHtml()
		{
			return GetHtml().RemoveCR().DoubleQuoted();
		}

		public override string GetHtml()
		{
			string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			   .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			   .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

			return ReplacePropsInHTML(EbCtrlHTML);
		}

		
	}

	public class AreaMarked
	{
		public string BP_AreaMarkedID { get; set; }

		public string AreaMarkedColour { get; set; }
		
		public string Area_DispalyName { get; set; }

		public string Area_Name { get; set; }

		public string Area_ParentFormID { get; set; }

		public string Area_FormID { get; set; }

		public string Area_HTMLtext { get; set; }

	}
}
