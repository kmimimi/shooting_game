using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public static class TimeManager
    {
        public static float deltaTime      => Time.deltaTime;
        public static float fixedDeltaTime => Time.fixedDeltaTime;
    }
}