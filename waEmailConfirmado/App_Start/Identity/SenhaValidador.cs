using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace waEmailConfirmado.App_Start.Identity
{
    /// <summary>
    /// Interface IIdentityValidator é a base para a validação de entidades dentro do Identity.
    /// </summary>
    public class SenhaValidador : IIdentityValidator<string>
    {
        #region Propriedades Obrigatórias da Senha
        public int TamanhoRequerido { get; set; }
        public bool ObrigatorioCaracteresEspeciais { get; set; }
        public bool ObrigatorioUpperCase { get; set; }
        public bool ObrigatorioLowerCase { get; set; }
        public bool ObrigatorioDigitos { get; set; }

        #endregion

        #region Métodos Obrigatórios da Senha

        /// <summary>
        /// Verifica se o tamanho requerido mínimo da senha está correto.
        /// 
        /// NÃO é necessária verificação abaixo, pois a variável senha no "Return" tem uma "?" e a
        ///propriedade "Length" acessada diretamente em um objeto NULL NÃO apresentará erro.
        ///
        /// if (senha == null) return false;
        /// "?" = operador "C# Null Propagator"
        /// https://riptutorial.com/csharp/example/51/null-propagation
        /// 
        ///  No C# quando temos um método que é somente uma expressão, podemos escrever
        /// usando uma "=>" (seta) simplificando o código.
        /// </summary>
        /// <param name="senha">String da Senha</param>
        /// <returns>True or False</returns>
        private bool  VerificaTamanhoRequerido(string senha) =>
             senha?.Length >= TamanhoRequerido;

        /// <summary>
        /// Verifica se a senha possui os caracteres especiais necessários.
        /// 
        /// Testadores de REGEX - Regular Expressions:
        /// https://regex101.com/ 
        /// https://regexr.com/
        /// 
        /// JavaScript RegExp Reference:
        /// https://www.w3schools.com/jsref/jsref_obj_regexp.asp
        /// </summary>
        /// <param name="senha">String da Senha</param>
        /// <returns>True or False</returns>
        private bool VerificaCaracteresEspeciais(string senha) =>
            Regex.IsMatch(senha, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        /// <summary>
        /// Verifica se a senha possui o caracter maiúsculo.
        /// </summary>
        /// <param name="senha">String da Senha</param>
        /// <returns>True or False</returns>
        private bool VerificaUpperCase(string senha) =>
            senha.Any(char.IsUpper);

        /// <summary>
        /// Verifica se a senha possui o caracter minúsculo.
        /// </summary>
        /// <param name="senha">String da Senha</param>
        /// <returns>True or False</returns>
        private bool VerificaLowerCase(string senha) =>
            senha.Any(char.IsLower);

        /// <summary>
        /// Verifica se a senha possui dígito.
        /// </summary>
        /// <param name="senha">String da Senha</param>
        /// <returns>True or False</returns>
        private bool VerificaDigito(string senha) =>
            senha.Any(char.IsDigit);

        #endregion

        /// <summary>
        /// Assinatura da Identidade "IIdentityValidator".
        /// A interface IIdentityValidator cria validador de senhas.
        /// 
        /// IdentityResult: Observa o resultado de uma operação à partir do objeto IdentityResult
        /// com as propriedades Succeeded e Errors.
        /// </summary>
        /// <param name="item">Senha</param>
        /// <returns>async Task<IdentityResult></returns>
        public Task<IdentityResult> ValidateAsync(string item)
        {
            List<string> erros = new List<string>();

            if (ObrigatorioCaracteresEspeciais && !VerificaCaracteresEspeciais(item))
                erros.Add("A senha deve conter caracteres especiais.");

            if (!VerificaTamanhoRequerido(item))
                erros.Add($"A senha deve conter no mínimo {TamanhoRequerido} caracteres.");

            if (ObrigatorioUpperCase && !VerificaUpperCase(item))
                erros.Add("A senha deve conter pelo menos uma letra maiúscula.");

            if (ObrigatorioLowerCase && !VerificaLowerCase(item))
                erros.Add("A senha deve conter pelo menos uma letra minúscula.");

            if (ObrigatorioDigitos && !VerificaDigito(item))
                erros.Add("A senha deve conter no mínimo um dígito.");

            if (erros.Any())
                return Task.FromResult(IdentityResult.Failed(erros.ToArray()));
            else
                return Task.FromResult(IdentityResult.Success);
        }
    }
}