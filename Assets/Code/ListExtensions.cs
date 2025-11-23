using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Mengambil 1 item random dari list
    /// dan sekalian menghapusnya dari list
    /// </summary>
    public static T PopRandom<T>(this List<T> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        T value = list[randomIndex];
        list.RemoveAt(randomIndex);
        return value;
    }
}
