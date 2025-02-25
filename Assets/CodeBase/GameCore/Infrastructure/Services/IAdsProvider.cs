﻿using System;

namespace CodeBase.GameCore.Infrastructure.Services
{
    public interface IAdsProvider : IService
    {
        event Action Opened;
        event Action Rewarded;
        event Action Closed;
        event Action Initialized;

        bool IsInitialized { get; }
        void Initialize();
        void ShowAds(Action onOpenCallback = null, Action onCloseCallback = null, Action onRewardCallback = null);
    }
}