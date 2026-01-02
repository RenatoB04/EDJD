using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MenuMusicPlayer : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Clip de música do menu (loop). Arrasta aqui.")]
    public AudioClip menuClip;

    [Tooltip("Cenas onde o BGM PODE tocar.")]
    public string[] menuScenes = { "Lobby", "MainMenu" };

    [Tooltip("Cenas onde o BGM DEVE parar.")]
    public string[] gameplayScenes = { "Prototype" };

    [Range(0f, 1f)]
    public float volume = 0.6f;

    private static MenuMusicPlayer _instance;
    private AudioSource _source;

    private void Awake()
    {
        // Singleton simples para evitar duplicados
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();

        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = volume;
        _source.clip = menuClip; // podes deixar vazio até arrastares um clip
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        TryPlayIfAllowed(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryPlayIfAllowed(scene.name);
    }

    private void TryPlayIfAllowed(string sceneName)
    {
        // Se for cena de gameplay, para e destrói
        if (IsInList(sceneName, gameplayScenes))
        {
            StopAndDestroy();
            return;
        }

        // Se for cena de menu, toca se tiver clip
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
        Destroy(gameObject);
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