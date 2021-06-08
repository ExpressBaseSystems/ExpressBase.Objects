using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]

	public class EbQuestionnaireConfigurator : EbControlUI, IEbExtraQryCtrl
	{
		public EbQuestionnaireConfigurator()
		{

			this.QuestionBankList = new Dictionary<int, EbQuestion>();
			this.QuestionBankCtlHtmlList=new Dictionary<int, string> ();
	}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string ToolIconHtml { get { return "<i class='fa fa-question-circle  '></i>"; } set { } }

		public override string ToolNameAlias { get { return "Questionnaire Configurator"; } set { } }

		public override string ToolHelpText { get { return "Questionnaire Configurator"; } set { } }
		public override string UIchangeFns
		{
			get
			{
				return @"EbQuestionnaireConfigurator = {
                
            }";
			}
		}
		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[DefaultPropValue("100")]
		[PropertyGroup(PGConstants.APPEARANCE)]
		[Alias("DropdownMaxHeight")]
		public int DropdownHeight { get; set; }


		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public Dictionary<int, EbQuestion> QuestionBankList { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public Dictionary<int, string> QuestionBankCtlHtmlList { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		//[JsonIgnore]
		public string OptionHtml { get; set; }
		public override string GetBareHtml()
		{
			return @"<div class='input-group @ebsid@_cont'>
				<select id='@ebsid@' ui-inp class='' multiple @selOpts@ @MaxLimit@  @IsSearchable@ name='@ebsid@'   style='width: 100%;'>
					@options@
				</select>
<div>
<button id='@ebsid@_queBtn' class='btn ebbtn eb_btnblue'  >Add Questions</button>
</div>

<div id='@ebsid@_queRender'>
</div>
			</div>"
			.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
			.Replace("@name@", this.Name)
			.Replace("@HelpText@", this.HelpText)
			.Replace("@toolTipText@", this.ToolTipText)
			.Replace("@options@", this.OptionHtml);

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


		public void InitFromDataBase(JsonServiceClient ServiceClient)
		{
			////query is directly written- table, column namesa are given explicitly in query (only for postgre)
			var result = ServiceClient.Get<GetQuestionsBankResponse>(new GetQuestionsBankRequest { });
			if (result.Questionlst.Count > 0)
			{
				this.OptionHtml = string.Empty;
				foreach (var dct in result.Questionlst)
				{
					EbQuestion qstn = EbSerializers.Json_Deserialize<EbQuestion>(dct.Value);
					var ctlHtml=qstn.GetHtml();
					this.QuestionBankList.Add(dct.Key, qstn);
					this.QuestionBankCtlHtmlList.Add(dct.Key, ctlHtml);
					this.OptionHtml += $"<option  value='{dct.Key}'>{qstn.Name}</option>";
				}
			}



		}

		public string TableName { get; set; }

		public string GetSelectQuery(IDatabase DataDB, string mTbl)
		{
			string Qry = string.Format("SELECT id, ques_id, ext_props FROM eb_ques_config " +
				"WHERE form_refid = @{0}_refid AND form_data_id = @{0}_id AND COALESCE(eb_del, 'F') = 'F';", mTbl);

			return Qry;
		}

		private string GetSaveQuery(ParameterizeCtrl_Params args, Ques_Confi Que, int ctrli, int op)
        {
			string Qry = string.Empty;
			if (op == 1)//insert
            {
				Qry += string.Format("INSERT INTO eb_ques_config (ques_id, ext_props, form_refid, form_data_id, control_id, eb_created_by, eb_created_at) " +
					"VALUES (@ques_id_{0}, @ext_props_{0}, @{1}_refid, {2}, @ctrlid_{3}, @eb_createdby, {4});", 
					args.i,
					args.tbl,
					args.ins ? $"eb_currval('{args.tbl}_id_seq')" : $"@{args.tbl}_id",
					ctrli,
					args.DataDB.EB_CURRENT_TIMESTAMP);
				args.param.Add(args.DataDB.GetNewParameter("ques_id_" + args.i, EbDbTypes.Int32, Que.ques_id));
				args.param.Add(args.DataDB.GetNewParameter("ext_props_" + args.i, EbDbTypes.String, JsonConvert.SerializeObject(Que.ext_props)));				
			}
			else if (op == 2)//update
            {
				Qry += string.Format("UPDATE eb_ques_config SET ques_id = @ques_id_{0}, ext_props = @ext_props_{0}, control_id = @ctrlid_{1}, " +
					"eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {2} WHERE id = @id_{0};", 
					args.i,
					ctrli,
					args.DataDB.EB_CURRENT_TIMESTAMP);
				args.param.Add(args.DataDB.GetNewParameter("id_" + args.i, EbDbTypes.Int32, Que.id));
				args.param.Add(args.DataDB.GetNewParameter("ques_id_" + args.i, EbDbTypes.Int32, Que.ques_id));
				args.param.Add(args.DataDB.GetNewParameter("ext_props_" + args.i, EbDbTypes.String, JsonConvert.SerializeObject(Que.ext_props)));
			}
			else if (op == 3)//delete
            {
				Qry += string.Format("UPDATE eb_ques_config SET eb_del = 'T', eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {1} WHERE id = @id_{0};", 
					args.i,
					args.DataDB.EB_CURRENT_TIMESTAMP);
				args.param.Add(args.DataDB.GetNewParameter("id_" + args.i, EbDbTypes.Int32, Que.id));
			}
			args.i++;
			return Qry;
        } 

		public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
		{
			string Qry = string.Empty;
			List<Ques_Confi> Ques = JsonConvert.DeserializeObject<List<Ques_Confi>>(Convert.ToString(args.cField.Value));
			int Ctrli = args.i++;
			args.param.Add(args.DataDB.GetNewParameter("ctrlid_" + Ctrli, EbDbTypes.String, this.Name));

			if (args.ins)
            {
				foreach (Ques_Confi Que in Ques)
					Qry += this.GetSaveQuery(args, Que, Ctrli, 1);//insert
			}
            else
            {
				List<Ques_Confi> QuesOld = JsonConvert.DeserializeObject<List<Ques_Confi>>(Convert.ToString(args.ocF.Value));
				foreach (Ques_Confi Que in Ques)
                {
					if (Que.id > 0)
                    {
						if (QuesOld.RemoveAll(e => e.id == Que.id) > 0)
							Qry += this.GetSaveQuery(args, Que, Ctrli, 2);//update
						else
							Qry += this.GetSaveQuery(args, Que, Ctrli, 1);//insert
					}
                    else
						Qry += this.GetSaveQuery(args, Que, Ctrli, 1);//insert
				}
				foreach (Ques_Confi QueOld in QuesOld)
					Qry += this.GetSaveQuery(args, QueOld, Ctrli, 3);//delete
			}
			args._extqry += Qry;
			return true;
		}

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
			return new SingleColumn()
			{
				Name = this.Name,
				Type = (int)this.EbDbType,
				Value = "[]",
				Control = this,
				ObjType = this.ObjType,
				F = string.Empty
			};
		}

		[JsonIgnore]
		public override string EnableJSfn { get { return @"this.__IsDisable = false;debugger; $(`.queOuterDiv`).find('*').attr('disabled', 'disabled').css('background-color', 'var(--eb-disablegray)');"; } set { } }

	}

	public class Ques_Confi
	{
		public int id { get; set; }
		public int ques_id { get; set; }
		public Ques_ext_props ext_props { get; set; }
	}

	public abstract class Ques_ext_propsAbstract { }

	[UsedWithTopObjectParent(typeof(EbObject))]
	[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
	public class Ques_ext_props:Ques_ext_propsAbstract
	{

		public Ques_ext_props() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public int ques_id { get; set; } 

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[DefaultPropValue("false")]
		[Alias("Required")]
		public bool required { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[DefaultPropValue("false")]
		[Alias("Unique")]
		public bool unique { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		//[Alias("Validator")]
		//public List<EbValidator> validator { get; set; }
	}
}
