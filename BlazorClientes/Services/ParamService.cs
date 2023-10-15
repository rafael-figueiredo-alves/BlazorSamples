using BlazorClientes.Services.Interfaces;

namespace BlazorClientes.Services
{
    public class ParamService : IParamService
    {
        private object? Parametro { get; set; } = null;

        public object? GetParam()
        {
            return Parametro;
        }

        public void setParam(object? value)
        {
            Parametro = value;
        }
    }
}
