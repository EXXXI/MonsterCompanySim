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

        public static List<Employee> UpperLREmployees
        {
            get
            {
                return new List<Employee>(Employees.Where(o => o.Rarity >= EmployeeRarity.LR));
            }
        }

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
