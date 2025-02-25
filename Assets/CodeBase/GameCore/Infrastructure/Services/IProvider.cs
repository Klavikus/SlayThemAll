namespace CodeBase.GameCore.Infrastructure.Services
{
    public interface IProvider : IService
    {
        public void Bind<TModel>(TModel model) where TModel : class;
        TModel Get<TModel>() where TModel : class;
    }
}