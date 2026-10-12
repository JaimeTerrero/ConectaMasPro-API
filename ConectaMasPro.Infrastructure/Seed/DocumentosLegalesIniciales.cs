using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Infrastructure.Seed
{
    public static class DocumentosLegalesIniciales
    {
        public static readonly Guid TerminosCondicionesId =
            Guid.Parse("a83ee82a-f62a-4e19-b6f6-ef359d467101");

        public static readonly Guid PoliticaPrivacidadId =
            Guid.Parse("a83ee82a-f62a-4e19-b6f6-ef359d467102");

        private static readonly DateTime FechaVersion =
            new(2026, 8, 21, 0, 0, 0, DateTimeKind.Utc);

        public static DocumentoLegal CrearTerminosCondiciones() => new()
        {
            Id = TerminosCondicionesId,
            Tipo = TipoDocumentoLegal.TerminosCondiciones,
            Version = "1.0",
            Titulo = "Términos y Condiciones de ConectaMás Pro",
            Contenido = TerminosCondiciones,
            FechaPublicacion = FechaVersion,
            FechaCreacion = FechaVersion,
            EstaVigente = true,
            RequiereAceptacion = true,
            Activo = true
        };

        public static DocumentoLegal CrearPoliticaPrivacidad() => new()
        {
            Id = PoliticaPrivacidadId,
            Tipo = TipoDocumentoLegal.PoliticaPrivacidad,
            Version = "1.0",
            Titulo = "Política de Privacidad de ConectaMás Pro",
            Contenido = PoliticaPrivacidad,
            FechaPublicacion = FechaVersion,
            FechaCreacion = FechaVersion,
            EstaVigente = true,
            RequiereAceptacion = true,
            Activo = true
        };

        private const string TerminosCondiciones = """
            # Términos y Condiciones de ConectaMás Pro

            Versión 1.0 - Vigente desde el 21 de agosto de 2026.

            ## 1. Aceptación y alcance

            Estos Términos regulan el acceso y uso de ConectaMás Pro. Al crear una cuenta, el usuario declara que leyó y aceptó esta versión y la Política de Privacidad vigente. La plataforma conserva evidencia de la versión y fecha de aceptación para consulta posterior.

            El usuario debe tener capacidad legal para contratar. Para este MVP, el registro está dirigido a personas de dieciocho años o más. Quien no esté de acuerdo con estas reglas no debe crear ni utilizar una cuenta.

            ## 2. Naturaleza de la plataforma

            ConectaMás Pro proporciona infraestructura tecnológica para que clientes encuentren proveedores y para que proveedores publiquen y ofrezcan servicios. La plataforma actúa como intermediaria y no presta directamente los servicios publicados por terceros.

            ConectaMás Pro no garantiza que una solicitud termine en contratación ni que todas las expectativas particulares de las partes sean satisfechas. Sí podrá establecer controles de seguridad, reputación, moderación y atención de reportes para proteger el funcionamiento de la comunidad.

            ## 3. Cuentas y veracidad de la información

            Cada usuario es responsable de proporcionar información veraz, completa y actualizada; custodiar sus credenciales; utilizar una cuenta propia; y notificar cualquier acceso no autorizado. Está prohibido suplantar identidades, crear cuentas para defraudar o manipular calificaciones y reportes.

            La cuenta puede operar como Cliente, Proveedor o bajo las modalidades habilitadas por la plataforma. Algunas funciones pueden requerir información adicional o verificaciones.

            ## 4. Publicación y contratación de servicios

            El Proveedor es responsable de describir con claridad el servicio, alcance, precio base, disponibilidad, experiencia y demás condiciones relevantes. El Cliente es responsable de describir correctamente su necesidad.

            Antes de aceptar una solicitud, las partes deben revisar y acordar, cuando corresponda, el objeto del servicio, precio, fecha, duración, ubicación, modalidad, método de pago y condiciones particulares. Las condiciones acordadas entre Cliente y Proveedor son responsabilidad de esas partes.

            ## 5. Acuerdos y mensajes electrónicos

            Las solicitudes, aceptaciones, confirmaciones, chats y demás mensajes de datos generados mediante la plataforma pueden documentar ofertas, aceptaciones y acuerdos electrónicos. Las partes reconocen que una aceptación realizada desde su cuenta puede producir efectos respecto del servicio acordado.

            ConectaMás Pro podrá conservar la trazabilidad de estados, fechas y comunicaciones conforme a la Política de Privacidad, con fines operativos, de seguridad, soporte y resolución de controversias.

            ## 6. Proveedores independientes

            Los Proveedores actúan por cuenta propia y como prestadores independientes. El uso de ConectaMás Pro no crea una relación laboral, de subordinación, sociedad, representación, agencia ni exclusividad con la plataforma.

            Cada Proveedor decide, dentro de las funciones disponibles, qué servicios publicar, qué solicitudes aceptar y cómo organizar la ejecución acordada. ConectaMás Pro no fija horarios laborales ni garantiza un volumen de contrataciones o ingresos.

            ## 7. Pagos y obligaciones económicas

            Las partes son responsables de revisar el precio y método de pago antes de confirmar el servicio. Cuando la plataforma habilite mecanismos de pago, se mostrarán las condiciones aplicables antes de utilizarlos.

            Cada Proveedor es responsable de sus obligaciones fiscales, registros, comprobantes y declaraciones relacionadas con los ingresos obtenidos. ConectaMás Pro solo practicará retenciones o emitirá documentos fiscales cuando exista obligación legal y la funcionalidad correspondiente haya sido implementada.

            ## 8. Calificaciones y reputación

            Después de completar una solicitud, Cliente y Proveedor pueden calificarse mutuamente. Las reseñas deben basarse en una interacción real, ser honestas y no contener amenazas, discriminación, datos personales innecesarios ni contenido ilícito.

            La plataforma puede impedir calificaciones duplicadas y moderar o retirar contenido fraudulento, abusivo o ajeno a la experiencia evaluada. Una calificación representa la opinión de su autor y no una afirmación propia de ConectaMás Pro.

            ## 9. Conductas prohibidas

            Está prohibido utilizar la plataforma para estafas, acoso, amenazas, discriminación, explotación, actividades ilícitas, ofertas fraudulentas, contenido malicioso, vulneración de derechos de terceros, manipulación de sistemas, acceso no autorizado o evasión deliberada de controles de seguridad.

            También se prohíbe publicar servicios ilegales, peligrosos o engañosos y utilizar información obtenida en la plataforma para fines incompatibles con el servicio solicitado.

            ## 10. Reportes, moderación y suspensión

            Los usuarios pueden reportar incidentes relacionados con solicitudes, perfiles, mensajes o conductas. ConectaMás Pro podrá investigar, solicitar información, limitar contenido, suspender funciones o desactivar cuentas cuando existan indicios razonables de incumplimiento o riesgo.

            Las medidas se adoptarán procurando proporcionalidad, seguridad y conservación de evidencia. En situaciones que puedan constituir delitos o requerimientos legales, la plataforma podrá colaborar con las autoridades competentes conforme a la ley.

            ## 11. Privacidad y seguridad

            El tratamiento de datos personales se describe en la Política de Privacidad. ConectaMás Pro aplica controles técnicos y organizativos razonables, pero ningún sistema conectado a Internet puede garantizar seguridad absoluta.

            ## 12. Propiedad intelectual

            El software, diseño, marcas y contenidos propios de ConectaMás Pro están protegidos por las normas aplicables. El usuario conserva los derechos que posea sobre el contenido que publica y autoriza su tratamiento y visualización en la medida necesaria para operar la plataforma.

            El usuario garantiza que tiene derecho a publicar dicho contenido y que este no infringe derechos de terceros.

            ## 13. Responsabilidad

            Cada Cliente y Proveedor responde por sus decisiones, acuerdos, actos, omisiones y por la ejecución del servicio. ConectaMás Pro no será parte del contrato particular de prestación salvo que una función indique expresamente lo contrario.

            Ninguna cláusula pretende excluir responsabilidades que legalmente no puedan excluirse. Cuando la plataforma tenga conocimiento efectivo de contenido o actividad presuntamente ilícita, podrá actuar diligentemente para restringirla, investigarla o eliminarla.

            ## 14. Cambios y terminación

            ConectaMás Pro puede actualizar estos Términos por razones legales, técnicas o funcionales. Cada versión tendrá identificación y fecha de publicación. Cuando un cambio sea sustancial o requiera nueva aceptación, el usuario será informado antes de continuar utilizando las funciones afectadas.

            El usuario puede dejar de utilizar la plataforma y solicitar la gestión de su cuenta conforme a la Política de Privacidad. La terminación no elimina obligaciones, evidencias o responsabilidades que deban conservarse legalmente.

            ## 15. Legislación aplicable

            Estos Términos se interpretan conforme a las leyes de la República Dominicana, incluyendo el Código Civil y, según corresponda, la Ley núm. 126-02 sobre Comercio Electrónico, Documentos y Firmas Digitales; la Ley núm. 172-13 sobre Protección de Datos de Carácter Personal; el Código de Trabajo; y el Código Tributario.

            Las partes procurarán resolver de buena fe cualquier reclamación utilizando primero los canales de soporte y reporte habilitados en la plataforma, sin limitar los derechos que les reconozca la legislación aplicable.

            ## 16. Contacto

            Las consultas relacionadas con estos Términos pueden presentarse mediante los canales de soporte habilitados dentro de ConectaMás Pro.
            """;

        private const string PoliticaPrivacidad = """
            # Política de Privacidad de ConectaMás Pro

            Versión 1.0 - Vigente desde el 21 de agosto de 2026.

            ## 1. Alcance

            Esta Política explica qué datos personales trata ConectaMás Pro, para qué los utiliza y qué opciones tiene el usuario. Se aplica a Clientes, Proveedores y demás personas que utilicen las funciones habilitadas.

            ## 2. Datos que podemos tratar

            Según las funciones utilizadas, podemos tratar datos de registro y contacto; tipo de cuenta; información de perfil, biografía, foto, intereses y experiencia; ubicación aproximada o exacta cuando el usuario la proporcione; servicios publicados; solicitudes, estados y condiciones acordadas; mensajes; pagos y referencias transaccionales; calificaciones; reportes y evidencias; preferencias; y datos técnicos básicos de seguridad y acceso.

            No solicitamos deliberadamente datos sensibles que no sean necesarios. El usuario debe evitar compartir en perfiles, chats, reseñas o reportes información personal excesiva o de terceros sin autorización.

            ## 3. Finalidades

            Utilizamos los datos para crear y administrar cuentas; autenticar usuarios; mostrar perfiles y servicios; facilitar solicitudes, comunicación y acuerdos; gestionar pagos cuando estén disponibles; ofrecer reputación bidireccional; atender reportes; prevenir fraude y abuso; proteger la plataforma; cumplir obligaciones legales; y mejorar su funcionamiento.

            Los mensajes operativos y de seguridad necesarios para administrar o proteger la cuenta no se consideran comunicaciones comerciales.

            ## 4. Fundamentos del tratamiento

            El tratamiento se apoya, según corresponda, en la autorización informada del usuario, la ejecución de las funciones y acuerdos solicitados, el cumplimiento de obligaciones legales y la protección legítima de la seguridad de la plataforma y sus usuarios.

            No condicionamos las funciones básicas a tratamientos de datos que no sean necesarios para prestar el servicio.

            ## 5. Información pública y compartida

            Parte del perfil del usuario, sus servicios, reputación y reseñas puede mostrarse a otros usuarios para permitir el funcionamiento de la plataforma. Los datos de contacto privados y la configuración de seguridad no deben mostrarse públicamente salvo decisión informada del usuario o necesidad funcional claramente indicada.

            Cuando Cliente y Proveedor participan en una solicitud, se comparte entre ellos la información necesaria para coordinar y ejecutar el servicio.

            ## 6. Proveedores tecnológicos y autoridades

            Podemos utilizar proveedores de infraestructura, correo, almacenamiento, mapas, pagos u otros servicios técnicos que traten datos bajo instrucciones y medidas de protección apropiadas. Solo se les facilitará la información necesaria para su función.

            También podremos comunicar información cuando exista una obligación legal, orden válida de autoridad competente o necesidad de proteger derechos, seguridad e integridad de personas o de la plataforma.

            ## 7. Ubicación

            La ubicación se utiliza para facilitar búsquedas y coordinación de servicios. Cuando sea suficiente, la plataforma procurará mostrar una ubicación aproximada. Las coordenadas exactas no deben hacerse públicas de forma general y solo se tratarán para funciones que las requieran.

            ## 8. Conservación

            Los datos se conservarán durante el tiempo necesario para prestar el servicio, mantener la seguridad, resolver reclamaciones y cumplir obligaciones legales. La evidencia de aceptación de documentos, transacciones, reportes y comunicaciones vinculadas a acuerdos puede conservarse durante los plazos necesarios para demostrar dichas operaciones.

            Cuando los datos dejen de ser necesarios, se eliminarán, anonimizarán o bloquearán conforme a las capacidades técnicas y obligaciones aplicables.

            ## 9. Seguridad y confidencialidad

            Aplicamos controles razonables de autenticación, autorización, acceso restringido, validación y protección de información. Las credenciales deben mantenerse confidenciales. Aunque se adoptan medidas preventivas, ningún sistema ofrece seguridad absoluta.

            La plataforma toma como referencia buenas prácticas de seguridad de la información, pero no declara estar certificada bajo ISO/IEC 27001.

            ## 10. Derechos y decisiones del usuario

            El usuario puede solicitar información sobre sus datos, acceder a ellos, pedir correcciones o actualizaciones y, cuando corresponda, solicitar su eliminación, oposición o limitación. Algunas solicitudes pueden estar sujetas a verificación de identidad y a obligaciones legales de conservación.

            El usuario puede actualizar la información disponible desde su perfil y presentar solicitudes relacionadas con sus datos mediante los canales de soporte.

            ## 11. Menores de edad

            El MVP está dirigido a personas de dieciocho años o más. No se pretende recopilar deliberadamente información de menores. Si se detecta una cuenta que incumpla esta regla, podrán adoptarse medidas para restringirla y gestionar sus datos.

            ## 12. Cambios a esta Política

            Cada versión será identificada y conservará su fecha de publicación. Si se introducen cambios sustanciales o nuevas finalidades que requieran autorización, se informará al usuario y se solicitará una nueva aceptación cuando corresponda.

            ## 13. Marco normativo y contacto

            Esta Política toma como base la Ley núm. 172-13 sobre Protección de Datos de Carácter Personal de la República Dominicana. Los principios de minimización, finalidad, transparencia y seguridad también se utilizan como criterios de diseño, sin afirmar que toda norma extranjera resulte automáticamente aplicable.

            Las consultas o solicitudes de privacidad pueden presentarse mediante los canales de soporte habilitados dentro de ConectaMás Pro.
            """;
    }
}
