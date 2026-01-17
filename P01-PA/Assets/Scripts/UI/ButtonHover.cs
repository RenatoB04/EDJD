using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 normalScale; // Escala normal do botão
    Vector3 targetScale; // Escala alvo quando o botão é hoverado
    [SerializeField] float scaleFactor = 1.05f; // Factor de aumento da escala
    [SerializeField] float speed = 8f; // Velocidade da interpolação da escala
    [Header("Som de Hover")]
    [SerializeField] AudioClip hoverSound; // Som que toca ao passar o rato
    AudioSource audioSource; // Componente AudioSource do botão
    static AudioSource currentHoverSource; // Mantém o último som de hover a tocar para evitar sobreposição

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Não tocar automaticamente
        audioSource.loop = false; // Não fazer loop
        audioSource.volume = 1f; // Volume máximo
    }

    void Start()
    {
        normalScale = transform.localScale; // Guarda a escala inicial
        targetScale = normalScale; // Inicialmente o alvo é a escala normal
    }

    // Chamado quando o ponteiro entra no botão
    public void OnPointerEnter(PointerEventData e)
    {
        targetScale = normalScale * scaleFactor; // Aumenta a escala alvo

        if (hoverSound == null)
        {
            Debug.LogWarning($"[{name}] Nenhum som de hover atribuído!");
            return;
        }

        // Interrompe o som anterior se estiver a tocar
        if (currentHoverSource != null && currentHoverSource.isPlaying)
            currentHoverSource.Stop();

        audioSource.Stop(); 
        audioSource.PlayOneShot(hoverSound); // Toca o som de hover
        currentHoverSource = audioSource; // Marca este AudioSource como o atual
    }

    // Chamado quando o ponteiro sai do botão
    public void OnPointerExit(PointerEventData e)
    {
        targetScale = normalScale; // Regressa à escala normal
    }

    void Update()
    {
        // Interpola suavemente entre a escala atual e a escala alvo
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }
}
