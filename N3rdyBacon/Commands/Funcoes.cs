using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Net;
using DSharpPlus.VoiceNext;
using MySql.Data.MySqlClient;
using N3rdyBacon.Commands;

using MySql;
using MySql.Data;
using N3rdyBacon;
using DSharpPlus.Interactivity.Extensions;

namespace N3rdyBacon.Commands
{

    public class Funcoes : BaseCommandModule
    {


        ///Banco de Dados Flexível
        public static string db_dados = "server=" + N3rdyBacon.Config.db_server + ";database=" + N3rdyBacon.Config.db_data + ";Uid=" + N3rdyBacon.Config.db_user + ";pwd=" + N3rdyBacon.Config.db_pass;
        MySqlConnection con_db = new MySqlConnection(db_dados);

        string emoji_error = "<:error:1005870378473771089>";
        string emoji_sucesso = "<a:sucesso:1005870400196055040>";
        string emoji_loading = "<a:loading:1005865161715875901>";
        string emoji_notstonks = "<:stonks_not:1005868742418366646>";
        string emoji_stonks = "<a:stonks:1005867620446253096>";

        [Command("roleta")]
        public async Task roleta(CommandContext ctx, double valor)
        {

            double saldo = banco.get_saldo(ctx.User.Id);

            if (saldo < valor)
            {
                await ctx.RespondAsync(emoji_error + " Erro! Sem saldo para fazer aposta...");
            }
            else { 
            var teste = await ctx.RespondAsync("<@" + ctx.User.Id + ">\n" + emoji_loading + " Rodando... (24% de sorte)");

            Thread.Sleep(2500);


            double valor_ganho = banco.roleta(ctx.User.Id, valor, saldo);

            string verify = Convert.ToString(valor_ganho);

            await teste.DeleteAsync();

                string mensagem = string.Empty;

            if (verify.Contains("-"))
            {

                    mensagem = (emoji_notstonks + " Perdeu! " + valor_ganho);
            }
            else
            {
                    mensagem = (emoji_stonks + " Ganhou! " + valor_ganho);
            }

                var deletar_after = await ctx.RespondAsync(mensagem);
                Thread.Sleep(15000);
                await deletar_after.DeleteAsync();
            }





        }






        [Command("transferir")]
        public async Task user_transfer(CommandContext ctx, DiscordMember membro, double valor)
        {
            string valor_conv = Convert.ToString(valor);
            if (valor_conv.Contains("-"))
            {
                Console.WriteLine("Erro! Há simbolo de menos");
                await ctx.RespondAsync("<@" + ctx.User.Id + ">\n" + emoji_error + "Erro! Valor está incorreto...");
            }
            else { 
            
                Console.WriteLine("Comando | Transferir foi utilizado por " + ctx.User.Username + " Valor: R$" + valor);
                var emoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                var teste2 = await ctx.RespondAsync("<@" + ctx.Member.Id + ">\nVocê está Transferindo R$" + valor + " para <@" + membro.Id + ">\nConfirma?");
                var react = teste2.CreateReactionAsync(emoji);

                var member = ctx.Member;

                var result = await teste2.WaitForReactionAsync(member, emoji);

            

            if (!result.TimedOut)
            {
                Console.WriteLine("Comando | Transferir foi aceito pelo Usuário");
               
                

                bool concluido = await Task.Run(() => banco.user_transfer(ctx.User.Id, membro.Id, valor));

                    if (concluido == true)
                {
                    await ctx.RespondAsync(emoji_sucesso + " Valor foi transferido com sucesso!");
                }
                else
                {

                    Console.WriteLine("Sem Saldo");
                    await ctx.RespondAsync(emoji_error + " Erro! Sem Saldo! ANIMAL...");
                }
            }
            }   






        }

        [Command("verificar")]
        public async Task verificar(CommandContext ctx, DiscordMember membro)
        {
            double espiao = banco.get_saldo(membro.Id);
            await ctx.RespondAsync("<@" + ctx.User.Id + ">\nSaldo de <@" + membro.Id + "> : R$" + espiao);
        }

        [Command("saldo")]
        public async Task saldo(CommandContext ctx)
        {
            Console.WriteLine("Comando | saldo foi utilizado");

            

            ulong user_id = ctx.Member.Id;
            try
            {
                    double saldinho = banco.get_saldo(user_id);
                    await ctx.RespondAsync("<@" + user_id + ">\nSeu Saldo: R$" + saldinho);
                   
                
            }
            catch (Exception error)
            {
                await ctx.RespondAsync("Erro! " + error.Message);
            }
        }
    }

}