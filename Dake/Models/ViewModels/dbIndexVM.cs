using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models.ViewModels
{
    public class DbIndexVM
    {
        public DbIndexVM()
        {
            Message = "";
            IsSuccess = false;
        }
        public string Message { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
