using RegistrationApi.Business.Interfaces;
using RegistrationApi.Data.Interfaces;
using RegistrationApi.Models;
using RegistrationApi.Utilities;

namespace RegistrationApi.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            // Remove sensitive data
            foreach (var user in users)
            {
                user.Password = string.Empty;
            }
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                // Remove sensitive data
                user.Password = string.Empty;
            }

            return user;
        }

        public async Task<(bool Success, string Message, int? UserId)> CreateUserAsync(User user)
        {
            // Validate user input
            if (string.IsNullOrWhiteSpace(user.FullName))
                return (false, "Full name is required.", null);

            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "Email is required.", null);

            if (string.IsNullOrWhiteSpace(user.Phone))
                return (false, "Phone number is required.", null);

            if (string.IsNullOrWhiteSpace(user.Password))
                return (false, "Password is required.", null);

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(user.Email))
                return (false, "Email already exists.", null);

            // Hash the password
            user.Password = PasswordHasher.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;

            // Create user
            int newUserId = await _userRepository.CreateUserAsync(user);

            return (true, "User created successfully.", newUserId);
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(User user)
        {
            // Validate user input
            if (user.Id <= 0)
                return (false, "Invalid user ID.");

            if (string.IsNullOrWhiteSpace(user.FullName))
                return (false, "Full name is required.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "Email is required.");

            if (string.IsNullOrWhiteSpace(user.Phone))
                return (false, "Phone number is required.");

            // Check if user exists
            var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
            if (existingUser == null)
                return (false, "User not found.");

            // Check if email already exists (excluding current user)
            if (await _userRepository.EmailExistsAsync(user.Email, user.Id))
                return (false, "Email already used by another user.");

            // Only hash password if it's provided (if not, use existing)
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = existingUser.Password;
            }
            else
            {
                user.Password = PasswordHasher.HashPassword(user.Password);
            }

            // Keep the original creation date
            user.CreatedAt = existingUser.CreatedAt;

            // Update user
            bool result = await _userRepository.UpdateUserAsync(user);

            return result
                ? (true, "User updated successfully.")
                : (false, "Failed to update user.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            // Check if user exists
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                return (false, "User not found.");

            // Delete user
            bool result = await _userRepository.DeleteUserAsync(id);

            return result
                ? (true, "User deleted successfully.")
                : (false, "Failed to delete user.");
        }
    }
}