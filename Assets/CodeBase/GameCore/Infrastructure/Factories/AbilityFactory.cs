using CodeBase.GameCore.Configs;
using CodeBase.GameCore.Domain.Abilities;
using CodeBase.GameCore.Domain.Data;
using CodeBase.GameCore.Infrastructure.Pools;
using CodeBase.GameCore.Infrastructure.Services;

namespace CodeBase.GameCore.Infrastructure.Factories
{
    public class AbilityFactory
    {
        private readonly AbilityProjectionBuilder _abilityProjectionBuilder;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly AbilityUpgradesProvider _abilityUpgradesProvider;
        private readonly ProjectionPool _projectionPool;
        private IGameLoopService _gameLoopService;

        public AbilityFactory(AbilityProjectionBuilder abilityProjectionBuilder,
            ICoroutineRunner coroutineRunner,
            AbilityUpgradesProvider abilityUpgradesProvider)
        {
            _abilityProjectionBuilder = abilityProjectionBuilder;
            _coroutineRunner = coroutineRunner;
            _abilityUpgradesProvider = abilityUpgradesProvider;
        }

        public void BindGameLoopService(IGameLoopService gameLoopService) => _gameLoopService = gameLoopService;

        public Ability Create(AbilityConfigSO initialAbilityConfig)
        {
            // AbilityData abilityData = _abilityUpgradesProvider.ConfigsByAbilityData[initialAbilityConfig];
            AbilityData abilityData = new AbilityData(initialAbilityConfig);
            //TODO: Replace _abilityProjectionBuilder.GetOrCreateProjectionPool() to projectionPoolFactory.Create()
            return Create(abilityData, _abilityProjectionBuilder.GetOrCreateProjectionPool(abilityData),
                initialAbilityConfig.UpgradeData);
        }

        private Ability Create(AbilityData abilityConfig, ProjectionPool projectionPool,
            AbilityUpgradeData[] upgradesData) =>
            new(_abilityProjectionBuilder, abilityConfig, _coroutineRunner, projectionPool, upgradesData,
                _gameLoopService);
    }
}