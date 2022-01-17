using System.ComponentModel.DataAnnotations;

namespace waEmailConfirmado.ViewModels
{
    public class ContaRegistrarViewModel
    {

        /// <summary>
        /// Identificação do Usuário no Sistema (chave primária).
        /// </summary>
        [Required]
        public string ID { get; set; }

        /// <summary>
        /// Nome Completo do Usuário.
        /// </summary>
        [Required]
        [Display (Name = "Nome Completo")]
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Email do Usuário que será validado.
        /// </summary>
        [Required]
        [Display(Name = "Email Usuário")]
        [EmailAddress]
        public string EMailUsuario { get; set; }

        /// <summary>
        /// Senha apra a validação do email do usuário.
        /// </summary>
        [Required]
        [Display(Name = "Senha Usuário")]
        [DataType(DataType.Password)]
        public string SenhaUsuario { get; set; }

    }
}