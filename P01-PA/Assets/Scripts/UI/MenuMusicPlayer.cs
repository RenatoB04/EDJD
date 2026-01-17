using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MenuMusicPlayer : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Clip de música do menu (loop). Arrasta aqui.")]
    public AudioClip menuClip;                       // Clip de música de fundo do menu
    [Tooltip("Cenas onde o BGM PODE tocar.")]
    public string[] menuScenes = { "Lobby", "MainMenu" }; // Cenas onde a música deve tocar
    [Tooltip("Cenas onde o BGM DEVE parar.")]
    public string[] gameplayScenes = { "Prototype" };     // Cenas onde a música deve parar
    [Range(0f, 1f)]
    public float volume = 0.6f;                      // Volume da música

    private static MenuMusicPlayer _instance;        // Instância singleton
    private AudioSource _source;                      // Componente AudioSource

    private void Awake()
    {
        // Implementa singleton para não ter várias instâncias do mesmo BGM
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Mantém o objeto entre cenas

        // Garante que existe um AudioSource
        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();

        // Configura o AudioSource
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = volume;
        _source.clip = menuClip; 
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;   // Subscrição ao evento de cena carregada
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;   // Remoção da subscrição

    private void Start()
    {
        TryPlayIfAllowed(SceneManager.GetActiveScene().name); // Tenta tocar a música na cena atual
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryPlayIfAllowed(scene.name); // Verifica a música quando uma nova cena é carregada
    }

    private void TryPlayIfAllowed(string sceneName)
    {
        // Se a cena atual estiver na lista de gameplay, para e destrói o BGM
        if (IsInList(sceneName, gameplayScenes))
        {
            StopAndDestroy();
            return;
        }

        // Se a cena estiver na lista de menus, toca o BGM
        if (IsInList(sceneName, menuScenes))
        {
            if (_source != null && !_source.isPlaying && _source.clip != null)
                _source.Play();
        }
    }

    private void StopAndDestroy()
    {
        if (_source != null && _source.isPlaying)
            _source.Stop();
        Destroy(gameObject); // Remove a instância do BGM
    }

    private bool IsInList(string sceneName, string[] list)
    {
        if (list == null) return false;
        foreach (var s in list)
            if (!string.IsNullOrEmpty(s) && s == sceneName)
                return true;
        return false;
    }
}
