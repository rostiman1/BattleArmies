using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleArmies.Shared
{
    public class BattleResults
    {
        public List<string> Log { get; set; } = new List<string>();
        public int AttackerDmgSum { get; set; }
        public int OpponentDmgSum { get; set; }
        public bool IsVictory { get; set; }
        public int RoundsFought { get; set; }
    }
}
