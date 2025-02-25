﻿using CodeBase.GameCore.Infrastructure.Factories;
using UnityEngine;

namespace CodeBase.GameCore.Infrastructure.Services
{
    public interface ITargetService : IService
    {
        Vector3 GetPlayerPosition();
        Vector3 GetPlayerDirection();
        Vector3 GetClosestEnemyToPlayer(float radius, LayerMask layerMask);
        void BindPlayerBuilder(PlayerBuilder playerBuilder);
        Camera GetCamera();
        Vector3 GetRandomEnemyPosition();
    }
}