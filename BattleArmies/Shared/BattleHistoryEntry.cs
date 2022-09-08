using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleArmies.Shared
{
    public class BattleHistoryEntry
    {
        public int BattleId { get; set; }
        public DateTime BattleDate { get; set; }
        public int AttackerId { get; set; }
        public int OpponentId { get; set; }
        public bool IsVictory { get; set; }
        public string AttackerName { get; set; }
        public string OpponentName { get; set; }
        public int RoundsFought { get; set; }
        public int VictoriousDamage { get; set; }
    }
}
