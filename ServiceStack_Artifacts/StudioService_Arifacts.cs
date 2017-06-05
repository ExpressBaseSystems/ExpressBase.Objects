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
    public class EbObjectRequest : IReturn<EbObjectResponse>, IEbSSRequest
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
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
    public class EbObjectWrapper: IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public EbObjectType EbObjectType { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public byte[] Bytea { get; set; }

        [DataMember(Order = 5)]
        public EbObject EbObject { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 8)]
        public ObjectLifeCycleStatus Status { get; set; }

        [DataMember(Order = 9)]
        public string Description { get; set; }

        [DataMember(Order = 10)]
        public string ChangeLog { get; set; }

        [DataMember(Order = 11)]
        public int VersionNumber { get; set; }

        [DataMember(Order = 12)]
        public string IsSave { get; set; }

        public int UserId { get; set; }

        public EbObjectWrapper() { }

        //public EbObjectWrapper(int id, EbObjectType type, string name, byte[] bytea)
        //{
        //    Id = id;
        //    EbObjectType = type;
        //    this.Name = name;
        //    this.Bytea = bytea;
        //}
    }
}
