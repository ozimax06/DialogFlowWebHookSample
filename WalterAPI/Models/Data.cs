using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WalterAPI.Models
{
    [DataContract]
    public class Data
    {
        [DataMember]
        public string speech { get; set; }

        [DataMember]
        public string source { get; set; }

        [DataMember]
        public string text { get; set; }

        [DataMember]
        public string displayText { get; set; }

        [DataMember]
        public List<Message> messages { get; set; }
    }
}