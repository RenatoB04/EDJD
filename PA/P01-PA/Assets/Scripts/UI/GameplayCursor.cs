using UnityEngine;

public static class GameplayCursor
{
    // Bloqueia o cursor no centro do ecrã e torna-o invisível
    public static void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Liberta o cursor, permitindo mover livremente e torna-o visível
    public static void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
