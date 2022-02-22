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
    static public class Masters
    {
        static public List<Employee> Employees { get; set; } = new List<Employee>();
        static public List<Employee> EnemyEmployees { get; set; } = new List<Employee>();
        static private List<SimpleEmployee> IncludeEmployees { get; set; } = new List<SimpleEmployee>();

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

        static public void SaveIncludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(IncludeEmployees);
            File.WriteAllText("data/Includes.json",json);
        }

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

        // Debug用
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
