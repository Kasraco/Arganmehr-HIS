using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.AM.Extentions
{
    public static class CentralAdminExtensions
    {
        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>Comma-separated hosts</returns>
        public static string[] ParseHostValues(this CentralAdmin centralAdmin)
        {
            if (centralAdmin == null)
                throw new ArgumentNullException("centralmanagement");

            var parsedValues = new List<string>();
            if (!string.IsNullOrEmpty(centralAdmin.Url))
            {
                var hosts = centralAdmin.Url.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                parsedValues.AddRange(hosts.Select(host => host.Trim()).Where(tmp => !string.IsNullOrEmpty(tmp)));
            }

            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public static bool ContainsHostValue(this CentralAdmin centralAdmin, string host)
        {
            if (centralAdmin == null)
                throw new ArgumentNullException("institution");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = centralAdmin.ParseHostValues()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
    }
}
