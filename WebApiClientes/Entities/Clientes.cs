namespace WebApiClientes.Entities
{
    public class Clientes
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Endereco { get; set;}
        public string? Telefone { get;}
        public string? Celular { get; set;}
        public string? Email { get; set;}

        public Clientes(int _id, string _nome,  string _endereco, string _telefone, string _celular, string _email)
        {
            Id = _id;
            Nome = _nome;
            Endereco = _endereco;
            Telefone = _telefone;
            Celular = _celular;
            Email = _email;
        }
    }
}
