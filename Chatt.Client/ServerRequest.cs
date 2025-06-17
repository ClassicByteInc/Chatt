using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatt.Client
{
	internal class ServerRequest
	{
		private string _type;
		private object _data;
		private object _result; // Assuming this is the result of the request

		public ServerRequest Request()
		{
			using var tcpClient = new System.Net.Sockets.TcpClient(Program._defaultServer, Program.Port); // Example port
			using var stream = tcpClient.GetStream();
			if (!stream.CanWrite || !stream.CanRead)
			{
				throw new InvalidOperationException("Stream is not writable or readable.");
			}
			var requestData = new { Type = _type, Data = _data }; // Example request data structure
			stream.Write(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(requestData), 0, System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(requestData).Length);
			var buffer = new byte[1024]; // Example buffer size
			return this;
		}

		public ServerRequest Create()
		{
			return this;
		}

		public ServerRequest SetType(string type)
		{
			_type = type;
			return this;
		}

		public ServerRequest SetData(object data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data), "Data cannot be null.");
			}
			_data = data;
			// Assuming we store
			// data in some internal structure, e.g., a dictionary or property
			// this.Data = data; // Uncomment and implement as needed
			return this;
		}

		public object GetResult()
		{
			return _result; // Assuming this returns the result of the request
		}
	}
}
