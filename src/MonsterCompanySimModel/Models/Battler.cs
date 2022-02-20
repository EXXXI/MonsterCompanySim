using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    public class Battler
    {
        public Employee Employee { get; }
        public double Modifier { get; set; } = 1;
        public CriticalState AtkCritState { get; set; }
        public CriticalState DefCritState { get; set; }
        public CriticalState OnceAtkCritState { get; set; }
        public CriticalState OnceDefCritState { get; set; }
        public int FixedTarget { get; set; } = 0;
        public long Level { get; set; }
        public bool IsReduced { get; set; } = false;
        public bool IsBoost { get; set; } = false;

        public long Atk
        {
            get
            {
                return (long)(Employee.Atk * (1 + (Level - 1) * Employee.GuerrillaModifier));
            }
        }
        public long Dex
        {
            get
            {
                return (long)(Employee.Dex * (1 + (Level - 1) * Employee.GuerrillaModifier));
            }
        }
        public long Eng
        {
            get
            {
                return (long)(Employee.Eng * (1 + (Level - 1) * Employee.GuerrillaModifier));
            }
        }




        public Battler(Employee employee)
        {
            Employee = employee;
        }

        public void ResetAttackProperty()
        {
            ResetAttackProperty(false);
        }
        public void ResetAttackProperty(bool boost)
        {
            IsBoost = boost;
            Modifier = 1;
            AtkCritState = CriticalState.normal;
            DefCritState = CriticalState.normal;
            IsReduced = false;
        }

        public override string ToString()
        {
            return Employee?.ToString() ?? string.Empty;
        }
    }
}
