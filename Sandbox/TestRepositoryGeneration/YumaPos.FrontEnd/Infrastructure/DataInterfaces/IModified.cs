using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YumaPos.FrontEnd.Infrastructure.DataInterfaces
{
    public interface IModified
    {
        /// <summary>
        /// EmployeeId, not constrait for perfomance
        /// </summary>
        DateTime? Modified { get; set; }
    }
}
