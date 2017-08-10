using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class CrossMap<K, V>
	{
		private Dictionary<K, V> _map = new Dictionary<K, V>();
		private Dictionary<V, K> _inv = new Dictionary<V, K>();

		public CrossMap()
		{ }

		public void Add(K key, V value)
		{
			AddTo(_map, key, value);
			AddTo(_inv, value, key);
		}

		private static void AddTo<KK, VV>(Dictionary<KK, VV> map, KK key, VV value)
		{
			if (map.ContainsKey(key))
				map.Remove(key);

			map.Add(key, value);
		}

		public Dictionary<K, V> Map
		{
			get
			{
				return _map;
			}
		}

		public Dictionary<V, K> Inv
		{
			get
			{
				return _inv;
			}
		}
	}
}
