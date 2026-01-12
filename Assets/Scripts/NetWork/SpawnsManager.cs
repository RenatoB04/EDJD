using UnityEngine;
using Unity.Netcode;   

public class SpawnsManager : MonoBehaviour
{
    public static SpawnsManager I; // Instância singleton para acesso global

    [Header("Pontos de spawn")]
    public Transform[] points; // Lista de pontos de spawn na cena

    int nextIdx = 0; // Índice do próximo spawn (circular)

    void Awake() => I = this; // Inicializa singleton

    /// <summary>
    /// Obtém a posição e rotação do próximo spawn.
    /// </summary>
    public void GetNext(out Vector3 pos, out Quaternion rot)
    {
        // Se não houver pontos definidos, devolve valores padrão
        if (points == null || points.Length == 0)
        {
            pos = Vector3.zero;
            rot = Quaternion.identity;
            return;
        }

        // Seleciona o ponto seguinte em loop circular
        var t = points[nextIdx % points.Length];
        nextIdx++;

        // Levanta ligeiramente a posição para evitar colisão com o chão
        pos = t.position + Vector3.up * 0.1f;
        rot = t.rotation;
    }

    /// <summary>
    /// Coloca um objeto NetworkObject na posição e rotação do próximo spawn.
    /// </summary>
    public void Place(NetworkObject playerObj)
    {
        GetNext(out var pos, out var rot);
        playerObj.transform.SetPositionAndRotation(pos, rot);
    }
}
