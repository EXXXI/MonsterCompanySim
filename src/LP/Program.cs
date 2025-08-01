using Google.OrTools.LinearSolver;
using LPModel;
using MonsterCompanySimModel.Models;
using MonsterCompanySimModel.Service;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LP
{
    internal class Program
    {
        /// <summary>
        /// 秘伝の書の使用可能数
        /// </summary>
        const int ScrollCount = 0;

        /// <summary>
        /// 最大凸数
        /// </summary>
        const int MaxBreak = 12;

        // 変数名プレフィックス
        const string EmpVarPrefix = "emp";
        const string ReqVarPrefix = "req";
        const string ScrollTierVarPrefix = "scrolltier";
        const string ScrollNormalVarPrefix = "scrollnormal";

        // 制約式名プレフィックス
        const string StageConPrefix = "stage";
        const string ReqConPrefix = "req";
        const string BreakConPrefix = "break";
        const string ScrollCountConPrefix = "scrollcount";
        const string ScrollTierCheckConPrefix = "scrolltire";
        const string ScrollNormalCheckConPrefix = "scrollnormal";

        /// <summary>
        /// シミュ本体(社員データ等読み込み用)
        /// </summary>
        private static readonly Simulator Sim = new();

        /// <summary>
        /// ソルバ
        /// </summary>
        public static Solver BreakSolver { get; set; }

        /// <summary>
        /// 変数の辞書
        /// </summary>
        public static Dictionary<string, Variable> Variables { get; set; } = new();

        /// <summary>
        /// 制約式の辞書
        /// </summary>
        public static Dictionary<string, Constraint> Constraints { get; set; } = new();

        static void Main(string[] args)
        {
            // シミュ起動
            Sim.LoadData();

            // json読み込み
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());
            string json = File.ReadAllText("RequiredBreakData.json");
            List<RequiredBreakData>? breakDatas = JsonSerializer.Deserialize<List<RequiredBreakData>>(json, options);
            if (breakDatas == null)
            {
                throw new FileFormatException("RequiredBreakData.json");
            }

            // ソルバ定義
            BreakSolver = Solver.CreateSolver("SCIP");

            // 変数・目的関数設定
            SetVariables(breakDatas);

            // 制約式・係数設定
            SetConstraints(breakDatas);

            // 計算
            BreakSolver.Solve();

            // 結果まとめ
            MakeResult(breakDatas);
        }

        /// <summary>
        /// 変数・目的関数設定
        /// </summary>
        private static void SetVariables(List<RequiredBreakData> breakDatas)
        {
            // 目的関数
            Objective objective = BreakSolver.Objective();

            // 社員凸情報
            foreach (var emp in Masters.Employees)
            {
                if (emp.EvolState != 0)
                {
                    // 進化の有無は考慮しないためパス
                    continue;
                }
                for (int breakCount = 1; breakCount <= MaxBreak; breakCount++)
                {
                    string key = $"{EmpVarPrefix}_{emp.Id}_{breakCount}";
                    Variables.Add(key, BreakSolver.MakeIntVar(0, 1, key));
                    objective.SetCoefficient(Variables[key], Score(emp, breakCount));
                }
            }

            // ステージ要求凸情報
            foreach (var breakData in breakDatas)
            {
                foreach (var pattern in breakData.Pattrns)
                {
                    string key = $"{ReqVarPrefix}_{breakData.StageStr}_{pattern.PatternStr}";
                    Variables.Add(key, BreakSolver.MakeIntVar(0, 1, key));
                }
            }

            // 秘伝の書
            for (int breakCount = 1; breakCount <= MaxBreak; breakCount++)
            {
                string normalKey = $"{ScrollNormalVarPrefix}_{breakCount}";
                Variables.Add(normalKey, BreakSolver.MakeIntVar(0, double.PositiveInfinity, normalKey));
                objective.SetCoefficient(Variables[normalKey], -ScoreNormal(breakCount));
                string tierKey = $"{ScrollTierVarPrefix}_{breakCount}";
                Variables.Add(tierKey, BreakSolver.MakeIntVar(0, double.PositiveInfinity, tierKey));
                objective.SetCoefficient(Variables[tierKey], -ScoreTire(breakCount));
            }

            // 目的は最小化
            objective.SetMinimization();
        }

        /// <summary>
        /// 必要チケット数
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="breakCount"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static double Score(Employee emp, int breakCount)
        {
            if (emp.Rarity < EmployeeRarity.Tier1)
            {
                return ScoreNormal(breakCount);
            }
            else
            {
                return ScoreTire(breakCount);
            }
        }

        /// <summary>
        /// Tier以外の社員の必要チケット計算
        /// </summary>
        /// <param name="breakCount"></param>
        /// <returns></returns>
        private static double ScoreNormal(int breakCount)
        {
            return breakCount switch
            {
                1 => 10,
                2 => 18,
                3 => 26,
                4 => 34,
                5 => 42,
                6 => 50,
                7 => 58,
                8 => 72,
                9 => 92,
                10 => 150,
                11 => 230,
                12 => 332,
                _ => 0
            };
        }

        /// <summary>
        /// Tierの社員の必要チケット計算
        /// </summary>
        /// <param name="breakCount"></param>
        /// <returns></returns>
        private static double ScoreTire(int breakCount)
        {
            return breakCount switch
            {
                1 => 14,
                2 => 25,
                3 => 36,
                4 => 47,
                5 => 58,
                6 => 70,
                7 => 81,
                8 => 100,
                9 => 128,
                10 => 210,
                11 => 322,
                12 => 464,
                _ => 0
            };
        }

        /// <summary>
        /// 制約式設定
        /// </summary>
        private static void SetConstraints(List<RequiredBreakData> breakDatas)
        {
            // 秘伝の書の数
            string scrollCountKey = $"{ScrollCountConPrefix}";
            Constraints.Add(scrollCountKey, BreakSolver.MakeConstraint(0, ScrollCount, scrollCountKey));

            for (int breakCount = 1; breakCount <= MaxBreak; breakCount++)
            {
                // 秘伝の書の数
                Constraints[scrollCountKey].SetCoefficient(Variables[$"{ScrollNormalVarPrefix}_{breakCount}"], 1);
                Constraints[scrollCountKey].SetCoefficient(Variables[$"{ScrollTierVarPrefix}_{breakCount}"], 1);

                // 存在しない凸に秘伝の書は使えない
                string tireCheckKey = $"{ScrollTierCheckConPrefix}_{breakCount}";
                string normalCheckKey = $"{ScrollNormalCheckConPrefix}_{breakCount}";
                Constraints.Add(tireCheckKey, BreakSolver.MakeConstraint(0, double.PositiveInfinity, tireCheckKey));
                Constraints.Add(normalCheckKey, BreakSolver.MakeConstraint(0, double.PositiveInfinity, normalCheckKey));
                Constraints[tireCheckKey].SetCoefficient(Variables[$"{ScrollTierVarPrefix}_{breakCount}"], -1);
                Constraints[normalCheckKey].SetCoefficient(Variables[$"{ScrollNormalVarPrefix}_{breakCount}"], -1);
            }


            // 凸は直前までの凸が終わっている場合のみ許可
            // 社員凸情報
            foreach (var emp in Masters.Employees)
            {
                if (emp.EvolState != 0)
                {
                    // 進化の有無は考慮しないためパス
                    continue;
                }
                for (int breakCount = 1; breakCount <= MaxBreak; breakCount++)
                {
                    // 存在しない凸に秘伝の書は使えない
                    if (emp.Rarity < EmployeeRarity.Tier1)
                    {
                        string normalCheckKey = $"{ScrollNormalCheckConPrefix}_{breakCount}";
                        Constraints[normalCheckKey].SetCoefficient(Variables[$"{EmpVarPrefix}_{emp.Id}_{breakCount}"], 1);
                    }
                    else
                    {
                        string tireCheckKey = $"{ScrollTierCheckConPrefix}_{breakCount}";
                        Constraints[tireCheckKey].SetCoefficient(Variables[$"{EmpVarPrefix}_{emp.Id}_{breakCount}"], 1);
                    }

                    // 凸はそれ以前の凸が終わっていないとできない
                    if (breakCount > 1)
                    {
                        string key = $"{BreakConPrefix}_{emp.Id}_{breakCount}";
                        Constraints.Add(key, BreakSolver.MakeConstraint(0, double.PositiveInfinity, key));
                        Constraints[key].SetCoefficient(Variables[$"{EmpVarPrefix}_{emp.Id}_{breakCount}"], -1);
                        Constraints[key].SetCoefficient(Variables[$"{EmpVarPrefix}_{emp.Id}_{breakCount - 1}"], 1);
                    }
                }
            }

            // 解法
            foreach (var breakData in breakDatas)
            {
                // 各ステージで1つ以上の解法がある
                string stageKey = $"{StageConPrefix}_{breakData.StageStr}";
                Constraints.Add(stageKey, BreakSolver.MakeConstraint(1, double.PositiveInfinity, stageKey));

                foreach (var pattern in breakData.Pattrns)
                {
                    // 各ステージで1つ以上の解法がある　解法の変数の係数を1に
                    Constraints[stageKey].SetCoefficient(Variables[$"{ReqVarPrefix}_{breakData.StageStr}_{pattern.PatternStr}"], 1);

                    // 各解法
                    string reqKey = $"{ReqConPrefix}_{breakData.StageStr}_{pattern.PatternStr}";
                    Constraints.Add(reqKey, BreakSolver.MakeConstraint(0, double.PositiveInfinity, reqKey));

                    int empCount = 0;
                    if ((pattern.Ally1Break != null) && (pattern.Ally1Break.Id != 0) && (pattern.Ally1Break.Break != 0))
                    {
                        empCount++;
                        Constraints[reqKey].SetCoefficient(Variables[$"{EmpVarPrefix}_{pattern.Ally1Break.Id}_{pattern.Ally1Break.Break}"], 1);
                    }
                    if ((pattern.Ally2Break != null) && (pattern.Ally2Break.Id != 0) && (pattern.Ally2Break.Break != 0))
                    {
                        empCount++;
                        Constraints[reqKey].SetCoefficient(Variables[$"{EmpVarPrefix}_{pattern.Ally2Break.Id}_{pattern.Ally2Break.Break}"], 1);
                    }
                    if ((pattern.Ally3Break != null) && (pattern.Ally3Break.Id != 0) && (pattern.Ally3Break.Break != 0))
                    {
                        empCount++;
                        Constraints[reqKey].SetCoefficient(Variables[$"{EmpVarPrefix}_{pattern.Ally3Break.Id}_{pattern.Ally3Break.Break}"], 1);
                    }
                    Constraints[reqKey].SetCoefficient(Variables[$"{ReqVarPrefix}_{breakData.StageStr}_{pattern.PatternStr}"], -empCount);
                }
            }
        }


        /// <summary>
        /// 結果まとめ
        /// </summary>
        private static void MakeResult(List<RequiredBreakData> breakDatas)
        {
            // 社員の凸情報
            foreach (var emp in Masters.Employees)
            {
                if (emp.EvolState != 0)
                {
                    // 進化の有無は考慮しないためパス
                    continue;
                }
                for (int breakCount = MaxBreak; breakCount > 0; breakCount--)
                {
                    Variable variable = Variables[$"{EmpVarPrefix}_{emp.Id}_{breakCount}"];
                    if (variable.SolutionValue() > 0)
                    {
                        Console.WriteLine($"{emp.Name}:{breakCount}");
                        break;
                    }
                }
            }

            // 要求チケット数
            Console.WriteLine($"要求チケ:{BreakSolver.Objective().Value()}");

            // ステージ要求凸情報
            foreach (var breakData in breakDatas)
            {
                foreach (var pattern in breakData.Pattrns)
                {
                    Variable variable = Variables[$"{ReqVarPrefix}_{breakData.StageStr}_{pattern.PatternStr}"];
                    if (variable.SolutionValue() > 0)
                    {
                        Console.Write(breakData.StageStr);
                        Console.Write(":");
                        if ((pattern.Ally1Break != null) && (pattern.Ally1Break.Id != 0))
                        {
                            string name = Masters.Employees.Where(emp => emp.Id == pattern.Ally1Break.Id).First().Name;
                            Console.Write($"{name}{pattern.Ally1Break.Break}");
                        }
                        if ((pattern.Ally2Break != null) && (pattern.Ally2Break.Id != 0))
                        {
                            string name = Masters.Employees.Where(emp => emp.Id == pattern.Ally2Break.Id).First().Name;
                            Console.Write($",{name}{pattern.Ally2Break.Break}");
                        }
                        if ((pattern.Ally3Break != null) && (pattern.Ally3Break.Id != 0))
                        {
                            string name = Masters.Employees.Where(emp => emp.Id == pattern.Ally3Break.Id).First().Name;
                            Console.Write($",{name}{pattern.Ally3Break.Break}");
                        }
                        Console.WriteLine();
                        break;
                    }
                }
            }

            // 秘伝の書使用箇所
            for (int breakCount = 1; breakCount <= MaxBreak; breakCount++)
            {
                Variable normal = Variables[$"{ScrollNormalVarPrefix}_{breakCount}"];
                Variable tier = Variables[$"{ScrollTierVarPrefix}_{breakCount}"];
                if (normal.SolutionValue() > 0)
                {
                    Console.WriteLine($"通常秘伝の書_{breakCount}凸:{normal.SolutionValue()}");
                }
                if (tier.SolutionValue() > 0)
                {
                    Console.WriteLine($"Tier秘伝の書_{breakCount}凸:{tier.SolutionValue()}");
                }
            }
        }
    }
}
