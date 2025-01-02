using MonsterCompanySimModel.Models;
using MonsterCompanySimModel.Service;

namespace SpecialSearch
{
    /// <summary>
    /// 解法をリストアップしてJsonとして保存
    /// </summary>
    class Program
    {
        /// <summary>
        /// 最大レベル
        /// </summary>
        const int MaxLevel = 209999;

        /// <summary>
        /// 最大話数
        /// </summary>
        const int MaxGrade = 31;

        /// <summary>
        /// 最小話数
        /// </summary>
        const int MinGrade = 20;

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

            int part;
            bool isBoost;
            Employee core = Masters.Employees.Where(emp => emp.Id == 168).First();
            Employee thief = Masters.Employees.Where(emp => emp.Id == 133).First();

            // アルカコア
            part = 2;
            isBoost = false;
            Masters.AddRequired(core);
            Console.WriteLine("アルカコア");
            for (int grade = MaxGrade; grade >= MinGrade; grade--)
            {
                for (int stage = 10; stage > 0; stage--)
                {
                    CheckStage(part, grade, stage, isBoost);
                }
            }
            Masters.DeleteRequired(core);

            // 盗人ブーストあり
            part = 2;
            isBoost = true;
            Masters.AddRequired(thief);
            Console.WriteLine("盗人ブーストあり");
            for (int grade = MaxGrade; grade >= MinGrade; grade--)
            {
                for (int stage = 10; stage > 0; stage--)
                {
                    CheckStage(part, grade, stage, isBoost);
                }
            }
            Masters.DeleteRequired(thief);

            // 盗人ブーストなし
            part = 2;
            isBoost = false;
            Masters.AddRequired(thief);
            Console.WriteLine("盗人ブーストなし");
            for (int grade = MaxGrade; grade >= MinGrade; grade--)
            {
                for (int stage = 10; stage > 0; stage--)
                {
                    CheckStage(part, grade, stage, isBoost);
                }
            }
            Masters.DeleteRequired(thief);

            // 終了時に一度停止
            Console.WriteLine("Hello World!");
            Console.ReadKey(true);
        }

        /// <summary>
        /// 特定のステージがクリア可能か調べる
        /// </summary>
        /// <param name="part">部</param>
        /// <param name="grade">グレード</param>
        /// <param name="stage">ステージ</param>
        /// <param name="isBoost">ブースト有無</param>
        private static void CheckStage(int part, int grade, int stage, bool isBoost)
        {
            var stageData = Masters.StageDatas.Where(s => (s.Part == part) && (s.Grade == grade) && (s.Stage == stage)).First();
            // 敵社員取得
            Employee? emp1 = Masters.GetEnemyEmployee(stageData.Enemy1Id, stageData.Enemy1EvolState);
            Battler? enemy1 = (emp1 == null) ? null : new(emp1) { Level = stageData.Enemy1Level };
            Employee? emp2 = Masters.GetEnemyEmployee(stageData.Enemy2Id, stageData.Enemy3EvolState);
            Battler? enemy2 = (emp2 == null) ? null : new(emp2) { Level = stageData.Enemy2Level };
            Employee? emp3 = Masters.GetEnemyEmployee(stageData.Enemy3Id, stageData.Enemy2EvolState);
            Battler? enemy3 = (emp3 == null) ? null : new(emp3) { Level = stageData.Enemy3Level };

            // 存在チェック
            List<SearchResult> defResults = Sim.Search(enemy1, enemy2, enemy3, isBoost, MaxLevel, part, stageData.StageCondition, null);
            if (defResults.Count > 0)
            {
                Console.WriteLine($"{part}-{grade}-{stage}");
            }
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
