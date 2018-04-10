using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFPtest.Models
{
    public class Node
    {
        public string label { get; set; }
        public string id { get; set; }
        public bool load_on_demand { get; set; }

        public Node(string label, string id, bool loadOnDemand = true)
        {
            this.label = label;
            this.id = id;
            this.load_on_demand = loadOnDemand;
        }
    }
}