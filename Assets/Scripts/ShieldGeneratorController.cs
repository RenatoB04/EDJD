using UnityEngine;

public class ShieldGeneratorController : MonoBehaviour
{
    // Guardamos a referencia ao material instanciado para o podermos manipular.
    private Material generatorMaterial;
    
    // A variavel atual que vai de 0 a 1. Comeca a 0 (gerador desligado/liso).
    private float currentActiveState = 0f;

    // Etiquetas [Header] e [Tooltip] servem apenas para organizar o Inspector do Unity 
    // e mostrar dicas quando a equipa passa o rato por cima das variaveis.
    [Header("Generator Settings")]
    [Tooltip("How fast the physical displacement deforms when activating/deactivating.")]
    public float transitionSpeed = 2.0f;
    
    [Tooltip("Is the shield currently powered on?")]
    public bool isShieldActive = true;

    void Start()
    {
        // 1. OBTER O MATERIAL INSTANCIADO
        // Apanhamos o componente Renderer (MeshRenderer) que esta neste mesmo objeto.
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Ao chamar 'rend.material' (em vez de 'rend.sharedMaterial'), o Unity cria 
            // uma copia autonoma (instancia) deste material so para este gerador.
            // Isto garante que se tivermos 5 geradores no nivel, podemos desligar um
            // sem que os outros 4 se desliguem ao mesmo tempo.
            generatorMaterial = rend.material;
        }
    }

    void Update()
    {
        // Medida de seguranca: se nao houver material, cancela a execucao deste frame.
        if (generatorMaterial == null) return;

        // 2. DEFINIR O OBJETIVO
        // Um operador ternario (condicao ? verdadeiro : falso). 
        // Se isShieldActive for true, o alvo e 1.0f. Se for false, o alvo e 0.0f.
        float target = isShieldActive ? 1.0f : 0.0f;
        
        // 3. INTERPOLACAO LINEAR (A Magia da Animacao Suave)
        // Usamos Mathf.MoveTowards em vez de Mathf.Lerp porque queremos uma velocidade de 
        // transformacao constante e previsivel. O Lerp abranda a medida que se aproxima do alvo.
        // Multiplicamos por Time.deltaTime para garantir que a velocidade da animacao e 
        // igual em computadores rapidos (144 FPS) e lentos (30 FPS) - chama-se a isto "Frame-Rate Independence".
        currentActiveState = Mathf.MoveTowards(currentActiveState, target, Time.deltaTime * transitionSpeed);
        
        // 4. COMUNICAR COM O SHADER DE TESSELLATION
        // Enviamos o valor que calculamos (que esta sempre a subir ou a descer suavemente)
        // diretamente para a variavel _ActiveState que definimos dentro do ShaderGenerator_Pure.shader.
        // E isto que faz o relevo "crescer" e a luz "acender".
        generatorMaterial.SetFloat("_ActiveState", currentActiveState);
    }

    // Funcao publica que pode ser chamada por outros scripts (como o ShieldInteract)
    // para ligar ou desligar o gerador remotamente quando a vida do escudo chega a zero.
    public void SetShieldActive(bool active)
    {
        isShieldActive = active;
    }
}