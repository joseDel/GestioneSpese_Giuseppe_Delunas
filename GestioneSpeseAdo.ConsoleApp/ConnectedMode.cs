using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpeseAdo.ConsoleApp
{
    public static class ConnectedMode
    {
        static string connectionStringSQL = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GestioneSpese;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        internal static void DeleteSpesa()
        {
            using SqlConnection connection = new SqlConnection(connectionStringSQL);

            try
            {
                connection.Open();
                SqlCommand deleteCommand = connection.CreateCommand();
                deleteCommand.CommandType = System.Data.CommandType.Text;
                deleteCommand.CommandText = "DELETE FROM Spese WHERE ID = @id";

                int idValue = CheckIdSpesa("eliminare");

                deleteCommand.Parameters.AddWithValue("@id", idValue);

                int result = deleteCommand.ExecuteNonQuery();

                if (result != 1)
                    Console.WriteLine("Si è verificato un problema nella cancellazione.");
                else
                    Console.WriteLine("Spesa cancellata correttamente.");
            }
            catch (Exception ex)
            {
                Console.Write($"Errore: {ex.Message}");
            }
            finally
            {
                connection.Close();
                Console.WriteLine("---- Premi un tasto ----");
                Console.ReadKey();
            }
        }

        private static int CheckIdSpesa(string operation)
        {
            Console.WriteLine("---------- Lista delle spese ----------.");
            int idValue;
            var choice = false;
            (List<int> idList, List<string> idNomi) = DisconnectedMode.FetchAllSpese();

            Console.WriteLine("\nDigita l'Id della spesa da " + operation);

            while (!(choice = int.TryParse(Console.ReadLine(), out idValue)) || (idList.Contains(idValue) == false))
            {
                if (choice == false)
                    Console.WriteLine("Inserisci un id numerico!");
                else
                {
                    if (idList.Contains(idValue) == false)
                    {
                        Console.WriteLine("L'id selezionato non esiste.");
                    }
                    else
                        break;
                }
            }
            return idValue;
        }

        internal static void TotSpeseByCategory()
        {
            using SqlConnection connessione = new SqlConnection(connectionStringSQL);
            try
            {
                connessione.Open();
                if (connessione.State == System.Data.ConnectionState.Open)
                    Console.WriteLine("Siamo connessi al DB.");
                else
                    Console.WriteLine("Non connessi al DB.");

                string query = @"select c.Categoria, joinByCat.TotImporto
                                from Categorie c join (
                                select s.CategoriaId, sum(s.Importo) as TotImporto
                                from spese s
                                group by s.CategoriaId) joinByCat
                                on c.Id = joinByCat.CategoriaId";

                // Istanziare sql command
                SqlCommand comando = new SqlCommand(query, connessione);

                SqlDataReader reader = comando.ExecuteReader();
                Console.WriteLine("--- Totale delle spese per categoria ---");
                while (reader.Read())
                {
                    var categ = (string)reader["Categoria"];
                    var tot = (decimal)reader["TotImporto"];

                    Console.WriteLine($"{categ} - {tot}");
                }
            }
            catch (Exception ex) // L'ultimo catch deve essere quello più generico
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
            finally
            {
                connessione.Close();
                Console.WriteLine("Connessione chiusa");
            }
        }

        internal static void ShowSpeseUser()
        {
            using SqlConnection connessione = new SqlConnection(connectionStringSQL);
            try
            {
                connessione.Open();
                if (connessione.State == System.Data.ConnectionState.Open)
                    Console.WriteLine("Siamo connessi al DB.");
                else
                    Console.WriteLine("Non connessi al DB.");

                Console.WriteLine("Inserisci Nome dell'utente di cui si vogliono visualizzare le spese");
                string nome = CheckNomeUser();

                string query = "select * from spese s where s.Utente = " + "'" + nome + "'";

                // Istanziare sql command
                SqlCommand comando = new SqlCommand(query, connessione);

                SqlDataReader reader = comando.ExecuteReader();
                Console.WriteLine("--- Elenco spese dell'utente specificato ---");
                while (reader.Read())
                {

                    var data = (DateTime)reader["DataSpesa"];
                    var categ = (int)reader["CategoriaId"];
                    var descr = (string)reader["Descrizione"];
                    var u = (string)reader["Utente"];
                    var importo = (decimal)reader["Importo"];

                    Console.WriteLine($"{data} - {categ} - {descr} - {u} - {importo}");
                }
            }
            catch (Exception ex) // L'ultimo catch deve essere quello più generico
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
            finally
            {
                connessione.Close();
                Console.WriteLine("Connessione chiusa");
            }
        }

        private static string CheckNomeUser()
        {
            Console.WriteLine("---------- Lista delle spese ----------.");
            string nome;
            (List<int> idList, List<string> nomiList) = DisconnectedMode.FetchAllSpese();

            Console.WriteLine("\nDigita nome utente.");
            nome = Console.ReadLine();

            while (nomiList.Contains(nome) == false)
            {
                Console.WriteLine("\nDigita un nome presente nella lista.");
                nome = Console.ReadLine();
            }
            return nome;
        }


        internal static void ShowSpeseApproved()
        {
            using SqlConnection connessione = new SqlConnection(connectionStringSQL);
            try
            {
                connessione.Open();
                if (connessione.State == System.Data.ConnectionState.Open)
                    Console.WriteLine("Siamo connessi al DB.");
                else
                    Console.WriteLine("Non connessi al DB.");

                string query = "select * from spese s where s.Approvato = 1";

                // Istanziare sql command 
                SqlCommand comando = new SqlCommand(query, connessione);

                SqlDataReader reader = comando.ExecuteReader();
                Console.WriteLine("--- Elenco spese approvate ---");
                while (reader.Read())
                {
                   
                    var data = (DateTime)reader["DataSpesa"];
                    var categ = (int)reader["CategoriaId"];
                    var descr = (string)reader["Descrizione"];
                    var u = (string)reader["Utente"];
                    var importo = (decimal)reader["Importo"];

                    Console.WriteLine($"{data} - {categ} - {descr} - {u} - {importo}");
                }
            }
            catch (Exception ex) // L'ultimo catch deve essere quello più generico
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
            finally
            {
                connessione.Close();
                Console.WriteLine("Connessione chiusa");
            }
        }

        internal static void ApproveSpesa()
        {
            using SqlConnection connection = new SqlConnection(connectionStringSQL);

            try
            {
                connection.Open();
                SqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandType = System.Data.CommandType.Text;
                updateCommand.CommandText = "update Spese set Approvato = 1 WHERE ID = @id";

                Console.WriteLine();

                Console.WriteLine("\nDigita l'Id della spesa da approvare.");
                int idValue = CheckIdSpesa("approvare");

                updateCommand.Parameters.AddWithValue("@id", idValue);

                int result = updateCommand.ExecuteNonQuery();

                if (result != 1)
                    Console.WriteLine("Si è verificato un problema nell'aggiornamento.");
                else
                    Console.WriteLine("Spesa aggiornata correttamente.");
            }
            catch (Exception ex)
            {
                Console.Write($"Errore: {ex.Message}");
            }
            finally
            {
                connection.Close();
                Console.WriteLine("---- Premi un tasto ----");
                Console.ReadKey();
            }
        }
    }
}
