namespace FidoAuthServer.Data;

public static class DbInitialiser
{
    public static void Initialise()
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(Consts.ConnectionString);
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS Users (
                Username TEXT PRIMARY KEY,
                DisplayName TEXT NOT NULL,
                Id BLOB NOT NULL,
                Credentials TEXT NOT NULL
            );
        ";
        
        tableCmd.ExecuteNonQuery();
    }
}
