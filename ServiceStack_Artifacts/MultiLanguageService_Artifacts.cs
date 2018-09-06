using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
 //   public class MLKeyRequest : EbServiceStackRequest, IReturn<MLKeyResponse>
 //   {
 //       public string Key { get; set; }
 //   }

 //   public class MLKeyResponse : IEbSSResponse
 //   {
 //       [DataMember(Order = 1)]
 //       public Dictionary<int, string> Data { get; set; }

 //       [DataMember(Order = 2)]
 //       public string Token { get; set; }

 //       [DataMember(Order = 3)]
 //       public ResponseStatus ResponseStatus { get; set; }
 //   }

 //   public class MLLangValueRequest : EbServiceStackRequest, IReturn<MLLangValueResponse>
 //   {
 //       public int Keyid { get; set; }
 //   }

 //   public class MLLangValueResponse : IEbSSResponse
 //   {
 //       [DataMember(Order = 1)]
 //       public List<MLRowEntry> Data { get; set; }

 //       [DataMember(Order = 2)]
 //       public string Token { get; set; }

 //       [DataMember(Order = 3)]
 //       public ResponseStatus ResponseStatus { get; set; }
 //   }

	

	//public class MLSetKeyRequest : EbServiceStackRequest, IReturn<MLSetKeyResponse>
	//{
	//	public string key { get; set; }
	//	public string value { get; set; }
	//	public int langid { get; set; }
	//}

	//public class MLSetKeyResponse : IEbSSResponse
	//{
	//	[DataMember(Order = 1)]
	//	public int Data { get; set; }

	//	[DataMember(Order = 2)]
	//	public string Token { get; set; }

	//	[DataMember(Order =3)]
	//	public ResponseStatus ResponseStatus { get; set; }
	//}

	//[DataContract]
 //   public class MLRowEntry
 //   {
 //       [DataMember(Order = 1)]
 //       public int id;

 //       [DataMember(Order = 2)]
 //       public string lang;

 //       [DataMember(Order = 3)]
 //       public string value;

 //       public MLRowEntry(int i, string l, string v)
 //       {
 //           id = i;
 //           lang = l;
 //           value = v;
 //       }
 //   }

	//public class MLRequest : EbServiceStackRequest, IReturn<MLResponse>
	//{
	//	public int D_id { get; set; }
	//	public List<MLRecord> D_member { get; set; }
	//}

	//public class MLResponse : IEbSSResponse
	//{
	//	[DataMember(Order = 1)]
	//	public int D_id { get; set; }

	//	[DataMember(Order = 2)]
	//	public List<MLRecord> D_member { get; set; }

	//	[DataMember(Order = 3)]
	//	public string Token { get; set; }

	//	[DataMember(Order = 4)]
	//	public ResponseStatus ResponseStatus { get; set; }
	//}



	//[DataContract]
	//public class MLRecord
	//{
	//	[DataMember(Order = 1)]
	//	public List<string> dmembers;

	//	public MLRecord(List<string> d1)
	//	{
	//		dmembers = d1;
	//	}
	//}

		
	public class MLLoadLangRequest : EbServiceStackAuthRequest, IReturn<MLLoadLangResponse>
	{
		public int Lang { get; set; }
	}
	public class MLLoadLangResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<string, int> Data { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}


	public class MLGetSearchResultRqst: EbServiceStackAuthRequest, IReturn<MLGetSearchResultRspns>
	{
		public string Key_String { get; set; }
		public int Offset { get; set; }
		public int Limit { get; set; }
	}
	public class MLGetSearchResultRspns : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<int,List<MLSearchResult>> D_member { get; set; }

		[DataMember(Order = 2)]
		public int Count { get; set; }

		[DataMember(Order = 3)]
		public string Token { get; set; }

		[DataMember(Order = 4)]
		public ResponseStatus ResponseStatus { get; set; }
	}


	public class MLGetStoredKeyValueRequest : EbServiceStackAuthRequest, IReturn<MLGetStoredKeyValueResponse>
	{
		public string Key { get; set; }
	}
	public class MLGetStoredKeyValueResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<int, MLKeyValue> Data { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class MLUpdateKeyValueRequest : EbServiceStackAuthRequest, IReturn<MLUpdateKeyValueResponse>
	{
		public List<MLKeyValue> Data { get; set; }
	}
	public class MLUpdateKeyValueResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Data { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class MLAddKeyRequest : EbServiceStackAuthRequest, IReturn<MLAddKeyResponse>
	{
		public string Key { get; set; }
		public List<MLAddKey> Data { get; set; }
	}
	public class MLAddKeyResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int KeyId { get; set; }

		[DataMember(Order = 1)]
		public int RowAffected { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}


	[DataContract]
	public class MLAddKey
	{
		[DataMember(Order = 1)]
		public string Lang_Id;

		[DataMember(Order = 2)]
		public string Key_Value;
	}

	[DataContract]
	public class MLKeyValue
	{
		[DataMember(Order = 1)]
		public string KeyVal_Id;

		[DataMember(Order = 2)]
		public string Key;

		[DataMember(Order = 3)]
		public string Key_Id;

		[DataMember(Order = 4)]
		public string Lang_Id;

		[DataMember(Order = 5)]
		public string KeyVal_Value;
	}


	[DataContract]
	public class MLSearchResult
	{
		[DataMember(Order = 1)]
		public long KeyId;

		[DataMember(Order = 2)]
		public string Key;
		
		[DataMember(Order = 3)]
		public int LangId;

		[DataMember(Order = 4)]
		public string Language;

		[DataMember(Order = 5)]
		public long KeyValId;

		[DataMember(Order = 6)]
		public string KeyValue;
	}
	
}
