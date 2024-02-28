using ExpressBase.Common;
using ExpressBase.Common.Connections;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Security;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.SqlProfiler;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using RestSharp;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using ServiceStack.Redis;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class EbBaseService : Service
    {
        private EbConnectionFactory __EbConnectionFactory;
        public EbConnectionFactory EbConnectionFactory
        {
            get { return __EbConnectionFactory; }
            set
            {
                if (value != null)
                    __EbConnectionFactory = value;
            }
        }

        protected RestClient RestClient { get; private set; }

        protected RabbitMqProducer MessageProducer3 { get; private set; }

        protected RabbitMqQueueClient MessageQueueClient { get; private set; }

        private EbConnectionFactory _infraConnectionFactory = null;

        protected EbServerEventClient ServerEventClient { get; private set; }

        protected EbMqClient MQClient { get; private set; }

        protected EbStaticFileClient FileClient { get; private set; }

        protected PooledRedisClientManager PooledRedisManager { get; private set; }

        protected EbConnectionFactory InfraConnectionFactory
        {
            get
            {
                if (_infraConnectionFactory == null)
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

        public EbBaseService(IEbConnectionFactory _dbf, PooledRedisClientManager pooledRedisManager)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.PooledRedisManager = pooledRedisManager;
        }

        public EbBaseService(IMessageProducer _mqp)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbBaseService(RestSharp.IRestClient _rest)
        {
            this.RestClient = _rest as RestClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbStaticFileClient _sfc)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.FileClient = _sfc as EbStaticFileClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbStaticFileClient _sfc, PooledRedisClientManager pooledRedisManager)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.FileClient = _sfc as EbStaticFileClient;
            this.PooledRedisManager = pooledRedisManager;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbServerEventClient _sec)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.ServerEventClient = _sec as EbServerEventClient;
        }

        public EbBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbStaticFileClient _sfc, IMessageProducer _mqp)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.FileClient = _sfc as EbStaticFileClient;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc, IEbServerEventClient _se)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.ServerEventClient = _se as EbServerEventClient;
        }

        public EbBaseService(IEbMqClient _mq, IEbServerEventClient _se)
        {
            this.MQClient = _mq as EbMqClient;
            this.ServerEventClient = _se as EbServerEventClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, PooledRedisClientManager pooledRedisManager)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.PooledRedisManager = pooledRedisManager;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbMqClient _mq)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MQClient = _mq as EbMqClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbMqClient _mqc, IEbServerEventClient _sec)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MQClient = _mqc as EbMqClient;
            this.ServerEventClient = _sec as EbServerEventClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc, IEbServerEventClient _sec)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.ServerEventClient = _sec as EbServerEventClient;
        }
        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IEbServerEventClient _sec)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.ServerEventClient = _sec as EbServerEventClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc, IEbMqClient _mq)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.MQClient = _mq as EbMqClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbStaticFileClient _sfc, IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.FileClient = _sfc as EbStaticFileClient;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        public EbBaseService(IEbConnectionFactory _dbf, IEbStaticFileClient _sfc, IMessageProducer _mqp, IMessageQueueClient _mqc, PooledRedisClientManager pooledRedisManager)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.FileClient = _sfc as EbStaticFileClient;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.PooledRedisManager = pooledRedisManager;
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

        public bool InsertExecutionLog(string rows, TimeSpan t, DateTime starttime, int userid, List<Param> param, string refid)
        {
            try
            {
                List<JsonParams> _ParamList = new List<JsonParams>();
                if (param != null)
                    foreach (Param item in param)
                    {
                        JsonParams obj = new JsonParams { Name = item.Name, Type = Enum.GetName(typeof(EbDbTypes), Convert.ToInt32(item.Type)), Value = item.Value };
                        _ParamList.Add(obj);
                    }
                string _param = JsonConvert.SerializeObject(_ParamList);
                string query = EbConnectionFactory.ObjectsDB.EB_INSERT_EXECUTION_LOGS;
                DbParameter[] parameters = {
                     EbConnectionFactory.ObjectsDB.GetNewParameter("rows", EbDbTypes.String,rows),
                     EbConnectionFactory.ObjectsDB.GetNewParameter("exec_time", EbDbTypes.Int32,t.TotalMilliseconds),
                     EbConnectionFactory.ObjectsDB.GetNewParameter("created_by", EbDbTypes.Int32,userid),
                     EbConnectionFactory.ObjectsDB.GetNewParameter("created_at", EbDbTypes.DateTime,starttime),
                     EbConnectionFactory.ObjectsDB.GetNewParameter("params", EbDbTypes.Json,_param),
                     EbConnectionFactory.ObjectsDB.GetNewParameter("refid", EbDbTypes.String,refid)
                    };
                this.EbConnectionFactory.ObjectsDB.DoNonQuery(query, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool GetLogEnabled(string _refId)
        {
            //Add LogEnabled prop in dr obj instead of eb_objects table  

            //List<DbParameter> parameter = new List<DbParameter>();

            //string query = EbConnectionFactory.ObjectsDB.EB_GET_LOG_ENABLED;

            //parameter.Add(EbConnectionFactory.ObjectsDB.GetNewParameter("refid", EbDbTypes.String, _refId));

            //EbDataTable dt = EbConnectionFactory.ObjectsDB.DoQuery(query, parameter.ToArray());
            //if (dt.Rows.Count > 0)
            //    return ((dt.Rows[0][0].ToString()) == "T") ? true : false;
            return false;
        }

        public Eb_Solution GetSolutionObject(string cid)
        {
            Eb_Solution s_obj = null;
            try
            {
                if (this.PooledRedisManager != null)
                {
                    using (IRedisClient RedisReadOnly = this.PooledRedisManager.GetReadOnlyClient())
                        s_obj = RedisReadOnly.Get<Eb_Solution>(String.Format("solution_{0}", cid));
                }
                else
                    s_obj = this.Redis.Get<Eb_Solution>(String.Format("solution_{0}", cid));

                if (s_obj == null)
                {
                    Gateway.Send<UpdateSolutionObjectResponse>(new UpdateSolutionObjectRequest() { SolnId = cid });
                    s_obj = this.Redis.Get<Eb_Solution>(String.Format("solution_{0}", cid));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return s_obj;
        }

        public User GetUserObject(string userAuthId, bool forceUpdate = false)
        {
            User user = null;
            try
            {
                if (!string.IsNullOrEmpty(userAuthId))
                {
                    string[] parts = userAuthId.Split(":"); // iSolutionId:UserId:WhichConsole
                    if (parts.Length == 3)
                    {
                        if (this.PooledRedisManager != null)
                        {
                            using (IRedisClient RedisReadOnly = this.PooledRedisManager.GetReadOnlyClient())
                                user = RedisReadOnly.Get<User>(userAuthId);
                        }
                        else
                            user = this.Redis.Get<User>(userAuthId);
                        if (user == null || forceUpdate)
                        {
                            Gateway.Send<UpdateUserObjectResponse>(new UpdateUserObjectRequest() { SolnId = parts[0], UserId = Convert.ToInt32(parts[1]), UserAuthId = userAuthId, WC = parts[2] });
                            user = this.Redis.Get<User>(userAuthId);
                        }
                    }
                    else
                    { Console.WriteLine("userAuthId incorrect" + userAuthId); }
                }
                else
                { Console.WriteLine("userAuthId incorrect" + userAuthId); }
            }
            catch (Exception e) { Console.WriteLine(e.Message + e.StackTrace); }
            return user;
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

    public class EbBaseTenatService : Service
    {
        private EbConnectionFactory _infraConnectionFactory = null;

        protected EbConnectionFactory InfraConnectionFactory
        {
            get
            {
                if (_infraConnectionFactory == null)
                    _infraConnectionFactory = new EbConnectionFactory(EbConnectionsConfigProvider.InfraConnections, CoreConstants.EXPRESSBASE);

                return _infraConnectionFactory;
            }
        }
    }
}
