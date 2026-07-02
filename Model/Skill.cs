using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Model
{

    public class Skill
    {
        public int Id { get; set; }

        public string SkillName { get; set; } = string.Empty;

        public int PositionId { get; set; }

        public string? SkillComplexity { get; set; }
    }
}
