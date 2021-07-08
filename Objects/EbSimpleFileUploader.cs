using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ExpressBase.Security;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
	public class EbSimpleFileUploader: EbControlUI, IEbExtraQryCtrl
	{

		public EbSimpleFileUploader()
		{

		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public string TableName { get; set; } 

		public override string ToolIconHtml { get { return "<i class='fa fa-cloud-upload'></i>"; } set { } }
		public override string ToolNameAlias { get { return "Simple FileUpload"; } set { } }
		public override string ToolHelpText { get { return "Simple FileUploader"; } set { } }

		public override string UIchangeFns
		{
			get
			{
				return @"EbSimpleFileUploader = {
					minFilesFn : function(elementId, props) {
									if(props.MinFiles>props.MaxFiles){
										props.MinFiles=props.MaxFiles;
									}	
								}
				}";
			}
		}


		public override string GetBareHtml()
		{


			return @"<div class='simpleFUPdiv ' id='@ebsid@' name='@name@'>
						<div id='@ebsid@_SFUP' class='SFUPcontainer' >						
							<div class='SFUPcontent'>
								<i class='fa fa-cloud-upload'></i><span>Drag &amp; Drop files here or click to browse</span>
							 </div>
						</div>
					</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "");

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

		public override string DesignHtml4Bot
		{
			get => this.GetBareHtml();
			set => base.DesignHtml4Bot = value;
		}



		//[JsonIgnore]
		//public override string IsRequiredOKJSfn
		//{
		//	get
		//	{
		//		return @"
		//				let count= $('#' + this.EbSid).attr('fileCount');
  //                      if(this.MinFiles<=count){
  //                          return true;
  //                      }
  //                      else{
  //                          return false;
  //                  }
  //              ";
		//	}
		//	set { }
		//}


		[JsonIgnore]
		public override string DisableJSfn { get { return @"this.__IsDisable = true;
            $(`#cont_${this.EbSid_CtxId}*`).attr('disabled', 'disabled').css('pointer-events', 'none').find('[ui-inp]').css('background-color', 'var(--eb-disablegray)');
            $(`#${this.EbSid_CtxId}`).find('.filethumb').attr('disabled', false).css('pointer-events', 'all');
			 $(`#${this.EbSid_CtxId}`).find('.delete-image').css('display', 'none');"; } set { }
		}

		[JsonIgnore]
		public override string EnableJSfn { get { return @"this.__IsDisable = false; 
			$('#cont_' + this.EbSid_CtxId + ' *').prop('disabled',false).css('pointer-events', 'all');
			$(`#${this.EbSid_CtxId}`).find('.delete-image').css('display', '');"; } set { } }


		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript HiddenExpr { get; set; }
		
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DisableExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		public override bool SelfTrigger { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }
		
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[PropertyGroup(PGConstants.VALIDATIONS)]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[DefaultPropValue("1")]
		[Alias("Maximum Files")]
		public int MaxFiles { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[OnChangeUIFunction("EbSimpleFileUploader.minFilesFn")]
		[DefaultPropValue("0")]
		[HideInPropertyGrid]
		[Alias("Minimum Files")]
		public int MinFiles { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[DefaultPropValue("2")]
		[Alias("Maximum File Size(MB)")]
		public int MaxSize { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		//[DefaultPropValue("image/jpeg,image/png,image/jpg")]
		//[Alias("File Types")]
		//public string FileTypes { get; set; }


		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[PropertyGroup("General")]
		public FileClass FileType { set; get; }

		public string GetSelectQuery(IDatabase DataDB, string MasterTable)
		{
			string idCol = this.TableName == MasterTable ? "id" : MasterTable + "_id";
			if (DataDB.Vendor == DatabaseVendors.MYSQL)
			{
				return $@"SELECT B.id, B.filename, B.tags, B.uploadts,B.filecategory
                    FROM {this.TableName} A, eb_files_ref B
                    WHERE FIND_IN_SET(B.id, A.{this.Name}) AND A.{idCol} = @{MasterTable}_id AND COALESCE(B.eb_del, 'F') = 'F'; ";
			}
			else
			{
				return $@"SELECT B.id, B.filename, B.tags, B.uploadts,B.filecategory
                    FROM {this.TableName} A, eb_files_ref B
                    WHERE B.id = ANY(STRING_TO_ARRAY(A.{this.Name}::TEXT, ',')::INT[]) AND A.{idCol} = @{MasterTable}_id AND COALESCE(B.eb_del, 'F') = 'F'; ";
			}
		}

		public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
		{
			return new SingleColumn()
			{
				Name = this.Name,
				Type = (int)this.EbDbType,
				Value = Value,
				Control = this,
				ObjType = this.ObjType,
				F = "[]"
			};
		}
	}
}
