using MVC_Project.Models.Auth;

namespace MVC_Project.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);

    }
}