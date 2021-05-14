using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KShooting
{
    /// <summary>
    /// 오른쪽에 퀘스트 진행상황을 간단하게 볼수 있는뷰어
    /// </summary>
    public class QuestQuickView : MonoBehaviour
    {
        [SerializeField] private RectTransform layoutRoot = null;

        private List<QuestQuickView_Element> elements = new List<QuestQuickView_Element>();


        private void OnEnable()
        {
            // 기존에 있던 Element는 모두 지운다
            for (int i = 0 ; i < this.elements.Count ; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);

            this.elements.Clear();

            // 현재 퀵뷰 정보를 받아와 Element를 생성한다
            for (int i = 0 ; i < QuestManager.inst.quickViewQuests.Count ; i++)
            {
                AddObserving(QuestManager.inst.quickViewQuests[i]);
            }

            // 퀵뷰 변경점을 확인할 수 있게 이벤트를 등록한다
            QuestManager.inst.onAddQuestQuickView += AddObserving;
            QuestManager.inst.onRemoveQuestQuickView += RemoveObserving;
        }

        private void OnDisable()
        {
            // 오브젝트가 비활성화 되면 이벤트를 제거한다.
            if(QuestManager.inst != null)
            {
                QuestManager.inst.onAddQuestQuickView -= AddObserving;
                QuestManager.inst.onRemoveQuestQuickView -= RemoveObserving;
            }
        }

        /// <summary>
        /// 퀘스틀를 퀵뷰에 표시한다.
        /// </summary>
        private void AddObserving(string questKey)
        {
            QuestQuickView_Element element = ObjectPoolManager.inst.New<QuestQuickView_Element>(PrefabPath.UI.QuestQuickView_Element);
            element.transform.SetParent(this.layoutRoot, false);
            (element.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            element.transform.localRotation = Quaternion.identity;
            element.transform.localScale = Vector3.one;

            element.Init(questKey);

            this.elements.Add(element);
        }

        /// <summary>
        /// 퀘스트를 퀵뷰에서 제거한다
        /// </summary>
        private void RemoveObserving(string questKey)
        {
            for(int i=0; i<this.elements.Count; i++)
            {
                if(this.elements[i].key == questKey)
                {
                    ObjectPoolManager.inst.Return(this.elements[i].gameObject);
                    this.elements.RemoveAt(i);
                    break;
                }
            }
        }
    }
}