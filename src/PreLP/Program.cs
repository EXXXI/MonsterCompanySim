using LPModel;
using MonsterCompanySimModel.Models;
using MonsterCompanySimModel.Service;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PreLP
{
    /// <summary>
    /// 解法をリストアップしてJsonとして保存
    /// </summary>
    class Program
    {
        /// <summary>
        /// デフォルトの最大レベル
        /// </summary>
        const int DefMax = 99999;

        /// <summary>
        /// 凸1回のレベル上限上昇値
        /// </summary>
        const int OneBreakLevel = 10000;

        /// <summary>
        /// シミュ本体
        /// </summary>
        private static readonly Simulator Sim = new();

        /// <summary>
        /// エントリポイント
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // シミュ起動
            Sim.LoadData();

            // 除外情報をデフォルトにセット
            ResetTarget();

            // ステージ情報用変数
            int part;
            int grade;
            int maxBreak;

            // 計算結果格納変数
            List<RequiredBreakData> results = new();

            // 1部42
            part = 1;
            grade = 42;
            maxBreak = 4;
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 172).First()); // ひかり
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 174).First()); // キュンシー
            CalcGradeData(part, grade, maxBreak, results);

            // 1部43
            grade = 43;
            maxBreak = 5;
            CalcGradeData(part, grade, maxBreak, results);

            // 1部44
            grade = 44;
            maxBreak = 6;
            CalcGradeData(part, grade, maxBreak, results);

            // 1部45
            grade = 45;
            maxBreak = 7;
            CalcGradeData(part, grade, maxBreak, results);

            // 1部46
            // 2-23が8凸ほぼ必須なため、ここまで2-23,25の5体が使用不可だった
            // 正確には7凸も可能だが、余計な凸が必要で、1-45への影響がもないので今回は考慮しない
            grade = 46;
            maxBreak = 8;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            CalcGradeData(part, grade, maxBreak, results);

            // 1部47
            // 2-27が9凸必須なため、ここまで2-27の3体が使用不可だった
            grade = 47;
            maxBreak = 9;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            CalcGradeData(part, grade, maxBreak, results);

            // 1部48
            // 2-29が10凸必須なため、ここまで2-29の2体が使用不可だった
            grade = 48;
            maxBreak = 10;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            CalcGradeData(part, grade, maxBreak, results);

            // 1部49
            // 2-31が11凸必須なため、ここまで2-31の2体が使用不可だった
            grade = 49;
            maxBreak = 11;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 172).First()); // ひかり
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 174).First()); // キュンシー
            CalcGradeData(part, grade, maxBreak, results);

            // 1部50
            grade = 50;
            maxBreak = 12;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部23
            part = 2;
            grade = 23;
            maxBreak = 8;
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 172).First()); // ひかり
            Masters.ExcludeTarget(Masters.Employees.Where(emp => emp.Id == 174).First()); // キュンシー
            CalcGradeData(part, grade, maxBreak, results);

            // 2部24
            grade = 24;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            CalcGradeData(part, grade, maxBreak, results);

            // 2部25
            grade = 25;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部26
            // 1-46で覚醒岬が必須だったため、ここまで8凸制限だった
            grade = 26;
            maxBreak = 9;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            CalcGradeData(part, grade, maxBreak, results);

            // 2部27
            grade = 27;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部28
            // 1-47でタナトスが必須だったため、ここまで9凸制限だった
            grade = 28;
            maxBreak = 11;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            CalcGradeData(part, grade, maxBreak, results);

            // 2部29
            grade = 29;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部30
            grade = 30;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            CalcGradeData(part, grade, maxBreak, results);

            // 2部31
            grade = 31;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部32
            // 1-49でひかりが必須だったため、ここまで11凸制限だった
            grade = 32;
            maxBreak = 13;
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 172).First()); // ひかり
            Masters.IncludeTarget(Masters.Employees.Where(emp => emp.Id == 174).First()); // キュンシー
            CalcGradeData(part, grade, maxBreak, results);

            // 結果をJsonに書き出し
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());
            string json = JsonSerializer.Serialize(results);
            File.WriteAllText("RequiredBreakData.json", json);

            // 終了時に一度停止
            Console.WriteLine("Hello World!");
            Console.ReadKey(true);
        }

        /// <summary>
        /// 指定したグレードの各ステージについて、解法の凸パターンを調べる
        /// </summary>
        /// <param name="part"></param>
        /// <param name="grade"></param>
        /// <param name="maxBreak"></param>
        /// <param name="results"></param>
        private static void CalcGradeData(int part, int grade, int maxBreak, List<RequiredBreakData> results)
        {
            var stages = Masters.StageDatas.Where(s => (s.Part == part) && (s.Grade == grade));
            foreach (var stage in stages)
            {
                RequiredBreakData? result = CalcBreakData(stage, maxBreak);
                if (result != null)
                {
                    results.Add(result);
                }
            }
        }

        /// <summary>
        /// 各ステージの解法の凸パターンを調べる
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="maxBreak"></param>
        /// <returns></returns>
        private static RequiredBreakData? CalcBreakData(StageData stage, int maxBreak)
        {
            Console.WriteLine($"{stage.Part}-{stage.Grade}-{stage.Stage} CalcStart");

            // 敵社員取得
            Employee? emp1 = Masters.GetEnemyEmployee(stage.Enemy1Id, stage.Enemy1EvolState);
            Battler? enemy1 = (emp1 == null) ? null : new(emp1) { Level = stage.Enemy1Level};
            Employee? emp2 = Masters.GetEnemyEmployee(stage.Enemy2Id, stage.Enemy3EvolState);
            Battler? enemy2 = (emp2 == null) ? null : new(emp2) { Level = stage.Enemy2Level };
            Employee? emp3 = Masters.GetEnemyEmployee(stage.Enemy3Id, stage.Enemy2EvolState);
            Battler? enemy3 = (emp3 == null) ? null : new(emp3) { Level = stage.Enemy3Level };

            // 99999存在チェック
            List<SearchResult> defResults = Sim.Search(enemy1, enemy2, enemy3, true, DefMax, stage.Part, stage.StageCondition, null);
            if (defResults.Count > 0)
            {
                return null;
            }

            // 本検索
            List<SearchResult> results = Sim.Search(enemy1, enemy2, enemy3, true, (OneBreakLevel * maxBreak) + DefMax, stage.Part, stage.StageCondition, null);
            
            RequiredBreakData breakData = new(stage);
            foreach (SearchResult result in results)
            {
                // 社員情報取得
                Battler? ally1 = result.Ally1 == null ? null : new(result.Ally1);
                Battler? ally2 = result.Ally2 == null ? null : new(result.Ally2);
                Battler? ally3 = result.Ally3 == null ? null : new(result.Ally3);

                for (int ally1Break = 0; ally1Break <= maxBreak; ally1Break++)
                {
                    for (int ally2Break = 0; ally2Break <= maxBreak; ally2Break++)
                    {
                        for (int ally3Break = 0; ally3Break <= maxBreak; ally3Break++)
                        {
                            // 社員情報取得
                            if (ally1 != null)
                            {
                                ally1.Level = (OneBreakLevel * ally1Break) + DefMax;
                            }
                            if (ally2 != null)
                            {
                                ally2.Level = (OneBreakLevel * ally2Break) + DefMax;
                            }
                            if (ally3 != null)
                            {
                                ally3.Level = (OneBreakLevel * ally3Break) + DefMax;
                            }

                            // 計算
                            BattleResult battleResult = Sim.FullBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, true, stage.StageCondition);

                            if (battleResult.WinRate > 0)
                            {
                                breakData.AddPattern(result.Ally1?.Id ?? 0, result.Ally2?.Id ?? 0, result.Ally3?.Id ?? 0,
                                    ally1Break, ally2Break, ally3Break);
                            }
                        }
                    }
                }    
            }

            Console.WriteLine($"{stage.Part}-{stage.Grade}-{stage.Stage} CalcEnd:{breakData.Pattrns.Count}");

            // 返却
            return breakData;
        }

        /// <summary>
        /// 除外設定をおすすめの状態に戻す
        /// </summary>
        private static void ResetTarget()
        {
            foreach (Employee emp in Masters.Employees)
            {
                if (emp.Rarity >= EmployeeRarity.LXR ||
                    emp.Id == 33 ||
                    emp.Id == 41 ||
                    emp.Id == 62 ||
                    emp.Id == 63 ||
                    emp.Id == 65 ||
                    emp.Id == 72)
                {
                    Masters.IncludeTarget(emp);
                }
                else
                {
                    Masters.ExcludeTarget(emp);
                }
            }
        }
    }
}