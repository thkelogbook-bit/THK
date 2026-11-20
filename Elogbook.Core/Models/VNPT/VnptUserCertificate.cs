using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.VNPT
{
    public class VnptResGetCert
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public VnptGetCertData data { get; set; }
    }

    public class VnptGetCertData
    {
        public List<VnptUserCertificate> user_certificates { get; set; }
    }

    public class VnptUserCertificate
    {
        public string service_type { get; set; }
        public string service_name { get; set; }
        public string serial_number { get; set; }
        public string cert_subject { get; set; }
        public string cert_data { get; set; }
        public DateTime cert_valid_from { get; set; }
        public DateTime cert_valid_to { get; set; }
    }

    public class VnptResSign
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public VnptDataSign data { get; set; }
    }

    public class VnptDataSign
    {
        public string transaction_id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string error_message { get; set; }
    }

    public class VnptResStatus
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public VnptDataTransaction data { get; set; }
    }

    public class VnptDataTransaction
    {
        public string transaction_id { get; set; }
        public List<VnptSignature> signatures { get; set; }
    }

    public class VnptSignature
    {
        public string doc_id { get; set; }
        public string signature_value { get; set; }
    }
}
