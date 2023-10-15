namespace BlazorClientes.Services.Interfaces
{
    public interface IParamService
    {
        object? GetParam();
        void setParam(object? value);
    }
}
