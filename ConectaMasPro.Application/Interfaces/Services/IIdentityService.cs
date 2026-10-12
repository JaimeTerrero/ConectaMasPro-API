namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<bool> TieneDosFactoresHabilitadoAsync(Guid identityUserId);
    }
}
