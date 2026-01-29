using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;
using FidoAuthServer.Models.Entities;
using Microsoft.Data.Sqlite;

namespace FidoAuthServer.Data;

// ToDO: init DB and create tables if not exists. Make username primary key.
public class UserRepository : IUserRepository
{
    public User? GetByUsername(string username)
    {
        using var connection = new SqliteConnection(Consts.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();

        command.CommandText = @"SELECT * FROM Users WHERE Username = $username LIMIT 1";

        command.Parameters.AddWithValue("$username", username);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var uname = reader["Username"].ToString()!;
            var displayName = reader["DisplayName"].ToString()!;
            var id = (byte[])reader["Id"];
            var credentials = JsonSerializer.Deserialize<List<PublicKeyCredentialDescriptor>>(reader["Credentials"].ToString()!);
            return new User(uname, displayName, id, credentials);
        }

        return null;
    }

    // A bit lazy to load all of this into memory, but I'm not here to practice my sql stuff
    public bool IsCredentialAlreadyUsed(byte[] credentialId)
    {
        using var connection = new SqliteConnection(Consts.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Users";

        using var reader = command.ExecuteReader();
        var users = new List<User>();

        while (reader.Read())
        {
            var uname = reader["Username"].ToString()!;
            var displayName = reader["DisplayName"].ToString()!;
            var id = (byte[])reader["Id"];
            var credentials = JsonSerializer.Deserialize<List<PublicKeyCredentialDescriptor>>(reader["Credentials"].ToString()!);
            users.Add(new User(uname, displayName, id, credentials));
        }

        var base64CredentialId = Convert.ToBase64String(credentialId);

        return !users.Any(u => u.Credentials.Any(c => Convert.ToBase64String(c.Id) == base64CredentialId));
    }
    
    public void AddOrUpdateUser(User user)
    {
        using var connection = new SqliteConnection(Consts.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT OR REPLACE INTO Users(Username, DisplayName, Id, Credentials)
            VALUES (@username, @displayName, @id, @credentials)";
        
        var serialisedCredentials = JsonSerializer.Serialize(user.Credentials);

        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@displayName", user.DisplayName);
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@credentials", serialisedCredentials);
        
        command.ExecuteNonQuery();
    }

    public void AddCredentialToUser(Fido2User user, PublicKeyCredentialDescriptor credential)
    {
        // Get User
        var userEntity = GetByUsername(user.Name) 
                         ?? new User(user.Name, user.DisplayName, user.Id, []);

        userEntity.Credentials.Add(credential);
        
        AddOrUpdateUser(userEntity);
    }
}

public interface IUserRepository
{
    public User? GetByUsername(string username);
    public bool IsCredentialAlreadyUsed(byte[] credentialId);
    public void AddOrUpdateUser(User user);
    public void AddCredentialToUser(Fido2User user, PublicKeyCredentialDescriptor credential);
}
