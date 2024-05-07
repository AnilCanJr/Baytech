using System;
using System.Collections.Concurrent;

namespace BaytechBackend
{
	public class SharedDB
	{
		private readonly ConcurrentDictionary<string, UserConnection> _connections = new ConcurrentDictionary<string, UserConnection>();
		public ConcurrentDictionary<string, UserConnection> connections => _connections;

    }
}

