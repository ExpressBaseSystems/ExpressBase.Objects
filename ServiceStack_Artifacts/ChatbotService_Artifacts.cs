using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class CreateBotRequest : EbServiceStackRequest, IReturn<CreateBotResponse>
    {
        public string SolutionId { get; set; }  

        public string BotName { get; set; }

        public string WebURL { get; set; }

        public string WelcomeMsg{ get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class CreateBotResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string BotId { get; set; }


    }
   
}
