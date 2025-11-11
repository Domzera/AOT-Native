namespace WebApplication1.DTO
{
    /// <summary>
    /// O USERUPDATE é uma classe que representa um objeto de transferência de dados (DTO) para um usuário.
    /// O PASSWORD e o EMAIL são obrigatórios para garantir a segurança do acesso do usuário.
    /// Por causa do PASSWORD ser obrigatório, na hora de alterar o cadastro ele é exigido também; Por isso na classe
    /// RREPOSITO.CS é preciso fazer a pesquisa do usuário antes de alterar os dados para que o PASSWORD não fique nulo,
    /// evitando assim um erro de validação.
    /// </summary>
    public class UserUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string CPF { get; set; }
        public DateTime data { get; set; } = DateTime.Now;
    }
}
