using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Model
{
    public class JobFamily
    {
        public int Id { get; set; }

        public string JobName { get; set; }

        public List<Seniority> Seniorities { get; set; } = new();
    }
}
