using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbTable
    {
        [ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        public EbTable()
        {
            this.Id = Id;
            this.Name = Name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class EbTableCollection : Dictionary<int, EbTable>
    {
        public EbTable this[string tablename]
        {
            get
            {
                foreach (EbTable table in this.Values)
                {
                    if (table.Name == tablename)
                        return table;
                }

                return null;
            }
        }
    }
}
