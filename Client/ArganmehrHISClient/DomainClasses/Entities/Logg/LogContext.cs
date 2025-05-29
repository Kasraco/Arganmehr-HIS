using DomainClasses.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainClasses.Entities.Logg
{
    public class LogContext
    {
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public LogLevel LogLevel { get; set; }
        public User User { get; set; }

        public bool HashNotFullMessage { get; set; }
        public bool HashIpAddress { get; set; }
    }
}
