using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/ebo")]
    [Route("/ebo/{Id}")]
    public class EbObjectRequest : IReturn<EbObjectResponse>
    {
        public int Id { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class EbObjectResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }
    }

    [DataContract]
    [Route("/ebo", "POST")]
    public class EbObjectWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public EbObjectType EbObjectType { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public byte[] Bytea { get; set; }

        [DataMember(Order = 5)]
        public EbObject EbObject { get; set; }

        public EbObjectWrapper() { }
        public EbObjectWrapper(int id, EbObjectType type, string name, byte[] bytea)
        {
            Id = id;
            EbObjectType = type;
            this.Name = name;
            this.Bytea = bytea;
        }
    }
}
