using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject targetPrefab;
     [Header("stats")]
    public int MissedTargets = 0;
    [Header("Position de la Ligne")]
    [SerializeField] private Transform playerTransform; // Glisse ton Player ici
    [SerializeField] private float hauteurCible = 1.5f;   // Hauteur fixe (ex: 1.5m du sol)
    [SerializeField] private float distanceDevant = 5f;  // Distance devant le joueur
    [SerializeField] private float largeurLigne = 4f;    // Largeur totale de la zone de spawn

    [Header("Sécurité Murs")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float sphereRadius = 0.5f;

    [Header("Configuration")]
    [SerializeField] private int columns = 5;

    private GameObject current;
    private bool gameStarted = false;
    private int lastColIndex = -1;

    void Awake() 
    {
        // Si tu n'as pas assigné le joueur, on cherche celui qui a le tag "Player"
        if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartGame() { 
        MissedTargets = 0;
        gameStarted = true; 
        Respawn(); }
    public void StopGame() { gameStarted = false; if (current != null) Destroy(current); }
    public void ResetStats()
            {
                MissedTargets = 0;
            }
    public void Spawn()
    {
        if (!gameStarted || current != null || playerTransform == null) return;

        // 1. Choisir une colonne
        int col = Random.Range(0, columns);
        if (col == lastColIndex && columns > 1) col = (col + 1) % columns;
        lastColIndex = col;

        // 2. Calculer la position "de base" (en face du joueur au niveau du sol)
        Vector3 directionDevant = playerTransform.forward;
        directionDevant.y = 0; // On annule l'inclinaison de la vue (très important !)
        directionDevant.Normalize();

        Vector3 centreLigne = playerTransform.position + (directionDevant * distanceDevant);
        centreLigne.y = hauteurCible; // On force la hauteur fixe

        // 3. Calculer le décalage gauche/droite
        Vector3 directionDroite = playerTransform.right;
        directionDroite.y = 0;
        directionDroite.Normalize();

        float offset = Mathf.Lerp(-largeurLigne / 2f, largeurLigne / 2f, (float)col / (columns - 1));
        Vector3 spawnPos = centreLigne + (directionDroite * offset);

        // 4. Vérifier si un mur bloque
        if (Physics.CheckSphere(spawnPos, sphereRadius, obstructionMask))
        {
            // Si c'est bouché, on réessaie une autre colonne immédiatement
            Spawn(); 
            return;
        }

        current = Instantiate(targetPrefab, spawnPos, Quaternion.identity);
    }

    public void Respawn()
    {
        if (!gameStarted) return;
        if (current != null) Destroy(current);
        current = null;
        Spawn();
    }

    // Pour voir la ligne dans l'éditeur (fenêtre Scene)
    void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        Gizmos.color = Color.cyan;
        Vector3 dir = playerTransform.forward; dir.y = 0; dir.Normalize();
        Vector3 center = playerTransform.position + (dir * distanceDevant);
        center.y = hauteurCible;
        
        Vector3 right = playerTransform.right; right.y = 0; right.Normalize();
        Vector3 start = center - (right * largeurLigne / 2f);
        Vector3 end = center + (right * largeurLigne / 2f);
        Gizmos.DrawLine(start, end);
    }
}