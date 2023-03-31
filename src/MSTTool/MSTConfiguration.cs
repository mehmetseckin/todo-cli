using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Todo.MSTTool
{
    public class MSTConfiguration
    {
        public string ClientId { get; set; }

        public IEnumerable<string> Scopes { get; set; }

        public bool SupportsWrite { get; set; }

        // TODO: drp032323 - ConfigurationBuilder doesn't support this. Json.Net has DefaultValueHandling.
        //[DefaultValue("https://graph.microsoft.com/v1.0/me/tasks")]
        public string BaseUri { get; set; }

        public string TargetFolder { get; set; }
    }
}
