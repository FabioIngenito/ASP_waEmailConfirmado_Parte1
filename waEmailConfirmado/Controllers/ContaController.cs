using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using waEmailConfirmado.Models;
using waEmailConfirmado.ViewModels;

/// <summary>
/// * IdentityDbContext e IdentityUser:
/// A classe IdentityUser é implementada no AspNet Identity e ela possui várias propriedades implementadas.
/// O IdentityDbContext cria as tabelas necessárias para o AspNet Identity automaticamente.
/// 
/// *UserStore e UserManager:
/// A interface IUserStore faz a comunicação do UserManager com o Banco de Dados.
///
/// *Arquitetura do Identity:
/// Três características fundamentais sobre a arquitetura do AspNet Identity: 
///    1 - Baseado em interfaces; 
///    2 - Implementação genérica; 
///    3 - Aassincronicidade.  
///    
/// * VEJA TAMBÉM:
/// 
/// SQL script for creating an ASP.NET Identity Database
/// Mr.Thursday
/// Posted 1 Nov 2013
///
/// https://www.codeproject.com/Tips/677279/SQL-script-for-creating-an-ASP-NET-Identity-Databa
/// </summary>
namespace waEmailConfirmado.Controllers
{
    /// <summary>
    /// O Banco de Dados usado (ORM - Object Relational Mapping) foi o "EntityFramework", é possível trocar, por exemplos: NHibernate e "DB não Relacional".
    /// Veja que é outro pacote separado no NUGET, O "CORE" (núcleo) está em outro pacote independente da tecnologia de ORM (Object Relational Mapping).
    /// </summary>
    public class ContaController : Controller
    {
        #region Propriedades

        //Propriedade responsávvel pelo retorno dos contexztos do Owin:
        private UserManager<UsuarioAplicacao> _userManager;

        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    // GetOwinContext - Método de extensão do Owin
                    IOwinContext contextOwin = HttpContext.GetOwinContext();
                    // O GetuserManager vem com a extensão do Identity
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }

                return _userManager;
            }

            set { _userManager = value; }
        }

        #endregion

        /// <summary>
        /// Usada pelo usuário para acessar a página.
        /// </summary>
        /// <returns></returns>
        public ActionResult Registrar()
        {
            return View();
        }

        /// <summary>
        /// POST das informações do usuário - Método assíncrono (usa paralelismo)
        /// </summary>
        /// <param name="modelo">ContaRegistrarViewModel</param>
        /// <returns>Task<ActionResult></returns>
        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            // Tratando erro(s):
            if (!ModelState.IsValid) return View(modelo);

            // ESSE CÓDIGO FOI REMOVIDO, POIS FOI COLOCADO DENTRO DA CLASSE "Startup.cs" (Owin)
            // A implementação pode ser modificada para colocar a nossa estrutura de usuário "UsuarioAplicacao":
            //IdentityDbContext<UsuarioAplicacao> dbcontext = new IdentityDbContext<UsuarioAplicacao>("DefaultConnection");

            // ESSE CÓDIGO FOI REMOVIDO, POIS FOI COLOCADO DENTRO DA CLASSE "Startup.cs" (Owin)
            //Camada de abstração - Fica entre o Identity e o Banco de Dados - camada que conversa com o "dbcontext". - fornecedor (store)
            //UserStore<UsuarioAplicacao> userstore = new UserStore<UsuarioAplicacao>(dbcontext);

            // ESSE CÓDIGO FOI REMOVIDO, POIS FOI COLOCADO DENTRO DA CLASSE "Startup.cs" (Owin)
            // Gerenciamento - Desacomplar a dependência da tecnologia do Banco de Dados - gerenciador (manager)
            //UserManager<UsuarioAplicacao> userManager = new UserManager<UsuarioAplicacao>(userstore);

            // Se você usar desta forma, usará a extrutura que JÁ EXISTE dentro do Identity:
            //IdentityUser novoUsuarioIdentity = new IdentityUser();

            UsuarioAplicacao novoUsuario = new UsuarioAplicacao
            {
                // Atribuindo os valores digitados pelo usuário para Identity
                Email = modelo.EMailUsuario,
                UserName = modelo.NomeUsuario,
                Id = modelo.ID
            };


            // Estas 2 linhas abaixo fazem acesso DIRETO ao banco de dados aomenteando o acoplamento entre classes.
            //Isso é substituído pela variável "userManager".
            //dbcontext.Users.Add(novoUsuario);
            //dbcontext.SaveChanges();

            // ESSE CÓDIGO FOI REMOVIDO, POIS FOI COLOCADO DENTRO DA CLASSE "Startup.cs" (Owin)
            //await userManager.CreateAsync(novoUsuario, modelo.SenhaUsuario);

            // Procurar dentro do Banco se o e-mail já foi cadastrado
            UsuarioAplicacao usuario = await UserManager.FindByEmailAsync(modelo.EMailUsuario);
            bool usuarioJaExiste = usuario != null;

            //if (usuarioJaExiste)
            //    return RedirectToAction("Index", "Home");

            if (usuarioJaExiste)
                return View("AguardandoConfirmacao");

            // O "CreateAsync" retorna várias propriedades veja com o cursor em cima da palavra "CreateAsync" e clicando no botão "F12".
            IdentityResult resultado = await UserManager.CreateAsync(novoUsuario, modelo.SenhaUsuario);

            //Se o Modelo foi validado, é possível incluir o usuário sem erros de preenchimento de banco de dados.
            // Vamos usar a propriedade booleana ".Succeeded" paraa verificar se houve algum erro e indicar automaticamente na tela para ao usuário.
            if (resultado.Succeeded)
            {
                // Enviar um email de confirmação de registro no site.
                await EnviarEmailDeConfirmacaoAsync(novoUsuario);

                //return RedirectToAction("Index", "Home");
                return View("AguardandoConfirmacao");
            }
            else
            {
                AdicionaErros(resultado);
                return View(modelo);
            }
        }

        private void AdicionaErros(IdentityResult resultado)
        {
            foreach (string erro in resultado.Errors)
                ModelState.AddModelError("", erro);
        }

        /// <summary>
        /// Método para Enviar Email de confirmação.
        /// 
        /// Envia um Email para o usuário que contém o código (token) que validará
        /// a criação da nova conta do usuário - Isso GARANTE que o usuário tem acesso acesso a este email.
        /// 
        /// * Método UserManager.GenerateEmailConfirmationTokenAsync:
        /// Para a confirmação de email de nosso usuário, foi necessário criar um token de confirmação e conseguimos 
        /// à partir do método UserManager.GenerateEmailConfirmationTokenAsync.
        /// </summary>
        /// <param name="usuarioAplicacao">Usuário da Aplicação</param>
        /// <returns></returns>
        private async Task EnviarEmailDeConfirmacaoAsync(UsuarioAplicacao usuarioAplicacao)
        {
            // String da Token gerado por UserManager. Protocolo usado por esta aplicação: "Request.Url.Scheme".
            string tokenGerado = await UserManager.GenerateEmailConfirmationTokenAsync(usuarioAplicacao.Id);
            // String que contém o LINK de retorno.
            string linkCallBack = Url.Action(
                    "ConfirmacaoEmail",
                    "Conta",
                    new { usuarioId = usuarioAplicacao.Id, tokenGerado },
                    Request.Url.Scheme);

            // CASO NÃO QUEIRA PASSAR UM "LINK" PARA O USUÁRIO use este código:
            //await UserManager.SendEmailAsync(
            //        usuarioAplicacao.Id,
            //        "AUTENTICAÇÃO - Confirmação de Email",
            //        $"Bem-vindo! Use o {tokenGerado} gerado para confirmar seus dados.");

            // Método de envio de email assíncrono - clique no link.
            await UserManager.SendEmailAsync(
                usuarioAplicacao.Id,
                "AUTENTICAÇÃO - Confirmação de Email",
                $"Bem-vindo! Clique aqui {linkCallBack} para confirmar seus dados.");

        }

        /// <summary>
        /// Esta ActionRsult apresentará a tela de confirmação de Email do usuário.
        /// 
        /// * Método UserManager.ConfirmEmailAsync:
        /// Como token de confirmação criado, foi preciso confirmar a ligação entre o token e o usuário.
        /// Para tanto, fizemos uso do método UserManager.ConfirmEmailAsync.
        /// </summary>
        /// <param name="usuarioId">Id do Usuário</param>
        /// <param name="tokenGerado">Token Gerado pelo Sistema</param>
        /// <returns></returns>
        public async Task<ActionResult> ConfirmacaoEmail(string usuarioId, string tokenGerado)
        {
            // Tratar se o usuário ou token gerado não estão vazios...
            // Caso TRUE abre o Diretório: "Views" / "Shared" / Arquivo: "Error.cshtml"
            if (usuarioId == null || tokenGerado == null) return View("Error");

            IdentityResult resultado = await UserManager.ConfirmEmailAsync(usuarioId, tokenGerado);

            // Quando o Email for CONFIRMADO, a coluna "EmailConfirmed" do Banco de Dados para TRUE.
            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");
            else 
                return View("Error");
        }
    }
}