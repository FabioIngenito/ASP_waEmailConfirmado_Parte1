using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System.Data.Entity;
using waEmailConfirmado.App_Start.Identity;
using waEmailConfirmado.Models;

//Configura o tipo a ser usado pela inicialização do Owin = Class "Startup.cs".
[assembly: OwinStartup(typeof(waEmailConfirmado.Startup))]

/// <summary>
/// Owin é :
/// Uma biblioteca de comunicação entre aplicação e servidor. Owin é muito mais leve em comparação a biblioteca System.Web.
/// 
/// Uma "contexto do Owin" sempre acontece quando uma requisição é feita para nossa aplicação.
/// Exemplos: "Get" de uma página ou um "Post" de um formulário.
/// </summary>
namespace waEmailConfirmado
{
    /// <summary>
    /// Classe de Inicialização - EXPLICAÇÕES GERAIS:
    /// 
    /// 1 - O método contextoOwin.Get<T>() foi implementado no pacote Microsoft.AspNet.Identity.Owin.
    /// 
    /// 2 - A partir de um contexto "Http", obtemos o contexto "Owin" e usamos o "Get" ou "GetUserManager para obter uma 
    /// instância de "UserManager<UsuarioAplicacao>" com o código "contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>()".
    /// 
    /// 3 - Com o parâmetro "contextoOwin" do delegate de argumento, podemos recuperar outros objetos definidos com o 
    /// método "CreatePerOwinContext". Quando definimos um serviço pelo método "CreatePerOwinContext" podemos o recuperar 
    /// pelo método "Get".
    /// 
    /// 4 - O "design pattern" usado neste método é o "CreatePerOwinContext" atua como um alocador de serviços.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// OBRIGATÓRIO - Pipeline de Construção - Objeto que implementa a Interface do Owin "IAppBuilder".
        /// Configuration
        /// </summary>
        /// <param name="Builder">Interface do Owin "IAppBuilder".</param>
        public void Configuration(IAppBuilder Builder)
        {
            ///Per Owin - A cada contexto do Owin será criado um tipo dbContext.
            ///DbContext - Configuração de um Data Entity.
            Builder.CreatePerOwinContext<DbContext>(() =>
                new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            //UserStore - Configuração.
            //Usando a Interface para NÃO ficar dependente do EntityFramework.
            //Usando a sobrecarga do "GET" - nasceu de uma extensão do Owin pelo Identity.
            Builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>(
                (opcoes, contextoOwin) =>
                {
                    DbContext dbContext = contextoOwin.Get<DbContext>();
                    return new UserStore<UsuarioAplicacao>(dbContext);
                });

            //UserManager - Configuração. 
            Builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>(
                (opcoes, contextoOwin) =>
                {
                    IUserStore<UsuarioAplicacao> userStore = contextoOwin.Get<IUserStore<UsuarioAplicacao>>();
                    UserManager<UsuarioAplicacao> userManager = new UserManager<UsuarioAplicacao>(userStore);

                    //Validar o e-mail para que NÃO possa incluir um repetido no Banco de Dados. (unique)
                    // * UserValidator:
                    // Para validar emails únicos, crie uma classe que implemente IIdentityValidator<UsuarioAplicacao>.
                    // Usamos a classe UserValidator<TUser> com esta função.
                    UserValidator<UsuarioAplicacao> userValidator = new UserValidator<UsuarioAplicacao>(userManager)
                    {
                        AllowOnlyAlphanumericUserNames = false,
                        //Caso "true" aplica um "unique" para o Banco de Dados (não pode ter e-mail repetido)
                        RequireUniqueEmail = true
                    };

                    // Agora atribua ao "userManager" a configuração modificada (interface IIdentityValidator).
                    userManager.UserValidator = userValidator;

                    // Mudando a validação de senha (interface IIdentityValidator). Usa o "Property Initialize -> {}"
                    userManager.PasswordValidator = new SenhaValidador()
                    {
                        TamanhoRequerido = 6,
                        ObrigatorioCaracteresEspeciais = true,
                        ObrigatorioUpperCase = true,
                        ObrigatorioLowerCase = true,
                        ObrigatorioDigitos = true
                    };

                    //Conectado ao mecanismo de envio de e-mail.
                    userManager.EmailService = new EmailServico();

                    //REGISTRAR O "IUserTokenProvider":
                    IDataProtectionProvider dataProtectionProvider = opcoes.DataProtectionProvider;
                    IDataProtector dataProtectionProviderCreate = dataProtectionProvider.Create("waEmailConfirmado");

                    userManager.UserTokenProvider = new DataProtectorTokenProvider<UsuarioAplicacao>(dataProtectionProviderCreate);

                    return userManager;
                });
        }
    }
}