using CodeBase.GameCore.Configs;
using CodeBase.GameCore.Infrastructure.StateMachine;
using FMODUnity;

namespace CodeBase.GameCore.Infrastructure.Services
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly ConfigurationContainer _configurationContainer;

        public ConfigurationProvider(ConfigurationContainer configurationContainer) =>
            _configurationContainer = configurationContainer;

        public BasePropertiesConfigSO BasePropertiesConfig => _configurationContainer.BasePropertiesConfigSO;
        public MainMenuConfigurationSO MainMenuConfig => _configurationContainer.MainMenuConfigurationSo;
        public HeroesConfigSO HeroConfig => _configurationContainer.HeroConfigSO;
        public UpgradesConfigSO UpgradesConfig => _configurationContainer.UpgradesConfigSO;
        public ColorConfigSO ColorsConfig => _configurationContainer.ColorConfigSO;
        public GameLoopConfigSO GameLoopConfig => _configurationContainer.GameLoopConfigSO;
        public EnemyConfigSO EnemyConfig => _configurationContainer.EnemyConfigSO;
        public VfxConfig VfxConfig => _configurationContainer.VfxConfig;
        public EventReference FMOD_HitReference => _configurationContainer.FMOD_HitReference;
        public EventReference FMOD_UpgradeBuyReference => _configurationContainer.FMOD_UpgradeBuyReference;
        public EventReference FMOD_PlayerDiedReference => _configurationContainer.FMOD_PlayerDiedReference;
        public EventReference FMOD_StartLevelReference => _configurationContainer.FMOD_StartLevelReference;
        public EventReference FMOD_GameLoopAmbientReference => _configurationContainer.FMOD_GameLoopAmbientReference;
        public EventReference FMOD_MainMenuAmbientReference => _configurationContainer.FMOD_MainMenuAmbientReference;
        public EventReference FMOD_Thunder => _configurationContainer.FMOD_Thunder;
        public StageCompetitionConfigSO StageCompetitionConfig => _configurationContainer.StageCompetitionConfigSO;
        public AbilityConfigSO[] AbilityConfigs => _configurationContainer.AbilityConfigsSO;
    }
}