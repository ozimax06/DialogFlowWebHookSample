using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WalterAPI.Models
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string speech { get; set; }

        [DataMember]
        public string formattedText { get; set; }
    }
}