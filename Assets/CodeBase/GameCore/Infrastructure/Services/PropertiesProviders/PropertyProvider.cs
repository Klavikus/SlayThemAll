using System;
using System.Collections.Generic;
using CodeBase.GameCore.Configs;
using CodeBase.GameCore.Domain.Data;
using CodeBase.GameCore.Domain.Models;
using CodeBase.GameCore.Infrastructure.Services.UpgradeService;
using CodeBase.GameCore.Presentation.Views.HeroSelector;

namespace CodeBase.GameCore.Infrastructure.Services.PropertiesProviders
{
    public class PropertyProvider : IPropertyProvider
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IUpgradeService _upgradeService;
        private readonly IModelProvider _modelProvider;

        private MainProperties _baseData;
        private MainProperties _upgradesData;
        private MainProperties _heroData;
        private MainProperties _resultData;

        public event Action PropertiesUpdated;

        public PropertyProvider(
            IConfigurationProvider configurationProvider,
            IUpgradeService upgradeService,
            IModelProvider modelProvider)
        {
            _configurationProvider = configurationProvider;
            _upgradeService = upgradeService;
            _modelProvider = modelProvider;
        }

        public void Initialize()
        {
            _baseData = _configurationProvider.BasePropertiesConfig.GetPropertiesData();
            _upgradesData = _upgradeService.GetUpgradesPropertiesData();
            _heroData = _modelProvider.Get<HeroModel>().GetMainPropertiesData();
            RecalculateData();
            
            _upgradeService.Updated += OnUpgradesUpdated;
            _modelProvider.Get<HeroModel>().Changed += OnHeroChanged;
        }

        public MainProperties GetResultProperties() => _resultData;

        public MainPropertyViewData[] GetResultPropertiesViewData()
        {
            BasePropertiesConfigSO baseConfigurationSO = _configurationProvider.BasePropertiesConfig;
            IReadOnlyList<MainPropertyViewData> baseViews = baseConfigurationSO.PropertiesData;
            MainPropertyViewData[] result = new MainPropertyViewData[baseViews.Count];
          
            for (int i = 0; i < baseViews.Count; i++)
            {
                result[i] = baseViews[i];
                result[i].Value = _resultData.BaseProperties[result[i].BaseProperty];
            }

            return result;
        }

        public PropertyView GetBasePropertyView() => _configurationProvider.BasePropertiesConfig.PropertyView;

        private void OnHeroChanged(HeroData heroData)
        {
            _heroData = _modelProvider.Get<HeroModel>().GetMainPropertiesData();
            RecalculateData();
        }

        private void OnUpgradesUpdated()
        {
            _upgradesData = _upgradeService.GetUpgradesPropertiesData();
            RecalculateData();
        }

        private void RecalculateData()
        {
            _resultData = _baseData + _upgradesData + _heroData;
            _modelProvider.Get<PropertiesModel>().SetResultProperties(_resultData);
            PropertiesUpdated?.Invoke();
        }
    }
}