using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA_Filter
{
    class Affix
    {
        static public Dictionary<string,string> ItemAffix = new Dictionary<string, string>()
        {
               ["Defense"                        ] = "防御",
               ["Strength"                       ]="力量",
               ["Dexterity"                      ]="敏捷",
               ["Vitality"                       ]="活力",
               ["Energy"                         ]="能量",
               ["AllAttributes"                  ]="全属性",
               ["MaxLife"                        ]="最大生命",
               ["MaxMana"                        ]="最大法力",
               ["AttackRating"                   ]="命中",
               ["MinDamage"                      ]="最小伤害",
               ["MaxDamage"                      ]="最大伤害",
               ["DamageReduced"                  ]="伤害减少",
               ["LifeSteal"                      ]="生命偷取",
               ["ManaSteal"                      ]="法力偷取",
               ["ColdSkillDamage"                ]="冰技能伤害",
               ["LightningSkillDamage"           ]="电技能伤害",
               ["FireSkillDamage"                ]="火技能伤害",
               ["PoisonSkillDamage"              ]="毒技能伤害",
               ["IncreasedAttackSpeed"           ]="增加攻速",
               ["FasterRunWalk"                  ]="移动速度",
               ["FasterHitRecovery"              ]="打击恢复",
               ["FasterCastRate"                 ]="施法速度",
               ["MagicFind"                      ]="魔法掉落",
               ["GoldFind"                       ]="金币掉落",
               ["ColdResist"                     ]="冰抗",
               ["LightningResist"                ]="电抗",
               ["FireResist"                     ]="火抗",
               ["PoisonResist"                   ]="毒抗",
               ["SumResist"                      ]="总抗和",
               ["AllResist"                      ]="全抗",
               ["AllSkills"                      ]="全技能",
               ["FasterBlockRate"                ]="快速格挡",
               ["DeadlyStrike"                   ]= "几率造成致死打击",
               ["CrushingBlow"                   ]= "几率造成粉碎打击",
               ["OpenWounds"                     ]= "几率造成开创性伤口",
               ["CannotBeFrozen"                 ]="无法冰冻",
               ["SlainMonstersRestInPeace"       ]= "消灭的怪物就此安息",
               ["PreventMonsterHeal"             ]= "防止怪物治愈",
               ["AbsorbColdPercent"              ]="冰冷吸收",
               ["AbsorbFirePercent"              ]="火焰吸收",
               ["AbsorbLightningPercent"         ]="雷电吸收",
               ["MaxColdResist"                  ]="最大冰抗",
               ["MaxLightningResist"             ]="最大电抗",
               ["MaxFireResist"                  ]="最大火抗",
               ["MaxPoisonResist"                ]="最大毒抗",
               ["EnemyFireResist"                ]="降低火抗",
               ["EnemyLightningResist"           ]="降低电抗",
               ["EnemyColdResist"                ]="降低冰抗",
               ["EnemyPoisonResist"              ]="降低毒抗",
               ["MaxLifePercent"                 ]="最大生命百分比",
               ["MaxManaPercent"                 ]="最大法力百分比",
               ["DamageTakenGoesToMana"          ]= "受到的伤害转换为法力",
               ["EnhancedDamage"                 ]= "强化伤害",
               ["MinAreaLevel"                   ]="最小地区等级",
               ["MaxAreaLevel"                   ]="最大地区等级",
               ["MinPlayerLevel"                 ]="最小玩家等级",
               ["MaxPlayerLevel"                 ]="最大玩家等级",
        };
    }

}
