using MonsterCompanySimModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModel
{
    public class RequiredBreakData
    {
        public int Part { get; set; }
        public int Grade { get; set; }
        public int Stage { get; set; }

        public List<BreakPattrn> Pattrns { get; set; } = new();

        public RequiredBreakData()
        {
        }

        public RequiredBreakData(StageData stage)
        {
            Part = stage.Part;
            Grade = stage.Grade;
            Stage = stage.Stage;
        }

        public void AddPattern(int id1, int id2, int id3, int break1, int break2, int break3)
        {
            BreakPattrn newPattern = new BreakPattrn();
            newPattern.AddAlly(id1, break1);
            newPattern.AddAlly(id2, break2);
            newPattern.AddAlly(id3, break3);

            var noUse = Pattrns.Where(p => BreakPattrn.ComparePattern(p, newPattern) == 1).Any();
            if (noUse)
            {
                return;
            }

            List<BreakPattrn> deleteList = new();
            var toDelete = Pattrns.Where(p => BreakPattrn.ComparePattern(p, newPattern) == -1).ToList();
            foreach (var item in toDelete) 
            {
                Pattrns.Remove(item);
            }
            Pattrns.Add(newPattern);
        }

        public string StageStr { get => $"{Part}-{Grade}-{Stage}"; }
    }
}
