using Microsoft.Data.SqlClient;
using Xunit.Sdk;

namespace bowling_backend_persistence_tests
{
    public class RawDbAccess
    {
        private readonly string _serverConnectionString;

        public RawDbAccess(string databaseConnectionString)
        {
            this._serverConnectionString = databaseConnectionString;
        }

        public void WaitForConnection(TimeSpan? timeout = null)
        {
            var actualTimeout = timeout ?? TimeSpan.FromSeconds(60);
            var startedAt = DateTime.Now;

            Console.Write("Waiting for connection");

            bool connected = false;
            do
            {
                Console.Write(".");

                if ((DateTime.Now - startedAt) > actualTimeout)
                {
                    throw new TimeoutException();
                }

                try
                {
                    using var connection = new SqlConnection(_serverConnectionString);
                    connection.Open();
                    connected = true;
                }
                catch (SqlException)
                {
                    Thread.Sleep(2500);
                }
            } while (!connected);

            Console.WriteLine();
        }

        public int NumberOfRows()
        {
            using var connection = ConnectToDatabase();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM BowlingGame";
            return (int)command.ExecuteScalar();
        }

        public void ClearDatabase()
        {
            using var connection = ConnectToDatabase();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Frame";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM BowlingGame";
            command.ExecuteNonQuery();
        }

        public T GetValueByGameId<T>(Guid id, string field)
        {
            using var connection = ConnectToDatabase();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM BowlingGame WHERE Id=@Id";
            command.Parameters.AddWithValue("@Id", id);

            var reader = command.ExecuteReader();
            reader.Read();

            var columnOrdinal = reader.GetOrdinal(field);
            return reader.GetFieldValue<T>(columnOrdinal);
        }

        public T GetValueByFrameId<T>(Guid id, string field)
        {
            using var connection = ConnectToDatabase();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Frame WHERE Id=@Id";
            command.Parameters.AddWithValue("@Id", id);

            var reader = command.ExecuteReader();
            reader.Read();

            var columnOrdinal = reader.GetOrdinal(field);
            return reader.GetFieldValue<T>(columnOrdinal);
        }

        private SqlConnection ConnectToDatabase()
        {
            SqlConnection connection = new(_serverConnectionString);
            connection.Open();
            connection.ChangeDatabase("bowling");
            return connection;
        }

    }
}