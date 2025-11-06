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
    // public float attackRange = 2f; // <-- ESKİ YAKIN SALDIRI MENZİLİ (KULLANILMIYOR)
    public float shootRange = 10f;       // <-- YENİ: Ateş etme menzili
    public float patrolRadius = 20f;
    // public float attackCooldown = 2f; // <-- ESKİSİ
    public float shootCooldown = 2f;     // <-- YENİ: Ateş etme sıklığı
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    // public float attackDuration = 1.0f; // <-- ESKİSİ
    public float shootDuration = 1.0f;  // <-- YENİ: Ateş etme animasyon süresi (veya duraksama süresi)
    
    [Header("Vision Settings")]
    public float viewAngle = 120f; 
    public LayerMask obstacleLayerMask; 
    public float lostPatrolRadius = 8f; 
    public float lostDetectionRadius = 30f; 

    // --- ATEŞ ETME AYARLARI ---
    [Header("Ranged Attack (YENİ)")] 
    public GameObject projectilePrefab; // Düşmanın mermi prefab'ı
    public Transform firePoint;         // Merminin çıkacağı nokta
    public float shootForce = 15f;      // Merminin hızı
    public float aimOffset = 1.2f;      // Oyuncunun gövdesine nişan almak için (örn: 1.2f)
    public bool requireLineOfSight = false; // Ateş etmek için görüş zorunlu mu?
    public LayerMask shootLayerMask;    // <-- YENİ: Ateş ederken neleri vurabilsin
    // ----------------------------------------

    private NavMeshAgent agent;
    private Gun cachedGun; // Düşmanın kendi silah script'i varsa kullanmak için
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 spawnPosition; 
    private Vector3 lastSeenPlayerPosition; 
    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;
    private bool hasSeenPlayer = false; 

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
        cachedGun = GetComponentInChildren<Gun>(true);
        if (playerHealth == null && player != null) playerHealth = player.GetComponent<playerstats>();

        spawnPosition = transform.position;
        lastSeenPlayerPosition = spawnPosition;

        SetNewPatrolPoint(spawnPosition, patrolRadius);
        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;
        Debug.Log("Düşman Adı: " + name + " | Durum: " + currentState + " | Mesafe: " + Vector3.Distance(transform.position, player.position) + " | Cooldown: " + cooldownTimer + " | Görebiliyor mu: " + CanSeePlayer());
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            lastSeenPlayerPosition = player.position;
            hasSeenPlayer = true;
        }

        // --- DEĞİŞİKLİK: attackRange -> shootRange ---
        // Saldırı durumundaysa ve oyuncu menzilden çıktıysa, takibe dön
        if (isAttacking && distanceToPlayer > shootRange) 
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Saldırı animasyonu/süresi devam ediyorsa
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        // --- DÜZELTİLMİŞ STATE MACHINE (DURUM MAKİNESİ) MANTIĞI ---
        if (!isAttacking)
        {
            // Öncelik 1: Saldırı (Dur ve Ateş Et)
            // Ateş menzilindeysek, (görüş gerekliyse) görebiliyorsak VE cooldown bittiyse -> Saldır
            bool canShoot = !requireLineOfSight || canSeePlayer; // Görüş gerekmiyorsa veya görebiliyorsa

            if (distanceToPlayer <= shootRange && canShoot && cooldownTimer <= 0f)
            {
                currentState = State.Attack; // <-- "DUR VE ATEŞ ET" DURUMU
            }
            // Öncelik 2: Takip etme (Chase)
            // Algılama menzilindeysek VE (görebiliyorsak VEYA daha önce gördüysek) -> Takip Et
            else if (distanceToPlayer <= detectionRadius && (canSeePlayer || hasSeenPlayer))
            {
                currentState = State.Chase;
                hasSeenPlayer = true; 
            }
            // Öncelik 3: Oyuncuyu kaybettiyse (LostPatrol)
            // Daha önce gördüysek (ama artık menzilde değilsek) -> Son görülen yere git
            else if (hasSeenPlayer)
            {
                currentState = State.LostPatrol;
            }
            // Öncelik 4: Normal Devriye (Patrol)
            else
            {
                currentState = State.Patrol;
            }
        }
        // -----------------------------------------------------------------


        // --- DÜZELTİLMİŞ SWITCH BLOĞU ---
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
                Attack(); // <-- HATA DÜZELTİLDİ: Artık Attack() fonksiyonunu çağırıyor
                break;
        }
        // ---------------------------------

        // --- ÇAKIŞAN FONKSİYON SİLİNDİ ---
        // TryShootWhileMoving(distanceToPlayer, canSeePlayer); // <-- BU SATIR SİLİNDİ
        // ---------------------------------

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > detectionRadius) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f) return false;

        if (obstacleLayerMask.value != 0)
        {
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * 0.5f; 
            
            if (Physics.Raycast(rayStart, directionToPlayer, out hit, distanceToPlayer, obstacleLayerMask))
            {
                if (!hit.collider.CompareTag("Player") && !hit.transform.IsChildOf(player))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private float stuckTimer = 0f; 
    private const float STUCK_TIME_LIMIT = 3f; 

    void Patrol()
    {
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

        if (!isPatrolling)
        {
            SetNewPatrolPoint(spawnPosition, patrolRadius);
            stuckTimer = 0f;
        }

        if (agent.isOnNavMesh && agent.hasPath)
        {
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= STUCK_TIME_LIMIT)
                {
                    Debug.Log($"{name}: Patrol'de takıldı, yeni nokta seçiliyor...");
                    agent.ResetPath();
                    SetNewPatrolPoint(spawnPosition, patrolRadius);
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
                Debug.Log($"{name}: Patrol path bulunamadı, yeni nokta seçiliyor...");
                SetNewPatrolPoint(spawnPosition, patrolRadius);
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

    void LostPatrol()
    {
        float distanceToLastSeen = Vector3.Distance(transform.position, lastSeenPlayerPosition);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToLastSeen > lostDetectionRadius || distanceToPlayer > lostDetectionRadius)
        {
            if (isIdle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= patrolIdleTime * 0.5f) 
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

            if (agent.isOnNavMesh && agent.hasPath)
            {
                if (agent.velocity.sqrMagnitude < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= STUCK_TIME_LIMIT * 0.5f) 
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
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(lastSeenPlayerPosition);
                
                if (agent.velocity.sqrMagnitude < 0.01f && agent.hasPath)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= 1f)
                    {
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
                    SetNewPatrolPoint(lastSeenPlayerPosition, lostPatrolRadius);
                }
            }
        }
    }

    void SetNewPatrolPoint(Vector3 center, float radius)
    {
        const float MIN_DISTANCE = 3f; 
        
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);
            float distance = Random.Range(radius * 0.3f, radius);
            Vector3 targetPos = center + randomDirection * distance;

            NavMeshHit hit;
            float searchRadius = Mathf.Max(radius, distance);
            if (NavMesh.SamplePosition(targetPos, out hit, searchRadius, NavMesh.AllAreas))
            {
                float distanceToCurrent = Vector3.Distance(transform.position, hit.position);
                if (distanceToCurrent < MIN_DISTANCE)
                {
                    continue; 
                }

                patrolPoint = hit.position;
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

    // --- BU FONKSİYON ARTIK DOĞRU ZAMANDA ÇAĞRILACAK ---
    void Attack()
    {
        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > shootRange) 
        {
            currentState = State.Chase;
            return;
        }

        // Ek güvenlik: Ateş için görüş gerekiyorsa VE göremiyorsa, takibe dön
        if (requireLineOfSight && !CanSeePlayer())
        {
            currentState = State.Chase;
            return;
        }

        isAttacking = true;
        cooldownTimer = shootCooldown; 
        attackTimer = shootDuration;   

        if (agent.isOnNavMesh) agent.ResetPath(); // Hareketi durdur

        // Ateş ederken oyuncuya dön
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Ateş et
        ShootAtPlayer();
        
        // (İsteğe bağlı: Eğer "Shoot" adında bir animasyonunuz varsa burada tetikleyebilirsiniz)
        animator.SetTrigger("Shoot"); 

        agent.isStopped = true; // Hareket etmediğinden emin ol
    }

    // --- YAKIN SALDIRI HASARI (MELEE) DEVRE DIŞI ---
    /*
    public void DealDamage()
    {
        // Bu fonksiyon artık kullanılmıyor, çünkü mermiler hasar verecek
    }
    */

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        agent.isStopped = false; // Harekete devam et
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = shootCooldown; // İptal ederse de cooldown'a girsin
        agent.isStopped = false;

        animator.ResetTrigger("Attack"); 
        animator.ResetTrigger("Shoot"); // Shoot animasyonunuz varsa

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


    // --- HAREKETLİ ATEŞ ETME FONKSİYONU SİLİNDİ (TryShootWhileMoving) ---
    // ...
    // ...
    // -------------------------------------------------------------------


    // --- ATEŞ ETME FONKSİYONU (GÜNCELLENDİ) ---
    void ShootAtPlayer()
    {
        if (player == null)
        {
            return;
        }

        // 0) Öncelik sırası: EnemyAI alanları -> cachedGun -> child adından FirePoint
        GameObject useProjectile = projectilePrefab;
        Transform useFirePoint = firePoint;
        float useShootForce = shootForce;

        if ((useProjectile == null || useFirePoint == null) && cachedGun != null)
        {
            if (useProjectile == null) useProjectile = cachedGun.projectilePrefab;
            if (useFirePoint == null) useFirePoint = cachedGun.firePoint;
            if (useShootForce <= 0f) useShootForce = Mathf.Max(1f, cachedGun.shootForce);
        }

        if (useFirePoint == null)
        {
            Transform found = transform.Find("FirePoint");
            if (found != null) useFirePoint = found;
        }

        if (useFirePoint == null)
        {
            // Son çare: kendi transformundan ateş et
            useFirePoint = this.transform;
        }

        if (useProjectile == null)
        {
            Debug.LogWarning(name + ": Projectile atanmamış (ne EnemyAI ne de alt Gun bulundu).");
            return;
        }

        // 1) Oyuncunun göğüs hizasına nişan al
        Vector3 desiredTarget = player.position + Vector3.up * aimOffset;

        // 2) FirePoint'ten oyuncu doğrultusunda ray at (LayerMask kullanarak)
        Vector3 toTarget = (desiredTarget - useFirePoint.position);
        Vector3 rayDir = toTarget.normalized;
        RaycastHit hitInfo;
        Vector3 targetPoint = desiredTarget;

        // --- GÜNCELLEME: LayerMask eklendi ---
        // Maske atanmadıysa, her şeyi vursun (eski davranış)
        int mask = (shootLayerMask.value == 0) ? ~0 : shootLayerMask.value; 

        if (Physics.Raycast(useFirePoint.position, rayDir, out hitInfo, Mathf.Max(2f, toTarget.magnitude), mask))
        {
            targetPoint = hitInfo.point;
        }

        // 3) Son hedefe doğru yön
        Vector3 direction = (targetPoint - useFirePoint.position).normalized;

        // 4) Mermiyi oluştur ve yönlendir
        GameObject projectile = Instantiate(useProjectile, useFirePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Mermiye kuvvet uygula
            rb.AddForce(direction * useShootForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError(useProjectile.name + " üzerinde Rigidbody bulunamadı!");
        }
    }
}