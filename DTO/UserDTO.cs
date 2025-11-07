
namespace WebApplication1.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string CPF { get; set; }
        public int Ativo { get; set; }
        public DateTime data { get; set; }

    }
}
