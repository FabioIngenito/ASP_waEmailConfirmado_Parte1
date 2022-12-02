<h1 align="left">RECURSO DE SEGURANÇA DE EMAIL CONFIRMADO</h1>

| :placard: Vitrine.Dev |  |
| -------------  | --- |
| :sparkles: Nome        | **ASP_waEmailConfirmado_Parte1**
| :label: Tecnologias | ASP.NET C# MVC EntityFramework Owin EMail

![waEmailConfirmado](https://user-images.githubusercontent.com/24603753/149820394-19c5b84e-2c72-4ff4-9e67-fbf43790ef8e.png#vitrinedev)

<h2 align="left">Detalhes do projeto</h2>

Recurso de segurança de email confirmado - MVC Identity.EntityFramework (Versão 2.2.3) e .Owin (Versão 4.2.0)

---------------------------------
ATENÇÃO!!! Configurar o REMETENTE e SENHA dentro do arquivo "Web.config" para o exemplo funcionar:
---------------------------------
ATENÇÃO 2!!! A resposta do servidor foi: 5.7.0 Authentication Required.
---------------------------------

O GMail tem um "acesso menos seguro". 

É uma proteção da conta onde se você não habilitar esse "acesso menos seguro", os aplicativos de terceiros e não autorizados pela google (como o nosso bytebank) não poderão realizar envios pelo nosso e-mail.

  1 - Faça login na conta google que usa como remetente no aplicativo;

  2 - Procure a opção "Acesso menos seguro" ou use a URL (Uniform Resource Locator) abaixo: 
         https://myaccount.google.com/lesssecureapps

  3 - Ativa a opção de "Acesso menos seguro" (isso só será possível se sua conta não possuir autenticação por 2 fatores);

  4 - Veja se os envios funcionam agora.

---------------------------------

O nome do banco de dados é o mesmo do projeto (EmailConfirmado):

Os registros serão armazenados nesta tabela:

   Select * from dbo.AspNetUsers

---------------------------------

O AspNet Identity é um framework para o gerenciamento de identidades de usuários e tarefas como por exemplo, verificar: 

- Senhas;

- Nomes de usuários;

- E-mails;


Gerencia contas de usuários construindo uma autenticação.

O Identity é um framework ASP.NET para o gerenciamento de identidades. Ele NÃO é um framework de segurança!

Os usuários serão guardados no Identity.

.Core -> Pacotes completos.

.EntityFramework -> Framework com Interface.

------------------------------------
REGRA: - ALTA COESÃO e BAIXO ACOPLAMENTO.
------------------------------------

Sobre o OWIN: 

  1 - É um protocolo de comunicação entre aplicação e servidor.

  2 - É bem granular, se você NÃO usa um recurso, você NÃO precisa instalar ao contrário do System.Web que amarra tudo ao mesmo tempo.

  3 - É usado para a alocação de serviços.
