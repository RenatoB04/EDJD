using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public static class UtilitiesWeapons
    {
        public static T SelectAndSetActive<T>(this T[] array, int index) where T : MonoBehaviour
        {
            if (!array.IsValid()) 
                return null;
            array.ForEach(obj => obj.gameObject.SetActive(false));
            if (!array.IsValidIndex(index)) 
                return null;
            T behaviour = array[index];
            if(behaviour != null)
                behaviour.gameObject.SetActive(true);
            return behaviour;
        }
    }
}