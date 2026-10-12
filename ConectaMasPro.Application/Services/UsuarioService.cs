using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Usuarios;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> CrearAsync(CrearUsuarioRequest request, CancellationToken cancellationToken = default)
        {
            if (request.IdentityUserId == Guid.Empty)
                return Result<Guid>.Fail("El IdentityUserId es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                return Result<Guid>.Fail("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Apellido))
                return Result<Guid>.Fail("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<Guid>.Fail("El email es obligatorio.");

            var email = request.Email.Trim().ToLower();

            var existeEmail = await _unitOfWork
                .Repository<Usuario>()
                .FindAsync(x => x.Email == email && x.Activo, cancellationToken);

            if (existeEmail.Any())
                return Result<Guid>.Fail("Ya existe un perfil de usuario con ese correo.");

            var existeIdentityUser = await _unitOfWork
                .Repository<Usuario>()
                .FindAsync(x => x.IdentityUserId == request.IdentityUserId && x.Activo, cancellationToken);

            if (existeIdentityUser.Any())
                return Result<Guid>.Fail("Ya existe un perfil asociado a ese usuario de Identity.");

            var usuario = new Usuario
            {
                IdentityUserId = request.IdentityUserId,
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Email = email,
                Telefono = request.Telefono?.Trim(),
                TipoUsuario = TipoUsuario.Ambos
            };

            await _unitOfWork.Repository<Usuario>().AddAsync(usuario, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Ok(usuario.Id);
        }

        public async Task<Result<UsuarioDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(id, cancellationToken);

            if (usuario is null || !usuario.Activo)
                return Result<UsuarioDto>.Fail("Usuario no encontrado.");

            return Result<UsuarioDto>.Ok(new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono
            });
        }
    }
}
