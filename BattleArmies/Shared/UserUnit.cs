using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleArmies.Shared
{
    public class UserUnit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Unit Unit { get; set; }
        public int UnitId { get; set; }
        public int HitPoints { get; set; }
    }
}
