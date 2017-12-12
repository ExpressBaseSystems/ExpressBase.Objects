using ExpressBase.Common;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/chinchu")]
        [DataContract]

        //.................................................................
        public class CheckTblRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            [DataMember(Order = 1)]
            public string tblName { get; set; }
        }
        [DataContract]
        public class CheckTblResponse : IEbSSResponse
        {
            [DataMember(Order = 1)]
            public EbDataRow msg { get; set; }

            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }
        //........................................................................
        public class CreateTblRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            [DataMember(Order = 1)]
            public string tblName { get; set; }
            public CustomizeColParent headerList { get; set; }
        }
        public class CreateTblResponse : IEbSSResponse
        {
            //[DataMember(Order = 1)]
            //public EbDataRow msg { get; set; }

            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }
        //.............................................................
        public class InsertIntoTblResponseRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            [DataMember(Order = 1)]
            public string tblName { get; set; }
            public string dtTbl { get; set; }
            public Dictionary<string, string> dataType { get; set; }
        }
        public class InsertIntoTblResponse : IEbSSResponse
        {
            //[DataMember(Order = 1)]
            //public EbDataRow msg { get; set; }

            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }
        //................................................................
        public class ExcelCreateTableRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            [DataMember(Order = 1)]
            public string DataTbl { get; set; }
            public string tbl { get; set; }
        }
        [DataContract]
        public class ExcelCreateTableResponse : IEbSSResponse
        {
            [DataMember(Order = 1)]
            public string returnmsg { get; set; }

            //[DataMember(Order = 2)]
            //public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }
        public class DtObject
        {
            public DataTable dt { get; set; }
        }


        //..................................................................

        public class DBTableRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            //[DataMember(Order = 1)]
            //public string DataTbl { get; set; }
        }
        [DataContract]
        public class DBTableResponse : IEbSSResponse
        {
            [DataMember(Order = 1)]
            public List<string> list1 { get; set; }

            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }


        //.................................................................................

        public class DBColumnRequest : EbServiceStackRequest, IReturn<ExcelCreateTableResponse>
        {
            [DataMember(Order = 1)]
            public string tblName { get; set; }
        }
        [DataContract]
        public class DBColumnResponse : IEbSSResponse
        {
            [DataMember(Order = 1)]
            public EbDataTable tbl { get; set; }

            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }

        public class CustomizeColParent
        {
            public List<CustomizeCol> abc { get; set; }
        }
        public class CustomizeCol
        {
            public string colName { get; set; }
            public string dataType { get; set; }
        }
    }

