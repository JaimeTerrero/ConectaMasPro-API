# ConectaMasPro — API para una plataforma de servicios

**Proyecto de tesis desarrollado con C#, ASP.NET Core y .NET 9.**

ConectaMasPro es el backend de una plataforma que conecta a personas que necesitan un servicio con proveedores que pueden ofrecerlo. La API gestiona el recorrido desde la publicación y búsqueda de servicios hasta la solicitud, el acuerdo de precio, la confirmación de finalización, el pago y la valoración de la experiencia.

Fue creada como parte de mi proyecto de tesis para aplicar el desarrollo backend a un problema de negocio concreto, integrando persistencia relacional, autenticación, comunicación en tiempo real y una pasarela de pagos. También incorpora actividades sociales, recompensas y seguimiento del progreso de los proveedores.

## Funcionalidades

| Módulo | Alcance |
| --- | --- |
| Autenticación y cuentas | Registro, inicio de sesión, consulta del usuario autenticado, recuperación de contraseña y segundo factor por correo electrónico. |
| Usuarios y perfiles | Perfiles de cliente y proveedor, usuarios que combinan ambos tipos, administrador y actualización de fotografías. |
| Catálogo de servicios | Publicación, exploración, categorías, filtros y paginación. |
| Solicitudes | Aceptación, rechazo, cancelación, precio acordado y confirmación de finalización por el cliente. |
| Pagos | Intenciones de pago con Stripe, sincronización del estado, webhooks, comisión de plataforma, historial del cliente y ganancias del proveedor. |
| Mensajería | Conversaciones persistidas y envío de mensajes en tiempo real mediante SignalR. |
| Reputación | Reseñas y calificaciones bidireccionales entre clientes y proveedores. |
| Ubicaciones | Catálogos de provincias, municipios y sectores, y resolución de datos de ubicación recibidos del frontend a partir de Google. |
| Actividades sociales | Publicación y exploración de actividades, postulaciones y gestión de sus estados. |
| Recompensas y progreso | Movimientos de recompensas, niveles de proveedor y metas mensuales. |
| Administración y seguimiento | Dashboards, notificaciones y reportes de inconvenientes con evidencias. |
| Documentos legales | Términos y condiciones, documentos versionados y registro de aceptación. |

## Tecnologías y servicios

| Tecnología | Uso en el proyecto |
| --- | --- |
| **C# / .NET 9** | Lenguaje y plataforma; los cuatro proyectos utilizan `net9.0`. |
| **ASP.NET Core Web API** | Endpoints HTTP mediante controladores, middleware e inyección de dependencias. |
| **SQL Server** | Base de datos relacional para usuarios y módulos de negocio. |
| **Entity Framework Core 9** | Acceso a datos, consultas LINQ, relaciones, configuraciones de entidades y migraciones. |
| **ASP.NET Core Identity** | Gestión de cuentas, contraseñas y tokens para recuperación y segundo factor. |
| **JWT Bearer** | Autenticación de solicitudes y del hub de mensajería. |
| **SignalR** | Comunicación de mensajes en tiempo real mediante grupos de usuarios y conversaciones. |
| **Stripe / Stripe.net** | Integración de pagos con `PaymentIntent` y validación de firmas de webhooks. La implementación utiliza claves de prueba. |
| **MailKit / SMTP** | Envío de correos para recuperación de contraseña y autenticación de dos factores; configuración prevista para SMTP de Gmail. |
| **Swagger / OpenAPI** | Documentación interactiva de endpoints y envío de tokens Bearer en desarrollo. |
| **Almacenamiento local** | Fotografías y evidencias en `wwwroot/uploads`, con soporte para JPEG, PNG y WebP. |

## Arquitectura

El proyecto utiliza **Clean Architecture**, organizado en cuatro proyectos: **Domain, Application, Infrastructure y API**. Esta estructura separa la lógica de negocio de la persistencia y las integraciones externas, utilizando interfaces e inyección de dependencias.

## Base de datos: SQL Server y Code First

El proyecto utiliza **Code First con Entity Framework Core**: el modelo se define mediante entidades C# y configuraciones Fluent API, y sus cambios se reflejan en SQL Server mediante migraciones versionadas en `ConectaMasPro.Infrastructure/Migrations`.

`AppDbContext` hereda de `IdentityDbContext`, por lo que integra las tablas de autenticación con las entidades del negocio. Las configuraciones se cargan desde el ensamblado de Infrastructure mediante `ApplyConfigurationsFromAssembly`.

El modelo contempla usuarios y perfiles, servicios y solicitudes, pagos, reseñas, conversaciones y mensajes, ubicaciones, recompensas, actividades sociales y documentos legales. Incluye datos iniciales mediante migraciones y seeders.

## Autenticación y seguridad

- Tokens JWT firmados con **HMAC-SHA256**, con validación de firma, emisor, audiencia y expiración.
- Claims para identificar al usuario de Identity, su perfil de negocio y su tipo de usuario.
- Endpoints protegidos mediante `[Authorize]` y comprobaciones de permisos y participación dentro de los flujos de negocio.
- Segundo factor por correo con un token temporal de cinco minutos para completar el inicio de sesión. Ese token temporal no se acepta como autenticación general de la API.
- Recuperación de contraseña mediante tokens de ASP.NET Core Identity y envío de correo.
- Verificación de pertenencia a una conversación antes de incorporarse a su grupo de SignalR.
- Credenciales configuradas fuera del repositorio mediante User Secrets o variables de entorno. El administrador inicial requiere credenciales explícitas; no tiene una contraseña predeterminada.

## Aspectos técnicos destacados

El proyecto muestra la implementación de flujos que requieren coordinar varios módulos del backend:

- **Ciclo de contratación:** una solicitud tiene estados de aceptación, cancelación, confirmación de finalización y pago, con un precio acordado entre las partes.
- **Integración de pagos:** creación de intenciones con claves de idempotencia y recepción de eventos de Stripe con validación de firma.
- **Chat persistente y en tiempo real:** servicios de conversación respaldados por SQL Server y distribución de mensajes por grupos de SignalR.
- **Consultas de negocio:** filtros y paginación, consultas de lectura con `AsNoTracking` en el repositorio y operaciones asíncronas con `CancellationToken`.
- **Separación de contratos y modelos:** DTOs para solicitudes y respuestas, interfaces para servicios y repositorios, y resultados de aplicación con información de éxito o error.
- **Evolución del esquema:** migraciones que reflejan la incorporación progresiva de autenticación, perfiles, mensajería, pagos y otros módulos.

## Ejecutar en desarrollo

### Requisitos

- SDK de .NET 9.
- Una instancia de SQL Server accesible.
- Herramienta `dotnet-ef` de la versión principal 9 para aplicar las migraciones.
- Cuenta SMTP para probar correos y una cuenta de Stripe con claves de prueba para probar pagos.

### 1. Configurar los valores privados

Desde la raíz de la solución, guarda la conexión y una clave JWT nueva mediante User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "TU_CONEXION_SQL_SERVER" --project ConectaMasPro.API
dotnet user-secrets set "Jwt:Key" "TU_CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES" --project ConectaMasPro.API
```

Los valores entre comillas son marcadores: reemplázalos por tu configuración privada. La clave JWT debe ser aleatoria y tener al menos 32 bytes. User Secrets se carga al ejecutar en el entorno `Development`.

Para probar recuperación de contraseña y 2FA, configura:

```powershell
dotnet user-secrets set "Email:Usuario" "TU_USUARIO_SMTP" --project ConectaMasPro.API
dotnet user-secrets set "Email:Password" "TU_PASSWORD_SMTP" --project ConectaMasPro.API
dotnet user-secrets set "Email:CorreoRemitente" "TU_CORREO_REMITENTE" --project ConectaMasPro.API
```

Ajusta `Email:Host`, `Email:Puerto` y `Email:UrlRestablecimientoPassword` según tu proveedor y la URL del frontend.

Para probar pagos, configura `Stripe:SecretKey`, `Stripe:PublishableKey` y `Stripe:WebhookSecret` con tus valores de prueba. El endpoint receptor es `POST /api/Pagos/stripe/webhook`.

Opcionalmente, configura `AdminSeed:Email` y `AdminSeed:Password` para crear el administrador inicial. Retira esa configuración después de crearlo.

### 2. Restaurar y aplicar las migraciones

```powershell
dotnet restore
dotnet ef database update --project ConectaMasPro.Infrastructure --startup-project ConectaMasPro.API -- --environment Development
```

### 3. Iniciar la API

```powershell
dotnet run --project ConectaMasPro.API --launch-profile https
```

Swagger estará disponible en `https://localhost:7085/swagger`. El hub de mensajería utiliza la ruta `/hubs/mensajeria`.

Para probar endpoints protegidos, inicia sesión mediante `POST /api/Auth/login` e introduce el JWT en la opción **Authorize** de Swagger. Si la cuenta tiene 2FA habilitado, completa primero el flujo de verificación.

## Configuración para despliegue

En el servidor, utiliza variables de entorno para los valores privados, por ejemplo `ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Email__Usuario`, `Email__Password` y `Stripe__SecretKey`. ASP.NET Core interpreta el doble guion bajo como separador de configuración.

Configura los orígenes permitidos de CORS para el frontend que consumirá la API y una ubicación persistente para los archivos de `wwwroot/uploads`. La integración de Stripe incluida está preparada para pruebas.

## Contexto académico

ConectaMasPro forma parte de mi proyecto de tesis y reúne trabajo de modelado relacional, desarrollo de APIs, implementación de reglas de negocio e integración de servicios. Este repositorio contiene el backend; el frontend se desarrolla de forma independiente.