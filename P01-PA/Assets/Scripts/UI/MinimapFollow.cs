using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [Header("Target (player)")]
    public Transform target;                        // Transform do jogador a seguir

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 50f, 0f); // Offset da posição da câmara em relação ao jogador
    public float followSmooth = 10f;                // Velocidade de suavização do movimento da câmara

    [Header("Rotation")]
    public bool lockNorthUp = true;                 // Se verdadeiro, minimapa mantém o norte sempre para cima
    public float pitchDegrees = 90f;                // Inclinação da câmara para visão top-down

    void LateUpdate()
    {
        if (target == null) return;                // Se não houver alvo, não faz nada

        // Calcula posição desejada com base no alvo e offset
        Vector3 desired = new Vector3(target.position.x, target.position.y + offset.y, target.position.z);

        // Move suavemente a câmara até à posição desejada
        transform.position = Vector3.Lerp(transform.position, desired, followSmooth * Time.deltaTime);

        // Define a rotação: se lockNorthUp é true, norte fica sempre para cima, senão segue a rotação do jogador
        float yaw = lockNorthUp ? 0f : target.eulerAngles.y;
        transform.rotation = Quaternion.Euler(pitchDegrees, yaw, 0f);
    }
}
