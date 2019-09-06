using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using ExpressBase.Common.ProductionDBManager;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class CheckChangesInFunctionRequest : IEbTenentRequest, IReturn<CheckChangesInFunctionResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }
    }

    public class CheckChangesInFunctionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, List<Eb_FileChanges>> Changes { get; set; }
    }

    public class DBIntegrityCheckRequest : IEbTenentRequest, IReturn<DBIntegrityCheckResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class DBIntegrityCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<Eb_Changes_Log> ChangesLog { get; set; }
    }

    public class UpdateDBFunctionByDBRequest : IEbTenentRequest, IReturn<UpdateDBFunctionByDBResponse>
    {
        public string DBName { get; set; }

        public List<Eb_FileChanges> Changes { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class UpdateDBFunctionByDBResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, List<Eb_FileChanges>> Changes { get; set; }

        [DataMember(Order = 3)]
        public DateTime ModifiedDate { get; set; }
    }

}
