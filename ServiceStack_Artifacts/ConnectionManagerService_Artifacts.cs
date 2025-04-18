﻿using ExpressBase.Common;
using ExpressBase.Common.Connections;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Messaging;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class RefreshSolutionConnectionsRequest : EbMqRequest
    {
    }

    public class RefreshSolutionConnectionsResponse : IEbSSResponse
    {
        public bool Status { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RefreshSolutionConnectionsAsyncRequest : EbServiceStackAuthRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    public class RefreshSolutionConnectionsAsyncResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RefreshSolutionConnectionsBySolutionIdAsyncRequest : EbServiceStackAuthRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    //public class RefreshSolutionExtRequest : EbServiceStackNoAuthRequest
    //{
    //    public string SolnId { get; set; }
    //}

    public class RefreshSolutionExtResponse
    {
        public bool Status { get; set; }
    }

    public class GetConnectionsRequest : EbServiceStackAuthRequest, IReturn<GetConnectionsResponse>
    {
        public string SolutionId { get; set; }

        public int ConnectionType { get; set; }
    }

    public class sampletest
    {

    }

    public class GetConnectionsResponse : IEbSSResponse
    {
        public EbConnectionsConfig EBSolutionConnections { get; set; }
        public string Token { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    //public class ChangeDataDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }
    //    public string SolutionId { get; set; }
    //    public EbDataDbConnection DataDBConnection { get; set; }

    //}

    //public class ChangeObjectsDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }
    //    public EbObjectsDbConnection ObjectsDBConnection { get; set; }
    //    public string SolutionId { get; set; }
    //}

    //public class ChangeFilesDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }
    //    public string SolutionId { get; set; }
    //    public EbFilesDbConnection FilesDBConnection { get; set; }
    //}

    //public class ChangeFTPConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }
    //    public string SolutionId { get; set; }
    //    public EbFTPConnection FTPConnection { get; set; }
    //}

    //public class ChangeSMSConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }

    //    public string SolutionId { get; set; }

    //    public ISMSConnection SMSConnection { get; set; }
    //}

    public class InitialSolutionConnectionsRequest : EbServiceStackAuthRequest, IReturn<InitialSolutionConnectionsResponse>
    {
        public string NewSolnId { get; set; }

        [DataMember(Order = 2)]
        public EbDbUsers DbUsers { get; set; }
    }

    public class InitialSolutionConnectionsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ChangeSMTPConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }

        public string SolutionId { get; set; }

        public EbSmtp Email { get; set; }
    }

    //public class ChangeCloudinaryConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    //{
    //    public bool IsNew { get; set; }


    //    public string SolutionId { get; set; }

    //    public EbCloudinaryConnection ImageManipulateConnection { get; set; }
    //}

    public class ChangeConnectionResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class TestConnectionRequest : EbServiceStackAuthRequest
    {
        public EbDbConfig DataDBConfig { get; set; }
    }
    public class TestConnectionResponse
    {
        public bool ConnectionStatus { set; get; }
    }

    //[DataContract]
    //public class TestFileDbconnectionRequest : IReturn<TestFileDbconnectionRequest>, IEbSSRequest
    //{
    //    [DataMember(Order = 1)]
    //    public EbFilesDbConnection FilesDBConnection { get; set; }

    //    public int UserId { get; set; }

    //    [DataMember(Order = 2)]
    //    public string SolnId { get; set; }
    //}

    public class TestFileDbconnectionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool ConnectionStatus { set; get; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    //--------------------------------------------------------------------------Integrations--------------------------------------
    public class AddDBRequest : IReturn<AddDBResponse>, IEbTenentRequest
    {
        public EbDbConfig DbConfig { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }

        //public bool IsNew { get; set; }

    }

    public class AddDBResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }

        public int nid { get; set; }
    }

    public class GetIntegrationConfigsRequest : IReturn<GetIntegrationConfigsResponse>, IEbSSRequest
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class GetIntegrationConfigsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetIntegrationsRequest : IReturn<GetIntegrationsResponse>, IEbSSRequest
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class GetIntegrationsResponse : IEbSSResponse
    {
        public List<EbIntegration> Integrations { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddTwilioRequest : IReturn<AddTwilioResponse>, IEbTenentRequest
    {
        public EbTwilioConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddTwilioResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddUnifonicRequest : IReturn<AddUnifonicResponse>, IEbTenentRequest
    {
        public EbUnifonicConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddUnifonicResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddETRequest : IReturn<AddETResponse>, IEbTenentRequest
    {
        public EbExpertTextingConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddETResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddTLRequest : IReturn<AddTLResponse>, IEbTenentRequest
    {
        public EbTextLocalConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddTLResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddSBRequest : IReturn<AddSBResponse>, IEbTenentRequest
    {
        public EbSmsBuddyConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddSBResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddMongoRequest : IReturn<AddMongoResponse>, IEbTenentRequest
    {
        public EbMongoConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddMongoResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddSmtpRequest : IReturn<AddSmtpResponse>, IEbTenentRequest
    {
        public EbSmtpConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }

    }
    public class AddImapRequest : IReturn<AddSmtpResponse>, IEbTenentRequest
    {
        public EbImapConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }

    }
    public class AddPop3Request : IReturn<AddSmtpResponse>, IEbTenentRequest
    {
        public EbPop3Config Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }

    }

    public class AddSmtpResponse : IEbSSResponse
    {
        public int Id { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddCloudinaryRequest : IReturn<AddCloudinaryResponse>, IEbTenentRequest
    {
        public EbCloudinaryConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddCloudinaryResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddGoogleMapRequest : IReturn<AddGoogleMapResponse>, IEbTenentRequest
    {
        public EbGoogleMapConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddGoogleMapResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class AddOpenStreetMapRequest : IReturn<AddGoogleMapResponse>, IEbTenentRequest
    {
        public OpenStreetMapConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddOpenStreetMapResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddDropBoxRequest : IReturn<AddDropBoxResponse>, IEbTenentRequest
    {
        public EbDropBoxConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddDropBoxResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class MobileConfigRequest : IReturn<MobileConfigResponse>, IEbTenentRequest
    {
        public MobileConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class MobileConfigResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddAWSS3Request : IReturn<AddDropBoxResponse>, IEbTenentRequest
    {
        public EbAWSS3Config Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddAWSS3Response : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddSendGridRequest : IReturn<AddGoogleMapResponse>, IEbTenentRequest
    {
        public EbSendGridConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddSendGridResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddGoogleDriveRequest : IReturn<AddGoogleDriveResponse>, IEbTenentRequest
    {
        public EbGoogleDriveConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddGoogleDriveResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddSlackRequest : IReturn<AddSlackResponse>, IEbTenentRequest
    {
        public EbSlackConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddSlackResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddfacebookRequest : IReturn<AddfacebookResponse>, IEbTenentRequest
    {
        public EbfacebbokConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddfacebookResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbIntegrationRequest : IReturn<EbIntegrationResponse>, IEbTenentRequest
    {
        public EbIntegration IntegrationO { get; set; }

        public bool Deploy { get; set; }

        public bool Drop { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class EbIntegrationResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class CredientialBotRequest : IReturn<EbIntegrationResponse>, IEbTenentRequest
    {
        public int ConfId { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }
    public class CredientialBotResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }

        public string ConnObj { get; set; }
    }


    public class EbIntergationConfDeleteRequest : IReturn<EbIntegrationDeleteResponse>, IEbTenentRequest
    {
        public EbIntegrationConf IntegrationConfdelete { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class EbIntegrationConfDeleteResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbIntergationDeleteRequest : IReturn<EbIntegrationDeleteResponse>, IEbTenentRequest
    {
        public EbIntegration Integrationdelete { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class EbIntegrationDeleteResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbIntergationSwitchRequest : IReturn<EbIntegrationSwitchResponse>, IEbTenentRequest
    {
        public List<EbIntegration> Integrations { set; get; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }
    public class EbIntegrationSwitchResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    //public class _GetConectionsRequest : IReturn<_GetConectionsResponse>, IEbSSRequest
    //{
    //    public int UserId { get; set; }

    //    public string SolnId { get; set; }
    //}

    //public class _GetConectionsResponse : IEbSSResponse
    //{
    //    public ResponseStatus ResponseStatus { get; set; }
    //}

    [DataContract]
    public class GetSolutioInfoRequests : IReturn<GetSolutioInfoResponses>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string IsolutionId { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

    }

    [DataContract]
    public class GetSolutioInfoResponses : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public EbSolutionsWrapper SolutionInfo { get; set; }

        [DataMember(Order = 5)]
        public Dictionary<string, List<EbIntegrationConfData>> IntegrationsConfig { set; get; }

        [DataMember(Order = 6)]
        public Dictionary<string, List<EbIntegrationData>> Integrations { set; get; }

        [DataMember(Order = 7)]
        public bool IsSmsIntegrated { set; get; }

        [DataMember(Order = 8)]
        public bool IsEmailIntegrated { set; get; }


    }
    public class EbIntegrationConfData
    {
        public EbIntegrationConfData(EbDataRow _row)
        {
            Id = Convert.ToInt32(_row["id"]);
            SolutionId = _row["solution_id"].ToString();
            NickName = _row["nickname"].ToString();
            Type = _row["type"].ToString();
            CreatedOn = Convert.ToDateTime(_row["created_at"]).ToString("dddd, dd MMMM yyyy");
        }

        public EbIntegrationConfData() { }

        public int Id { get; set; }

        public string SolutionId { get; set; }

        public string NickName { get; set; }

        public string Type { get; set; }

        public string ConObject { get; set; }

        public string CreatedOn { get; set; }
    }

    public class EbIntegrationData
    {
        public EbIntegrationData(EbDataRow row)
        {
            Id = row["id"].ToString();
            ConfId = row["confid"].ToString();
            NickName = row["nickname"].ToString();
            Ctype = row["ctype"].ToString();
            Type = row["itype"].ToString();
            Preference = row["preference"].ToString();
            CreatedOn = Convert.ToDateTime(row["created_at"]).ToString("dddd, dd MMMM yyyy");
        }
        public string Id { get; set; }

        public string ConfId { get; set; }

        public string NickName { get; set; }

        public string Ctype { get; set; }

        public string Type { get; set; }

        public string Preference { get; set; }

        public string CreatedOn { get; set; }
    }
}
