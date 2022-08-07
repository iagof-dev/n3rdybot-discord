using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N3rdyBacon.Commands
{
    public class banco
    {
        public static string db_dados = "server=" + N3rdyBacon.Config.db_server + ";database=" + N3rdyBacon.Config.db_data + ";Uid=" + N3rdyBacon.Config.db_user + ";pwd=" + N3rdyBacon.Config.db_pass;
        static MySqlConnection con_db = new MySqlConnection(db_dados);




        public static double get_saldo(ulong id)
        {
            double user_sld = 0;
            banco.users_exists(id);
            con_db.Open();
            string comando_saldo = "select * from users where id='" + id + "'";
            MySqlCommand comm = new MySqlCommand();
            var com = con_db.CreateCommand();
            com.CommandText = comando_saldo;
            var reader = com.ExecuteReader();
            while (reader.Read())
            {
                user_sld = reader.GetDouble("saldo");
            }

            reader.Close();
            con_db.Close();
            return user_sld;
        }

        public static void users_exists(ulong id)
        {
            con_db.Close();
            Console.WriteLine("Verificação | Verificando ID('" + id + "')");
            con_db.Open();
            string verify_existuser = "select * from users where id='" + id + "';";
            MySqlCommand user_exists = new MySqlCommand();
            var user_existss = con_db.CreateCommand();
            user_existss.CommandText = verify_existuser;
            var user_ver = user_existss.ExecuteReader();
            if (user_ver.HasRows)
            {
                user_ver.Close();
                Console.WriteLine("Verificação | ID Existe no banco de dados");
            }
            else
            {
                user_ver.Close();
                Console.WriteLine("Verificação | ID Não Existe");

                string criar_user = "insert into users values ('" + id + "', '0');";
                MySqlCommand create_user = new MySqlCommand();
                var criarusr = con_db.CreateCommand();
                criarusr.CommandText = criar_user;
                var usrcr = criarusr.ExecuteNonQuery();
                Console.WriteLine("Verificação | ID (" + id + ") Registrado no banco!");

            }
            con_db.Close();
        }


        public static bool user_transfer(ulong user_id, ulong target_id, double transfer_saldo)
        {
            
            banco.users_exists(user_id);
            banco.users_exists(target_id);

            con_db.Open();
            double user_saldo = 0;
            double target_saldo = 0;

            string get_tgsaldo = "select * from users where id='" + target_id + "';";
            MySqlCommand com_tgsaldo = new MySqlCommand();
            var com = con_db.CreateCommand();
            com.CommandText = get_tgsaldo;
            var reader = com.ExecuteReader();
            while (reader.Read())
            {
                target_saldo = reader.GetDouble("saldo");
            }
            reader.Close();


            string get_usersaldo = "select * from users where id='" + user_id + "';";
            MySqlCommand com_usersaldo = new MySqlCommand();
            var com2 = con_db.CreateCommand();
            com2.CommandText = get_usersaldo;
            var reader2 = com2.ExecuteReader();
            while (reader2.Read())
            {
                user_saldo = reader2.GetDouble("saldo");
            }
            reader2.Close();


            if (user_saldo > transfer_saldo)
            {
                double final_saldouser = user_saldo - transfer_saldo;
                double final_saldotgt = target_saldo + transfer_saldo;
                Console.WriteLine("Verificação | Usuario Possui Saldo suficiente!");


                string set_usersaldo = "update users set saldo='" + final_saldouser + "' where id='" + user_id + "';";
                MySqlCommand setar_usersaldo = new MySqlCommand();
                var com3 = con_db.CreateCommand();
                com3.CommandText = set_usersaldo;
                var teste1 = com3.ExecuteNonQuery();

                string set_tgtsaldo = "update users set saldo='" + final_saldotgt + "' where id='" + target_id + "';";
                MySqlCommand setar_tgtsaldo = new MySqlCommand();
                var com4 = con_db.CreateCommand();
                com4.CommandText = set_tgtsaldo;
                var teste2 = com4.ExecuteNonQuery();

                Console.WriteLine("Comando | Concluido!");
                return true;
            }
            else
            {
                Console.WriteLine("Verificação | Usuario não possui Saldo suficiente!");
                return false;


            }



            con_db.Close();
        }

        //probabilidade de cair 76 pra cima
        //24% de chance de ganhar
        public static double roleta(ulong id, double valor, double saldo)
        {
            Random rnd = new Random();
            int sorte_azar = rnd.Next(0, 100);

            int chance = 76;

            double valor_setar = 0;             

            double perdeu = 0;




            if (sorte_azar >= chance)
            {
                Console.WriteLine(sorte_azar + " - Sorte");
                valor_setar = valor * 1.5;
                perdeu = valor_setar;

            }
            else
            {
                Console.WriteLine(sorte_azar + " - Azar");
                valor_setar = saldo - valor;
                perdeu = -valor;
            }
            con_db.Open();
            string set_usersaldo = "update users set saldo='" + valor_setar + "' where id='" + id + "';";
            MySqlCommand setar_usersaldo = new MySqlCommand();
            var com3 = con_db.CreateCommand();
            com3.CommandText = set_usersaldo;
            var teste1 = com3.ExecuteNonQuery();
            con_db.Close();
            return perdeu;

            //double teste = banco.get_saldo(id);
        }



    }
}
