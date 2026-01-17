using UnityEngine;
using Unity.Netcode;
using Unity.Collections; 
using Photon.Pun;        

public class PlayerName : NetworkBehaviour
{
    // Variável de rede que guarda o nome do jogador
    // Todos podem ler, apenas o dono pode escrever
    public NetworkVariable<FixedString32Bytes> netName = new NetworkVariable<FixedString32Bytes>(
        "Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Propriedade de conveniência para obter o nome como string
    public string Name => netName.Value.ToString();

    public override void OnNetworkSpawn()
    {
        // Só executa para o dono deste objeto
        if (IsOwner)
        {
            string myName = PhotonNetwork.NickName;

            // Se o nickname do Photon estiver vazio, usa um padrão
            if (string.IsNullOrEmpty(myName))
            {
                myName = "Player " + OwnerClientId;
            }

            // Limita o tamanho do nome a 30 caracteres
            if (myName.Length > 30) myName = myName.Substring(0, 30);

            // Define o nome na NetworkVariable
            netName.Value = new FixedString32Bytes(myName);
        }
    }

    // Método GUI vazio, pode ser usado futuramente para mostrar o nome
    void OnGUI()
    {
    }
}
