using MonsterCompanySimModel.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySim.ViewModels.Controls
{
    internal class BattlerSelectorViewModel : BindableBase
    {
        const string NoEmployeeName = "社員無し";


        public ReactivePropertySlim<List<Employee>> Employees { get; } = new();

        public ReactivePropertySlim<Employee> SelectedEmployee { get; } = new();

        public ReactivePropertySlim<string> Level { get; } = new();

        public ReactivePropertySlim<List<int>> Targets { get; } = new();

        public ReactivePropertySlim<int> SelectedTarget { get; } = new();

        public ReactivePropertySlim<long> Atk { get; } = new();
        public ReactivePropertySlim<long> Dex { get; } = new();
        public ReactivePropertySlim<long> Eng { get; } = new();
        public ReactivePropertySlim<bool> IsSkillDisabled { get; } = new(false);
        public ReactivePropertySlim<bool> IsEnemy { get; } = new(false);

        public BattlerSelectorViewModel(bool isEnemy)
        {
            List<Employee> emps = new() { new Employee() { Name = NoEmployeeName } };
            List<Employee> master;
            if (isEnemy)
            {
                master = Masters.EnemyEmployees;
            }
            else
            {
                master = Masters.Employees;
            }
            foreach (var emp in master)
            {
                emps.Add(emp);
            }
            Employees.Value = emps;
            SelectedEmployee.Value = emps[0];
            Level.Value = "99999";
            Targets.Value = new List<int>() { 1, 2, 3 };
            SelectedTarget.Value = 1;
            IsEnemy.Value = isEnemy;

            SelectedEmployee.Subscribe(_ => CalcStatus());
            Level.Subscribe(_ => CalcStatus());
        }

        public BattlerSelectorViewModel() : this(false)
        {

        }


        private void CalcStatus()
        {
            Battler? battler = Battler;
            if (battler == null)
            {
                Atk.Value = 0;
                Dex.Value = 0;
                Eng.Value = 0;
            }
            else
            {
                Atk.Value = battler.Atk;
                Dex.Value = battler.Dex;
                Eng.Value = battler.Eng;
            }

        }

        public Battler? Battler 
        {
            get
            {
                if (SelectedEmployee.Value.Id == 0)
                {
                    return null;
                }
                return new Battler(SelectedEmployee.Value) { Level = LevelParse(Level.Value), IsSkillDisabled = this.IsSkillDisabled.Value };
            }
        }

        public int LevelParse(string str)
        {
            if(int.TryParse(str, out int level))
            {
                return level;
            }
            return 99999;
        }

        internal void SetEmployee(Employee? emp)
        {
            if (emp == null)
            {
                SelectedEmployee.Value = Employees.Value[0];
            }
            else
            {
                SelectedEmployee.Value = emp;
            }
        }
    }
}
