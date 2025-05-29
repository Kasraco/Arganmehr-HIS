using Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Event
{
    public class EntityDeleted<T> : ComparableObject<T> where T : BaseEntity
    {

        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }

        [ObjectSignature]
        public T Entity { get; private set; }
    }
}
