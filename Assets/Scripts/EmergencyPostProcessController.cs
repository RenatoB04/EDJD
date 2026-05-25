using UnityEngine;

// Obriga o Unity a executar este codigo mesmo quando nao estamos em modo Play.
// Isto e muito util para podermos afinar o efeito visual diretamente na janela "Scene".
[ExecuteInEditMode]
// Medida de seguranca: Garante que este script so pode ser adicionado a um objeto 
// que tenha um componente Camera. Se nao tiver, o Unity adiciona uma automaticamente.
[RequireComponent(typeof(Camera))]
public class EmergencyPostProcessController : MonoBehaviour
{
    [Header("Shader Reference")]
    [Tooltip("The EmergencyPostProcessing shader file.")]
    public Shader postProcessShader;
    
    // O material e privado porque vai ser gerado dinamicamente por codigo, 
    // nao precisamos de criar um material manualmente na pasta do projeto.
    private Material postProcessMaterial;

    [Header("Emergency Settings")]
    [Range(0.0f, 1.0f)]
    [Tooltip("0 = disabled, 1 = full screen glitch and vignette effect.")]
    // Variavel central controlada pelo script do Escudo. Dita a intensidade do alarme.
    public float emergencyIntensity = 0.0f;
    
    [Range(0.0f, 0.1f)]
    [Tooltip("Maximum scale of horizontal noise screen displacement.")]
    public float glitchScale = 0.03f;
    
    [Range(0.0f, 0.05f)]
    [Tooltip("Offset distance for RGB split chromatic aberration.")]
    public float chromaticAberration = 0.015f;
    
    [Tooltip("Warning border glow color.")]
    public Color emergencyColor = new Color(0.8f, 0.0f, 0.0f, 1.0f);

    void Start()
    {
        // Se nos esquecermos de arrastar o shader no Inspector, o script procura-o 
        // automaticamente na base de dados do Unity pelo seu caminho/nome.
        if (postProcessShader == null)
            postProcessShader = Shader.Find("Custom/EmergencyPostProcessing");
    }

    // FUNCAO VITAL (OnRenderImage):
    // Esta e uma funcao especial do Unity. E chamada no final de cada frame, 
    // depois de a camara ter renderizado todo o cenario tridimensional 3D, 
    // mas antes de mostrar essa imagem no monitor do jogador.
    // 'source' e a imagem original do jogo. 'destination' e o ecra final.
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Otimizacao: Se o shader nao existir ou a intensidade for 0 (escudo com vida cheia),
        // nao gastamos processamento da placa grafica. Copiamos a imagem original 
        // diretamente para o destino sem passar por nenhum filtro.
        if (postProcessShader == null || emergencyIntensity <= 0.0f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Se o material ainda nao existir, criamo-lo virtualmente usando o nosso Shader.
        if (postProcessMaterial == null || postProcessMaterial.shader != postProcessShader)
        {
            postProcessMaterial = new Material(postProcessShader);
            // HideFlags.DontSave previne que o Unity tente guardar este material 
            // temporario na memoria do projeto, o que causaria "memory leaks".
            postProcessMaterial.hideFlags = HideFlags.DontSave;
        }

        // Comunicacao C# -> Shader: 
        // Enviamos os valores atuais das nossas variaveis para dentro do Shader.
        postProcessMaterial.SetFloat("_EmergencyIntensity", emergencyIntensity);
        postProcessMaterial.SetFloat("_GlitchScale", glitchScale);
        postProcessMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
        postProcessMaterial.SetColor("_EmergencyColor", emergencyColor);

        // FUNCAO VITAL (Graphics.Blit):
        // Pega na textura de origem (source), processa-a passando-a pelo 
        // nosso material de pos-processamento, e imprime o resultado na textura de destino.
        Graphics.Blit(source, destination, postProcessMaterial);
    }

    // Limpeza de memoria. Chamado quando apagamos o script ou mudamos de cena.
    void OnDisable()
    {
        // Destruir o material temporario para libertar a memoria RAM.
        if (postProcessMaterial != null)
        {
            DestroyImmediate(postProcessMaterial);
        }
    }
}