using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReregisterMVC.Models
{
    public class EmailOptionsDTO
    {
        public string Host { get; set; }
        public string ApiKey { get; set; }
        public string ApiKeySecret { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }

        public EmailOptionsDTO()
        {
            this.Host = "in-v3.mailjet.com";
            this.ApiKey = "cd1afc76f169f995319bab8008ff0412";
            this.ApiKeySecret = "13e0df8c6a89ace9e0e219dfea982093";
            this.Port = 587;
            this.SenderEmail = "re-register@loganyoung.co.za";
        }
    }
}