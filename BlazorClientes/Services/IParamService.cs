namespace BlazorClientes.Services
{
    public interface IParamService
    {
        object? GetParam();
        void setParam(object? value);
    }
}
