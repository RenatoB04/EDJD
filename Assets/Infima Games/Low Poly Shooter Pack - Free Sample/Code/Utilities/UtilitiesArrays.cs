using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public static class UtilitiesArrays
    {
        public static bool IsValidIndex<T>(this T[] array, int index) => array.Length > index && index >= 0;
        public static bool IsValid<T>(this T[] array) => !array.Equals(null) && array.Length > 0;
        public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];
    }
}