using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class Summon : SkillBase
    {
        public interface ISummon
        {
            void Summon();
        }


        public override string skillKey => SkillKeys.Summon;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.Summon;
        public override float skillUseTime => 1;


        private ISummon summonInterface;

        public Summon(ISummon iSummon, float cooltime) : base(cooltime)
        {
            this.summonInterface = iSummon;
        }
        public override void Action()
        {
            base.Action();
            this.summonInterface.Summon();
        }
    }
}