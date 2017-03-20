using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    [Route("/insert", "POST")]
    public class FormPersistRequest : IReturn<bool>
    {

        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public int TableId { get; set; }

    }
    [DataContract]
    [Route("/uc", "POST")]
    public class CheckIfUnique : IReturn<bool>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public int TableId { get; set; }

    }

    [DataContract]
    [Route("/view/{ColId}", "GET")]
    public class View : IReturn<ViewResponse>
    {

        [DataMember(Order = 1)]
        public int ColId { get; set; }

        [DataMember(Order = 2)]
        public int TableId { get; set; }

        [DataMember(Order = 3)]
        public int FId { get; set; }

    }

    [DataContract]
    public class ViewResponse
    {

        [DataMember(Order = 1)]
        public EbForm ebform { get; set; }
    }
}
