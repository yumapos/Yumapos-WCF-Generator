using System.Collections.Generic;

namespace WCFGenerator.WcfClientGeneration
{
    public class ServiceDetail
    {
        public string UserName { get; set; }
        public string FileName { get; set; }

        public string ProjectName { get; set; }
        public string FaultProject { get; set; }
        public string ProjectApi { get; set; }

        public List<string> ProjectApiFolders { get; set; }
    }
}