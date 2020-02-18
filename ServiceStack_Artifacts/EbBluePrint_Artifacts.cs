using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	//class EbBluePrint_Artifacts
	//{
	//}

	public class SaveBluePrintRequest : EbServiceStackAuthRequest, IReturn<SaveBluePrintResponse>
	{
		
		public string Txtsvg { get; set; }

		public string MetaBluePrint { get; set; }

		public object BgSvgFile { get; set; }

		public string BgFileName { get; set; }

		public string ContentType { get; set; }

		public int BluePrintID { get; set; }

		public byte[] BgFile { set; get; }

		public string FileDataURL { set; get; }

		public Dictionary<string, string> BP_FormData { set; get; } = new Dictionary<string, string>();

	}
	public class SaveBluePrintResponse
	{
		public string Svgdata { get; set; }

		public int Bprntid { get; set; }
	}

	public class RetriveBluePrintRequest : EbServiceStackAuthRequest, IReturn<RetriveBluePrintResponse>
	{
		public int Idno { get; set; }
	}
	public class RetriveBluePrintResponse
	{
		public string SvgPolyData { get; set; }

		public string FileDataURL { set; get; }

		public string BpMeta { set; get; }
	}

	public class UpdateBluePrint_DevRequest : EbServiceStackAuthRequest, IReturn<UpdateBluePrint_DevResponse>
	{
		public string Txtsvg { get; set; }

		public string MetaBluePrint { get; set; }

		public object BgSvgFile { get; set; }

		public string BgFileName { get; set; }

		public string ContentType { get; set; }

		public int BluePrintID { get; set; }

		public byte[] BgFile { set; get; }

		public string FileDataURL { set; get; }

		public Dictionary<string, string> BP_FormData_Dict { set; get; } = new Dictionary<string, string>();
	}
	public class UpdateBluePrint_DevResponse
	{
		public string Svgdata { get; set; }

		public int Bprntid { get; set; }
	}
}
