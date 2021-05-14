using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 오브젝트 생성, 풀링 관련 매니저.
    /// 일단 간단하게 구현해둠.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager inst { get; private set; }

        /// <summary>
        /// 풀링 데이터 정보
        /// </summary>
        private PoolInfo[] poolInfo = new PoolInfo[] {
            new PoolInfo(PrefabPath.Projectile.StandardBullet, 150, true),
            new PoolInfo(PrefabPath.Projectile.SnipeBullet, 1, true),
            new PoolInfo(PrefabPath.Projectile.PoisonBullet, 1, true),
            new PoolInfo(PrefabPath.Projectile.BombBullet, 1, true),

            new PoolInfo(PrefabPath.Particle.FireHit, 15, true),
            new PoolInfo(PrefabPath.Particle.FireExplosion, 5, true),
            new PoolInfo(PrefabPath.Particle.PoisonHitEffect, 5, true),
            new PoolInfo(PrefabPath.Particle.SnipeShotEffect, 1, true),
            new PoolInfo(PrefabPath.Particle.HPPotionEffect, 1, true),
            new PoolInfo(PrefabPath.Particle.JumpAttackEffect, 1, true),

            new PoolInfo(PrefabPath.Particle.Condition.StunConditionEffect, 3, true),
            new PoolInfo(PrefabPath.Particle.Condition.PoisonConditionEffect, 3, true),
            new PoolInfo(PrefabPath.Particle.Condition.SlowConditionEffect, 3, true),

            new PoolInfo(PrefabPath.UI.DamageViewer, 15, true),
            new PoolInfo(PrefabPath.UI.UnitStatusBar, 10, true),
            new PoolInfo(PrefabPath.UI.UnitStatusBar_ConditionElement, 10, false),
            new PoolInfo(PrefabPath.UI.QuestUI_QuestElement, 5, false),
            new PoolInfo(PrefabPath.UI.DungeonUI_Element, 2, false),
            new PoolInfo(PrefabPath.UI.GameResultUI_Element, 3, false),
            new PoolInfo(PrefabPath.UI.InventoryUI_IconElement, 3, false),
            new PoolInfo(PrefabPath.UI.StoreUI_ItemElement, 2, false),
            new PoolInfo(PrefabPath.UI.QuestDetailView_RewardElement, 3, false),
            new PoolInfo(PrefabPath.UI.GameEventViewer_TextElement, 10, false),
            new PoolInfo(PrefabPath.UI.AggroIcon, 3, true),

            new PoolInfo(PrefabPath.SpiderWeb, 5, true),
            new PoolInfo(PrefabPath.AOEProjector, 5, true),
            new PoolInfo(PrefabPath.Coin, 10, true),
            new PoolInfo(PrefabPath.Unit.MetalonMini, 4, true),
        };


        /// <summary>
        /// 풀링된 오브젝트를 담고 있는 빈 부모 오브젝트
        /// </summary>
        private Dictionary<string, GameObject> poolObjectRoot = new Dictionary<string, GameObject>();
        /// <summary>
        /// 오브젝트 추가생성을 위한 리소스 캐싱
        /// </summary>
        private Dictionary<string, GameObject> objectResource = new Dictionary<string, GameObject>();



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Construct()
        {
            inst = new GameObject("Object Pool Manager").AddComponent<ObjectPoolManager>();
            DontDestroyOnLoad(inst);

            // 필요한 오브젝트들을 미리 준비한다.
            foreach(var p in inst.poolInfo)
                inst.ReadyPool(p.path, p.count, p.autoCollecting);
        }

        /// <summary>
        /// 풀을 준비함.
        /// </summary>
        private void ReadyPool(string path, int count, bool autoCollecting)
        {
            // 중복처리
            if(this.poolObjectRoot.ContainsKey(path))
            {
                KLog.LogWarning (string.Format("풀링 중복: {0}", path));
                return;
            }


            // 리소스정보 불러오기
            GameObject resource = Resources.Load<GameObject>(path);
            if (resource == null)
            {
                KLog.LogError("리소스 정보를 찾지 못함. " + path);
                return;
            }
            // 리소스 정보 캐싱
            this.objectResource.Add(path, resource);


            // 생성된 오브젝트들을 담을 빈 부모 오브젝트 생성
            GameObject rootObj = new GameObject(path); 
            rootObj.transform.SetParent(this.transform);
            
            this.poolObjectRoot.Add(path, rootObj); // 컨테이너에 등록

            
            // 오브젝트를 count에 맞춰 생성
            for(int i=0; i<count; i++)
            {
                GameObject newObj = Instantiate(resource);

                PoolingObject po = newObj.AddComponent<PoolingObject>();
                po.path           = path;
                po.autoCollection = autoCollecting;


                newObj.transform.SetParent(rootObj.transform);
                newObj.gameObject.SetActive(false);
            }

            //KLog.Log(string.Format("Pooling Resource. Path: {0}, Count: {1}", path, count));
        }

        /// <summary>
        /// 오브젝트를 가져옴.
        /// 풀링된 오브젝트: 풀링된 오브젝트중 하나를 줌. 없으면 추가생성
        /// 풀링안된 오브젝트: 매번 Resources.Load를 통해 가져옴
        /// </summary>
        public T Get<T>(string path) where T : Component
        {
            if(this.poolObjectRoot.ContainsKey(path))
            {
                // 풀링이 되어 있는 오브젝트
                GameObject parentObj = this.poolObjectRoot[path];

                if (parentObj.transform.childCount > 0) // 풀링이 된 오브젝트가 남아있다.
                {
                    Transform ret = parentObj.transform.GetChild(0);

                    ret.transform.SetParent(null);
                    ret.gameObject.SetActive(true);

                    return ret.GetComponent<T>();
                }
                else // 풀링된 오브젝트가 없다.
                {
                    // 새로 생성
                    GameObject newObj = Instantiate(this.objectResource[path]);

                    PoolingObject po = newObj.AddComponent<PoolingObject>();

                    po.path           = path;
                    po.autoCollection = false;

                    KLog.LogWarning("Inc Object. " + path);
                    return newObj.GetComponent<T>();
                }
            }
            else
            {
                // 풀링이 안된 오브젝트는 바로 생성함
                KLog.LogWarning("Resource.Load Object. " + path);
                return Instantiate(Resources.Load<T>(path));
            }
        }

        /// <summary>
        /// 풀링을 거치지 않고 바로 생성함
        /// </summary>
        public T New<T>(string path) where T : Component
        {
            return Instantiate(Resources.Load<T>(path));
        }

        /// <summary>
        /// 오브젝트 풀로 되돌리거나 풀링되지 않은 오브젝트 삭제
        /// </summary>
        /// <param name="obj"></param>
        public void Return(GameObject obj)
        {
            PoolingObject po = obj.GetComponent<PoolingObject>();
            if (po != null)
            {
                // 풀링된 오브젝트 처리
                obj.transform.SetParent(this.poolObjectRoot[po.path].transform);
                obj.gameObject.SetActive(false);
            }
            else
            {
                // 풀링 안된 오브젝트 처리
                Destroy(obj);
            }
        }

        /// <summary>
        /// 모든 풀링된 오브젝트들을 회수한다
        /// </summary>
        public void CollectAll()
        {
            foreach(var p in FindObjectsOfType<PoolingObject>())
            {
                // 자동회수 옵션이 켜져 있을경우만 회수한다.
                if(p.autoCollection)
                    Return(p.gameObject);
            }
        }

        private struct PoolInfo
        {
            /// <summary>
            /// 리소스 경로
            /// </summary>
            public string path;

            /// <summary>
            /// 기본 풀링 개수
            /// </summary>
            public int    count;

            /// <summary>
            /// 씬 전환시 자동 회수기능
            /// </summary>
            public bool autoCollecting;



            public PoolInfo(string path, int count, bool autoCollecting)
            {
                this.path           = path;
                this.count          = count;
                this.autoCollecting = autoCollecting;
            }
        }
    }
}