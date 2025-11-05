using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public playerstats playerHealth;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f; // Duration of attack animation
    
    [Header("Vision Settings")]
    public float viewAngle = 120f; // Görüş açısı (derece) - daha geniş
    public LayerMask obstacleLayerMask; // Görüşü engelleyen katmanlar (varsayılan: boş, sadece duvarlar için)
    public float lostPatrolRadius = 8f; // Oyuncuyu kaybettiği yerde küçük alan devriye
    public float lostDetectionRadius = 30f; // Çok uzaklaşma kontrolü (detectionRadius'un 2 katı)

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 spawnPosition; // Doğduğu yer
    private Vector3 lastSeenPlayerPosition; // Oyuncuyu son gördüğü yer
    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;
    private bool hasSeenPlayer = false; // Oyuncuyu hiç gördü mü?

    private enum State { Patrol, LostPatrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent missing on " + name);
            enabled = false;
            return;
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null && player != null) playerHealth = player.GetComponent<playerstats>();

        // Spawn pozisyonunu kaydet
        spawnPosition = transform.position;
        lastSeenPlayerPosition = spawnPosition;

        SetNewPatrolPoint(spawnPosition, patrolRadius);
        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        // Oyuncuyu görürse pozisyonunu kaydet
        if (canSeePlayer)
        {
            lastSeenPlayerPosition = player.position;
            hasSeenPlayer = true;
        }

        // Cancel attack if player leaves range
        if (isAttacking && distanceToPlayer > attackRange)
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Handle attack duration manually
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        // State logic (only switch when not attacking)
        if (!isAttacking)
        {
            // Önce mesafe kontrolü (basit ve güvenilir)
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
            {
                // Görüş kontrolü varsa ekle, yoksa sadece mesafe
                if (canSeePlayer || distanceToPlayer <= attackRange * 0.5f)
                    currentState = State.Attack;
                else
                    currentState = State.Chase;
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                // Görüş kontrolü varsa ekle
                if (canSeePlayer || distanceToPlayer <= detectionRadius * 0.7f)
                {
                    currentState = State.Chase;
                    hasSeenPlayer = true;
                }
                else if (hasSeenPlayer)
                {
                    // Oyuncuyu kaybetti - LostPatrol moduna geç
                    currentState = State.LostPatrol;
                }
                else
                {
                    currentState = State.Patrol;
                }
            }
            else if (hasSeenPlayer && distanceToPlayer > detectionRadius)
            {
                // Oyuncuyu kaybetti - LostPatrol moduna geç
                currentState = State.LostPatrol;
            }
            else
            {
                currentState = State.Patrol;
            }
        }

        // Execute state behavior
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.LostPatrol:
                LostPatrol();
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.Attack:
                Attack();
                break;
        }

        // Animation states
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        // Rotate only when moving normally
        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    // Görüş kontrolü: Mesafe + açı + raycast
    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Mesafe kontrolü
        if (distanceToPlayer > detectionRadius) return false;

        // Açı kontrolü
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f) return false;

        // Raycast ile engel kontrolü - sadece obstacleLayerMask varsa kontrol et
        if (obstacleLayerMask.value != 0)
        {
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * 0.5f; // Göz seviyesi
            
            if (Physics.Raycast(rayStart, directionToPlayer, out hit, distanceToPlayer, obstacleLayerMask))
            {
                // Bir engel var - Player'a çarptı mı kontrol et
                if (!hit.collider.CompareTag("Player") && !hit.transform.IsChildOf(player))
                {
                    // Player olmayan bir şeye çarptı - engel var
                    return false;
                }
            }
        }

        // Görüş açısında ve mesafede - görüyor
        return true;
    }

    private float stuckTimer = 0f; // Hareketsiz kalma süresi
    private const float STUCK_TIME_LIMIT = 3f; // 3 saniye hareketsiz kalırsa yeni nokta seç

    void Patrol()
    {
        // Spawn pozisyonu etrafında devriye
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint(spawnPosition, patrolRadius);
                idleTimer = 0f;
                stuckTimer = 0f;
            }
            return;
        }

        // Move to patrol point
        if (!isPatrolling)
        {
            SetNewPatrolPoint(spawnPosition, patrolRadius);
            stuckTimer = 0f;
        }

        // Agent path durumunu kontrol et
        if (agent.isOnNavMesh && agent.hasPath)
        {
            // Path var ama agent hareket etmiyor mu?
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= STUCK_TIME_LIMIT)
                {
                    // Takıldı, yeni nokta seç
                    Debug.Log($"{name}: Patrol'de takıldı, yeni nokta seçiliyor...");
                    agent.ResetPath();
                    SetNewPatrolPoint(spawnPosition, patrolRadius);
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f; // Hareket ediyor, timer'ı sıfırla
            }
        }
        else if (agent.isOnNavMesh && !agent.hasPath && isPatrolling)
        {
            // Path yok ama patrol etmeye çalışıyor - yeni nokta seç
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= 1f)
            {
                Debug.Log($"{name}: Patrol path bulunamadı, yeni nokta seçiliyor...");
                SetNewPatrolPoint(spawnPosition, patrolRadius);
                stuckTimer = 0f;
            }
        }

        // Patrol point'e ulaşma kontrolü (mesafe biraz artırıldı)
        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoint);
        if (distanceToPatrolPoint < 2f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
            stuckTimer = 0f;
        }
    }

    // LostPatrol: Oyuncuyu kaybettiği yerde küçük alanda rastgele hareket (zombi gibi)
    void LostPatrol()
    {
        float distanceToLastSeen = Vector3.Distance(transform.position, lastSeenPlayerPosition);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Çok uzaklaştıysa (lostDetectionRadius), kayıp edilen pozisyon etrafında küçük alanda rastgele hareket
        if (distanceToLastSeen > lostDetectionRadius || distanceToPlayer > lostDetectionRadius)
        {
            // Kayıp edilen pozisyon etrafında küçük alanda rastgele hareket
            if (isIdle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= patrolIdleTime * 0.5f) // Daha kısa bekleme süresi
                {
                    SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                    idleTimer = 0f;
                    stuckTimer = 0f;
                }
                return;
            }

            if (!isPatrolling)
            {
                SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                stuckTimer = 0f;
            }

            // Agent path durumunu kontrol et
            if (agent.isOnNavMesh && agent.hasPath)
            {
                if (agent.velocity.sqrMagnitude < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= STUCK_TIME_LIMIT * 0.5f) // Daha hızlı yeni nokta seç
                    {
                        agent.ResetPath();
                        SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                        stuckTimer = 0f;
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
            }
            else if (agent.isOnNavMesh && !agent.hasPath && isPatrolling)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= 1f)
                {
                    SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                    stuckTimer = 0f;
                }
            }

            float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoint);
            if (distanceToPatrolPoint < 2f)
            {
                isIdle = true;
                isPatrolling = false;
                agent.ResetPath();
                stuckTimer = 0f;
            }
        }
        else
        {
            // Hala kayıp edilen pozisyon yakınında, oraya doğru git
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(lastSeenPlayerPosition);
                
                // Takılma kontrolü
                if (agent.velocity.sqrMagnitude < 0.01f && agent.hasPath)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= 1f)
                    {
                        // Takıldı, rastgele hareket et
                        SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                        stuckTimer = 0f;
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
                
                if (distanceToLastSeen < 2f)
                {
                    // Ulaştı, rastgele hareket et
                    SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                }
            }
        }
    }

    void SetNewPatrolPoint(Vector3 center, float radius)
    {
        const float MIN_DISTANCE = 3f; // Minimum mesafe (çok yakın nokta seçilmesin)
        
        // Birden fazla deneme yap
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // Rastgele yön (ileri, geri, sağa, sola, çapraz)
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);
            float distance = Random.Range(radius * 0.3f, radius);
            Vector3 targetPos = center + randomDirection * distance;

            // NavMesh üzerinde nokta ara
            NavMeshHit hit;
            float searchRadius = Mathf.Max(radius, distance);
            if (NavMesh.SamplePosition(targetPos, out hit, searchRadius, NavMesh.AllAreas))
            {
                // Mevcut pozisyondan minimum mesafe kontrolü
                float distanceToCurrent = Vector3.Distance(transform.position, hit.position);
                if (distanceToCurrent < MIN_DISTANCE)
                {
                    continue; // Çok yakın, başka bir nokta dene
                }

                patrolPoint = hit.position;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(patrolPoint);
                    isPatrolling = true;
                    isIdle = false;
                    return; // Başarılı
                }
            }
        }

        // Hiçbir nokta bulunamazsa, mevcut pozisyondan küçük bir adım at
        for (int fallbackAttempt = 0; fallbackAttempt < 5; fallbackAttempt++)
        {
            Vector2 smallStep = Random.insideUnitCircle.normalized * Random.Range(3f, 5f);
            Vector3 fallbackPos = transform.position + new Vector3(smallStep.x, 0, smallStep.y);
            
            NavMeshHit fallbackHit;
            if (NavMesh.SamplePosition(fallbackPos, out fallbackHit, 5f, NavMesh.AllAreas))
            {
                float distanceToCurrent = Vector3.Distance(transform.position, fallbackHit.position);
                if (distanceToCurrent >= MIN_DISTANCE)
                {
                    patrolPoint = fallbackHit.position;
                    if (agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(patrolPoint);
                        isPatrolling = true;
                        isIdle = false;
                        return;
                    }
                }
            }
        }
        
        // Son çare: mevcut pozisyonda kal ve idle ol
        Debug.LogWarning($"{name}: NavMesh noktası bulunamadı, devriye yapılamıyor!");
        isPatrolling = false;
        isIdle = true;
        agent.ResetPath();
    }

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void Attack()
    {
        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        if (agent.isOnNavMesh) agent.ResetPath();

        // Instantly face player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        // Optionally stop movement while attacking
        agent.isStopped = true;
    }

    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerHealth.TakeDamage(10); // Damage amount
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        agent.isStopped = false;
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;
        agent.isStopped = false;

        animator.ResetTrigger("Attack");

        // Instantly cut to movement animation
        if (animator.HasState(0, Animator.StringToHash("Walk")))
            animator.CrossFade("Walk", 0.1f);
        else if (animator.HasState(0, Animator.StringToHash("Idle")))
            animator.CrossFade("Idle", 0.1f);

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.position);
    }

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
