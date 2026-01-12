using UnityEngine;
using UnityEngine.Rendering;

public class LocalPlayerVisualHider : MonoBehaviour
{
    [Tooltip("Se true, o corpo fica invisível mas continua a projetar sombra.")]
    public bool shadowsOnly = true; // Se verdadeiro, apenas a sombra é renderizada

    [Tooltip("Se preenchido, só estes Renderers serão afetados (senão procura todos nos filhos).")]
    public Renderer[] targetRenderers; // Renderers específicos a esconder

    void Start()
    {
        // Se não foram especificados renderers, procura todos nos filhos
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);

        // Aplica a lógica de invisibilidade a cada renderer
        foreach (var r in targetRenderers)
        {
            if (!r) continue; // Ignora null

            if (shadowsOnly)
            {
                // Apenas projeta sombra, não renderiza objecto
                r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                // Desativa completamente o renderer
                r.enabled = false;
            }
        }
    }

    void OnDisable()
    {
        // Quando este script é desativado, restaura todos os renderers
        if (targetRenderers == null) return;

        foreach (var r in targetRenderers)
        {
            if (!r) continue; // Ignora null

            r.shadowCastingMode = ShadowCastingMode.On; // Restaura sombras normais
            r.enabled = true; // Restaura visibilidade
        }
    }
}
