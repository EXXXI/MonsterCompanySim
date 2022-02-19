using MonsterCompanySim.ViewModels.Controls;
using MonsterCompanySimModel.Models;
using MonsterCompanySimModel.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySim.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        private readonly Simulator simulator = new();

        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy1VM { get; } = new();
        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy2VM { get; } = new();
        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy3VM { get; } = new();
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally1VM { get; } = new();
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally2VM { get; } = new();
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally3VM { get; } = new();
        public ReactivePropertySlim<string> ResultText { get; } = new();
        public ReactivePropertySlim<string> TargetText { get; } = new();
        public ReactivePropertySlim<string> SearchPart { get; } = new();
        public ReactivePropertySlim<string> SearchLevel { get; } = new();
        public ReactivePropertySlim<bool> IsBoost { get; } = new();
        public ReactivePropertySlim<bool> IsBusy { get; } = new(false);
        public ReadOnlyReactivePropertySlim<bool> IsFree { get; }
        public ReactivePropertySlim<List<SearchResult>> Results { get; } = new();
        public ReactivePropertySlim<SearchResult> DetailSet { get; } = new();
        public ReactivePropertySlim<string> License { get; } = new();
        public ReactiveCommand CalcWinRateCommand { get; } = new ReactiveCommand();
        public ReactiveCommand CalcRequireCommand { get; } = new ReactiveCommand();
        public ReactiveCommand CalcOneBattleCommand { get; } = new ReactiveCommand();
        public AsyncReactiveCommand SearchCommand { get; }
        public ReactiveCommand SetAllyCommand { get; } = new ReactiveCommand();
        

        public MainViewModel()
        {
            simulator.LoadData();
            Ally1VM.Value = new BattlerSelectorViewModel();
            Ally2VM.Value = new BattlerSelectorViewModel();
            Ally3VM.Value = new BattlerSelectorViewModel();
            Enemy1VM.Value = new BattlerSelectorViewModel();
            Enemy2VM.Value = new BattlerSelectorViewModel();
            Enemy3VM.Value = new BattlerSelectorViewModel();
            CalcWinRateCommand.Subscribe(_ => CalcWinRate());
            CalcRequireCommand.Subscribe(_ => CalcRequire());
            CalcOneBattleCommand.Subscribe(_ => CalcOneBattle());
            SetAllyCommand.Subscribe(_ => SetAlly());

            IsFree = IsBusy.Select(x => !x).ToReadOnlyReactivePropertySlim();
            SearchCommand = IsFree.ToAsyncReactiveCommand().WithSubscribe(async () => await Search());

            // ライセンス表示
            StringBuilder sb = new();
            sb.Append("■このシミュのライセンス\n");
            sb.Append("MIT License\n");
            sb.Append('\n');
            sb.Append("■使わせていただいたOSS(+必要であればライセンス)\n");
            sb.Append("・System.Text.Json\n");
            sb.Append("プロジェクト：https://dot.net/\n");
            sb.Append("ライセンス：https://github.com/dotnet/runtime/blob/main/LICENSE.TXT\n");
            sb.Append('\n');
            sb.Append("・Prism.Wpf\n");
            sb.Append("プロジェクト：https://github.com/PrismLibrary/Prism\n");
            sb.Append("ライセンス：https://www.nuget.org/packages/Prism.Wpf/8.1.97/license\n");
            sb.Append('\n');
            sb.Append("・ReactiveProperty\n");
            sb.Append("プロジェクト：https://github.com/runceel/ReactiveProperty\n");
            sb.Append("ライセンス：https://github.com/runceel/ReactiveProperty/blob/main/LICENSE.txt\n");
            sb.Append('\n');
            License.Value = sb.ToString();

            // TODO:DEBUG
            simulator.Debug();
        }

        private void SetAlly()
        {
            Ally1VM.Value.SetEmployee(DetailSet.Value.Ally1);
            Ally2VM.Value.SetEmployee(DetailSet.Value.Ally2);
            Ally3VM.Value.SetEmployee(DetailSet.Value.Ally3);
        }

        private async Task Search()
        {
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            int level = Parse(SearchLevel.Value);
            int part = Parse(SearchPart.Value);

            List<SearchResult> results = await Task.Run(() => simulator.Search(enemy1, enemy2, enemy3, IsBoost.Value, level, part));

            Results.Value = results;

            StringBuilder sb = new();

            sb.AppendLine("■検索条件 LV:" + level + ", 部:" + part);

            sb.AppendLine("■検索結果 " + results.Count + "件");

            ResultText.Value = sb.ToString();
        }

        private void CalcOneBattle()
        {
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;


            BattleResult result = simulator.Battle(
                ally1, ally2, ally3, enemy1, enemy2, enemy3,
                Ally1VM.Value.SelectedTarget.Value , Ally2VM.Value.SelectedTarget.Value, Ally3VM.Value.SelectedTarget.Value,
                Enemy1VM.Value.SelectedTarget.Value, Enemy2VM.Value.SelectedTarget.Value, Enemy3VM.Value.SelectedTarget.Value,
                IsBoost.Value);

            StringBuilder sb = new();

            sb.AppendLine("勝率：" + (result.WinRate * 100).ToString() + "%");

            sb.AppendLine("■敵");
            sb.AppendLine(result.MinEnemyDamage?.Log + result.MinEnemyDamage?.Value);
            sb.AppendLine("■味方");
            sb.AppendLine(result.MaxAllyDamage?.Log + result.MaxAllyDamage?.Value);

            ResultText.Value = sb.ToString();
        }

        private void CalcRequire()
        {
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            int? level = simulator.CalcRequireLevel(ally1, ally2, ally3, enemy1, enemy2, enemy3, IsBoost.Value);

            StringBuilder sb = new();

            if (level == null)
            {
                sb.AppendLine("無理");
            }
            else
            {
                sb.AppendLine("要求Lv：" + level.ToString());
            }

            ResultText.Value = sb.ToString();
        }

        private void CalcWinRate()
        {
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            BattleResult result = simulator.FullBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, IsBoost.Value);

            StringBuilder sb = new();

            sb.AppendLine("勝率：" + (result.WinRate * 100).ToString() + "%");

            sb.AppendLine("■敵");
            sb.AppendLine(result.MinEnemyDamage?.Log + result.MinEnemyDamage?.Value);
            sb.AppendLine("■味方");
            sb.AppendLine(result.MaxAllyDamage?.Log + result.MaxAllyDamage?.Value);

            ResultText.Value = sb.ToString();

        }

        private int Parse(string value, int def)
        {
            if (int.TryParse(value, out int parsed))
            {
                return parsed;
            }
            return def;
        }
        private int Parse(string value)
        {
            return Parse(value, 0);
        }
    }
}
