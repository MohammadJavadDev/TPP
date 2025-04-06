using RegistrationApi.Models;

namespace RegistrationApi.Business.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<(bool Success, string Message, int? UserId)> CreateUserAsync(User user);
        Task<(bool Success, string Message)> UpdateUserAsync(User user);
        Task<(bool Success, string Message)> DeleteUserAsync(int id);
    }
}