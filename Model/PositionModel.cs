using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MetanetA_MobileApp.Model
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int SeniorityId { get; set; }

        public List<Skill> Skills { get; set; } = new();

    }
}
