using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 게임내 사용되는 스프라이트 이미지들 관리
    /// 필요한것만 일단 최소한으로 구현
    /// </summary>
    public static class SpriteManager
    {
        public static Sprite Get(string path)
        {
            return Resources.Load<Sprite>(path);
        }
    }
}