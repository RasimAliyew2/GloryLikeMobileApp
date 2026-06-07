using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Model
{
    public class Seniority
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int JobFamilyId { get; set; }

        public List<Position> Positions { get; set; } = new();

    }
}
