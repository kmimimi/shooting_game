using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KShooting
{
    public static class KUtils
    {
        private static List<Unit> _units = new List<Unit>(100);
        private static Collider[] _colliders = new Collider[100];

        /// <summary>
        /// 디버깅(범위표시)할때 사용할 Material;
        /// </summary>
        private static Material _debugMaterial;

        public static StringBuilder stringBuilder = new StringBuilder();


        public static string GetThousandCommaText(int data)
        {
            if (data == 0)
                return "0";

            return string.Format("{0:#,###}", data);
        }
        public static string GetThousandCommaText(long data)
        {
            if (data == 0)
                return "0";

            return string.Format("{0:#,###}", data);
        }

        public static bool IsZero(this Vector3 value)
        {
            return Mathf.Approximately(0, value.x)
                && Mathf.Approximately(0, value.y)
                && Mathf.Approximately(0, value.z);
        }

        public static bool IsZero(this Vector2 value)
        {
            return Mathf.Approximately(0, value.x)
                && Mathf.Approximately(0, value.y);
        }

        public static Vector3 SamplePosition_NavMesh(Vector3 pos)
        {
            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 10, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }

            return pos;
        }

        /// <summary>
        /// 메모리 할당없이 오버랩 되는 유닛들을 모두 가져온다.
        /// 검색용으로만 사용할것.
        /// </summary>
        public static List<Unit> GetOverlapUnitsNonAlloc(Vector3 position, float range)
        { 
            _units.Clear();


            int count = Physics.OverlapSphereNonAlloc(position, range, _colliders, Layers.Unit);

            for(int i=0; i<count; i++)
            {
                Unit unit = _colliders[i].GetComponentInParent<Unit>();
                if(unit != null)
                {
                    if (!_units.Contains(unit))
                        _units.Add(unit);
                }
            }

            return _units;
        }

        /// <summary>
        /// 디버그 모드일때 범위를 표시해준다.
        /// </summary>
        public static void DebugDrawSphere(Vector3 position, float radius)
        {
#if DEBUG_MODE
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * radius * 2;

            // 디버깅용 메테리얼이 없으면 생성
            if(_debugMaterial == null)
            {
                _debugMaterial = new Material(Shader.Find("UI/Default"));
                _debugMaterial.color = new Color(0, 0.8f, 0, 0.5f);
            }

            sphere.GetComponent<Renderer>().material = _debugMaterial;
            Object.Destroy(sphere.GetComponent<Collider>());
            Object.Destroy(sphere, 1);
#endif
        }
    }
}