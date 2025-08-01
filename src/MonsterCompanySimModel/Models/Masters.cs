﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// マスタデータ管理クラス
    /// </summary>
    static public class Masters
    {
        /// <summary>
        /// 社員リスト
        /// </summary>
        static public List<Employee> Employees { get; set; } = new List<Employee>();

        /// <summary>
        /// 社員リスト(敵用)
        /// </summary>
        static public List<Employee> EnemyEmployees { get; set; } = new List<Employee>();

        /// <summary>
        /// 検索除外対象社員リスト(ファイル入出力用)
        /// </summary>
        static private List<SimpleEmployee> ExcludeEmployees { get; set; } = new List<SimpleEmployee>();

        /// <summary>
        /// 固定対象社員リスト(内部管理用、保存はしない)
        /// </summary>
        static private List<SimpleEmployee> RequiredEmployees { get; set; } = new List<SimpleEmployee>();

        /// <summary>
        /// ステージ条件リスト
        /// </summary>
        static public List<StageCondition> StageConditions { get; set; } = new List<StageCondition>();

        /// <summary>
        /// ステージリスト
        /// </summary>
        static public List<StageData> StageDatas { get; set; } = new List<StageData>();

        /// <summary>
        /// 設定ファイルデータ
        /// </summary>
        static public Config ConfigData { get; set; } = new Config();

        /// <summary>
        /// 検索対象社員リスト
        /// </summary>
        static public List<Employee> SearchTargets 
        {
            get
            {
                List<Employee> searchTargets = new();
                foreach (var emp in Employees)
                {
                    if (IsTarget(emp))
                    {
                        searchTargets.Add(emp);
                    }
                }
                return searchTargets;
            }
        }

        /// <summary>
        /// 各種データ読み込み
        /// </summary>
        static public void LoadDatas()
        {
            LoadEmployee();
            LoadExcludeEmployees();
            LoadEnemyEmployee();
            LoadStageCondition();
            LoadStageDatas();
            LoadConfig();
        }

        /// <summary>
        /// 社員情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadEmployee()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Employees.json");
            List<Employee>? employees = JsonSerializer.Deserialize<List<Employee>>(json, options);
            if (employees == null)
            {
                throw new FileFormatException("data/Employees.json");
            }
            Employees = employees;
        }

        /// <summary>
        /// 敵社員情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadEnemyEmployee()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/EnemyEmployees.json");
            List<Employee>? diffEmployees = JsonSerializer.Deserialize<List<Employee>>(json, options);
            if (diffEmployees == null)
            {
                throw new FileFormatException("data/EnemyEmployees.json");
            }
            List<Employee> enemyEmployees = new();
            foreach (var emp in Employees)
            {
                bool hasDiff = false;
                foreach (var diffEmp in diffEmployees)
                {
                    if (emp.Id == diffEmp.Id && emp.EvolState == diffEmp.EvolState)
                    {
                        hasDiff = true;
                        enemyEmployees.Add(diffEmp);
                        break;
                    }
                }
                if (!hasDiff)
                {
                    enemyEmployees.Add(emp);
                }
            }
            EnemyEmployees = enemyEmployees;
        }

        /// <summary>
        /// 検索除外対象社員情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadExcludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());
            if (File.Exists("data/Excludes.json"))
            {
                string json = File.ReadAllText("data/Excludes.json");
                List<SimpleEmployee>? excludeEmployees = JsonSerializer.Deserialize<List<SimpleEmployee>>(json, options);
                if (excludeEmployees == null)
                {
                    throw new FileFormatException("data/Excludes.json");
                }
                ExcludeEmployees = excludeEmployees;
            }
            else
            {
                // 旧Include.jsonしか存在しない場合はエラー落ちさせないように固定のオススメリストを初期状態として作成
                int[] includes = new[] { 33, 41, 62, 63, 65 };
                List<SimpleEmployee> excludeEmployees = new();
                for (int i = 1; i < 72; i++)
                {
                    if (!includes.Contains(i))
                    {
                        excludeEmployees.Add(new SimpleEmployee() { Id = i, EvolState = 0 });
                    }
                }
                ExcludeEmployees = excludeEmployees;
            }
        }

        /// <summary>
        /// 検索対象社員情報保存
        /// </summary>
        static public void SaveExcludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(ExcludeEmployees);
            File.WriteAllText("data/Excludes.json",json);
        }

        /// <summary>
        /// 検索対象社員追加
        /// </summary>
        /// <param name="emp">追加社員</param>
        static public void IncludeTarget(Employee emp)
        {
            foreach (var target in ExcludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    ExcludeEmployees.Remove(target);
                    SaveExcludeEmployees();
                    return;
                }
            }
        }

        /// <summary>
        /// 検索対象社員情報削除
        /// </summary>
        /// <param name="emp">削除社員</param>
        static public void ExcludeTarget(Employee emp)
        {
            foreach (var target in ExcludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return;
                }
            }
            ExcludeEmployees.Add(new SimpleEmployee() { Id = emp.Id, EvolState = emp.EvolState });
            SaveExcludeEmployees();
        }

        /// <summary>
        /// 検索対象か否かを取得
        /// </summary>
        /// <param name="emp">社員</param>
        /// <returns>検索対象の場合true</returns>
        public static bool IsTarget(Employee emp)
        {
            foreach (var target in ExcludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 固定対象社員追加
        /// </summary>
        /// <param name="emp">追加社員</param>
        static public void AddRequired(Employee emp)
        {
            foreach (var target in RequiredEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return;
                }
            }
            RequiredEmployees.Add(new SimpleEmployee() { Id = emp.Id, EvolState = emp.EvolState });
        }

        /// <summary>
        /// 固定対象社員情報削除
        /// </summary>
        /// <param name="emp">削除社員</param>
        static public void DeleteRequired(Employee emp)
        {
            foreach (var target in RequiredEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {

                    RequiredEmployees.Remove(target);
                    return;
                }
            }
        }

        /// <summary>
        /// 固定検索対象か否かを取得
        /// </summary>
        /// <param name="emp">社員</param>
        /// <returns>固定検索対象の場合true</returns>
        public static bool IsRequired(Employee emp)
        {
            foreach (var target in RequiredEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 編成が固定条件を満たしているか否かを取得
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <returns>検索対象の場合true</returns>
        public static bool IsRequierdValid(Employee? ally1, Employee? ally2, Employee? ally3)
        {
            foreach (var emp in RequiredEmployees)
            {
                if (ally1?.Id == emp.Id && ally1?.EvolState == emp.EvolState)
                {
                    continue;
                }
                if (ally2?.Id == emp.Id && ally2?.EvolState == emp.EvolState)
                {
                    continue;
                }
                if (ally3?.Id == emp.Id && ally3?.EvolState == emp.EvolState)
                {
                    continue;
                }
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 設定ファイルの情報を取得
        /// (ステージデータを先に読み込むこと)
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadConfig()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Config.json");
            Config? config = JsonSerializer.Deserialize<Config>(json, options);
            if (config == null)
            {
                throw new FileFormatException("data/Config.json");
            }
            // 最大レベルが指定されていない場合、1部の最大グレードから算出
            if (config.MaxLevel == 0)
            {
                config.MaxLevel = (StageDatas.Where(s => s.Part == 1).Max(s => s.Grade) - 27) * 10000 - 1;
            }

            ConfigData = config;
        }

        /// <summary>
        /// ステージ特殊条件取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadStageCondition()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/StageCondition.json");
            List<StageCondition>? stageConditions = JsonSerializer.Deserialize<List<StageCondition>>(json, options);
            if (stageConditions == null)
            {
                throw new FileFormatException("data/StageCondition.json");
            }
            StageConditions = stageConditions;
        }

        /// <summary>
        /// ステージ情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static private void LoadStageDatas()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Stages.json");
            List<StageData>? stages = JsonSerializer.Deserialize<List<StageData>>(json, options);
            if (stages == null)
            {
                throw new FileFormatException("data/Stages.json");
            }
            StageDatas = stages;
        }

        /// <summary>
        /// 社員リスト(敵用)から社員を取得
        /// </summary>
        /// <param name="id">社員番号</param>
        /// <param name="evol">進化状況</param>
        /// <returns>社員</returns>
        static public Employee? GetEnemyEmployee(int id, int evol)
        {
            foreach (var emp in EnemyEmployees)
            {
                if (emp.Id == id && emp.EvolState == evol)
                {
                    return emp;
                }
            }
            return null;
        }

        /// <summary>
        /// 開発用：社員リストから社員を取得
        /// </summary>
        /// <param name="id">社員番号</param>
        /// <returns>社員</returns>
        static public Employee? GetEmployee(int id)
        {
            foreach (var emp in Employees)
            {
                if (emp.Id == id)
                {
                    return emp;
                }
            }
            return null;
        }

        /// <summary>
        /// 開発用：社員リストから社員を取得
        /// </summary>
        /// <param name="id">社員番号</param>
        /// <param name="evol">進化状況</param>
        /// <returns>社員</returns>
        static public Employee? GetEmployee(int id, int evol)
        {
            foreach (var emp in Employees)
            {
                if (emp.Id == id && emp.EvolState == evol)
                {
                    return emp;
                }
            }
            return null;
        }
    }
}
