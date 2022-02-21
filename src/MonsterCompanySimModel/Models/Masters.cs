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



        static private List<SimpleEmployee> IncludeEmployees { get; set; } = new List<SimpleEmployee>();

        static public void LoadEmployee()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Employees.json");
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
            Employees = JsonSerializer.Deserialize<List<Employee>>(json, options);
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
        }

        static public void LoadIncludeEmployees()
        {
            JsonSerializerOptions options = new();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            options.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("data/Includes.json");
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
            IncludeEmployees = JsonSerializer.Deserialize<List<SimpleEmployee>>(json, options);
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
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
