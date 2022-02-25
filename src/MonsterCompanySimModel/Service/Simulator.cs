using MonsterCompanySimModel.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Service
{
    /// <summary>
    /// シミュ本体
    /// </summary>
    public class Simulator
    {
        /// <summary>
        /// 開発用
        /// </summary>
        public void Debug()
        {

        }

        /// <summary>
        /// データロード
        /// </summary>
        public void LoadData()
        {
            Masters.LoadDatas();
        }

        /// <summary>
        /// 編成検索
        /// </summary>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <param name="level">味方レベル</param>
        /// <param name="part">部制限</param>
        /// <param name="progress">プログレスバー用ReactiveProperty</param>
        /// <returns>編成検索結果リスト</returns>
        public List<SearchResult> Search(Battler? enemy1, Battler? enemy2, Battler? enemy3, bool boost, int level,int part, ReactivePropertySlim<double>? progress)
        {
            // 結果用List
            List<SearchResult> resultList = new();

            // 社員無し(null)入り検索対象リスト
            List<Employee?> emps = new(Masters.SearchTargets) { null };

            // プログレスバー用
            if(progress != null)
            {
                progress.Value = 0;
            }
            
            // 総当たり
            foreach (var ally1 in emps)
            {
                foreach (var ally2 in emps)
                {
                    foreach (var ally3 in emps)
                    {
                        // 同一社員不可
                        if ((ally1 != null && ally1?.Id == ally2?.Id) ||
                            (ally2 != null && ally2?.Id == ally3?.Id) ||
                            (ally3 != null && ally3?.Id == ally1?.Id) ||
                            (ally1 == null && ally2 == null && ally3 == null))
                        {
                            continue;
                        }

                        // 部制限
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

                        // 戦闘データ生成
                        Battler? battler1 = ally1 == null ? null : new Battler(ally1) { Level = level };
                        Battler? battler2 = ally2 == null ? null : new Battler(ally2) { Level = level };
                        Battler? battler3 = ally3 == null ? null : new Battler(ally3) { Level = level };

                        // 勝率計算
                        BattleResult battleResult = FullBattle(battler1, battler2, battler3, enemy1, enemy2, enemy3, boost);

                        // 結果確認
                        if (battleResult.WinRate > 0)
                        {
                            SearchResult result = new()
                            {
                                Ally1 = ally1,
                                Ally2 = ally2,
                                Ally3 = ally3,
                                WinRate = battleResult.WinRate,
                                SumEng = (battler1?.Eng ?? 0) + (battler2?.Eng ?? 0) + (battler3?.Eng ?? 0)
                            };
                            resultList.Add(result);
                        }
                    }
                }

                // プログレスバー増加
                if (progress != null)
                {
                    progress.Value += 1.0 / emps.Count;
                }
            }

            // 要求レベル計算
            if (resultList.Count < Masters.ConfigData.RequireThreshold)
            {
                foreach (var result in resultList)
                {
                    // 戦闘データ再生成
                    Battler? battler1 = result.Ally1 == null ? null : new Battler(result.Ally1) { Level = level };
                    Battler? battler2 = result.Ally2 == null ? null : new Battler(result.Ally2) { Level = level };
                    Battler? battler3 = result.Ally3 == null ? null : new Battler(result.Ally3) { Level = level };

                    // 要求レベル計算
                    result.MinLevel = CalcRequireLevel(battler1, battler2, battler3, enemy1, enemy2, enemy3, boost) ?? 0;
                }
            }

            // プログレスバーリセット
            if (progress != null)
            {
                progress.Value = 0;
            }

            // 返却
            return resultList;
        }

        /// <summary>
        /// 要求レベル計算
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <returns>要求レベル(勝てない場合null)</returns>
        public int? CalcRequireLevel(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            bool boost)
        {
            int max = Masters.ConfigData.MaxLevel;
            int min = 1;
            
            bool won = false;
            while (max != min)
            {
                // 半分ずつ絞る
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

                // 勝率計算
                BattleResult battleResult = FullBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, boost);

                // 結果確認
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

            // 勝てていたら要求レベルを返す
            if (won)
            {
                return max;
            }

            // 勝ち目がなければnullを返す
            return null;
        }



        /// <summary>
        /// 戦闘シミュレート(ターゲットランダム)
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <returns>戦闘結果</returns>
        public BattleResult FullBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            bool boost
            )
        {
            // 整理
            List<Battler?> allys = new() { ally1, ally2, ally3 };
            List<Battler?> enemys = new() { enemy1, enemy2, enemy3 };

            // タゲ計算をリセット
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

            // 正面関連スキル計算
            CalcFrontSkill(allys, enemys);
            CalcFrontSkill(enemys, allys);

            // 全引き付けスキル計算
            CalcDecoySkill(allys, enemys);
            CalcDecoySkill(enemys, allys);

            // 味方戦闘
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

            // 敵戦闘
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

            // 結果まとめ
            BattleResult result = new();
            result.AllyDamages = allyDamages;
            result.EnemyDamages = enemyDamages;

            // 返却
            return result;
        }

        /// <summary>
        /// 戦闘シミュレート(ターゲット指定)
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="allyTarget1">味方1ターゲット</param>
        /// <param name="allyTarget2">味方2ターゲット</param>
        /// <param name="allyTarget3">味方3ターゲット</param>
        /// <param name="enemyTarget1">敵1ターゲット</param>
        /// <param name="enemyTarget2">敵2ターゲット</param>
        /// <param name="enemyTarget3">敵3ターゲット</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <returns>戦闘結果</returns>
        public BattleResult Battle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int allyTarget1, int allyTarget2, int allyTarget3,
            int enemyTarget1, int enemyTarget2, int enemyTarget3,
            bool boost
            )
        {
            // 戦闘結果クラス
            BattleResult result = new();

            // 味方戦闘
            result.AllyDamages = AllyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, allyTarget1, allyTarget2, allyTarget3, boost);

            // 敵戦闘
            result.EnemyDamages = EnemyBattle(ally1, ally2, ally3, enemy1, enemy2, enemy3, enemyTarget1, enemyTarget2, enemyTarget3, boost);

            // 返却
            return result;
        }

        /// <summary>
        /// 敵戦闘
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="enemyTarget1">敵1ターゲット</param>
        /// <param name="enemyTarget2">敵2ターゲット</param>
        /// <param name="enemyTarget3">敵3ターゲット</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <returns>ダメージ情報</returns>
        private List<Damage> EnemyBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int enemyTarget1, int enemyTarget2, int enemyTarget3,
            bool boost
            )
        {
            // 戦闘用の情報を初期化
            ally1?.ResetAttackProperty(boost);
            ally2?.ResetAttackProperty(boost);
            ally3?.ResetAttackProperty(boost);
            enemy1?.ResetAttackProperty();
            enemy2?.ResetAttackProperty();
            enemy3?.ResetAttackProperty();

            // 戦闘結果クラスを利用
            BattleResult result = new();

            // 相手に依存しないスキルを計算
            CalcNormalSkills(ally1, ally2, ally3);
            CalcNormalSkills(ally2, ally3, ally1);
            CalcNormalSkills(ally3, ally1, ally2);
            CalcNormalSkills(enemy1, enemy2, enemy3);
            CalcNormalSkills(enemy2, enemy3, enemy1);
            CalcNormalSkills(enemy3, enemy1, enemy2);

            // 攻撃してダメージ情報を格納
            result.CombineEnemyDamages(Attack(enemy1, TargetBattler(enemyTarget1, ally1, ally2, ally3)),
                enemy1?.Employee?.Name + "→" + TargetBattler(enemyTarget1, ally1, ally2, ally3)?.Employee?.Name);
            result.CombineEnemyDamages(Attack(enemy2, TargetBattler(enemyTarget2, ally1, ally2, ally3)),
                enemy2?.Employee?.Name + "→" + TargetBattler(enemyTarget2, ally1, ally2, ally3)?.Employee?.Name);
            result.CombineEnemyDamages(Attack(enemy3, TargetBattler(enemyTarget3, ally1, ally2, ally3)),
                enemy3?.Employee?.Name + "→" + TargetBattler(enemyTarget3, ally1, ally2, ally3)?.Employee?.Name);

            // ダメージ情報返却
            return result.EnemyDamages;
        }

        /// <summary>
        /// 味方戦闘
        /// </summary>
        /// <param name="ally1">味方1</param>
        /// <param name="ally2">味方2</param>
        /// <param name="ally3">味方3</param>
        /// <param name="enemy1">敵1</param>
        /// <param name="enemy2">敵2</param>
        /// <param name="enemy3">敵3</param>
        /// <param name="allyTarget1">味方1ターゲット</param>
        /// <param name="allyTarget2">味方2ターゲット</param>
        /// <param name="allyTarget3">味方3ターゲット</param>
        /// <param name="enemyTarget1">敵1ターゲット</param>
        /// <param name="enemyTarget2">敵2ターゲット</param>
        /// <param name="enemyTarget3">敵3ターゲット</param>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        /// <returns>ダメージ情報</returns>
        private List<Damage> AllyBattle(
            Battler? ally1, Battler? ally2, Battler? ally3,
            Battler? enemy1, Battler? enemy2, Battler? enemy3,
            int allyTarget1, int allyTarget2, int allyTarget3,
            bool boost
            )
        {
            // 戦闘用の情報を初期化
            ally1?.ResetAttackProperty(boost);
            ally2?.ResetAttackProperty(boost);
            ally3?.ResetAttackProperty(boost);
            enemy1?.ResetAttackProperty();
            enemy2?.ResetAttackProperty();
            enemy3?.ResetAttackProperty();

            // 戦闘結果クラスを利用
            BattleResult result = new();

            // 相手に依存しないスキルを計算
            CalcNormalSkills(ally1, ally2, ally3);
            CalcNormalSkills(ally2, ally3, ally1);
            CalcNormalSkills(ally3, ally1, ally2);
            CalcNormalSkills(enemy1, enemy2, enemy3);
            CalcNormalSkills(enemy2, enemy3, enemy1);
            CalcNormalSkills(enemy3, enemy1, enemy2);

            // 攻撃してダメージ情報を格納
            result.CombineAllyDamages(Attack(ally1, TargetBattler(allyTarget1, enemy1, enemy2, enemy3)),
                ally1?.Employee?.Name + "→" + TargetBattler(allyTarget1, enemy1, enemy2, enemy3)?.Employee?.Name);
            result.CombineAllyDamages(Attack(ally2, TargetBattler(allyTarget2, enemy1, enemy2, enemy3)),
                ally2?.Employee?.Name + "→" + TargetBattler(allyTarget2, enemy1, enemy2, enemy3)?.Employee?.Name);
            result.CombineAllyDamages(Attack(ally3, TargetBattler(allyTarget3, enemy1, enemy2, enemy3)),
                ally3?.Employee?.Name + "→" + TargetBattler(allyTarget3, enemy1, enemy2, enemy3)?.Employee?.Name);
           
            // ダメージ情報返却
            return result.AllyDamages;
        }

        /// <summary>
        /// 攻撃シミュレート
        /// </summary>
        /// <param name="attacker">攻撃側社員</param>
        /// <param name="defender">防御側社員</param>
        /// <returns>ダメージ情報</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private List<Damage> Attack(Battler? attacker, Battler? defender)
        {
            if (attacker == null)
            {
                // 攻撃側nullは0ダメージ
                List<Damage> noList = new();
                noList.Add(new Damage());
                return noList;
            }
            if (defender == null)
            {
                // 防御側nullはバグ
                throw new ArgumentNullException(nameof(defender));
            }

            // 一時的なクリティカル補正を初期化(タイプキラー・タイプ軽減)
            attacker.OnceDefCritState = CriticalState.normal;
            attacker.OnceAtkCritState = CriticalState.normal;
            defender.OnceDefCritState = CriticalState.normal;
            defender.OnceAtkCritState = CriticalState.normal;

            // 攻撃側の相手依存スキル計算
            CalcAttackSkills(attacker, defender);

            // 防御側の相手依存スキル計算
            CalcDefenceSkills(attacker, defender);

            // 属性・属性関連スキル計算
            CalcElement(attacker, defender);

            // クリティカル率計算
            double crit = CalcCritical(attacker, defender);

            // ダメージ値計算
            double damageValue = CalcDamage(attacker);

            // ダメージ情報をセット
            List<Damage> damages = new();
            damages.Add(new Damage(crit, damageValue * 1.5));
            damages.Add(new Damage(1 - crit, damageValue));

            // 返却
            return damages;
        }

        /// <summary>
        /// ターゲット計算
        /// </summary>
        /// <param name="battler">攻撃社員</param>
        /// <param name="opponents">相手社員たち</param>
        /// <returns>攻撃パターン</returns>
        private List<int> CalcTargets(Battler? battler, List<Battler?> opponents)
        {
            List<int> list = new();
            if (battler == null)
            {
                // nullは0を返却
                // 戦闘は行わないので値はなんでもいいが、「戦闘しない」の1パターンを行うため1項目で返す必要がある
                list.Add(0);
                return list;
            }
            if (battler.FixedTarget != 0)
            {
                // 固定時は攻撃対象だけを返却
                list.Add(battler.FixedTarget);
                return list;
            }

            // 特にスキルの影響等がない場合存在する相手全員を返却
            for (int i = 0; i < opponents.Count; i++)
            {
                if (opponents[i] != null)
                {
                    list.Add(i + 1);
                }
            }
            return list;
        }

        /// <summary>
        /// 引き付けスキル計算
        /// </summary>
        /// <param name="we">自分側社員たち</param>
        /// <param name="opponents">相手側社員たち</param>
        private void CalcDecoySkill(List<Battler?> we, List<Battler?> opponents)
        {
            // 引き付けスキル持ちがいた場合、相手のターゲットをその社員に固定する
            // 右側が優先なので左から計算して上書き
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

        /// <summary>
        /// 正面スキル計算
        /// </summary>
        /// <param name="we">自分側社員たち</param>
        /// <param name="opponents">相手側社員たち</param>
        /// <exception cref="ArgumentException"></exception>
        private void CalcFrontSkill(List<Battler?> we, List<Battler?> opponents)
        {
            for (int i = 0; i < we.Count; i++)
            {
                Battler? skillOwner = we[i];
                if (skillOwner == null)
                {
                    // 社員がnullの場合次へ
                    continue;
                }

                // 正面攻撃
                Skill? attackFront = skillOwner.Employee.HasSkill(SkillType.正面攻撃);
                if (attackFront != null && !skillOwner.IsSkillDisabled)
                {
                    // 正面攻撃持ちはターゲットを正面に固定
                    if (attackFront.Range == Models.Range.自分)
                    {
                        skillOwner.FixedTarget = Front(i, opponents) + 1;
                    }
                    if (attackFront.Range == Models.Range.全体)
                    {
                        for (int j = 0; j < we.Count; j++)
                        {
                            Battler? battler = we[j];
                            if (battler != null)
                            {
                                battler.FixedTarget = Front(j, opponents) + 1;
                            }
                        }
                    }
                }

                // 正面引き付け
                Skill? attractFront = skillOwner.Employee.HasSkill(SkillType.正面引き付け);
                if (attractFront != null && !skillOwner.IsSkillDisabled)
                {
                    // 正面引き付け持ちは相手のターゲットを自分に固定
                    Battler? opponent = opponents[i];
                    // 自分限定の場合は正面がいる場合のみ有効
                    if (attractFront.Range == Models.Range.自分 && opponent != null)
                    {
                        opponent.FixedTarget = i + 1;
                    }
                    // 全体の場合は敵全員に正面攻撃(いなかったら右)を付与
                    if (attractFront.Range == Models.Range.全体)
                    {
                        for (int j = 0; j < opponents.Count; j++)
                        {
                            Battler? battler = opponents[j];
                            if (battler != null)
                            {
                                battler.FixedTarget = Front(j, we) + 1;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 正面社員の位置番号(0オリジン)を取得
        /// </summary>
        /// <param name="self">自分の位置番号(0オリジン)</param>
        /// <param name="opponents">相手達</param>
        /// <returns>正面社員の位置番号(0オリジン)</returns>
        /// <exception cref="ArgumentException"></exception>
        private int Front(int self, List<Battler?> opponents)
        {
            // 正面がいるなら正面
            if (opponents[self % 3] != null)
            {
                return self % 3;
            }

            // 正面がいないなら右、右端を超えたら左端へ
            if (opponents[(self + 1) % 3] != null)
            {
                return (self + 1) % 3;
            }

            // 右もいないならさらに右、右端を超えたら左端へ
            if (opponents[(self + 2) % 3] != null)
            {
                return (self + 2) % 3;
            }

            // 相手社員がいない
            throw new ArgumentException("opponents is all null.");
        }

        /// <summary>
        /// ターゲットの社員を取得
        /// </summary>
        /// <param name="target">ターゲット番号(1オリジン)</param>
        /// <param name="battler1">相手社員1</param>
        /// <param name="battler2">相手社員2</param>
        /// <param name="battler3">相手社員3</param>
        /// <returns></returns>
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

        /// <summary>
        /// 相手に依存しないスキルを計算
        /// </summary>
        /// <param name="self">自分</param>
        /// <param name="right">右側社員</param>
        /// <param name="left">左側社員</param>
        private void CalcNormalSkills(Battler? self, Battler? right, Battler? left)
        {
            // 自分がnullまたはスキル無効化されているなら計算終了
            if (self == null || self.IsSkillDisabled)
            {
                return;
            }

            // 右がnullで左がいるなら、左の社員を右扱いする
            if (right == null && left != null)
            {
                right = left;
                left = null;
            }

            // 各スキルを計算する
            foreach (var skill in self.Employee.Skills)
            {
                CalcNormalSkill(skill, self, right, left);
            }

        }

        /// <summary>
        /// 相手に依存しないスキルを計算(本体)
        /// </summary>
        /// <param name="skill">スキル</param>
        /// <param name="self">自分</param>
        /// <param name="right">右側社員</param>
        /// <param name="left">左側社員</param>
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

        /// <summary>
        /// 攻撃側の相手に依存するスキルを計算
        /// </summary>
        /// <param name="attacker">攻撃側社員</param>
        /// <param name="defender">防御側社員</param>
        private void CalcAttackSkills(Battler attacker, Battler defender)
        {
            // スキル無効化されている場合計算終了
            if (attacker.IsSkillDisabled)
            {
                return;
            }

            // 各スキルを計算
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

        /// <summary>
        /// 防御側側の相手に依存するスキルを計算
        /// </summary>
        /// <param name="attacker">攻撃側社員</param>
        /// <param name="defender">防御側社員</param>
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

        /// <summary>
        /// 属性・属性関係のスキルを計算
        /// </summary>
        /// <param name="attacker">攻撃側社員</param>
        /// <param name="defender">防御側社員</param>
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
            if (normal) 
            {
                // この分岐は不要だが可読性のため追加
                return;
            }
        }

        /// <summary>
        /// クリティカル率計算
        /// </summary>
        /// <param name="attacker">攻撃側社員</param>
        /// <param name="defender">防御側社員</param>
        /// <returns>クリティカル率</returns>
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

        /// <summary>
        /// ダメージ計算
        /// </summary>
        /// <param name="attacker">攻撃社員</param>
        /// <returns>ダメージ(クリティカル無し)</returns>
        private double CalcDamage(Battler attacker)
        {
            double atkAtk = attacker.Atk * (attacker.IsBoost ? 1.3 : 1.0);
            return atkAtk * attacker.Modifier;
        }
    }
}
