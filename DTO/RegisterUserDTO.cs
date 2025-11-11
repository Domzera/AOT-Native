namespace WebApplication1.DTO
{
    /// <summary>
    /// O RegisterUserDTO é uma classe de transferência de dados (DTO) usada para encapsular as informações
    /// necessárias para registrar um novo usuário no sistema.
    /// Ela não contém o ID do usuário, pois este será gerado automaticamente pelo sistema ao criar o registro.
    /// Já o ATIVO é definido como 1 por padrão, indicando que o usuário está ativo ao ser registrado.
    /// Quando o USUARIO é deletado, na verdade ele não é removido do banco de dados, apenas o campo ATIVO é alterado para 0,
    /// E a DATA é definida como a data e hora atuais no momento do registro automaticamente.
    /// O PASSWORD e o EMAIL são obrigatórios para garantir a segurança do acesso do usuário.
    /// Por causa do PASSWORD ser obrigatório, na hora de alterar o cadastro ele é exigido também; Por isso na classe
    /// RREPOSITO.CS é preciso fazer a pesquisa do usuário antes de alterar os dados para que o PASSWORD não fique nulo,
    /// evitando assim um erro de validação.
    /// </summary>
    public class RegisterUserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string CPF { get; set; }
        public int Ativo { get; set; } = 1;
        public DateTime data { get; set; } = DateTime.Now;
    }
}
