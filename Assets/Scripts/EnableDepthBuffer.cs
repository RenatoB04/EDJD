using UnityEngine;

// O Unity, por questões de otimização e poupança de processamento, não renderiza
// a profundidade do cenário (Z-Buffer) para uma textura global de forma automática 
// na Built-in Render Pipeline. 
// Como o nosso escudo precisa dessa textura para calcular a interseção com o chão
// (lendo a variável global _CameraDepthTexture no shader), este script obriga a câmara
// a gerar essa informação gráfica a cada frame.

// Garante que o efeito de interseção do escudo é visível na janela "Scene" enquanto 
// editamos o jogo, sem termos de estar no modo Play.
[ExecuteInEditMode]
public class EnableDepthBuffer : MonoBehaviour
{
    // A função OnEnable corre imediatamente assim que o objeto/script é ativado.
    void OnEnable()
    {
        // Apanha o componente da câmara onde este script tem de estar colocado.
        var cam = GetComponent<Camera>();
        
        // Medida de segurança: se o script for arrastado por engano para um objeto sem câmara, 
        // aborta a execução silenciosamente (return) em vez de rebentar o jogo com um erro.
        if (!cam) return;

        //  Operador Bitwise '|='
        // Se usássemos 'cam.depthTextureMode = DepthTextureMode.Depth;', estaríamos a apagar 
        // brutalmente quaisquer outras texturas especiais que a câmara já estivesse a gerar 
        // (como o DepthNormals, usado para gerar sombras e luzes complexas).
        // Ao usar o operador lógico OR ('|='), estamos a dizer ao Unity: 
        // "Mantém tudo o que já estavas a fazer, mas ADICIONA também a flag de Depth".
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }
}