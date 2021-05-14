using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public static class Defines
    {
        /// <summary>
        /// 대미지 랜덤 변화값
        /// </summary>
        public const float DamageRange = 0.1f;

        /// <summary>
        /// 긍적적일때 표시되는 컬러기본값
        /// </summary>
        public const string TRUE_COLOR_CODE = "#0B610B";

        /// <summary>
        /// 부정적일때 표시되는 컬러기본값
        /// </summary>
        public const string FALSE_COLOR_CODE = "#B40404";
    }

    public static class SkillKeys
    {
        /// <summary>
        /// 기본공격
        /// </summary>
        public const string DefaultAttack = "DefaultAttack";

        /// <summary>
        /// 저격
        /// </summary>
        public const string SnipeShot = "SnipeShot";

        /// <summary>
        /// 독
        /// </summary>
        public const string PoisonShot = "PoisonShot";

        /// <summary>
        /// 범위공격
        /// </summary>
        public const string BombShot = "BombShot";

        /// <summary>
        /// 전방위 샷
        /// </summary>
        public const string Bullet360Shot = "Bullet360Shot";

        /// <summary>
        /// 점프 어택
        /// </summary>
        public const string JumpAttack = "JumpAttack";

        /// <summary>
        /// 소환
        /// </summary>
        public const string Summon = "Summon";
    }

    public static class UnitKeys
    {
        /// <summary>
        /// 유니티짱
        /// </summary>
        public const string UnityChan = "UnityChan";

        /// <summary>
        /// 머쉬룸 레드
        /// </summary>
        public const string Mushroom_Red = "Mushroom_Red";

        /// <summary>
        /// 머쉬룸 그린
        /// </summary>
        public const string Mushroom_Green = "Mushroom_Green";

        /// <summary>
        /// 머쉬룸 블루
        /// </summary>
        public const string Mushroom_Blue = "Mushroom_Blue";

        /// <summary>
        /// 거미
        /// </summary>
        public const string Metalon = "Metalon";

        /// <summary>
        /// 거미 미니
        /// </summary>
        public const string MetalonMini = "Metalon_Mini";
    }

    public static class PrefabPath
    {
        public static class Unit
        {
            public const string UnityChan      = "Units/" + UnitKeys.UnityChan;
            public const string Mushroom_Red   = "Units/" + UnitKeys.Mushroom_Red;
            public const string Mushroom_Green = "Units/" + UnitKeys.Mushroom_Green;
            public const string Mushroom_Blue  = "Units/" + UnitKeys.Mushroom_Blue;
            public const string Metalon        = "Units/" + UnitKeys.Metalon;
            public const string MetalonMini    = "Units/" + UnitKeys.MetalonMini;
            public const string PlayerMetalon = "Units/Player" + UnitKeys.Metalon;
        }

        public static class Projectile
        {
            public const string StandardBullet = "Projectiles/StandardBullet";
            public const string SnipeBullet    = "Projectiles/SnipeBullet";
            public const string PoisonBullet   = "Projectiles/PoisonBullet";
            public const string BombBullet     = "Projectiles/BombBullet";
        }

        public static class Particle
        {
            public const string FireHit       = "Particles/FireHit";
            public const string FireExplosion = "Particles/FireExplosion";
            public const string SnipeShotEffect = "Particles/SnipeShotEffect";
            public const string PoisonHitEffect = "Particles/PoisonHitEffect";
            public const string HPPotionEffect = "Particles/HPPotionEffect";
            public const string MPPotionEffect = "Particles/MPPotionEffect";
            public const string JumpAttackEffect = "Particles/JumpAttackEffect";

            public static class Condition
            {
                public const string StunConditionEffect = "Particles/Conditions/StunConditionEffect";
                public const string PoisonConditionEffect = "Particles/Conditions/PoisonConditionEffect";
                public const string SlowConditionEffect = "Particles/Conditions/SlowConditionEffect";
            }
        }

        public static class UI
        {
            public const string DamageViewer                   = "UI/DamageViewer";
            public const string GameHUD                        = "UI/GameHud";
            public const string GameUI                         = "UI/GameUI";
            public const string InteractionIcon                = "UI/InteractionIcon";
            public const string QuestQuickView_Element         = "UI/QuestQuickView_Element";
            public const string QuestUI_QuestElement           = "UI/QuestUI_QuestElement";
            public const string SkillViewer_IconElement        = "UI/SkillViewer_IconElement";
            public const string UnitStatusBar                  = "UI/UnitStatusBar";
            public const string UnitStatusBar_ConditionElement = "UI/UnitStatusBar_ConditionElement";
            public const string DungeonUI_Element              = "UI/DungeonUI_Element";
            public const string GameResultUI_Element           = "UI/GameResultUI_Element";
            public const string MonsterRemainCountHud          = "UI/MonsterRemainCountHud";
            public const string TimeAttackHud                  = "UI/TimeAttackHud";
            public const string UnitNameViewer                 = "UI/UnitNameViewer";
            public const string QuestStateIcon                 = "UI/QuestStateIcon";
            public const string InventoryUI_IconElement        = "UI/InventoryUI_IconElement";
            public const string StoreUI_ItemElement            = "UI/StoreUI_ItemElement";
            public const string ItemViewer_IconElement         = "UI/ItemViewer_IconElement";
            public const string QuestDetailView_RewardElement  = "UI/QuestDetailView_RewardElement";
            public const string GameEventViewer_TextElement    = "UI/GameEventViewer_TextElement";
            public const string AggroIcon                      = "UI/AggroIcon";
        }

        public const string AOEProjector = "AOEProjector";
        public const string SpiderWeb    = "SpiderWeb";
        public const string Coin         = "Coin";
    }

    public static class SpritePath
    {
        public const string SkillIcon_Path     = "Sprites/SkillIcons/";
        public const string ConditionIcon_Path = "Sprites/ConditionIcons/";
        public const string Item_Path          = "Sprites/Items/";
    }

    public static class Layers
    {
        public static readonly int Default       = 1 << LayerMask.NameToLayer("Default");
        public static readonly int Unit          = 1 << LayerMask.NameToLayer("Unit");
        public static readonly int Projectile    = 1 << LayerMask.NameToLayer("Projectile");
        public static readonly int Weapon        = 1 << LayerMask.NameToLayer("Weapon");
        public static readonly int DynamicObject = 1 << LayerMask.NameToLayer("DynamicObject");
        public static readonly int StaticObject  = 1 << LayerMask.NameToLayer("StaticObject");
        public static readonly int UnitObstacle  = 1 << LayerMask.NameToLayer("UnitObstacle");
        public static readonly int Terrain       = 1 << LayerMask.NameToLayer("Terrain");
        public static readonly int Item          = 1 << LayerMask.NameToLayer("Item");


        public static readonly int Group_Ground = Terrain | Unit | StaticObject | DynamicObject;
    }

    public static class QuestCheckKey
    {
        /// <summary>
        /// 누군가에게 말을 걸었다.
        /// </summary>
        public const string QuestKey_Talk_ = "QuestKey_Talk_";

        /// <summary>
        /// 몬스터를 사냥했다.
        /// </summary>
        public const string QuestKey_Hunt_ = "QuestKey_Hunt_";

        /// <summary>
        /// 무언가를 수집했다
        /// </summary>
        public const string QuestKey_Collect_ = "QuestKey_Collect_";
    }

    public static class ItemKeys
    {
        /// <summary>
        /// 체력포션
        /// </summary>
        public const string HPPotion     = "HPPotion";

        /// <summary>
        /// 마력포션
        /// </summary>
        public const string MPPotion     = "MPPotion";

        /// <summary>
        /// 치료 열매
        /// </summary>
        public const string CuredBerries = "CuredBerries";
    }

    public static class SoundKeys
    {
        public const string BGM_TOWN        = "HYP_PRODUCING_Cat_Song";
        public const string BGM_DUNGEON_1_1 = "HYP_Never_Mind";
        public const string BGM_DUNGEON_1_2 = "Background Music";
        public const string BGM_BATTLE_BOSS = "HYP_Dark_Space";

        public const string EFFECT_DEFAULT_ATTACK = "DefaultAttack";
        public const string EFFECT_HIT_SOUND      = "HitSound";
        public const string EFFECT_STRONG_HIT_SOUND = "Hand Gun 1";
        public const string EFFECT_DIALOG = "dialog";
        public const string EFFECT_COIN = "coin";
        public const string EFFECT_PURCHASE = "purchase";
        public const string EFFECT_DRINK = "drink";
        public const string EFFECT_VICTORY = "victory";
        public const string EFFECT_GAMEOVER = "gameover";
        public const string EFFECT_CLEAR_QUEST = "clearquest";
        public const string EFFECT_SKILL_FIRE1 = "Flare gun 5-2";
        public const string EFFECT_EXPLOSION = "etfx_explosion_rocket";
        public const string EFFECT_POISON = "poison";
        public const string EFFECT_PUNCH = "punch_hook";
        public const string EFFECT_DIE_MONSETR = "monster_die";
        public const string EFFECT_JUMP_ATTACK = "Cannon impact 9";
        public const string EFFECT_CREATE_METALON_MINI = "Monster Bite on Armor";
        public const string EFFECT_CREATE_SPIDER_WEB = "Bloody punch";
        public const string EFFECT_JUMP = "boooing";
    }

    /// <summary>
    /// 유닛기준 절대적인 값.
    /// </summary>
    public enum UnitSide
    {
        /// <summary>
        /// 아군
        /// </summary>
        Friendly,
        /// <summary>
        /// 적군(대미지를 입힐 수 있음)
        /// </summary>
        Enemy
    }

    /// <summary>
    /// 유닛 컨디션
    /// </summary>
    public enum UnitCondition
    {
        /// <summary>
        /// 넉백(밀림)
        /// </summary>
        Knockback,

        /// <summary>
        /// 독
        /// </summary>
        Poison,

        /// <summary>
        /// 스턴
        /// </summary>
        Stun,

        /// <summary>
        /// 이동속도 감소
        /// </summary>
        Slow,

        /// <summary>
        /// 개수 셀 때 씀
        /// </summary>
        Cnt,
    }

    /// <summary>
    /// 유닛 상태
    /// </summary>
    public enum UnitState
    {
        /// <summary>
        /// 유닛이 초기화가 안됨
        /// </summary>
        NotInitialize,
        /// <summary>
        /// 유닛이 살아있음
        /// </summary>
        Live,
        /// <summary>
        /// 유닛이 죽음
        /// </summary>
        Death
    }

    /// <summary>
    /// 아이템 종류
    /// </summary>
    public enum ItemKind
    {
        /// <summary>
        /// 아무것도 아님(상점 판매용???)
        /// </summary>
        None,
        /// <summary>
        /// 코인
        /// </summary>
        Coin,
        /// <summary>
        /// 퀘스트 아이템
        /// </summary>
        Quest,
        /// <summary>
        /// 먹는아이템(포션)
        /// </summary>
        Potion
    }

    /// <summary>
    /// 선택지 타입
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        /// 잡담
        /// </summary>
        Chat,
        /// <summary>
        /// 퀘스트
        /// </summary>
        Quest,
        /// <summary>
        /// 상점
        /// </summary>
        Store,
    }

    /// <summary>
    /// 유닛 피벗정도
    /// </summary>
    [System.Serializable]
    public struct UnitPivots
    {
        public Transform head;
        public Transform center;
        public Transform foot;
        public Transform overhead;


        /// <summary>
        /// 피벗데이터가 유효한지 확인할 때 사용
        /// </summary>
        /// <returns></returns>
        public bool IsVaild()
        {
            return this.head     != null
                && this.center   != null
                && this.foot     != null
                && this.overhead != null;
        }
    }

    /// <summary>
    /// 선택지 정보
    /// </summary>
    public struct SelectionData
    {
        public SelectionType type;
        /// <summary>
        /// Type이 퀘스트이면 퀘스트키,
        /// Type이 상점이면 상점 종류가 들어있음(상점은 미구현)
        /// </summary>
        public string key;
        public string text;

        public SelectionData(SelectionType type, string key, string text)
        {
            this.type = type;
            this.key  = key;
            this.text = text;
        }
    }
    
    /// <summary>
    /// 퀘스트 정보
    /// </summary>
    [System.Serializable]
    public struct QuestInfo
    {
        /// <summary>
        /// 퀘스트 키
        /// </summary>
        public string questKey;

        /// <summary>
        /// 선행 조건(선행 퀘스트를 달성했거나, 특정 무언가가 필요할 때)
        /// </summary>
        public string[] preKey;

        /// <summary>
        /// 퀘스트 클리어시 얻는 키(다음 퀘스트를 받거나, 특정 기능 언락)
        /// </summary>
        public string[] unlockKey;

        /// <summary>
        /// 제목
        /// </summary>
        public string title;

        /// <summary>
        /// 내용
        /// </summary>
        public string content;

        /// <summary>
        /// 조건 텍스트(requestKey, request의 배열개수랑 반드시 동일해야 함)
        /// </summary>
        public string[] ifText;

        /// <summary>
        /// 요구사항
        /// </summary>
        public string[] requestKey;

        /// <summary>
        /// 요구 사항 필요 횟수
        /// </summary>
        public int[] request;

        /// <summary>
        /// 보상 키
        /// </summary>
        public string[] rewardKey;

        /// <summary>
        /// 보상
        /// </summary>
        public int[] reward;



        public QuestInfo(string questKey, string[] preKey, string[] unlockKey, string title, string content, string[] ifText, string[] requestKey, int[] request, string[] rewardKey, int[] reward)
        {
            this.questKey   = questKey;
            this.preKey     = preKey;
            this.unlockKey  = unlockKey;
            this.title      = title;
            this.content    = content;
            this.ifText     = ifText;
            this.requestKey = requestKey;
            this.request    = request;
            this.rewardKey  = rewardKey;
            this.reward     = reward;
        }
    }
    
    /// <summary>
    /// 던전 정보
    /// </summary>
    [System.Serializable]
    public struct DungeonInfo
    {
        /// <summary>
        /// 던전 키
        /// </summary>
        public string dungeonKey;

        /// <summary>
        /// 선행 조건(선행 퀘스트를 달성했거나, 특정 무언가가 필요할 때)
        /// </summary>
        public string[] preKey;

        /// <summary>
        /// 던전 클리어시 얻는 키(다음 퀘스트를 받거나, 특정 기능 언락)
        /// </summary>
        public string[] unlockKey;

        /// <summary>
        /// 제목
        /// </summary>
        public string title;

        /// <summary>
        /// 내용
        /// </summary>
        public string content;

        /// <summary>
        /// 보스던전
        /// </summary>
        public bool isBoss;


        public DungeonInfo(string dungeonKey, string[] preKey, string[] unlockKey, string title, string content, bool isBoss)
        {
            this.dungeonKey = dungeonKey;
            this.preKey     = preKey;
            this.unlockKey  = unlockKey;
            this.title      = title;
            this.content    = content;
            this.isBoss     = isBoss;
        }
    }

    /// <summary>
    /// 아이템 정보
    /// </summary>
    [System.Serializable]
    public struct ItemInfo
    {
        /// <summary>
        /// 아이템 키
        /// </summary>
        public string itemKey;

        /// <summary>
        /// 아이템 이름
        /// </summary>
        public string itemName;

        /// <summary>
        /// 아이템 설명
        /// </summary>
        public string explain;

        /// <summary>
        /// 아이템 종류
        /// </summary>
        public ItemKind itemKind;

        /*
         * ItemType에 따라 적절한 값들이 들어있다.
         */
        public string stringValue;
        public int intValue;
        public float floatValue;

        public ItemInfo(string itemKey, ItemKind itemKind, string itemName, string explain, string stringValue, int intValue, float floatValue)
        {
            this.itemKey = itemKey;
            this.itemName = itemName;
            this.explain = explain;
            this.itemKind = itemKind;
            this.stringValue = stringValue;
            this.intValue = intValue;
            this.floatValue = floatValue;
        }
    }

    /// <summary>
    /// 상점 정보
    /// </summary>
    public struct StoreInfo
    {
        /// <summary>
        /// 아이템 키
        /// </summary>
        public string itemKey;

        /// <summary>
        /// 가격
        /// </summary>
        public long price;

        public StoreInfo(string itemKey, long price)
        {
            this.itemKey = itemKey;
            this.price = price;
        }
    }

    /// <summary>
    /// 스코어 정보
    /// </summary>
    public struct ScoreInfo
    {
        /// <summary>
        /// 시작시간
        /// </summary>
        public long startTicks;

        /// <summary>
        /// 종료시간
        /// </summary>
        public long endTicks;

        /// <summary>
        /// 몬스터를 죽인 수
        /// </summary>
        public int killMonster;

        /// <summary>
        /// 보스 처치
        /// </summary>
        public int killBoss;

        /// <summary>
        /// 실제 플레이 시간
        /// </summary>
        public long playTicks => System.DateTime.UtcNow.Ticks - startTicks;
    }

    /// <summary>
    /// 플레이어의 퀘스트 진행정보
    /// </summary>
    [System.Serializable]
    public class QuestProgressInfo
    {
        public string key;
        public int[] progress;

        public QuestProgressInfo(string key, int[] progress)
        {
            this.key = key;
            this.progress = progress;
        }
    }
}