using System;
using System.Collections.Generic;
using CodeBase.GameCore.Domain.Abilities;
using CodeBase.GameCore.Domain.Abilities.Attack;
using CodeBase.GameCore.Domain.Abilities.Movement;
using CodeBase.GameCore.Domain.Abilities.Size;
using CodeBase.GameCore.Domain.Enums;
using CodeBase.GameCore.Infrastructure.Pools;
using CodeBase.GameCore.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameCore.Infrastructure.Factories
{
    public class AbilityProjectionBuilder
    {
        private readonly ITargetService _targetFinderService;
        private readonly IAudioPlayerService _audioPlayerService;
        private readonly ProjectionPool _projectionPool;
        private readonly Transform _poolContainer;

        private Dictionary<MoveType, Type> _dictionaryMoveTypes = new Dictionary<MoveType, Type>()
        {
            [MoveType.Orbital] = typeof(OrbitalMove),
            [MoveType.OrbitalMovePoint] = typeof(OrbitalMovePoint),
            [MoveType.MoveUp] = typeof(MoveUp),
            [MoveType.Follow] = typeof(Follow),
        };

        private Dictionary<AttackType, Type> _dictionaryAttackTypes = new Dictionary<AttackType, Type>()
        {
            [AttackType.Continuous] = typeof(ContinuousAttack),
            [AttackType.Periodical] = typeof(PeriodicalAttack),
            [AttackType.Single] = typeof(SingleAttack),
        };

        private Dictionary<SizeType, Type> _dictionarySizeTypes = new Dictionary<SizeType, Type>()
        {
            [SizeType.Constant] = typeof(ConstantSize),
            [SizeType.OverLifetime] = typeof(SizeOverLifetime),
            [SizeType.OverLifetimeFixed] = typeof(SizeOverLifetimeFixed),
        };

        private readonly Dictionary<AbilityData, ProjectionPool> _projectionPools =
            new Dictionary<AbilityData, ProjectionPool>();

        public AbilityProjectionBuilder(ITargetService targetService, IAudioPlayerService audioPlayerService)
        {
            _targetFinderService = targetService;
            _audioPlayerService = audioPlayerService;
            _poolContainer = GameObject.Instantiate(new GameObject("poolContainer")).transform;
        }

        public ProjectionPool GetOrCreateProjectionPool(AbilityData abilityConfigSo)
        {
            if (_projectionPools.ContainsKey(abilityConfigSo))
            {
                _projectionPools[abilityConfigSo].Clear();
                return _projectionPools[abilityConfigSo];
            }

            ProjectionPool projectionPool = new ProjectionPool(_poolContainer, abilityConfigSo.AbilityView);
            _projectionPools.Add(abilityConfigSo, projectionPool);
            return projectionPool;
        }

        public AbilityProjection[] Build(AbilityData abilityData, Transform pivotObject, ProjectionPool projectionPool)
        {
            AbilityProjection[] projections = projectionPool.GetProjections(abilityData.SpawnCount);

            for (var i = 0; i < projections.Length; i++)
            {
                projections[i].Initialize(
                    _audioPlayerService,
                    _targetFinderService,
                    abilityData,
                    GetAttackBehaviour(abilityData),
                    GetMovementBehaviour(abilityData),
                    GetSizeBehaviour(abilityData),
                    GetSpawnData(abilityData, pivotObject, i));
            }

            return projections;
        }

        private ISizeBehaviour GetSizeBehaviour(AbilityData abilityConfig)
        {
            Type sizeType = _dictionarySizeTypes.ContainsKey(abilityConfig.SizeBehaviourData.SizeType)
                ? _dictionarySizeTypes[abilityConfig.SizeBehaviourData.SizeType]
                : throw new NullReferenceException(nameof(sizeType));

            return Activator.CreateInstance(sizeType) as ISizeBehaviour;
        }

        private IAttackBehaviour GetAttackBehaviour(AbilityData abilityConfig)
        {
            Type attackType = _dictionaryAttackTypes.ContainsKey(abilityConfig.AttackType)
                ? _dictionaryAttackTypes[abilityConfig.AttackType]
                : throw new NullReferenceException(nameof(attackType));

            return Activator.CreateInstance(attackType, new object[] {abilityConfig}) as IAttackBehaviour;
        }

        private IMovementBehaviour GetMovementBehaviour(AbilityData abilityConfig)
        {
            Type movementType = _dictionaryMoveTypes.ContainsKey(abilityConfig.MoveType)
                ? _dictionaryMoveTypes[abilityConfig.MoveType]
                : throw new NullReferenceException(nameof(movementType));

            return Activator.CreateInstance(movementType) as IMovementBehaviour;
        }

        private SpawnData GetSpawnData(AbilityData abilityData, Transform pivotObject, int i)
        {
            Vector3 enemyPositionFromTargetService =
                _targetFinderService.GetClosestEnemyToPlayer(abilityData.AttackRadius,
                    abilityData.WhatIsEnemy.layerMask);
            Vector3 directionToClosest = (enemyPositionFromTargetService - pivotObject.position).normalized;
            Vector3 enemyPosition = Vector3.zero;

            if (abilityData.TargetingType == TargetingType.ByDirection)
                directionToClosest = _targetFinderService.GetPlayerDirection();

            if (abilityData.TargetingType == TargetingType.ToClosest)
            {
                enemyPosition = enemyPositionFromTargetService;

                directionToClosest = (enemyPositionFromTargetService - pivotObject.position).normalized;

                if (enemyPositionFromTargetService == Vector3.zero)
                    directionToClosest = _targetFinderService.GetPlayerDirection();
            }

            if (abilityData.TargetingType == TargetingType.RandomTarget)
            {
                enemyPosition = _targetFinderService.GetRandomEnemyPosition();
                directionToClosest = (enemyPosition - pivotObject.position).normalized;
                if (enemyPositionFromTargetService == Vector3.zero)
                    directionToClosest = _targetFinderService.GetPlayerDirection();
            }

            float distanceToTarget = (enemyPositionFromTargetService - pivotObject.position).magnitude;

            Vector3 startPosition = Vector3.zero;
            float newOffset = 0;

            switch (abilityData.SpawnPosition)
            {
                case SpawnType.Point:
                    // Vector3 startPosition = pivotObject.position + directionToClosest * abilityData.Radius;
                    startPosition = enemyPosition;
                    if (abilityData.MoveType == MoveType.Orbital)
                        newOffset = 360f / abilityData.SpawnCount * i;
                    else
                        newOffset = abilityData.Radius;


                    return new SpawnData(pivotObject, i, newOffset, startPosition, directionToClosest);

                case SpawnType.PivotPoint:
                    startPosition = pivotObject.position + directionToClosest * abilityData.Radius;
                    if (abilityData.MoveType == MoveType.Orbital)
                        newOffset = 360f / abilityData.SpawnCount * i;
                    else
                        newOffset = abilityData.Radius;


                    return new SpawnData(pivotObject, i, newOffset, startPosition, directionToClosest);

                case SpawnType.Circle:
                    float degreeStep = 360f / abilityData.SpawnCount;
                    float offset = degreeStep * i;
                    Vector3 newPosition = new Vector3(
                        Mathf.Cos(offset * Mathf.Deg2Rad) * abilityData.Radius + pivotObject.position.x,
                        Mathf.Sin(offset * Mathf.Deg2Rad) * abilityData.Radius + pivotObject.position.y,
                        0);
                    Vector3 newDirection = (newPosition - pivotObject.position).normalized;
                    return new SpawnData(pivotObject, i, offset, newPosition, newDirection);

                case SpawnType.Arc:
                    float arcDegrees = abilityData.Arc;
                    float arcStep = arcDegrees / abilityData.SpawnCount;
                    float arcOffset = arcStep / 2;
                    float degreeOffset = abilityData.Arc / 2 - arcOffset - arcStep * i;
                    Vector3 startPos = pivotObject.position +
                                       Quaternion.Euler(0, 0, degreeOffset) * directionToClosest *
                                       abilityData.Radius;
                    Vector3 newDir = (startPos - pivotObject.position).normalized;
                    return new SpawnData(pivotObject, i, abilityData.Radius, startPos, newDir);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}