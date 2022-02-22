using MonsterCompanySimModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Service
{
    public class Simulator
    {
        public void Debug()
        {
            /*
            LoadData();

            var another = new Battler(Masters.GetEmployee(72, 2));
            another.Level = 109999;
            var little = new Battler(Masters.GetEmployee(105));
            little.Level = 99999;
            var nowork = new Battler(Masters.GetEmployee(63));
            nowork.Level = 79999;
            var other1 = new Battler(Masters.GetEmployee(66));
            other1.Level = 4838700;
            var other2 = new Battler(Masters.GetEmployee(66));
            other2.Level = 4838700;
            var other3 = new Battler(Masters.GetEmployee(66));
            other3.Level = 4838700;
            var black = new Battler(Masters.Employees[0]);
            black.Level = 1;

            var aaa = Battle(
                null, another, null,
                black, null,null,
                3,1, 2,2, 1, 1,true);
            var bbb = Battle(
                another, little, nowork,
                other1, other2, other3,
                3, 1, 2, 1, 1, 1, true);

            var ccc = FullBattle(
                another, little, nowork,
                other1, other2, other3, true);

            var ddd = CalcRequireLevel(
                another, little, nowork,
                other1, other2, other3, true);

            int a = 4;
            */
            /*
            var en1 = new Battler(Masters.GetEmployee(99));
            en1.Level = 6775535;
            var en2 = new Battler(Masters.GetEmployee(103));
            en2.Level = 4211815;
            Battler en3 = null;
            var al1 = new Battler(Masters.GetEmployee(81));
            al1.Level = 109999;
            var al2 = new Battler(Masters.GetEmployee(87));
            al2.Level = 109999;
            Battler al3 = new Battler(Masters.GetEmployee(99));
            al3.Level = 109999;

            var bbb = Battle(
                al1, al2, al3,
                en1, en2, en3,
                2, 2, 2, 1, 2, 1, true);

            int a = 4;
            */
            /*
            var white1 = new Battler(Masters.GetEmployee(48));
            white1.Level = 30912896;
            var white2 = new Battler(Masters.GetEmployee(48));
            white2.Level = 30912896;
            Search(white1, white2, null, true, 119999, 1);
            */
        }

        public void LoadData()
        {
            Masters.LoadEmployee();
            Masters.LoadIncludeEmployees();
            Masters.LoadEnemyEmployee();
        }

        // 重い
        public List<SearchResult> Search(Battler? enemy1, Battler? enemy2, Battler? enemy3, bool boost, int level,int part)
        {
            List<SearchResult> resultList = new();


            // 社員無し入り検索対象リスト
            List<Employee?> emps = new(Masters.SearchTargets) { null };

            foreach (var ally1 in emps)
            {
                foreach (var ally2 in emps)
                {
                    foreach (var ally3 in emps)
                    {
                        if ((ally1 != null && ally1?.Id == ally2?.Id) ||
                            (ally2 != null && ally2?.Id == ally3?.Id) ||
                            (ally3 != null && ally3?.Id == ally1?.Id) ||
                            (ally1 == null && ally2 == null && ally3 == null))
                        {
                            continue;
                        }

                        bool otherPart = false;
                        if (ally1 != null && ally1.Part != part)
                        {
                            otherPart = true;
                        }
                        if (ally2 != null && ally2.Part != part)
                        {
                            if (otherPart)
                            {
                                continue;
                            }
                            otherPart = true;
                        }
                        if (ally3 != null && ally3.Part != part)
                        {
                            if (otherPart)
                            {
                                continue;
                            }
                        }

                        Battler? battler1 = ally1 == null ? null : new Battler(ally1) { Level = level };
                        Battler? battler2 = ally2 == null ? null : new Battler(ally2) { Level = level };
                        Battler? battler3 = ally3 == null ? null : new Battler(ally3) { Level = level };


                        BattleResult battleResult = FullBattle(battler1, battler2, battler3, enemy1, enemy2, enemy3, boost);

                        if (battleResult.WinRate > 0)
                        {
                            SearchResult result = new()
                            {
                                Ally1 = ally1,
                                Ally2 = ally2,
                                Ally3 = ally3,
                                //MinLevel = CalcRequireLevel(battler1, battler2, battler3, enemy1, enemy2, enemy3, boost) ?? 0,
                                WinRate = battleResult.WinRate,
                                SumEng = (battler1?.Eng ?? 0) + (battler2?.Eng ?? 0) + (battler3?.Eng ?? 0)
                            };

                            resultList.Add(result);
                        }
                    }
                }
            }

            // 要求レベル計算
            // TODO:閾値定数化
            if (resultList.Count < 200)
            {
                foreach (var result in resultList)
                {
                    Battler? battler1 = result.Ally1 == null ? null : new Battler(result.Ally1) { Level = level };
                    Battler? battler2 = result.Ally2 == null ? null : new Battler(result.Ally2) { Level = level };
                    Battler? battler3 = result.Ally3 == null ? null : new Battler(result.Ally3) { Level = level };
                    result.MinLevel = CalcRequireLevel(battler1, battler2, battler3, enemy1, enemy2, enemy3, boost) ?? 0;
                }
            }

            return resultList;
        }






        public int? CalcRequireLevel(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            bool boost)
        {
            // TODO:定数化
            int max = 139999;
            int min = 1;

            bool won = false;
            while (max != min)
            {
                int level = (max + min) / 2;
                if (ally1 != null)
                {
                    ally1.Level = level;
                }
                if (ally2 != null)
                {
                    ally2.Level = level;
                }
                if (ally3 != null)
                {
                    ally3.Level = level;
                }
                BattleResult battleResult = FullBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, boost);
                if (battleResult.WinRate > 0)
                {
                    won = true;
                    max = level;
                }
                else
                {
                    min = level + 1;
                }
            }

            if (won)
            {
                return max;
            }
            return null;
        }




        public BattleResult FullBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            bool boost
            )
        {
            List<Battler?> allys = new() { ally1, ally2, ally3 };
            List<Battler?> enemys = new() { enemy1, enemy2, enemy3 };

            // リセット
            foreach (var ally in allys)
            {
                if (ally != null)
                {
                    ally.FixedTarget = 0;
                }
            }
            foreach (var enemy in enemys)
            {
                if (enemy != null)
                {
                    enemy.FixedTarget = 0;
                }
            }

            // 正面タゲ計算
            CalcFrontSkill(allys, enemys);
            CalcFrontSkill(enemys, allys);

            // 全引き付け計算
            CalcDecoySkill(allys, enemys);
            CalcDecoySkill(enemys, allys);

            // 戦闘
            List<Damage> allyDamages = new();
            int allyTargetCount = 0;
            foreach (var a1 in CalcTargets(ally1, enemys))
            {
                foreach (var a2 in CalcTargets(ally2, enemys))
                {
                    foreach (var a3 in CalcTargets(ally3, enemys))
                    {
                        allyDamages.AddRange(AllyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, a1, a2, a3, boost));
                        allyTargetCount++;
                    }
                }
            }
            foreach (var damage in allyDamages)
            {
                damage.Probability /= allyTargetCount; 
            }


            List<Damage> enemyDamages = new();
            int enemyTargetCount = 0;
            foreach (var e1 in CalcTargets(enemy1, allys))
            {
                foreach (var e2 in CalcTargets(enemy2, allys))
                {
                    foreach (var e3 in CalcTargets(enemy3, allys))
                    {
                        enemyDamages.AddRange(EnemyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, e1, e2, e3, boost));
                        enemyTargetCount++;
                    }
                }
            }
            foreach (var damage in enemyDamages)
            {
                damage.Probability /= enemyTargetCount;
            }

            BattleResult result = new();
            result.AllyDamages = allyDamages;
            result.EnemyDamages = enemyDamages;

            return result;

        }

        public BattleResult Battle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int allyTarget1, int allyTarget2, int allyTarget3,
            int enemyTarget1, int enemyTarget2, int enemyTarget3,
            bool boost
            )
        {
            BattleResult result = new();
            result.AllyDamages = AllyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, allyTarget1, allyTarget2, allyTarget3, boost);
            result.EnemyDamages = EnemyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, enemyTarget1, enemyTarget2, enemyTarget3, boost);

            return result;

        }

        private List<Damage> EnemyBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int enemyTarget1, int enemyTarget2, int enemyTarget3,
            bool boost
            )
        {
            ally1?.ResetAttackProperty(boost);
            ally2?.ResetAttackProperty(boost);
            ally3?.ResetAttackProperty(boost);
            enemy1?.ResetAttackProperty();
            enemy2?.ResetAttackProperty();
            enemy3?.ResetAttackProperty();

            BattleResult result = new();

            CalcNormalSkills(ally1, ally2, ally3);
            CalcNormalSkills(ally2, ally3, ally1);
            CalcNormalSkills(ally3, ally1, ally2);
            CalcNormalSkills(enemy1, enemy2, enemy3);
            CalcNormalSkills(enemy2, enemy3, enemy1);
            CalcNormalSkills(enemy3, enemy1, enemy2);

            result.CombineEnemyDamages(Attack(enemy1, TargetBattler(enemyTarget1, ally1, ally2, ally3)),
                enemy1?.Employee?.Name + "→" + TargetBattler(enemyTarget1, ally1, ally2, ally3)?.Employee?.Name);
            result.CombineEnemyDamages(Attack(enemy2, TargetBattler(enemyTarget2, ally1, ally2, ally3)),
                enemy2?.Employee?.Name + "→" + TargetBattler(enemyTarget2, ally1, ally2, ally3)?.Employee?.Name);
            result.CombineEnemyDamages(Attack(enemy3, TargetBattler(enemyTarget3, ally1, ally2, ally3)),
                enemy3?.Employee?.Name + "→" + TargetBattler(enemyTarget3, ally1, ally2, ally3)?.Employee?.Name);

            return result.EnemyDamages;

        }

        private List<Damage> AllyBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int allyTarget1, int allyTarget2, int allyTarget3,
            bool boost
            )
        {
            ally1?.ResetAttackProperty(boost);
            ally2?.ResetAttackProperty(boost);
            ally3?.ResetAttackProperty(boost);
            enemy1?.ResetAttackProperty();
            enemy2?.ResetAttackProperty();
            enemy3?.ResetAttackProperty();

            BattleResult result = new();

            CalcNormalSkills(ally1, ally2, ally3);
            CalcNormalSkills(ally2, ally3, ally1);
            CalcNormalSkills(ally3, ally1, ally2);
            CalcNormalSkills(enemy1, enemy2, enemy3);
            CalcNormalSkills(enemy2, enemy3, enemy1);
            CalcNormalSkills(enemy3, enemy1, enemy2);

            result.CombineAllyDamages(Attack(ally1, TargetBattler(allyTarget1, enemy1, enemy2, enemy3)),
                ally1?.Employee?.Name + "→" + TargetBattler(allyTarget1, enemy1, enemy2, enemy3)?.Employee?.Name);
            result.CombineAllyDamages(Attack(ally2, TargetBattler(allyTarget2, enemy1, enemy2, enemy3)),
                ally2?.Employee?.Name + "→" + TargetBattler(allyTarget2, enemy1, enemy2, enemy3)?.Employee?.Name);
            result.CombineAllyDamages(Attack(ally3, TargetBattler(allyTarget3, enemy1, enemy2, enemy3)),
                ally3?.Employee?.Name + "→" + TargetBattler(allyTarget3, enemy1, enemy2, enemy3)?.Employee?.Name);

            return result.AllyDamages;

        }

        private List<Damage> Attack(Battler? attacker, Battler? defender)
        {
            if (attacker == null)
            {
                List<Damage> noList = new();
                noList.Add(new Damage());
                return noList;
            }
            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }

            attacker.OnceDefCritState = CriticalState.normal;
            attacker.OnceAtkCritState = CriticalState.normal;
            defender.OnceDefCritState = CriticalState.normal;
            defender.OnceAtkCritState = CriticalState.normal;

            CalcAttackSkills(attacker, defender);
            CalcDefenceSkills(attacker, defender);
            CalcElement(attacker, defender);

            double crit = CalcCritical(attacker, defender);
            double damageValue = CalcDamage(attacker);

            List<Damage> damages = new();
            damages.Add(new Damage(crit, damageValue * 1.5));
            damages.Add(new Damage(1 - crit, damageValue));

            return damages;

        }



        private List<int> CalcTargets(Battler? battler, List<Battler?> opponents)
        {

            List<int> list = new();
            if (battler == null)
            {
                // nullは0を返却
                // 値は利用されないのでなんでもいいが1項目で返す必要がある
                list.Add(0);
                return list;
            }
            if (battler.FixedTarget != 0)
            {
                // 固定時はそれだけ返却
                list.Add(battler.FixedTarget);
                return list;
            }

            for (int i = 0; i < opponents.Count; i++)
            {
                if (opponents[i] != null)
                {
                    list.Add(i + 1);
                }
            }
            return list;
        }

        private void CalcDecoySkill(List<Battler?> we, List<Battler?> opponents)
        {
            for (int i = 0; i < we.Count; i++)
            {
                Battler? skillOwner = we[i];
                if (skillOwner?.Employee.HasSkill(SkillType.全員引き付け) != null && !skillOwner.IsSkillDisabled)
                {
                    foreach (var opponent in opponents)
                    {
                        if (opponent != null)
                        {
                            opponent.FixedTarget = i + 1;
                        }
                    }
                }
            }
        }

        private void CalcFrontSkill(List<Battler?> we, List<Battler?> opponents)
        {
            for (int i = 0; i < we.Count; i++)
            {
                Battler? skillOwner = we[i];
                if (skillOwner?.Employee.HasSkill(SkillType.正面攻撃) != null && !skillOwner.IsSkillDisabled)
                {
                    skillOwner.FixedTarget = Front(i, opponents) + 1;
                }
                if (skillOwner?.Employee.HasSkill(SkillType.正面引き付け) != null && !skillOwner.IsSkillDisabled)
                {
                    Battler? opponent = opponents[Front(i, opponents)];
                    if (opponent == null)
                    {
                        // ここで発生するならFrontメソッドで発生しているはずではある
                        throw new ArgumentException("opponents is all null.");
                    }
                    opponent.FixedTarget = i + 1;
                }
            }
        }

        private int Front(int self, List<Battler?> opponents)
        {
            if (opponents[self % 3] != null)
            {
                return self % 3;
            }
            if (opponents[(self + 1) % 3] != null)
            {
                return (self + 1) % 3;
            }
            if (opponents[(self + 2) % 3] != null)
            {
                return (self + 2) % 3;
            }

            throw new ArgumentException("opponents is all null.");
        }

        private Battler? TargetBattler(int target, Battler? battler1, Battler? battler2, Battler? battler3)
        {
            return target switch
            {
                1 => battler1,
                2 => battler2,
                3 => battler3,
                _ => null,
            };
        }

        private void CalcNormalSkills(Battler? self, Battler? right, Battler? left)
        {
            if (self == null || self.IsSkillDisabled)
            {
                return;
            }
            if (right == null && left != null)
            {
                right = left;
                left = null;
            }
            foreach (var skill in self.Employee.Skills)
            {
                CalcNormalSkill(skill, self, right, left);
            }

        }

        private void CalcNormalSkill(Skill skill, Battler self, Battler? right, Battler? left)
        {
            switch (skill.SkillType)
            {
                case SkillType.属性強化:
                    if (skill.Range != Models.Range.右 && self.Employee.Element == skill.Element)
                    {
                        self.Modifier *= skill.Modifier;
                    }
                    if (right != null && skill.Range != Models.Range.自分 && right.Employee.Element == skill.Element)
                    {
                        right.Modifier *= skill.Modifier;
                    }
                    if (left != null && skill.Range == Models.Range.全体 && left.Employee.Element == skill.Element)
                    {
                        left.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.タイプ強化:
                    if (skill.Range != Models.Range.右 && self.Employee.Type == skill.Type)
                    {
                        self.Modifier *= skill.Modifier;
                    }
                    if (right != null && skill.Range != Models.Range.自分 && right.Employee.Type == skill.Type)
                    {
                        right.Modifier *= skill.Modifier;
                    }
                    if (left != null && skill.Range == Models.Range.全体 && left.Employee.Type == skill.Type)
                    {
                        left.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.CT確定:
                    if (skill.Range != Models.Range.右)
                    {
                        self.AtkCritState = CriticalState.Crit;
                    }
                    if (right != null && skill.Range != Models.Range.自分)
                    {
                        right.AtkCritState = CriticalState.Crit;
                    }
                    if (left != null && skill.Range == Models.Range.全体)
                    {
                        left.AtkCritState = CriticalState.Crit;
                    }
                    break;
                case SkillType.CT回避:
                    if (skill.Range != Models.Range.右)
                    {
                        self.DefCritState = CriticalState.noCrit;
                    }
                    if (right != null && skill.Range != Models.Range.自分)
                    {
                        right.DefCritState = CriticalState.noCrit;
                    }
                    if (left != null && skill.Range == Models.Range.全体)
                    {
                        left.DefCritState = CriticalState.noCrit;
                    }
                    break;
                case SkillType.被CT確定:
                    if (self.DefCritState != CriticalState.noCrit)
                    {
                        self.DefCritState = CriticalState.Crit;
                    }
                    break;
                case SkillType.一色編成強化:
                    if (right != null && self.Employee.Element != right.Employee.Element)
                    {
                        break;
                    }
                    if (left != null && self.Employee.Element != left.Employee.Element)
                    {
                        break;
                    }
                    self.Modifier *= skill.Modifier;
                    if (right != null)
                    {
                        right.Modifier *= skill.Modifier;
                    }
                    if (left != null)
                    {
                        left.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.三色編成強化:
                    if (right != null && left != null &&
                        self.Employee.Element != right.Employee.Element &&
                        right.Employee.Element != left.Employee.Element &&
                        left.Employee.Element != self.Employee.Element)
                    {
                        self.Modifier *= skill.Modifier;
                        right.Modifier *= skill.Modifier;
                        left.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.くる:
                    self.Modifier *= skill.Modifier;
                    if (right != null && (right.Employee.Type == EmployeeType.けもみみ || right.Employee.Element == Element.木))
                    {
                        right.Modifier *= skill.Modifier;
                    }
                    if (left != null && (left.Employee.Type == EmployeeType.けもみみ || left.Employee.Element == Element.木))
                    {
                        left.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.二人強化:
                    if (left == null && right != null)
                    {
                        self.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.一人強化:
                    if (right == null)
                    {
                        self.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.こなす:
                    bool exist = false;
                    if (right != null && right.Employee.Rarity <= EmployeeRarity.Rplus)
                    {
                        exist = true;
                        right.Modifier *= skill.Modifier;
                    }
                    if (left != null && left.Employee.Rarity <= EmployeeRarity.Rplus)
                    {
                        exist = true;
                        left.Modifier *= skill.Modifier;
                    }
                    if (exist)
                    {
                        self.Modifier *= skill.Modifier;
                    }
                    break;
                case SkillType.特定社員強化:
                    bool isEffective = true;
                    foreach (var buddy in skill.Buddys)
                    {
                        if (right?.Employee.Id != buddy && left?.Employee.Id != buddy)
                        {
                            isEffective = false;
                        }
                    }
                    if (isEffective)
                    {
                        self.Modifier *= skill.Modifier;
                        if (skill.Range == Models.Range.全体)
                        {
                            foreach (var buddy in skill.Buddys)
                            {
                                if (right?.Employee.Id == buddy)
                                {
                                    right.Modifier *= skill.Modifier;
                                }
                                if (left?.Employee.Id == buddy)
                                {
                                    left.Modifier *= skill.Modifier;
                                }
                            }
                        }
                    }
                    break;
                case SkillType.特定社員強化_進化指定:
                    bool isEffectiveEvol = true;
                    foreach (var buddy in skill.Buddys)
                    {
                        if ((right?.Employee.Id != buddy || right?.Employee.EvolState != skill.BuddyEvolState) &&
                            (left?.Employee.Id != buddy || left?.Employee.EvolState != skill.BuddyEvolState))
                        {
                            isEffectiveEvol = false;
                        }
                    }
                    if (isEffectiveEvol)
                    {
                        self.Modifier *= skill.Modifier;
                        if (skill.Range == Models.Range.全体)
                        {
                            foreach (var buddy in skill.Buddys)
                            {
                                if (right?.Employee.Id == buddy)
                                {
                                    right.Modifier *= skill.Modifier;
                                }
                                if (left?.Employee.Id == buddy)
                                {
                                    left.Modifier *= skill.Modifier;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void CalcAttackSkills(Battler attacker, Battler defender)
        {
            if (attacker.IsSkillDisabled)
            {
                return;
            }

            foreach (var skill in attacker.Employee.Skills)
            {
                switch (skill.SkillType)
                {
                    case SkillType.タイプCT確定:
                        if (defender.Employee.Type == skill.Type && attacker.AtkCritState != CriticalState.noCrit)
                        {
                            attacker.OnceAtkCritState = CriticalState.Crit;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CalcDefenceSkills(Battler attacker, Battler defender)
        {
            if (defender.IsSkillDisabled)
            {
                return;
            }

            foreach (var skill in defender.Employee.Skills)
            {
                switch (skill.SkillType)
                {
                    case SkillType.タイプ軽減:
                        if (attacker.Employee.Type == skill.Type && !defender.IsReduced)
                        {
                            attacker.Modifier *= skill.Modifier;
                            defender.OnceDefCritState = CriticalState.noCrit;
                            defender.IsReduced = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CalcElement(Battler attacker, Battler defender)
        {
            // タイプキラー時は属性無視
            if (!attacker.IsSkillDisabled)
            {
                foreach (var skill in attacker.Employee.Skills)
                {
                    switch (skill.SkillType)
                    {
                        case SkillType.タイプキラー:
                            if (defender.Employee.Type == skill.Type)
                            {
                                attacker.Modifier *= skill.Modifier;
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // 属性軽減スキル最優先
            if (!defender.IsSkillDisabled)
            {
                Skill? reduceSkill = defender.Employee.HasSkill(SkillType.属性軽減);
                if (reduceSkill != null && reduceSkill.Element == attacker.Employee.Element && !defender.IsReduced)
                {
                    attacker.Modifier *= reduceSkill.Modifier;
                    defender.IsReduced = true;
                    return;
                }
            }

            // 普通の計算
            bool reduce = false;
            bool normal = false;
            bool effective = false;
            switch (attacker.Employee.Element)
            {
                case Element.火:
                    switch (defender.Employee.Element)
                    {
                        case Element.火:
                            normal = true;
                            break;
                        case Element.水:
                            reduce = true;
                            break;
                        case Element.木:
                            effective = true;
                            break;
                    }
                    break;
                case Element.水:
                    switch (defender.Employee.Element)
                    {
                        case Element.火:
                            effective = true;
                            break;
                        case Element.水:
                            normal = true;
                            break;
                        case Element.木:
                            reduce = true;
                            break;
                    }
                    break;
                case Element.木:
                    switch (defender.Employee.Element)
                    {
                        case Element.火:
                            reduce = true;
                            break;
                        case Element.水:
                            effective = true;
                            break;
                        case Element.木:
                            normal = true;
                            break;
                    }
                    break;
            }

            // キラー計算
            if (!attacker.IsSkillDisabled)
            {
                if (normal && attacker.Employee.HasSkill(SkillType.同族キラー) != null)
                {
                    normal = false;
                    effective = true;
                }
                if (attacker.Employee.HasSkill(SkillType.オールキラー) != null)
                {
                    reduce = false;
                    normal = false;
                    effective = true;
                }
            }

            // 軽減計算
            if (!defender.IsSkillDisabled)
            {
                if (effective && defender.Employee.HasSkill(SkillType.弱点無効) != null)
                {
                    effective = false;
                    normal = true;

                }
                if (defender.Employee.HasSkill(SkillType.全属性軽減) != null)
                {
                    effective = false;
                    normal = false;
                    reduce = true;

                }
            }

            // 結果を反映
            if (reduce)
            {
                attacker.Modifier *= 0.66;
                return;
            }
            if (effective)
            {
                attacker.Modifier *= 1.5;
                return;
            }
            return;
        }

        private double CalcCritical(Battler attacker, Battler defender)
        {
            // 回避最優先
            if (defender.DefCritState == CriticalState.noCrit || defender.OnceDefCritState == CriticalState.noCrit)
            {
                return 0;
            }

            // 確定スキル時
            if (attacker.AtkCritState == CriticalState.Crit || defender.DefCritState == CriticalState.Crit ||
                attacker.OnceAtkCritState == CriticalState.Crit || defender.OnceDefCritState == CriticalState.Crit)
            {
                return 1;
            }

            // 普通に計算(暫定)
            double atkDex = attacker.Dex * (attacker.IsBoost ? 1.3 : 1.0);
            double defDex = defender.Dex * (defender.IsBoost ? 1.3 : 1.0);

            double rate = atkDex / defDex;

            if (rate < 0.8 / 5)
            {
                return 0;
            }
            if (rate < 0.8 / 4)
            {
                return 0.06;
            }
            if (rate < 0.8 / 3)
            {
                return 0.12;
            }
            if (rate < 0.8 / 2)
            {
                return 0.18;
            }
            if (rate < 0.8)
            {
                return 0.24;
            }
            if (rate < 0.8 * 2)
            {
                return 0.76;
            }
            if (rate < 0.8 * 3)
            {
                return 0.82;
            }
            if (rate < 0.8 * 4)
            {
                return 0.88;
            }
            if (rate < 0.8 * 5)
            {
                return 0.92;
            }
            return 1;
        }

        private double CalcDamage(Battler attacker)
        {
            double atkAtk = attacker.Atk * (attacker.IsBoost ? 1.3 : 1.0);
            return atkAtk * attacker.Modifier;
        }
    }
}
