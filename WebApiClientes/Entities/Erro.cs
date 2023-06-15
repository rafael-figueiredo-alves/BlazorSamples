namespace WebApiClientes.Entities
{
    public class Erro
    {
        public int statuscode;
        public string? message;

        public Erro(int _code, string _msg)
        {
            statuscode = _code;
            message = _msg;
        }
    }
}
