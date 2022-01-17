using Microsoft.AspNet.Identity.EntityFramework;

namespace waEmailConfirmado.Models
{
    public class UsuarioAplicacao : IdentityUser
    {
        public string NomeUsuario { get; set; }
    }
}