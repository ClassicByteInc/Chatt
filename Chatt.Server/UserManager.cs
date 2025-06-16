using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Chatt.Core;

namespace Chatt.Server
{
	internal class UserManager
	{
		private static readonly string DbPath = "User.DB";

		private static SQLiteConnection GetConnection()
		{
			bool newDb = !File.Exists(DbPath);
			var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;");
			conn.Open();
			if (newDb)
			{
				using var cmd = conn.CreateCommand();
				cmd.CommandText = @"
					CREATE TABLE IF NOT EXISTS Users (
						Id TEXT PRIMARY KEY,
						Username TEXT,
						Password TEXT,
						Email TEXT
					);
				";
				cmd.ExecuteNonQuery();
			}
			return conn;
		}

		public List<User> GetUsers()
		{
			var users = new List<User>();
			using var conn = GetConnection();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT Id, Username, Password, Email FROM Users";
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				users.Add(new User
				{
					Id = int.Parse(reader["Id"]?.ToString()??"-1"),
					Username = reader["Username"]?.ToString() ?? "",
					PasswordHash = User.HashPassword(reader["Password"]?.ToString() ?? ""),
					Email = reader["Email"]?.ToString() ?? ""
				});
			}
			return users;
		}

		public User? GetUserById(string id)
		{
			using var conn = GetConnection();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT Id, Name, Password, Email FROM Users WHERE Id = @id";
			cmd.Parameters.AddWithValue("@id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				return new User
				{
					Id = int.Parse(reader["Id"]?.ToString() ?? "-1"),
					Username = reader["Username"]?.ToString() ?? "",
					PasswordHash = User.HashPassword(reader["Password"]?.ToString() ?? ""),
					Email = reader["Email"]?.ToString() ?? ""
				};
			}
			return null;
		}

		public void AddUser(User user)
		{
			using var conn = GetConnection();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = @"
				INSERT OR IGNORE INTO Users (Id, Username, Password, Email)
				VALUES (@id, @Username, @password, @email);
			";
			cmd.Parameters.AddWithValue("@id", user.Id);
			cmd.Parameters.AddWithValue("@Username", user.Username);
			cmd.Parameters.AddWithValue("@password", user.PasswordHash);
			cmd.Parameters.AddWithValue("@email", user.Email);
			cmd.ExecuteNonQuery();
		}

		public void DeleteUser(string id)
		{
			using var conn = GetConnection();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = "DELETE FROM Users WHERE Id = @id";
			cmd.Parameters.AddWithValue("@id", id);
			cmd.ExecuteNonQuery();
		}
	}
}
