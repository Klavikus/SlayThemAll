using System;
using System.Collections.Generic;

namespace CodeBase.GameCore.Infrastructure.Services
{
    public class ModelProvider : IModelProvider
    {
        private readonly Dictionary<Type, object> _modelByType = new Dictionary<Type, object>();

        public void Bind<TModel>(TModel model) where TModel : class =>
            _modelByType[typeof(TModel)] = model;

        public TModel Get<TModel>() where TModel : class =>
            _modelByType[typeof(TModel)] as TModel;
    }
}