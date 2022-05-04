using MapAssist.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA_Filter
{
    class Rule
    {
        public string ItemName;
        public ItemQuality Quality;
        public List<Skill> Skills;
        public List<State> States;
        public int MapLevel;
        public int PlayerLevel;
        public int SocketMin;
        public int SocketMax;
        public bool Ethereal;
    }
}
