using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects.ReportRelated;
using RestSharp;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using System.Collections.Generic;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class EbBaseService : Service
    {
        protected EbConnectionFactory EbConnectionFactory { get; private set; }

        protected RestClient RestClient  { get; private set; }
        
        protected RabbitMqProducer MessageProducer3 { get; private set; }

        protected RabbitMqQueueClient MessageQueueClient { get; private set; }

        private EbConnectionFactory _infraConnectionFactory = null;

        protected EbConnectionFactory InfraConnectionFactory
        {
            get
            {
                if(_infraConnectionFactory == null)
                       _infraConnectionFactory = new EbConnectionFactory(CoreConstants.EXPRESSBASE, this.Redis);

                return _infraConnectionFactory; 
            }
        }

        //protected RedisServerEvents ServerEvents { get; private set; }

        public EbBaseService() { }

        public EbBaseService(IEbConnectionFactory _dbf)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
        }

        public EbBaseService(IMessageProducer _mqp)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbBaseService(RestSharp.IRestClient _rest)
        {
            this.RestClient = _rest as RestClient;
        }

        public EbBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        public EbBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc, IServerEvents _se)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            //this.ServerEvents = _se as RedisServerEvents;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        private static Dictionary<string, string> _infraDbSqlQueries;

        public static Dictionary<string, string> InfraDbSqlQueries
        {
            get
            {
                if (_infraDbSqlQueries == null)
                {
                    _infraDbSqlQueries = new Dictionary<string, string>();
                    _infraDbSqlQueries.Add("KEY1", "SELECT id, accountname, profilelogo FROM eb_tenantaccount WHERE tenantid=@tenantid");
                }

                return _infraDbSqlQueries;
            }
        }

        public ILog Log { get { return LogManager.GetLogger(GetType()); } }

        public byte[] GetFile(string solutionId, IEbFileService myFileService, EbImg field)
        {
            byte[] fileByte = myFileService.Post
                 (new DownloadFileRequest
                 {
                     TenantAccountId = solutionId,
                     FileDetails = new FileMeta
                     {
                         FileName = field.Image + ".jpg",
                         FileType = "jpg"
                     }
                 });

            return fileByte;
        }


        //private void LoadCache()
        //{
        //    using (var redisClient = this.Redis)
        //    {
        //        if (!string.IsNullOrEmpty(this.ClientID))
        //        {
        //            EbTableCollection tcol = redisClient.Get<EbTableCollection>(string.Format("EbTableCollection_{0}",this.ClientID));
        //            EbTableColumnCollection ccol = redisClient.Get<EbTableColumnCollection>(string.Format("EbTableColumnCollection_{0}", this.ClientID));
        //            if (tcol == null || ccol == null)
        //            {
        //                tcol = new EbTableCollection();
        //                ccol = new EbTableColumnCollection();
        //                string sql = "SELECT id,tablename FROM eb_tables;" + "SELECT id,columnname,columntype,table_id FROM eb_tablecolumns;";
        //                var dt1 = this.DatabaseFactory.ObjectsDB.DoQueries(sql);
        //                foreach (EbDataRow dr in dt1.Tables[0].Rows)
        //                {
        //                    EbTable ebt = new EbTable
        //                    {
        //                        Id = Convert.ToInt32(dr[0]),
        //                        Name = dr[1].ToString()
        //                    };

        //                    tcol.Add(ebt.Id, ebt);
        //                }

        //                foreach (EbDataRow dr1 in dt1.Tables[1].Rows)
        //                {
        //                    EbTableColumn ebtc = new EbTableColumn
        //                    {
        //                        Type = (DbType)(dr1[2]),
        //                        Id = Convert.ToInt32(dr1[0]),
        //                        Name = dr1[1].ToString(),
        //                        TableId = Convert.ToInt32(dr1[3])
        //                    };
        //                    ccol.Add(ebtc.Name, ebtc);

        //                }

        //                redisClient.Set<EbTableCollection>(string.Format("EbTableCollection_{0}", this.ClientID), tcol);
        //                redisClient.Set<EbTableColumnCollection>(string.Format("EbTableColumnCollection_{0}", this.ClientID), ccol);
        //            }
        //        }
        //        else
        //        {
        //            EbTableCollection tcol = redisClient.Get<EbTableCollection>("EbInfraTableCollection");
        //            EbTableColumnCollection ccol = redisClient.Get<EbTableColumnCollection>("EbInfraTableColumnCollection");

        //            if (tcol == null || ccol == null)
        //            {
        //                tcol = new EbTableCollection();
        //                ccol = new EbTableColumnCollection();

        //                string sql = "SELECT id,tablename FROM eb_tables;" + "SELECT id,columnname,columntype FROM eb_tablecolumns;";
        //                var dt1 = this.DatabaseFactory.ObjectsDB.DoQueries(sql);

        //                foreach (EbDataRow dr in dt1.Tables[0].Rows)
        //                {
        //                    EbTable ebt = new EbTable
        //                    {
        //                        Id = Convert.ToInt32(dr[0]),
        //                        Name = dr[1].ToString()
        //                    };

        //                    tcol.Add(ebt.Id, ebt);
        //                }

        //                foreach (EbDataRow dr1 in dt1.Tables[1].Rows)
        //                {
        //                    EbTableColumn ebtc = new EbTableColumn
        //                    {
        //                        Type = (DbType)(dr1[2]),
        //                        Id = Convert.ToInt32(dr1[0]),
        //                        Name = dr1[1].ToString(),
        //                    };
        //                   ccol.Add(ebtc.Name, ebtc);

        //                }

        //                redisClient.Set<EbTableCollection>("EbInfraTableCollection", tcol);
        //                redisClient.Set<EbTableColumnCollection>("EbInfraTableColumnCollection", ccol);
        //            }
        //        }
        //    }
        //}
    }
}
