using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	public class SaveSvgRequest : EbServiceStackAuthRequest, IReturn<SaveSvgResponse>
	{
		public string txtsvg;

		public string Txtsvg { get; set; }

		public string Jsonsvg { get; set; }

		public object BgSvgFile { get; set; }

		public string BgFileName { get; set; }

		public string ContentType { get; set; }

		public byte[] BgFile { set; get; }

		public string FileDataURL { set; get; }



	}
	public class SaveSvgResponse
	{
		public string Svgdata { get; set; }
	}

	public class RetriveSVGRequest : EbServiceStackAuthRequest, IReturn<RetriveSVGResponse>
	{
		public int Idno { get; set; }
	}
	public class RetriveSVGResponse
	{
		public string SvgPolyData { get; set; }

		public string FileDataURL { set; get; }
	}
}


