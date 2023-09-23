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
    /// <summary>
    /// MainViewModel
    /// </summary>
    internal class MainViewModel : BindableBase
    {
        /// <summary>
        /// シミュ本体
        /// </summary>
        private readonly Simulator simulator = new();


        /// <summary>
        /// 敵社員1選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy1VM { get; } = new();

        /// <summary>
        /// 敵社員2選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy2VM { get; } = new();

        /// <summary>
        /// 敵社員3選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Enemy3VM { get; } = new();

        /// <summary>
        /// 味方社員1選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally1VM { get; } = new();

        /// <summary>
        /// 味方社員2選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally2VM { get; } = new();

        /// <summary>
        /// 味方社員3選択部品のVM
        /// </summary>
        public ReactivePropertySlim<BattlerSelectorViewModel> Ally3VM { get; } = new();

        /// <summary>
        /// 検索対象設定部品のVMのリスト
        /// </summary>
        public ReactivePropertySlim<List<TargetSelectorViewModel>> TargetVMs { get; } = new();

        /// <summary>
        /// 結果出力文字列
        /// </summary>
        public ReactivePropertySlim<string> ResultText { get; } = new();

        /// <summary>
        /// ステージリスト
        /// </summary>
        public ReactivePropertySlim<List<StageData>> StageDatas { get; } = new();

        /// <summary>
        /// 全ステージ表示フラグ(trueで全ステージ表示)
        /// </summary>
        public ReactivePropertySlim<bool> ShowAllStage { get; } = new(false);

        /// <summary>
        /// 選択ステージ
        /// </summary>
        public ReactivePropertySlim<StageData> SelectedStage { get; } = new();

        /// <summary>
        /// ステージ特殊条件リスト
        /// </summary>
        public ReactivePropertySlim<List<StageCondition>> StageConditions { get; } = new();

        /// <summary>
        /// 選択ステージ特殊条件
        /// </summary>
        public ReactivePropertySlim<StageCondition> SelectedStageCondition { get; } = new();

        /// <summary>
        /// 編成検索用：部指定候補
        /// </summary>
        public ReactivePropertySlim<List<string>> Parts { get; } = new();

        /// <summary>
        /// 編成検索用：部指定
        /// </summary>
        public ReactivePropertySlim<string> SelectedPart { get; } = new();

        /// <summary>
        /// 編成検索用：レベル指定
        /// </summary>
        public ReactivePropertySlim<string> SearchLevel { get; } = new();

        /// <summary>
        /// ブースト有無(ブーストありでtrue)
        /// </summary>
        public ReactivePropertySlim<bool> IsBoost { get; } = new();

        /// <summary>
        /// ビジーフラグ(編成検索中にtrue)
        /// </summary>
        public ReactivePropertySlim<bool> IsBusy { get; } = new(false);

        /// <summary>
        /// ビジーじゃないフラグ(編成検索中以外にtrue)
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsFree { get; }

        /// <summary>
        /// 進捗
        /// </summary>
        public ReactivePropertySlim<double> Progress { get; set; } = new();

        /// <summary>
        /// 編成検索結果
        /// </summary>
        public ReactivePropertySlim<List<SearchResult>> Results { get; } = new();

        /// <summary>
        /// 編成検索結果の選択中編成
        /// </summary>
        public ReactivePropertySlim<SearchResult> DetailSet { get; } = new();

        /// <summary>
        /// ライセンス関連の文言
        /// </summary>
        public ReactivePropertySlim<string> License { get; } = new();

        /// <summary>
        /// 勝率計算(ターゲットランダム)のコマンド
        /// </summary>
        public ReactiveCommand CalcWinRateCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 要求Lv概算のコマンド
        /// </summary>
        public ReactiveCommand CalcRequireCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 勝率計算(ターゲット指定)のコマンド
        /// </summary>
        public ReactiveCommand CalcOneBattleCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 編成検索のコマンド
        /// </summary>
        public AsyncReactiveCommand SearchCommand { get; }

        /// <summary>
        /// 検索した編成を味方社員欄に入力するコマンド
        /// </summary>
        public ReactiveCommand SetAllyCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 検索対象設定：全員除外するコマンド
        /// </summary>
        public ReactiveCommand AllExcludeCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 検索対象設定：全員追加するコマンド
        /// </summary>
        public ReactiveCommand AllIncludeCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 検索対象設定：おすすめ設定コマンド
        /// </summary>
        public ReactiveCommand RecommendationCommand { get; } = new ReactiveCommand();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            // 各種データをロード
            simulator.LoadData();

            // VMセット
            Ally1VM.Value = new BattlerSelectorViewModel(false);
            Ally2VM.Value = new BattlerSelectorViewModel(false);
            Ally3VM.Value = new BattlerSelectorViewModel(false);
            Enemy1VM.Value = new BattlerSelectorViewModel(true);
            Enemy2VM.Value = new BattlerSelectorViewModel(true);
            Enemy3VM.Value = new BattlerSelectorViewModel(true);
            List<TargetSelectorViewModel> targetVMs = new();
            foreach (var emp in Masters.Employees)
            {
                targetVMs.Add(new TargetSelectorViewModel(emp));
            }
            TargetVMs.Value = targetVMs;

            // ビジーフラグとビジーじゃないフラグを紐づけ
            IsFree = IsBusy.Select(x => !x).ToReadOnlyReactivePropertySlim();

            // 編成検索条件の初期状態
            Parts.Value = new() { "1", "2", "All" };
            SelectedPart.Value = "1";
            SearchLevel.Value = "99999";

            // ステージリストを準備
            SetStageList();

            // ステージ特殊条件を準備
            StageConditions.Value = Masters.StageConditions;
            SelectedStageCondition.Value = StageConditions.Value[0];

            // Subscribe
            CalcWinRateCommand.Subscribe(_ => CalcWinRate());
            CalcRequireCommand.Subscribe(_ => CalcRequire());
            CalcOneBattleCommand.Subscribe(_ => CalcOneBattle());
            SetAllyCommand.Subscribe(_ => SetAlly());
            AllExcludeCommand.Subscribe(_ => AllExclude());
            AllIncludeCommand.Subscribe(_ => AllInclude());
            RecommendationCommand.Subscribe(_ => Recommendation());
            SearchCommand = IsFree.ToAsyncReactiveCommand().WithSubscribe(async () => await Search());
            SelectedStage.Subscribe(_ => SetStage());
            ShowAllStage.Subscribe(_ => SetStageList());

            // 初期表示
            ResultText.Value = "ここに計算結果などが表示されます\n編成検索の結果は下部の表に表示されます";

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

            // 開発用
            // simulator.Debug();
        }

        /// <summary>
        /// ステージの敵情報を入力
        /// </summary>
        private void SetStage()
        {
            if(SelectedStage?.Value != null && SelectedStage.Value.Part != 0)
            {
                Enemy1VM.Value.SetEmployee(Masters.GetEnemyEmployee(SelectedStage.Value.Enemy1Id, SelectedStage.Value.Enemy1EvolState));
                Enemy1VM.Value.Level.Value = SelectedStage.Value.Enemy1Level.ToString();
                Enemy2VM.Value.SetEmployee(Masters.GetEnemyEmployee(SelectedStage.Value.Enemy2Id, SelectedStage.Value.Enemy2EvolState));
                Enemy2VM.Value.Level.Value = SelectedStage.Value.Enemy2Level.ToString();
                Enemy3VM.Value.SetEmployee(Masters.GetEnemyEmployee(SelectedStage.Value.Enemy3Id, SelectedStage.Value.Enemy3EvolState));
                Enemy3VM.Value.Level.Value = SelectedStage.Value.Enemy3Level.ToString();
                SelectedStageCondition.Value = SelectedStage.Value.StageCondition;
            }
        }

        /// <summary>
        /// ステージリストを初期化
        /// </summary>
        /// <param name="isShowAllStage">trueの場合、全ステージ表示</param>
        private void SetStageList()
        {
            int part1StageThreshold = Masters.ConfigData.Part1StageThreshold;
            int part2StageThreshold = Masters.ConfigData.Part2StageThreshold;
            if (ShowAllStage.Value)
            {
                part1StageThreshold = 1;
                part2StageThreshold = 1;
            }
            StageData def = new();
            List<StageData> stages = new() { def };
            foreach (var stage in Masters.StageDatas)
            {
                if ((stage.Part == 1 && stage.Grade >= part1StageThreshold) ||
                    (stage.Part == 2 && stage.Grade >= part2StageThreshold))
                {
                    stages.Add(stage);
                }
            }
            StageDatas.Value = stages;
            SelectedStage.Value = def;
        }

        /// <summary>
        /// 検索した編成を味方社員欄に入力
        /// </summary>
        private void SetAlly()
        {
            if (DetailSet.Value == null)
            {
                return;
            }
            Ally1VM.Value.SetEmployee(DetailSet.Value.Ally1);
            Ally2VM.Value.SetEmployee(DetailSet.Value.Ally2);
            Ally3VM.Value.SetEmployee(DetailSet.Value.Ally3);
        }

        /// <summary>
        /// 編成検索
        /// </summary>
        /// <returns>Task</returns>
        private async Task Search()
        {
            // 敵社員取得
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            // 敵社員存在チェック
            if (enemy1 == null && enemy2 == null && enemy3 == null)
            {
                ResultText.Value = "★敵社員を入力してください";
                return;
            }

            // 検索条件取得
            int level = Parse(SearchLevel.Value);
            int part = SelectedPart.Value switch
            {
                "All" => 0,
                "1" => 1,
                "2" => 2,
                _ => -1
            };

            // ステージ特殊条件
            StageCondition condition = SelectedStageCondition.Value;

            // 編成検索(非同期)
            List<SearchResult> results = await Task.Run(() => simulator.Search(enemy1, enemy2, enemy3, IsBoost.Value, level, part, condition, Progress));

            // 結果出力
            Results.Value = results;

            // ログ出力
            StringBuilder sb = new();
            sb.AppendLine($"■検索条件 部:{part}, Lv:{level:#,0}");
            sb.AppendLine($"■検索結果 {results.Count:#,0}件");
            if (results.Count > Masters.ConfigData.RequireThreshold)
            {
                sb.AppendLine("★件数が多すぎるため、要求レベルの計算は行いません");
            }
            ResultText.Value = sb.ToString();
        }

        /// <summary>
        /// 勝率計算(ターゲット指定)
        /// </summary>
        private void CalcOneBattle()
        {
            // 社員情報取得
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            // 社員存在チェック
            if (ally1 == null && ally2 == null && ally3 == null)
            {
                ResultText.Value = "★味方社員を入力してください";
                return;
            }
            if (enemy1 == null && enemy2 == null && enemy3 == null)
            {
                ResultText.Value = "★敵社員を入力してください";
                return;
            }

            // ターゲット有効チェック
            bool isValidTargets = this.IsValidTargets(
                ally1, ally2, ally3, enemy1, enemy2, enemy3,
                Ally1VM.Value.SelectedTarget.Value, Ally2VM.Value.SelectedTarget.Value, Ally3VM.Value.SelectedTarget.Value,
                Enemy1VM.Value.SelectedTarget.Value, Enemy2VM.Value.SelectedTarget.Value, Enemy3VM.Value.SelectedTarget.Value);
            if (!isValidTargets)
            {
                ResultText.Value = "★ターゲットは社員がいる場所を入力してください";
                return;
            }

            // 計算
            BattleResult result = simulator.Battle(
                ally1, ally2, ally3, enemy1, enemy2, enemy3,
                Ally1VM.Value.SelectedTarget.Value , Ally2VM.Value.SelectedTarget.Value, Ally3VM.Value.SelectedTarget.Value,
                Enemy1VM.Value.SelectedTarget.Value, Enemy2VM.Value.SelectedTarget.Value, Enemy3VM.Value.SelectedTarget.Value,
                IsBoost.Value, SelectedStageCondition.Value);

            // 結果出力
            StringBuilder sb = new();
            sb.AppendLine($"勝率：{result.WinPercentage}");
            sb.AppendLine("■敵");
            sb.AppendLine($"{result.MinEnemyDamage?.Log}合計:{result.MinEnemyDamage?.Value:#,0}");
            sb.AppendLine("■味方");
            sb.AppendLine($"{result.MaxAllyDamage?.Log}合計:{result.MaxAllyDamage?.Value:#,0}");
            ResultText.Value = sb.ToString();
        }

        /// <summary>
        /// 要求Lv概算
        /// </summary>
        private void CalcRequire()
        {
            // 社員情報取得
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            // 社員存在チェック
            if (ally1 == null && ally2 == null && ally3 == null)
            {
                ResultText.Value = "★味方社員を入力してください";
                return;
            }
            if (enemy1 == null && enemy2 == null && enemy3 == null)
            {
                ResultText.Value = "★敵社員を入力してください";
                return;
            }

            // 計算
            int? level = simulator.CalcRequireLevel(ally1, ally2, ally3, enemy1, enemy2, enemy3, IsBoost.Value, SelectedStageCondition.Value);

            // ログ出力
            StringBuilder sb = new();
            if (level == null)
            {
                sb.AppendLine("無理");
            }
            else
            {
                sb.AppendLine($"要求Lv：{level}");
            }
            ResultText.Value = sb.ToString();
        }

        /// <summary>
        /// 勝率計算(ターゲットランダム)
        /// </summary>
        private void CalcWinRate()
        {
            // 社員情報取得
            Battler? ally1 = Ally1VM.Value.Battler;
            Battler? ally2 = Ally2VM.Value.Battler;
            Battler? ally3 = Ally3VM.Value.Battler;
            Battler? enemy1 = Enemy1VM.Value.Battler;
            Battler? enemy2 = Enemy2VM.Value.Battler;
            Battler? enemy3 = Enemy3VM.Value.Battler;

            // 社員存在チェック
            if (ally1 == null && ally2 == null && ally3 == null)
            {
                ResultText.Value = "★味方社員を入力してください";
                return;
            }
            if (enemy1 == null && enemy2 == null && enemy3 == null)
            {
                ResultText.Value = "★敵社員を入力してください";
                return;
            }

            // 計算
            BattleResult result = simulator.FullBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, IsBoost.Value, SelectedStageCondition.Value);

            // ログ出力
            StringBuilder sb = new();
            sb.AppendLine($"勝率：{result.WinPercentage}");
            sb.AppendLine("■敵");
            sb.AppendLine($"{result.MinEnemyDamage?.Log}合計:{result.MinEnemyDamage?.Value:#,0}");
            sb.AppendLine("■味方");
            sb.AppendLine($"{result.MaxAllyDamage?.Log}合計:{result.MaxAllyDamage?.Value:#,0}");
            ResultText.Value = sb.ToString();

        }

        /// <summary>
        /// 検索対象設定：おすすめ
        /// </summary>
        private void Recommendation()
        {
            foreach (var vm in TargetVMs.Value)
            {
                // TODO: ロジック側で制御するべき
                if (vm.Employee.Value.Rarity >= EmployeeRarity.LXR ||
                    vm.Employee.Value.Id == 33 ||
                    vm.Employee.Value.Id == 41 ||
                    vm.Employee.Value.Id == 62 ||
                    vm.Employee.Value.Id == 63 ||
                    vm.Employee.Value.Id == 65 ||
                    vm.Employee.Value.Id == 72)
                {
                    vm.IsTarget.Value = true;
                }
                else
                {
                    vm.IsTarget.Value = false;
                }
            }
        }

        /// <summary>
        /// 検索対象設定：全て含める
        /// </summary>
        private void AllInclude()
        {
            foreach (var vm in TargetVMs.Value)
            {
                vm.IsTarget.Value = true;
            }
        }

        /// <summary>
        /// 検索対象設定：すべて除外
        /// </summary>
        private void AllExclude()
        {
            foreach (var vm in TargetVMs.Value)
            {
                vm.IsTarget.Value = false;
            }
        }

        /// <summary>
        /// int.Parse
        /// 失敗時は0を返す
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns>Parse結果</returns>
        private int Parse(string value)
        {
            return Parse(value, 0);
        }

        /// <summary>
        /// int.Parse
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="def">Parse失敗時の値</param>
        /// <returns>Parse結果</returns>
        private int Parse(string value, int def)
        {
            if (int.TryParse(value, out int parsed))
            {
                return parsed;
            }
            return def;
        }

        /// <summary>
        /// ターゲット指定(全体)が有効かどうかチェック
        /// </summary>
        /// <param name="ally1">味方社員1</param>
        /// <param name="ally2">味方社員2</param>
        /// <param name="ally3">味方社員3</param>
        /// <param name="enemy1">敵社員1</param>
        /// <param name="enemy2">敵社員2</param>
        /// <param name="enemy3">敵社員3</param>
        /// <param name="a1">味方社員1のターゲット</param>
        /// <param name="a2">味方社員2のターゲット</param>
        /// <param name="a3">味方社員3のターゲット</param>
        /// <param name="e1">敵社員1のターゲット</param>
        /// <param name="e2">敵社員2のターゲット</param>
        /// <param name="e3">敵社員3のターゲット</param>
        /// <returns>有効時true</returns>
        private bool IsValidTargets(Battler? ally1, Battler? ally2, Battler? ally3, Battler? enemy1, Battler? enemy2, Battler? enemy3, int a1, int a2, int a3, int e1, int e2, int e3)
        {
            if (ally1 != null && !IsValidTarget(enemy1, enemy2, enemy3, a1))
            {
                return false;
            }
            if (ally2 != null && !IsValidTarget(enemy1, enemy2, enemy3, a2))
            {
                return false;
            }
            if (ally3 != null && !IsValidTarget(enemy1, enemy2, enemy3, a3))
            {
                return false;
            }
            if (enemy1 != null && !IsValidTarget(ally1, ally2, ally3, e1))
            {
                return false;
            }
            if (enemy2 != null && !IsValidTarget(ally1, ally2, ally3, e2))
            {
                return false;
            }
            if (enemy3 != null && !IsValidTarget(ally1, ally2, ally3, e3))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ターゲット指定(1つ)が有効かどうかのチェック
        /// </summary>
        /// <param name="battler1">相手1</param>
        /// <param name="battler2">相手2</param>
        /// <param name="battler3">相手3</param>
        /// <param name="target">ターゲット</param>
        /// <returns></returns>
        private bool IsValidTarget(Battler? battler1, Battler? battler2, Battler? battler3, int target)
        {
            return target switch
            {
                1 => battler1 != null,
                2 => battler2 != null,
                3 => battler3 != null,
                _ => false,
            };
        }
    }
}
