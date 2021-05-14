using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public static class SkillUtility
    {
        /// <summary>
        /// 타겟에게 대미지를 준다.
        /// </summary>
        public static void SimpleDamage(Unit attackedUnit, Unit target, float damage)
        {
            target.DealDamage(attackedUnit, damage);
        }

        /// <summary>
        /// 특정위치에 범위공격을 가한다.
        /// mySide : 공격하는 사람의 진영. 반대 side의 유닛에게 대미지를 준다.
        /// </summary>
        public static void SimpleRangeDamage(Unit attackedUnit, Vector3 position, float radius, UnitSide mySide, float damage, string hitEffectPath, string hitSoundPath)
        {
            var units = KUtils.GetOverlapUnitsNonAlloc(position, radius);
            KUtils.DebugDrawSphere(position, radius);

            for (int i = 0 ; i < units.Count ; i++)
            {
                //자기 자신이면 무시한다.
                if (units[i] == attackedUnit)
                    continue;

                //유닛끼리 서로 다른 진영일경우, 대미지를 준다
                if (units[i].unitSide != mySide)
                {
                    units[i].DealDamage(attackedUnit, Mathf.RoundToInt(Calculator.RandomDamage(damage)));


                    ///////////////////////////////////////////
                    // 이펙트 처리
                    bool useHitEffect = !string.IsNullOrEmpty(hitEffectPath);
                    bool useHitSound = !string.IsNullOrEmpty(hitSoundPath);

                    // 이펙트를 재생해야 한다면
                    if (useHitEffect || useHitSound)
                    {
                        Vector3 targetPoint; // 히트지점을 계산해야됨.

                        // 발화점과, 히트 유닛간의 레이를 쏜다.
                        Ray ray = new Ray(position, units[i].transform.position - position);
                        RaycastHit hit;

                        Debug.DrawRay(ray.origin, ray.direction, Color.red, 3);
                        if (Physics.Raycast(ray, out hit, radius, Layers.Unit, QueryTriggerInteraction.Collide))
                        {
                            // 레이로 검출이 된다면 검출된 지점에 히트이펙트
                            targetPoint = hit.point;
                        }
                        else
                        {
                            // 레이로 검출이 되지 않는다면 맞은 유닛에 그냥 표시
                            targetPoint = units[i].pivots.center.position;
                        }


                        // 이펙트가 필요하면 생성
                        if (useHitEffect)
                        {
                            ParticleSystem ps = ObjectPoolManager.inst.Get<ParticleSystem>(hitEffectPath);
                            ps.transform.position = targetPoint;
                            ps.transform.localRotation = Quaternion.identity;
                            ps.transform.localScale = Vector3.one;
                            ps.Play();
                        }

                        // 사운드가 필요하면 재생
                        if (useHitSound)
                        {
                            SoundManager.inst.PlaySound(hitSoundPath, targetPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 부채꼴 모양으로 Projectile을 발사해준다.
        /// 코루틴으로 호출할 것.
        /// </summary>
        /// <param name="owner">발사하는 유닛</param>
        /// <param name="startPos">발사 시작위치</param>
        /// <param name="power">발사 파워(speed)</param>
        /// <param name="wayCount">발사방향 개수</param>
        /// <param name="range">발사 각도(0, 360 사이값만 넣을것)</param>
        /// <param name="fireCount">발사횟수</param>
        /// <param name="fireCycle">발사 간격</param>
        /// <param name="isCross">연속으로 발사할때 Projectiled을 크로스 시킬것인지</param>
        /// <param name="effectPath">이펙트가 필요하면 이펙트 경로</param>
        /// <param name="soundPath">사운드가 필요하면 사운드 경로</param>
        /// <returns></returns>
        public static IEnumerator FireProjectile_CircularSector(Unit owner, Vector3 startPos, float power, float lifeTime, int wayCount, float range, int fireCount, float fireCycle, bool isCross = false, string soundPath = null)
        {
            float angle = range / (wayCount - 1);
            float halfRange = range * 0.5f;


            for (int i = 0 ; i < fireCount ; i++)
            {
                float deltaAngle = -halfRange;

                // 짝수이고 isCross활성화 시, 교차해서 발사한다.
                if (isCross && (i % 2) == 0)
                    deltaAngle = 0;

                for (int j = 0 ; j < wayCount ; j++)
                {
                    // 불릿 생성
                    var bullet = ObjectPoolManager.inst.Get<StandardBullet>(PrefabPath.Projectile.StandardBullet);

                    bullet.transform.position = startPos;
                    bullet.transform.rotation = Quaternion.AngleAxis(deltaAngle, Vector3.up) * Quaternion.Euler(0, owner.transform.eulerAngles.y, 0);
                    deltaAngle += angle;

                    // 불릿 초기화 및 발사
                    bullet.Init(owner, power, lifeTime);
                    bullet.FireDirection();
                }

                // 사운드를 재생해야 하면
                if (!string.IsNullOrEmpty(soundPath))
                    SoundManager.inst.PlaySound(soundPath, startPos);
                yield return new WaitForSeconds(fireCycle);
            }
        }

        /// <summary>
        /// Parabola로 이동(Transform)
        /// </summary>
        public static IEnumerator Parabola(Transform target, Vector3 targetPos, float angle = 45)
        {
            float targetDist = Vector3.Distance(target.position, targetPos);
            float vel = targetDist / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / -Physics.gravity.y);

            float vX = Mathf.Sqrt(vel) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float vY = Mathf.Sqrt(vel) * Mathf.Sin(angle * Mathf.Deg2Rad);

            float flightDuration = targetDist / vX;

            target.rotation = Quaternion.LookRotation(targetPos - target.position);


            float elapse_time = 0;
            while (elapse_time < flightDuration)
            {
                target.Translate(0, (vY - (-Physics.gravity.y * elapse_time)) * Time.deltaTime, vX * Time.deltaTime);

                elapse_time += Time.deltaTime;

                yield return null;
            }
        }
    }
}