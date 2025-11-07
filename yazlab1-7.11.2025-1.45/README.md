## Yapay Zeka Destekli TPS (Third Person Shooter) Oyunu
### Projeyi Yapan Kişiler
- Duha Yusuf BİNDERE / 231307077
- Ahmet ÖZ / 231307094
- Göksel BEKDEMİR / 231307038

### Kısa Tanım
Bu proje 3 üniversite öğrencisi tarafından üniversite projesi olarak tasarlanmıştır. Projenin amacı yapay zeka destekli, NPC karakterlerin devriye, takip, idle ve ateş mekaniklerinin olduğu bir TPS shooter oyun yapmaktır. Bu projeyi geliştirirken animasyonlar, efektler gibi assetler hazır alınmış bunun haricindeki mekanikler ve değişkenler el ile yapılmıştır.

### Görev Dağılımı
- Duha Yusuf BİNDERE: Karakter ve düşman animasyonların sağlanması ve karakterlerin texture ile görselleştirilmesi. Karakter hareket mekanikleri(çift zıplama ve dash vb.). Ateş etme mekaniği. Raporlama
- Ahmet ÖZ: Kameranın ayarlanması ve mermilerin hedeflenmesinin ayarlanması. Hasar verme. Enemy ai FSM durumlarının tanımlanması. Spawner oluşturulması ve nasmesh ile bake edilmesi. Menülerin tasarlanması. Raporlama
- Göksel BEKDEMİR: Ana oyun alanının oluşturulması ve VFX eklenmesi. Ui panelinin oluşturulması ve karakter istatistikleri ile eşlenmesi ve kaydedilmesi. Karakter ve düşman level atlaması ile statların artırılması. Raporlama

### İçindekiler
- Proje Hikayesi 
- Proje Senaryosu
- Platform ve Teknolojiler 
- Oynanış Mekanikleri ve Kontroller 
- Mimari Genel Bakış 
- Karakter Hareketi 
- TPS Kamera 
- Ateş Mekaniği 
- Oyuncu Can/XP/Seviye Sistemi 
- Düşman Yapay Zekası – FSM + NavMesh 
- Düşman İstatistikleri ve Dalgalar 
- Mermiler (Oyuncu/Düşman) 
- Kazanma/Kaybetme ve Menüler 
- Tasarlanan Sahneler 
- Zorluklar ve Çözümler 
- Literatür Notları ve Kısa Karşılaştırmalar 
- Bilinen Sınırlar ve İyileştirme Önerileri 
- Projenin Kattığı Fayda

---
## Proje Hikayesi
- Gelecekte, büyücülerin "Ebedi Kristaller" yüzünden zamanı yok etmesi üzerine, son insanlar Prophet (Kâhin) adında bir robotu geçmişe gönderir. Prophet'in görevi, bu felakete neden olan kristalleri bulup yok etmektir. Orta Çağ'a vardığında, düşük seviyeli büyücüleri kolayca yener ve kalenin yerini öğrenir. Gölge Ormanı'ndaki kaleyi ve onu koruyan druidleri öldürüp kristalleri patlatması gerekmektedir.
## Proje Senaryosu
- Oyuncu, kaleye benzer açık bir arenada dalgalar halinde ortaya çıkan düşmanlarla savaşır.
- Çevredeki “Shard” kristallerini yok etmek ana amaçtır. 3 kristal yok edildiğinde oyun kazanılır.
- Düşmanlar devriye gezer, oyuncuyu görürse kovalar ve menzile girince ateş eder.

## Platform ve Teknolojiler
- Platform: PC (Windows).
- Oyun Motoru: Unity (URP uyumlu proje yapısı).
- Navigasyon: Unity NavMesh + `NavMeshAgent`.

## Oynanış Mekanikleri ve Kontroller
- Hareket: W/A/S/D (kamera yönüne göre)
- Sprint: Left Shift
- Zıplama: Space (çift zıplama)
- Dash: Left Ctrl (kısa süreli hızlanma)
- Nişan alma (Zoom): Sağ Fare (RMB)
- Ateş etme: Sol Fare (LMB), otomatik atış hızı (fire rate)
- Duraklatma: Esc
- Kazanma: 3 `Shard` yok edildiğinde
- Kaybetme: Can bittiğinde ana menüye dönüş

## Mimari Genel Bakış
- Karakter: `Assets/scripts/character/character movement.cs`, `playerstats.cs`, `HealthBar.cs`, `XpBar.cs`
- Kamera: `Assets/scripts/camera/TPS_Camera.cs`
- Silah/Mermi: `Assets/scripts/fire mecanic/gun.cs`, `Projectile.cs`, `Projectileenemy.cs`
- Düşman: `Assets/scripts/enemy/enemyai.cs` (FSM), `EnemyStats.cs`/`EnemyLevel.cs`
- Dalga/Spawner: `Assets/scripts/enemy/spawn/EnemySpawner.cs`
- Oyun döngüsü: `Assets/scripts/enemy/Shard/ShardStats.cs` (win), `pausemenu/PauseMenu.cs`, `winmenu/WinMenu.cs`, `mainmenu/MainMenuManager.cs`

## Karakter Hareketi 
Sınıf: `SimpleTransformMovement` — Dosya: `Assets/scripts/character/character movement.cs`

Amaç: Kamera yönüne göre TPS karakter kontrolü; sprint, çift zıplama, dash, yerçekimi, animator parametre güncellemesi.

Başlıca Alanlar:
- Hareket: `moveSpeed`, `sprintMultiplier`, `rotationSpeed`
- Zıplama: `canJump`, `jumpcounter`, `jumpForce`, `groundCheckDistance`, `IsGrounded()`
- Dash: `dashMultipler`, `dashDuration`, `dashCooldown`, `dashDirection`
- Fizik: `playerRb` (Rigidbody), `gravityMultiplier` (düşüş ivmesini artırır)
- Çarpışma: `CapsuleCollider` (dash sırasında boy/merkez ayarı)
- Kamera: `cameraTransform` (yoksa `Camera.main` atanır)
- Animasyon: `Animator` parametreleri: `Vertical`, `Horizontal`, `IsSprint`, `IsGrounded`, tetikleyiciler: `JumpTrig`, `DashTrig`

Çift Zıplama:
- Çift zıplama mekaniğinin iyi çalışabilmesi için karakterin havada zıplama yaptıktan sonra üçüncü bir zıplama yapmaması gerekmektedir. Bunun için  `canJump`, `jumpcounter`,`IsGrounded()` değerleri kullanılmalıdır. Her zıplama yapıldığında `jumpcounter` değeri arttırılır ve belli bir değerin üstüne çıktığında `canJump` değeri false olur. Böylece karakterin ikiden fazla zıplama yapması engellenmiştir. Bunun yanında oyunuc eğer düşerken zıplamak isterse düşme hızını sıfıra indirerek zıplama yapılmaktadır bu sayede düşme hızıyla zıplama hızı birbirini nötrlememektedir.
Dash:
- Dash mekaniği oyuncunun koşu hızı `dashMultipler`ile çarparak hesaplanır. Hesaplanan hız `dashDuration` kadar devam ettirilir. Daha sonrasında `dashCooldown` kadar oyuncunun dash atması engellenir.
Blok Şeması:


Inspector Önerileri:
- `Rigidbody`: Use Gravity açık, Rotation kilitli.
- `CapsuleCollider`: Karakter boyuna uygun `height/center`.
- `Animator`: Parametre adları controller ile uyumlu olmalı.
- `cameraTransform`: TPS pivot ya da `Main Camera` transform’u.

---

## TPS Kamera
Sınıf: `TPS_CameraController` — Dosya: `Assets/scripts/camera/TPS_Camera.cs`

Amaç: Farenin X/Y hareketiyle kamera rotasyonu, nişan (RMB) iken FOV düşürme (zoom), nişanda karakter Y rotasyonunu kamera ile eşitleme.

Başlıca Alanlar:
- Hassasiyet: `normalSensitivity`, `aimSensitivity`
- FOV: `normalFOV`, `zoomedFOV`, `zoomSpeed`
- Açı limitleri: `minYAngle`, `maxYAngle`
- Karakter dönüş senkronu: `characterRotationSpeed`

Akış:
- `Start()`: Cursor kilidi ve görünürlüğü, başlangıç FOV.
- `Update()`: Mouse X/Y biriktirme, nişanda hassasiyet düşürme, Y clamp.
- `LateUpdate()`: Kamera poz/rot, FOV Lerp, nişanda karakter Y ekseni `Slerp` ile kameraya döndürülür.

---

## Ateş Mekaniği
Sınıf: `Gun` — Dosya: `Assets/scripts/fire mecanic/gun.cs`

Amaç: LMB basılıyken `fireRate` ile sınırlı otomatik atış. Kamera viewport merkezinden ray ile hedef noktayı belirler; mermiyi `firePoint`’tan hedefe doğru fırlatır.

Başlıca Alanlar:
- `projectilePrefab`, `firePoint`, `shootForce`
- `tpsCamera`, `maxShootDistance`, `aimLayerMask`
- Otomatik ateş: `fireRate`, `nextFireTime`

Akış:
- `Update()`: LMB basılı ve süresi dolmuşsa `Shoot()`.
- `Shoot()`: Viewport merkez ray → isabet varsa hit point, yoksa max mesafe; `Instantiate` → `Rigidbody.AddForce(Impulse)`.

---

## Oyuncu Can/XP/Seviye
Sınıf: `playerstats` — Dosya: `Assets/scripts/character/playerstats.cs`

Amaç: Can alma/verme, ölüm; XP artışı ve seviye atlama; seviye atlayınca XP eşiğini büyütme, canın %30’unu iyileştirme; UI güncellemeleri; seviye FX.

Akış:
- `Start()`: `currentHealth`, `healthBar`, `xpBar` kurulum.
- `Update()`: Can kontrolü (≤0 ise `Die()`).
- `TakeDamage(int)`: Can azaltma, bar güncelleme, kamera sarsıntısı.
- `GainXP(int)`: XP arttırma, bar güncelleme, eşik aşılırsa `LevelUp()`.
- `LevelUp()`: `level++`, XP azaltma, `xpToNextLevel` %25 artış, canın %30’u kadar iyileştirme, ses/FX.
- `Die()`: `SceneManager.LoadScene("Main MENU")`.

UI — `HealthBar.cs`:
- Slider + `Gradient` ile renklenme, saniyede `regenPerSecond` kadar pasif iyileşme.
- `OnLevelUp(newMaxHealth, regenBonus)`: Max can ayarla, regen’i artır.

UI — `XpBar.cs`:
- Slider max/değer ayarı, `fillImage.fillAmount = slider.normalizedValue`.

---

## Düşman Yapay Zekası – FSM + NavMesh
`Sınıf`: EnemyAI — `Dosya: Assets/scripts/enemy/enemyai.cs`

`Amaç`: FSM durumlarıyla düşmanın devriye gezmesi, oyuncuyu görürse kovalaması ve menzile girince ateş etmesi. NavMesh tabanlı yol bulma ve görüş kontrolü.

Durumlar:
- `Patrol`: Spawn çevresinde NavMesh üzerinde rastgele noktalara yapılan yürüyüşleri belirler.
- `LostPatrol`: Oyuncu kaybolduğunda son görülen konum çevresinde tekrardan patrol durumunun devreye girmesi.
- `Chase`: Oyuncu pozisyonuna doğru gitme. Update fonksiyonu ile düzenli güncellenen oyuncu pozisyonuna uygun hareket sağlanır.
- `Attack`: Menzilde (ve gerekiyorsa görüş hattında) durup nişan alma ve ateş etme.

Görüş:
- `detectionRadius`, `viewAngle`, `obstacleLayerMask` ile düşmanlar  bireysel görüş alanlarını `raycast` engel kontrolü dahilinde inceler.
- Oyuncu bu alanın içinde ise `lastSeenPlayerPosition` güncellenir, `hasSeenPlayer=true`.

Saldırı:
- `shootRange`, `shootCooldown`, `shootDuration` ile menzillerini inceler.
- Atak sırasında NavMeshAgent durur, karakter oyuncuya döner, `ShootAtPlayer()` mermi üretir ve ateş etme gerçekleşir.
- LOS gerekli ise `requireLineOfSight` kontrolü yapılır.

NavMesh/Devriye:
- `SetNewPatrolPoint(center, radius)`: `NavMesh.SamplePosition` ile tanımlanan zeminde hedef seçimiyle harita üzerindeki rastgele noktaların seçimi; zeminin veya objenin yere göre oluşan açısı ile takılma algısı ve fallback tekrar aramaları.
---

## Düşman İstatistikleri ve Dalgalar
- `EnemyStats.cs`: Spawn’da rastgele level; enemyhealth ve baseXP level ile artar; ölümde `player.GainXP` çağrılır.
- `EnemyLevelSystem.cs`: Alternatif, bar ile entegre bir seviye sistemi.
- `EnemySpawner.cs`: `totalWaves`, `enemiesPerWave`, `spawnInterval`, size (spawn için tanımlanan alan), `minDistanceFromPlayer`, `maxDistanceFromSpawner`; coroutine ile periyodik gerçekleşen düşman dalgası mekaniği; .

---

## Mermiler (Oyuncu/Düşman)
- `Projectile.cs`: Varsayılan `damage=50`; `OnCollisionEnter` içinde `Player`/`Enemy`/`Shard` tespiti ve uygun `TakeDamage` çağrısı; ardından self-destroy.
- `ProjectileEnemy.cs`: Düşman mermisi; `Player`’a hasar verir ve yok olur.


---

## Kazanma/Kaybetme ve Menüler
- `ShardStats.cs`: Her shard için can takip; 0 olduğunda yok edilir ve statik sayaç artırılır; 3 olduğunda `WinMenu.PauseGame()` ile kazanma paneli açılır (TimeScale=0, imleç serbest).
- `WinMenu.cs`: Kazanma paneli aç/kapat ve ana menüye dönüş.
- `PauseMenu.cs`: `Esc` ile aç/kapa; zaman ve imleç durumu yönetimi.
- `MainMenuManager.cs`: Ana menüden `main scene`’e geçiş; çıkış butonu.

---
## Tasarlanan Sahneler
- `Main MENU` ile oyunun ana ekranı tasarlanmıştır. Burada "Oyuna Başla" ve "Oyundan Çık" butonları bulunur. `main scene` üzerinde ana sahnemiz tasarlanmıştır. Oyun alanına ek olarak Pause menüsü de burada tasarlanmıştır. Oyuncu kazandığında veya öldüğünde ana menüye dönecek şekilde sahneler birbirine bağlanmıştır.
---
### Zorluklar ve Çözümler
- Matematiksel değerleri kod ekranına aktarmak. Vektörleri sayısal değerlerle işleme sokmamak gibi kod bilgisi eksikliği. Dene yanıl ile mekaniklerin matematiğe uygunluğu sağlandı ve syntax hakkında daha iyi bilgi sahibi olduk.
- Tecrübesizlikten ötürü oluşan GitHub Desktop ile ortak repository kullanımındaki entegrasyon ve versiyonlama sorunları. Herkes için ayrı bir branch oluşturduk ve bu branch'ler üzerinde düzenli pull işlemi yapılarak grubun tüm üyelerinde aynı değişikliklerin geçerli olmasını sağladık.
- Unity Hub tecrübesizliğinden yaşanan dosya kurulumları. Kodların aktarılırken sahnelerde tanımlanan metadata verisinin karşıya aktarılamaması. Repository üzerinden bu metadata verilerinin de aktarılmasını sağladık. Bu sayede sadece kodlar değil oyunda yapılan yerel değişikliklerin de aktarılmasını sağladık.
---

## Literatür Notları ve Kısa Karşılaştırmalar
- FSM: Basit düşman davranışları için hızlı ve anlaşılır. Genişlediğinde durum/transition sayısı artar.
- Behavior Trees: Karmaşık karar ağaçları için esnek. Gelecekte cover/strafe vb. için uygundur.
- NavMesh: Performanslı yol bulma; dinamik engeller için `NavMeshObstacle`/carving gerekebilir.
- Raycast tabanlı nişan: TPS türünde standart; balistik/drop ve recoil ile daha gerçekçi hâle getirilebilir.

---

## Bilinen Sınırlar ve İyileştirme Önerileri
- Object Pooling yok: Mermi/düşman yoğunluğunda GC ve Instantiate maliyeti artar.
- Kamera duvar çarpışması yok: Duvar arkası clip olabilir → kamera collision/shoulder switch eklenebilir.
- AI sade: Cover/strafe yok → BT veya geniş FSM ile gelişmiş davranışlar.
- Global `Physics.gravity` sahne genelinde set ediliyor → tek noktadan yönetim ve dokümantasyon önerilir.
- Eski Input API kullanımı → Yeni Input System’a geçiş (mobil desteği için özellikle).
---

## Projenin Kattığı Fayda
- Bu proje sayesinde planlamadan versiyonlamaya kadar oyun geliştirmenin her adımı hakkında kendimizi geliştirme fırsatı bulduk.
- Oyun geliştirme fizik tabanlı olduğundan teorik kodların görselize edilmesi ve kullanıcıdaki karşılığını da dikkate almak ve karşılıklı entegre etmek bu alanda bizleri geliştirdi.
- Blender kullanımı ile asset üretimi ve efekt kontrolleri hakkında kendimizi geliştirdik ve bilgi sahibi olduk.
- Github Desktop kullanımı ile başta zorlandığımız işlemler grup projesini daha hızlı geliştirmemizi sağladı. Gelecekteki iş hayatımız için simülasyon niteliğindeydi.

