using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public interface ICloneable<T> : ICloneable
    {
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned instance</returns>
        new T Clone();
    }
}
