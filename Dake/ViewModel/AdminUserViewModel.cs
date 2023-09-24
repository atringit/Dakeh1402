using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
    public class AdminUserViewModel
    {
        public string deleted { get; set; }
        public string passwordShow { get; set; }
        public string cellphone { get; set; }
        public List<string> adminRole { get; set; }
        public List<int> adminCitys { get; set; }
        public string adminCitysEdit { get; set; }
        public string adminRoleEdit { get; set; }
        public DateTime oTPDate { get; set; }
        public int roleId { get; set; }
        public int id { get; set; }        

    }
}
