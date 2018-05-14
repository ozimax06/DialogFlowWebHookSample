using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WalterAPI.Models
{
    [DataContract]
    public class ContextOut
    {
        [DataMember]
        public string name { get; set; }
        
        [DataMember]
        public int lifespan { get; set; }

        [DataMember]
        public Object parameters { get; set; }

    }
}