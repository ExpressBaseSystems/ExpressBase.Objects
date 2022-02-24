using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using ExpressBase.Common.ProductionDBManager;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class CheckChangesInFilesRequest : IEbTenentRequest, IReturn<CheckChangesInFilesResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }
    }

    public class CheckChangesInFilesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<Eb_FileDetails> Changes { get; set; }

        [DataMember(Order = 3)]
        public string ModifiedDate { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }
    }

    public class GetSolutionForIntegrityCheckRequest : IEbTenentRequest, IReturn<GetSolutionForIntegrityCheckResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class GetSolutionForIntegrityCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<Eb_Changes_Log> ChangesLog { get; set; }
    }

    public class UpdateDBFileByDBRequest : IEbTenentRequest, IReturn<UpdateDBFilesByDBResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }
    }

    public class UpdateDBFilesByDBResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<Eb_FileDetails> Changes { get; set; }

        [DataMember(Order = 3)]
        public string ModifiedDate { get; set; }
    }


    public class UpdateInfraWithSqlScriptsRequest : IEbTenentRequest, IReturn<UpdateInfraWithSqlScriptsResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }
    }

    public class UpdateInfraWithSqlScriptsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 3)]
        public bool isOwner { get; set; }
    }

    public class GetFunctionOrProcedureQueriesRequest : IEbTenentRequest, IReturn<GetFunctionOrProcedureQueriesResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }

        public Eb_FileDetails ChangeList { get; set; }
    }

    public class GetFunctionOrProcedureQueriesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string Query { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class GetTableQueriesRequest : IEbTenentRequest, IReturn<GetTableQueriesResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }

        public Eb_FileDetails ChangeList { get; set; }
    }

    public class GetTableQueriesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string Query { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ExecuteQueriesRequest : IEbTenentRequest, IReturn<ExecuteQueriesResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }

        public string Query { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }
    }

    public class ExecuteQueriesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 3)]
        public List<Eb_FileDetails> Changes { get; set; }

    }

    public class GetScriptsForDiffViewRequest : IEbTenentRequest, IReturn<GetScriptsForDiffViewResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

        public string SolutionId { get; set; }

        public string FileHeader { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }
    }

    public class GetScriptsForDiffViewResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        public string InfraFileContent { get; set; }

        public string TenantFileContent { get; set; }

        public List<string> Result { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class LastSolnAccessRequest : IEbTenentRequest, IReturn<LastSolnAccessResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }

    }

    public class LastSolnAccessResponse : IEbSSResponse
    {
        public byte[] FileBytea { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class LastDbAccess
    {
        public string ISolution { get; set; }

        public string ESolution { get; set; }

        public string ObjCount { get; set; }

        public string LastObject { get; set; }

        public string UsrCount { get; set; }

        public string LastUser { get; set; }

        public string LastLogin { get; set; }

        public string AppCount { get; set; }

        public string Applications { get; set; }

        public string DbName { get; set; }

        public string DbVendor { get; set; }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

        public string ReadWriteUserName { get; set; }

        public string ReadWritePassword { get; set; }

        public string ReadOnlyUserName { get; set; }

        public string ReadOnlyPassword { get; set; }
    }
}
