using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WalterAPI.Models
{
    [DataContract]
    public class DialogflowResponse
    {
        [DataMember]
        public string speech { get; set; }

        [DataMember(IsRequired = false)]
        public List<ContextOut> contextOut { get; set; }

        [DataMember]
        public Data data { get; set; }
    }
}