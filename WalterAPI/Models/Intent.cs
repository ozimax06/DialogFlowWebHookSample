using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WalterAPI.Models
{
    public class Intent
    {
        public string name { get; set; }
        public bool auto { get; set; }
        public List<String> contexts { get; set; }
        public List<String> templates { get; set; }
        public Array userSays { get; set; }
        public Array responses { get; set; }

        public int priority = 500000;

        public Intent()
        {
            contexts = new List<String>();
            templates = new List<String>();
        }

    }
}