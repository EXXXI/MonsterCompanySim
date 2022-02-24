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
    /// <summary>
    /// 社員選択部品のViewModel
    /// </summary>
    internal class BattlerSelectorViewModel : BindableBase
    {
        /// <summary>
        /// 社員なし時の表示名
        /// </summary>
        private string NoEmployeeName { get; } = Masters.ConfigData.NoEmproyeeName;

        /// <summary>
        /// 社員一覧
        /// </summary>
        public ReactivePropertySlim<List<Employee>> Employees { get; } = new();

        /// <summary>
        /// 選択中の社員
        /// </summary>
        public ReactivePropertySlim<Employee> SelectedEmployee { get; } = new();

        /// <summary>
        /// 社員のレベル
        /// </summary>
        public ReactivePropertySlim<string> Level { get; } = new();

        /// <summary>
        /// ターゲットのリスト(1, 2, 3)
        /// </summary>
        public ReactivePropertySlim<List<int>> Targets { get; } = new();

        /// <summary>
        /// 選択中ターゲット
        /// </summary>
        public ReactivePropertySlim<int> SelectedTarget { get; } = new();

        /// <summary>
        /// 戦闘
        /// </summary>
        public ReactivePropertySlim<long> Atk { get; } = new();

        /// <summary>
        /// 器用
        /// </summary>
        public ReactivePropertySlim<long> Dex { get; } = new();

        /// <summary>
        /// 消費
        /// </summary>
        public ReactivePropertySlim<long> Eng { get; } = new();

        /// <summary>
        /// スキル無効フラグ(無効時true)
        /// </summary>
        public ReactivePropertySlim<bool> IsSkillDisabled { get; } = new(false);

        /// <summary>
        /// 敵フラグ(敵時true)
        /// </summary>
        public ReactivePropertySlim<bool> IsEnemy { get; } = new(false);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="isEnemy">敵フラグ</param>
        public BattlerSelectorViewModel(bool isEnemy)
        {
            // 敵の場合は敵用、味方の場合は味方用の社員リストを読み込む
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

            // 初期状態をセット
            SelectedEmployee.Value = emps[0];
            Level.Value = "99999";
            Targets.Value = new List<int>() { 1, 2, 3 };
            SelectedTarget.Value = 1;
            IsEnemy.Value = isEnemy;

            // Subscribe
            SelectedEmployee.Subscribe(_ => CalcStatus());
            Level.Subscribe(_ => CalcStatus());
        }

        /// <summary>
        /// ステータス再計算
        /// </summary>
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

        /// <summary>
        /// 選択中のEmployeeをBattlerにして返却
        /// 社員無しの場合nullを返却
        /// </summary>
        public Battler? Battler 
        {
            get
            {
                if (SelectedEmployee.Value.Id == 0)
                {
                    // 社員無し
                    return null;
                }
                return new Battler(SelectedEmployee.Value) { Level = LevelParse(Level.Value), IsSkillDisabled = this.IsSkillDisabled.Value };
            }
        }

        /// <summary>
        /// レベルをint型にParse
        /// </summary>
        /// <param name="str">レベル(string)</param>
        /// <returns>レベル(int)</returns>
        private int LevelParse(string str)
        {
            if(int.TryParse(str, out int level))
            {
                return level;
            }

            // Parseできないときはレベル0とする
            return 0;
        }

        /// <summary>
        /// 引数で指定した社員を選択する
        /// </summary>
        /// <param name="emp">社員</param>
        public void SetEmployee(Employee? emp)
        {
            if (emp == null)
            {
                // null時は「社員無し」
                SelectedEmployee.Value = Employees.Value[0];
            }
            else
            {
                SelectedEmployee.Value = emp;
            }
        }
    }
}
