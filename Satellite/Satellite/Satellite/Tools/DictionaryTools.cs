﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public static class DictionaryTools
	{
		public static Dictionary<string, V> Create<V>()
		{
			return new Dictionary<string, V>(new StringTools.IEComp());
		}

		public static Dictionary<string, V> CreateIgnoreCase<V>()
		{
			return new Dictionary<string, V>(new StringTools.IECompIgnoreCase());
		}

		public static V Get<K, V>(Dictionary<K, V> dict, K key, V defval)
		{
			if (dict.ContainsKey(key))
			{
				return dict[key];
			}
			return defval;
		}

		public static void Put<K, V>(Dictionary<K, V> dict, K key, V value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
			}
			else
			{
				dict.Add(key, value);
			}
		}

		public static void Remove<K, V>(Dictionary<K, V> dict, K key)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
			}
		}
	}
}
