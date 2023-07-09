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

        // TODO: drp032323 - ConfigurationBuilder doesn't support DefaultValue. Could switch to Json.Net which has DefaultValueHandling.
        //[DefaultValue("https://graph.microsoft.com/v1.0/me/tasks")]
        public string BaseUri { get; set; }

        /// <summary>
        /// Default target folder for Export/Sync
        /// </summary>
        public string TargetFolder { get; set; }

        /// <summary>
        /// Subfolder of TargetFolder where deleted tasks will be moved: "{TargetFolder}/{DeletedSubFolder}/{ListName}".
        /// </summary>
        public string DeletedSubFolder { get; set; }

        // TODO: drp070823 - [DefaultValue] doesn't work, need to put in appsettings.json
        [DefaultValue("all")]
        public string TargetListAllAlias { get; set; }
    }
}
