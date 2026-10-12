using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace ConectaMasPro.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> TieneDosFactoresHabilitadoAsync(
            Guid identityUserId)
        {
            var identityUser = await _userManager.FindByIdAsync(
                identityUserId.ToString());

            return identityUser?.TwoFactorEnabled ?? false;
        }
    }
}
