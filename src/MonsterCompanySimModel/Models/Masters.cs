using System;
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
        /// 検索対象社員リスト(ファイル入出力用)
        /// </summary>
        static private List<SimpleEmployee> IncludeEmployees { get; set; } = new List<SimpleEmployee>();

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

        // TODO: 呼び出す順番に決まりがあるからここはprivateにして全部ロードするメソッドを公開するべき
        /// <summary>
        /// 社員情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static public void LoadEmployee()
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
        static public void LoadEnemyEmployee()
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
        /// 検索対象社員情報取得
        /// </summary>
        /// <exception cref="FileFormatException"></exception>
        static public void LoadIncludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Includes.json");
            List<SimpleEmployee>? includeEmployees = JsonSerializer.Deserialize<List<SimpleEmployee>>(json, options);
            if (includeEmployees == null)
            {
                throw new FileFormatException("data/Includes.json");
            }
            IncludeEmployees = includeEmployees;
        }

        /// <summary>
        /// 検索対象社員情報保存
        /// </summary>
        static public void SaveIncludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(IncludeEmployees);
            File.WriteAllText("data/Includes.json",json);
        }

        /// <summary>
        /// 検索対象社員追加
        /// </summary>
        /// <param name="emp">追加社員</param>
        static public void AddTarget(Employee emp)
        {
            foreach (var target in IncludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return;
                }
            }
            IncludeEmployees.Add(new SimpleEmployee() { Id = emp.Id, EvolState = emp.EvolState });
            SaveIncludeEmployees();
        }

        /// <summary>
        /// 検索対象社員情報削除
        /// </summary>
        /// <param name="emp">削除社員</param>
        static public void DeleteTarget(Employee emp)
        {
            foreach (var target in IncludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {

                    IncludeEmployees.Remove(target);
                    SaveIncludeEmployees();
                    return;
                }
            }
        }


        /// <summary>
        /// 検索対象か否かを取得
        /// </summary>
        /// <param name="emp">社員</param>
        /// <returns>検索対象の場合true</returns>
        public static bool IsTarget(Employee emp)
        {
            foreach (var target in IncludeEmployees)
            {
                if (emp.Id == target.Id && emp.EvolState == target.EvolState)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 開発用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// 開発用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="evol"></param>
        /// <returns></returns>
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
