
using Common.Helpers.security;
using Common.Helpers.Extentions;
using DomainClasses.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.AM
{
    public class CentralAdmin:BaseEntity
    {
        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string Title { get; set; }

        [DataMember]
        public virtual byte[] Logo { get; set; }

        [DataMember]
        public bool SslEnabled { get; set; }

        [DataMember]
        public string SecureUrl { get; set; }

         [DataMember]
        public string PWSLinkServer { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public virtual DateTime CreateDate { get; set; }

        [DataMember]
        public virtual DateTime EstablishmentDate { get; set; }

        public HttpSecurityMode GetSecurityMode(bool? useSsl = null)
        {
            if (useSsl ?? SslEnabled)
            {
                if (SecureUrl.HasValue() && Url.HasValue() && !Url.StartsWith("https"))
                {
                    return HttpSecurityMode.SharedSsl;
                }
                else
                {
                    return HttpSecurityMode.Ssl;
                }
            }
            return HttpSecurityMode.Unsecured;
        }
       
    }
}
