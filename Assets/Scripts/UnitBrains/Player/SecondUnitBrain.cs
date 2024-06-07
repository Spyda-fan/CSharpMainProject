using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;

        private static int unitCounter = 0;
        private int unitNumber;
        private const int maxTargets = 3;

        private List<Vector2Int> TargetsOutOfReach = new List<Vector2Int>(); 
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            float currentTemperature = GetTemperature();

            if (currentTemperature >= overheatTemperature)
                return;

            for (int i = 0; i <= currentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            ///////////////////////////////////////
        }
        public override Vector2Int GetNextStep()
        {
            if (TargetsOutOfReach.Count <= 0 || IsTargetInRange(TargetsOutOfReach[0]))
                return unit.Pos;
            else 
                return unit.Pos.CalcNextStepTowards(TargetsOutOfReach[0]);
        }
        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int targetPosition;

            TargetsOutOfReach.Clear();

            foreach (Vector2Int target in GetAllTargets())
            {
                TargetsOutOfReach.Add(target);
            }

            if (TargetsOutOfReach.Count == 0)
            {
                int enemyBaseId = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId;
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyBaseId];
                TargetsOutOfReach.Add(enemyBase);
            }
            else
            {
                SortByDistanceToOwnBase(TargetsOutOfReach);

                int targetIndex = unitNumber % maxTargets;

                if (targetIndex > (TargetsOutOfReach.Count - 1))
                {
                    targetPosition = TargetsOutOfReach[0];
                }
                else
                {
                    if (targetIndex == 0)
                    {
                        targetPosition = TargetsOutOfReach[targetIndex];
                    }
                    else
                    {
                        targetPosition = TargetsOutOfReach[targetIndex - 1];
                    }

                }

                if (IsTargetInRange(targetPosition))
                    result.Add(targetPosition);
            }

            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }
        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }
        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}
