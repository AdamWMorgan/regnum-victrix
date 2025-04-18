using System;
using System.Collections.Generic;
using System.Linq;

public static class LevellingUtil<T> where T : Enum
{
	public static T LevelUp(int currLevel) {
		T maxLevel = Enum.GetValues(typeof(T)).Cast<T>().Max();
		// If not at max, increment
		if (currLevel < Convert.ToInt32(maxLevel))
		{
			return (T)Enum.ToObject(typeof(T), currLevel + 1);
		}
		// Already at max, return current level
		return (T)Enum.ToObject(typeof(T), currLevel);
		}
}
