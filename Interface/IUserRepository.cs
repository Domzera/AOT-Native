using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;

namespace WebApplication1.Interface
{
    public interface IUserRepository
    {
        Task<RegisterUserDTO> Add(RegisterUserDTO usuario);
        bool Delete(int id);
        Task<UserDTO> Update(int userId, [FromBody] RegisterUserDTO usuario);
        Task<UserDTO> GetByName(string name);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<List<UserDTO>> GetActiveUsersAsync();
        Task<UserDTO> GetUserById(int id);
    }
}