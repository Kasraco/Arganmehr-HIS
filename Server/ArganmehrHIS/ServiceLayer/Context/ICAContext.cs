using DomainClasses.Entities.AM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Context
{
    public interface ICAContext
    {
        void SetRequestCA(int? centralAdminId);


        void SetPreviewCA(int? centralAdminId);


        int? GetRequestCA();


        int? GetPreviewCA();

        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        CentralAdmin CurrentCA { get; }


        int CurrentCAIdIfMultiCAMode { get; }
    }
}
