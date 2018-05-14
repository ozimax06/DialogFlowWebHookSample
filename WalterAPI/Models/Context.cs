using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WalterAPI.Models
{
    public class Context
    {
        public int lifespan { get; set; }
        public string name { get; set; }
        public Parameters parameters { get; set; }

        public Context()
        {

        }
    }

    public class Parameters
    {

    }
}