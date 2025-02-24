﻿using Dake.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
   public interface IReportNotice
    {
       PagedList<ReportNotice> GetReportNotice(int pageId = 1);
    }
}
