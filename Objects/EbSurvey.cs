using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbSurvey : EbControl
	{
		[EnableInBuilder(BuilderType.BotForm)]
		public override string Name { get; set; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public int SurveyId { get; set; }

		public List<EbSurveyQueries> QueryList { get; set; }

		public EbSurvey()
		{
			QueryList = new List<EbSurveyQueries>();
		}

		public void InitFromDataBase(JsonServiceClient ServiceClient)
		{
			GetParticularSurveyResponse res = ServiceClient.Post<GetParticularSurveyResponse>(new GetParticularSurveyRequest { SurveyId = this.SurveyId });
			foreach(KeyValuePair<int, Eb_SurveyQuestion> item in res.Queries)
			{
				List<EbSurveyQueriesOptions> _templist = new List<EbSurveyQueriesOptions>();
				foreach (string ch in item.Value.Choices)
				{
					_templist.Add(new EbSurveyQueriesOptions { Option = ch});
				}
				this.QueryList.Add(new EbSurveyQueries {
					QueryId = item.Value.Id,
					Query = item.Value.Question,
					OptionTypeId = item.Value.ChoiceType,
					OptionsList = _templist
				});				
			}			
		}

		public override string DesignHtml4Bot
		{
			get => @"<div id=@id class='Eb-ctrlContainer'>						
						<div style='height: 60px; padding: 20px; font-size: 20px; text-align: center;'>Survey Control</div>
						<div style='text-align: right; height: 30px;'><button class='btnselectsurvey' style='height: 30px; background-color: white; border: 1px solid #ccc;'>Select Survey</button></div>						
					</div>"; set => base.DesignHtml4Bot = value;
		}

		public override string GetBareHtml()
		{
			return @"<div id='@name@' class='Eb-ctrlContainer'>qqqqqqqq</div>";
		}
	}
	
	
	[HideInToolBox]
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbSurveyQueries : EbControl
	{
		public int QueryId { get; set; }

		public string Query{ get; set; }

		public List<EbSurveyQueriesOptions> OptionsList { get; set; }
		
		public int OptionTypeId { get; set; }

		public EbSurveyQueries()
		{
			OptionsList = new List<EbSurveyQueriesOptions>();
		}
	}

	[HideInToolBox]
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbSurveyQueriesOptions : EbControl
	{
		public int OptionId { get; set; }

		public string Option { get; set; }
		
		public EbSurveyQueriesOptions() { }
	}
}
