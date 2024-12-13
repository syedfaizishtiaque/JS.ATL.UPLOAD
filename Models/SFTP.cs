using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL_UPLOAD.Models
{
    public class SFTPConfig
    {

        public List<Source> Sources { get; set; }

        public List<Destination> Destinations { get; set; }
        public string Ek { get; set; }
        public string KeyPath { get; set; }
    }

    public class Source
    {
        public int SrcId { get; set; }
        public string SrcType { get; set; }
        public string SrcFolder { get; set; }
        public string SrcArchFolder { get; set; }
        public bool SrcStatus { get; set; }
        public string SrcEndPoint { get; set; }

        public string SrcUsr { get; set; }
        public string SrcPwd { get; set; }
        public string DownloadFolder { get; set; }


    }

    public class Destination
    {
        public int SrcId { get; set; }
        public string DestType { get; set; }
        public string DestEndPoint { get; set; }
        public string DestUsr { get; set; }
        public string DestPwd { get; set; }
        public string DestFolder { get; set; }

        public string ToMove { get; set; }

        public string DestArchFolder { get; set; }

    }
}
