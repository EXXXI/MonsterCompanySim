using LPModel;
using MonsterCompanySimModel.Models;
using MonsterCompanySimModel.Service;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PreLP
{
    class Program
    {
        const int DefMax = 99999;
        const int OneBreakLevel = 10000;

        /// <summary>
        /// シミュ本体
        /// </summary>
        private static readonly Simulator Sim = new();

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
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
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
            grade = 46;
            maxBreak = 8;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            CalcGradeData(part, grade, maxBreak, results);

            // 1部47
            grade = 47;
            maxBreak = 9;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            CalcGradeData(part, grade, maxBreak, results);

            // 1部48
            grade = 48;
            maxBreak = 10;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            CalcGradeData(part, grade, maxBreak, results);

            // 2部23
            part = 2;
            grade = 23;
            maxBreak = 8;
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.DeleteTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            CalcGradeData(part, grade, maxBreak, results);

            // 2部24
            grade = 24;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 146).First()); // うさこ
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 145).First()); // ころね
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 150).First()); // 九龍
            CalcGradeData(part, grade, maxBreak, results);

            // 2部25
            grade = 25;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部26
            grade = 26;
            maxBreak = 9;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 158).First()); // 覚醒奈々
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 159).First()); // 覚醒岬
            CalcGradeData(part, grade, maxBreak, results);

            // 2部27
            grade = 27;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部28
            grade = 28;
            maxBreak = 11;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 160).First()); // リピド
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 161).First()); // タナトス
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 162).First()); // こあCh
            CalcGradeData(part, grade, maxBreak, results);

            // 2部29
            grade = 29;
            CalcGradeData(part, grade, maxBreak, results);

            // 2部30
            grade = 30;
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 163).First()); // なの
            Masters.AddTarget(Masters.Employees.Where(emp => emp.Id == 167).First()); // シルフィ
            CalcGradeData(part, grade, maxBreak, results);

            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(results);
            File.WriteAllText("RequiredBreakData.json", json);


            Console.WriteLine("Hello World!");
            Console.ReadKey(true);
        }

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
                    Masters.AddTarget(emp);
                }
                else
                {
                    Masters.DeleteTarget(emp);
                }
            }
        }
    }
}