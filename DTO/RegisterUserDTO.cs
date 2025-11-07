namespace WebApplication1.DTO
{
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
