using System.Text.Json.Serialization;
using System.Text.Json;
using LPModel;
using MonsterCompanySimModel.Service;
using MonsterCompanySimModel.Models;

namespace MakeBreakDataSummary
{
    internal class Program
    {
        /// <summary>
        /// シミュ本体(社員データ等読み込み用)
        /// </summary>
        private static readonly Simulator Sim = new();

        static void Main(string[] args)
        {
            // データ読み込み
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

            foreach (RequiredBreakData breakData in breakDatas)
            {

                int reqBreak = int.MaxValue;
                foreach (var pattern in breakData.Pattrns)
                {
                    int max = 0;
                    max = Math.Max(max, pattern.Ally1Break?.Break ?? 0);
                    max = Math.Max(max, pattern.Ally2Break?.Break ?? 0);
                    max = Math.Max(max, pattern.Ally3Break?.Break ?? 0);
                    reqBreak = Math.Min(reqBreak, max);
                }

                int id1 = breakData.Pattrns[0].Ally1Break?.Id ?? 0;
                int id2 = breakData.Pattrns[0].Ally2Break?.Id ?? 0;
                int id3 = breakData.Pattrns[0].Ally3Break?.Id ?? 0;
                bool id1req = id1 != 0;
                bool id2req = id2 != 0;
                bool id3req = id3 != 0;
                foreach (var pattern in breakData.Pattrns)
                {
                    if (id1req &&
                        (pattern.Ally1Break?.Id ?? 0) != id1 &&
                        (pattern.Ally2Break?.Id ?? 0) != id1 &&
                        (pattern.Ally3Break?.Id ?? 0) != id1)
                    {
                        id1req = false;
                    }
                    if (id2req &&
                        (pattern.Ally1Break?.Id ?? 0) != id2 &&
                        (pattern.Ally2Break?.Id ?? 0) != id2 &&
                        (pattern.Ally3Break?.Id ?? 0) != id2)
                    {
                        id2req = false;
                    }
                    if (id3req &&
                        (pattern.Ally1Break?.Id ?? 0) != id3 &&
                        (pattern.Ally2Break?.Id ?? 0) != id3 &&
                        (pattern.Ally3Break?.Id ?? 0) != id3)
                    {
                        id3req = false;
                    }
                }

                string reqEmpStr = "None";
                if (id1req || id2req || id3req)
                {
                    reqEmpStr = "";
                    if (id1req)
                    {
                        var emp = Masters.GetEmployee(id1);
                        reqEmpStr += emp?.Name ?? "Error";
                    }
                    if (id2req)
                    {
                        var emp = Masters.GetEmployee(id2);
                        reqEmpStr += emp?.Name ?? "Error";
                    }
                    if (id3req)
                    {
                        var emp = Masters.GetEmployee(id3);
                        reqEmpStr += emp?.Name ?? "Error";
                    }
                }


                Console.WriteLine($"{breakData.StageStr} reqBreak:{reqBreak} reqEmp:{reqEmpStr}");

            }
            Console.WriteLine("Hello,world!");
        }
    }
}
