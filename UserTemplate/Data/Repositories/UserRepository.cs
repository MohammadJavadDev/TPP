using Npgsql;
using RegistrationApi.Data.Interfaces;
using RegistrationApi.Models;
using System.Data;

namespace RegistrationApi.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM get_all_users()", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FullName = reader.GetString(reader.GetOrdinal("fullname")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Phone = reader.GetString(reader.GetOrdinal("phone")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
                            });
                        }
                    }
                }
            }

            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM get_user_by_id(@id)", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FullName = reader.GetString(reader.GetOrdinal("fullname")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Phone = reader.GetString(reader.GetOrdinal("phone")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT create_user(@fullname, @email, @phone, @password)", connection))
                {
                    command.Parameters.AddWithValue("@fullname", user.FullName);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);
                    command.Parameters.AddWithValue("@password", user.Password);

                    // The function returns the new user's ID
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT update_user(@id, @fullname, @email, @phone, @password)", connection))
                {
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@fullname", user.FullName);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);
                    command.Parameters.AddWithValue("@password", user.Password);

                    // The function returns boolean indicating success
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToBoolean(result);
                }
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT delete_user(@id)", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    // The function returns boolean indicating success
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToBoolean(result);
                }
            }
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT check_email_exists(@email, @excludeId)", connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@excludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);

                    // The function returns boolean indicating if email exists
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToBoolean(result);
                }
            }
        }
    }
}