﻿using MonsterCompanySimModel.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySim.ViewModels.Controls
{
    /// <summary>
    /// 検索対象設定部品のVM
    /// </summary>
    class TargetSelectorViewModel : BindableBase
    {
        /// <summary>
        /// 社員
        /// </summary>
        public ReactivePropertySlim<Employee> Employee { get; set; } = new();

        /// <summary>
        /// 検索対象フラグ(含める場合にtrue)
        /// </summary>
        public ReactivePropertySlim<bool> IsTarget { get; set; } = new();

        /// <summary>
        /// ラジオボタン表示用：検索対象でないフラグ(含めない場合にtrue)
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsNotTarget { get; set; }

        /// <summary>
        /// 検索固定フラグ(含める場合にtrue)
        /// </summary>
        public ReactivePropertySlim<bool> IsRequired { get; set; } = new(false);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="emp">社員</param>
        public TargetSelectorViewModel(Employee emp)
        {
            // 初期状態
            Employee.Value = emp;
            IsTarget.Value = Masters.IsTarget(emp);
            IsRequired.Value = Masters.IsRequired(emp);

            // Subscribe
            IsTarget.Subscribe(isTarget => ChangeTarget(isTarget));
            IsRequired.Subscribe(isRequired => ChangeRequired(isRequired));

            // IsNotTargetはIsTargetの変更に追従する
            IsNotTarget = IsTarget.Select(x => !x).ToReadOnlyReactivePropertySlim();
        }

        /// <summary>
        /// 検索対象フラグの変更
        /// </summary>
        /// <param name="isTarget">変更後の値(検索対象に含める場合true)</param>
        private void ChangeTarget(bool isTarget)
        {
            if (isTarget)
            {
                Masters.IncludeTarget(Employee.Value);
            }
            else
            {
                Masters.ExcludeTarget(Employee.Value);
                IsRequired.Value = false;
            }
        }

        /// <summary>
        /// 検索固定フラグの変更
        /// </summary>
        /// <param name="isTarget">変更後の値(検索対象に含める場合true)</param>
        private void ChangeRequired(bool isRequired)
        {
            if (isRequired)
            {
                Masters.AddRequired(Employee.Value);
                IsTarget.Value = true;
            }
            else
            {
                Masters.DeleteRequired(Employee.Value);
            }
        }
    }
}
