using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpeseAdo.ConsoleApp
{
    public static class DisconnectedMode
    {
        static string connectionStringSQL = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GestioneSpese;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        internal static void InsertSpesa()
        {
            DataSet dataset = new DataSet();
            using SqlConnection connection = new SqlConnection(connectionStringSQL);
            try
            {
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                    Console.WriteLine("Connessi al db");
                else
                    Console.WriteLine("NON connessi al db");

                var adapter = InitAdapter(connection);

                adapter.Fill(dataset, "Spese");

                connection.Close();
                Console.WriteLine("Connessione chiusa");

                (DateTime dataSpesa, int categoriaId, string descriz, string utente, decimal importo) = 
                    TakeDatiSpesa();

                // insert
                DataRow newRow = dataset.Tables["Spese"].NewRow();
                newRow["DataSpesa"] = dataSpesa;
                newRow["CategoriaId"] = categoriaId;
                newRow["Descrizione"] = descriz;
                newRow["Utente"] = utente;
                newRow["Importo"] = importo;
                newRow["Approvato"] = 0;

                dataset.Tables["Spese"].Rows.Add(newRow);

                // qui avviene la riconciliazione col DB e quindi il vero salvataggio lato DB
                adapter.Update(dataset, "Spese");
                Console.WriteLine("Database correttamente aggiornato");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Errore SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore generico: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        private static (DateTime, int, string, string, decimal) TakeDatiSpesa()
        {
            
            DateTime dataSpesa;
            int categoriaId;
            string descriz;
            string utente;
            decimal importo;

            Console.WriteLine("Inserisci dati della spesa.");

            Console.WriteLine("Data: ");
            while (!DateTime.TryParse(Console.ReadLine(), out dataSpesa))
            {
                Console.WriteLine("Inserisci un formato corretto di data!");
            }

            Console.WriteLine("Id della categoria: ");
            while (!int.TryParse(Console.ReadLine(), out categoriaId))
            {
                Console.WriteLine("Inserisci un formato corretto di id!");
            }

            Console.WriteLine("Descrizione: ");
            descriz = Console.ReadLine();

            Console.WriteLine("Utente: ");
            utente = Console.ReadLine();

            Console.WriteLine("Importo: ");
            while (!decimal.TryParse(Console.ReadLine(), out importo))
            {
                Console.WriteLine("Inserisci un formato corretto di importo!");
            }

            return (dataSpesa, categoriaId, descriz, utente, importo);   
        }

        private static SqlDataAdapter InitAdapter(SqlConnection connection)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            //SELECT (serve al metodo FILL)
            adapter.SelectCommand = new SqlCommand("Select * from Spese", connection);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            //INSERT
            adapter.InsertCommand = GenerateInsertCommand(connection);

            //UPDATE
            adapter.UpdateCommand = GenerateUpdateCommand(connection);

            //DELETE
            adapter.DeleteCommand = GenerateDeleteCommand(connection);

            return adapter;
        }

        private static SqlCommand GenerateDeleteCommand(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Delete from Spese where Id=@id";

            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int, 0, "Id"));

            return cmd;
        }

        private static SqlCommand GenerateUpdateCommand(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Update Spese set Approvato = @approvato";

            cmd.Parameters.Add(new SqlParameter("@approvato", SqlDbType.Bit, 0, "Approvato"));

            return cmd;
        }

        private static SqlCommand GenerateInsertCommand(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Insert into Spese values (@data, @categoria, @descr, @utente, @importo, 0)";
            cmd.Parameters.Add(new SqlParameter("@data", SqlDbType.DateTime, 0, "DataSpesa"));
            cmd.Parameters.Add(new SqlParameter("@categoria", SqlDbType.Int, 0, "CategoriaId"));
            cmd.Parameters.Add(new SqlParameter("@descr", SqlDbType.NVarChar, 50, "Descrizione"));
            cmd.Parameters.Add(new SqlParameter("@utente", SqlDbType.NVarChar, 100, "Utente"));
            cmd.Parameters.Add(new SqlParameter("@importo", SqlDbType.Decimal, 0, "Importo"));
     

            return cmd;
        }

        internal static (List<int>, List<string>) FetchAllSpese()
        {
            DataSet dataset = new DataSet();
            List<int> idList = new List<int>();
            List<string> nomiList = new List<string>();
            using SqlConnection connessione = new SqlConnection(connectionStringSQL);

            try
            {
                connessione.Open();
                if (connessione.State == System.Data.ConnectionState.Open)
                    Console.WriteLine("Siamo connessi al DB.");
                else
                    Console.WriteLine("Non connessi al DB.");

                // inizializziamo dataset e data adapter
                var adapter = InitAdapter(connessione);
                adapter.Fill(dataset, "Spese");
                connessione.Close();
                Console.WriteLine("Connessione chiusa");
                // da qui lavoro in modalità disconnected -> offline

                foreach (DataRow row in dataset.Tables["Spese"].Rows)
                {
                    Console.WriteLine($"{row["ID"]} - {row["Descrizione"]} - {row["Utente"]} - {row["Importo"]}");
                    idList.Add((int)row["ID"]);
                    nomiList.Add((string)row["Utente"]);
                }
                return (idList, nomiList);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
                return (idList, nomiList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
                return (idList, nomiList);
            }
            finally
            {
                connessione.Close();
            }
        }

        internal static void GetSpesaById(int id)
        {
            
        }
    }
}
