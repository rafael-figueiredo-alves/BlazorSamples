namespace WebApiClientes.Entities
{
    public class Erro
    {
        public string? Mensagem { get; set; }
        public string? Info { get; set; }
        public Erro(string? _msg, string? _info)
        {
            Mensagem = _msg;
            Info = _info;
        }
    }
}
