using System;
namespace FirstApp.Services
{
    public interface IRazorRenderer
    {
        string RenderPartialToString<TModel>(string partialName, TModel model);
    }
}
