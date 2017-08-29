using ServiceStack.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack;

namespace ExpressBase.Objects.Objects.MQRelated
{
    public class EmailRequest
    {
        public string FromAdressTitle { get; set; }
        public string ToAddress { get; set; }
        public string ToAdressTitle { get; set; }
        public string Subject { get; set; }
        public string BodyContent { get; set; }
    }
}
