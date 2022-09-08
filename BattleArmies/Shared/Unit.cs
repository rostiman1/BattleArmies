using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleArmies.Shared
{
    public class Unit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Title { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int HitPoints { get; set; } = 100;
        public int CoinsCost { get; set; }
    }
}
