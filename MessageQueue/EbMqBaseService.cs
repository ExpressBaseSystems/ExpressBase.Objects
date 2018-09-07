using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.ServiceClients;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;

namespace ExpressBase.MessageQueue.Services
{
    public class EbMqBaseService : Service
    {
        protected EbConnectionFactory EbConnectionFactory { get; private set; }

        protected RabbitMqProducer MessageProducer3 { get; private set; }

        protected RabbitMqQueueClient MessageQueueClient { get; private set; }

        private EbConnectionFactory _infraConnectionFactory = null;

        protected EbServerEventClient ServerEventClient { get; private set; }

        protected JsonServiceClient ServiceStackClient { get; private set; }

        protected EbConnectionFactory InfraConnectionFactory
        {
            get
            {
                if (_infraConnectionFactory == null)
                    _infraConnectionFactory = new EbConnectionFactory(CoreConstants.EXPRESSBASE, this.Redis);

                return _infraConnectionFactory;
            }
        }

        public EbMqBaseService()
        {
        }

        //public EbMqBaseService(IEbConnectionFactory _dbf)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //}

        public EbMqBaseService(IMessageProducer _mqp)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
        }

        public EbMqBaseService(IEbServerEventClient _sec)
        {
            this.ServerEventClient = _sec as EbServerEventClient;
        }
        public EbMqBaseService(IServiceClient _ssclient)
        {
            this.ServiceStackClient = _ssclient as JsonServiceClient;
        }

        public EbMqBaseService(IEbConnectionFactory _dbf, IServiceClient _ssclient)
        {
            this.EbConnectionFactory = _dbf as EbConnectionFactory;
            this.ServiceStackClient = _ssclient as JsonServiceClient;
        }

        //public EbMqBaseService(IEbConnectionFactory _dbf, IEbServerEventClient _sec)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //    this.ServerEventClient = _sec as EbServerEventClient;
        //}

        public EbMqBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        }

        public EbMqBaseService(IMessageProducer _mqp, IMessageQueueClient _mqc, IEbServerEventClient _sec)
        {
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.ServerEventClient = _sec as EbServerEventClient;
        }
        public EbMqBaseService(IServiceClient _ssclient, IMessageProducer _mqp, IMessageQueueClient _mqc, IEbServerEventClient _sec)
        {
            this.ServiceStackClient = ServiceStackClient as JsonServiceClient;
            this.MessageProducer3 = _mqp as RabbitMqProducer;
            this.MessageQueueClient = _mqc as RabbitMqQueueClient;
            this.ServerEventClient = _sec as EbServerEventClient;
        }
        //public EbMqBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //    this.MessageProducer3 = _mqp as RabbitMqProducer;
        //}

        //public EbMqBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IEbServerEventClient _sec)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //    this.MessageProducer3 = _mqp as RabbitMqProducer;
        //    this.ServerEventClient = _sec as EbServerEventClient;
        //}

        //public EbMqBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //    this.MessageProducer3 = _mqp as RabbitMqProducer;
        //    this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        //}

        //public EbMqBaseService(IEbConnectionFactory _dbf, IMessageProducer _mqp, IMessageQueueClient _mqc, IEbServerEventClient _sec)
        //{
        //    this.EbConnectionFactory = _dbf as EbConnectionFactory;
        //    this.MessageProducer3 = _mqp as RabbitMqProducer;
        //    this.MessageQueueClient = _mqc as RabbitMqQueueClient;
        //    this.ServerEventClient = _sec as EbServerEventClient;
        //}

        public ILog Log { get { return LogManager.GetLogger(GetType()); } }
    }
}