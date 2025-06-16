#nullable disable
using System.Security.Cryptography;

namespace Chatt.Core
{

	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string PublicKey { get; set; }

		public static string HashPassword(string password)
		{
			using var sha512 = SHA512.Create();
			var bytes = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			return Convert.ToBase64String(bytes);
		}

		public bool VerifyPassword(string password)
		{
			return PasswordHash == HashPassword(password);
		}
	}

}
