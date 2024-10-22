using System.Collections.Generic;

static class DictionaryUtil
{
    public static bool HasAnyKey<K, V>(Dictionary<K, V> dict, List<K> potentialKeys)
    {
        foreach (K potentialKey in potentialKeys)
        {
            if (dict.ContainsKey(potentialKey))
            {
                return true;
            }
        }
        return false;
    }

    public static Dictionary<K, V> RemoveKeys<K, V>(Dictionary<K, V> dict, List<K> keysToRemove)
    {
        Dictionary<K, V> result = new(dict);

        foreach (K keyToRemove in keysToRemove)
        {
            if (result.ContainsKey(keyToRemove))
            {
                result.Remove(keyToRemove);
            }
        }
        return result;
    }
}
