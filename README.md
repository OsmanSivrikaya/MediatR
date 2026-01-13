# AuthService – Authentication & Token Management Service

## 1. Servisin Amacı

AuthService, microservice mimarisi içerisinde sistemin **kimlik doğrulama (Authentication)** ve  
**token üretimi (Authorization)** sorumluluğunu üstlenen **tek merkez (Identity Provider)** servisidir.

Bu servis sistemdeki tüm kullanıcıların ve servislerin:
- Kim olduğunu
- Hangi yetkilere sahip olduğunu
- Hangi servis adına işlem yaptığını

belirleyen **tek doğruluk kaynağıdır**.

---

## 2. Temel Sorumluluklar

### AuthService NELER YAPAR

- Kullanıcı girişini doğrular (Login)
- JWT üretir (RS256 – Asimetrik imzalama)
- Service-to-service (Client Credentials) token üretir
- Token içindeki identity bilgisini oluşturur
- Authentication standartlarını belirler

### AuthService NELER YAPMAZ

- User CRUD işlemleri
- Kullanıcı profil yönetimi
- İş / domain logic
- Authorization business rule

AuthService **iş yapmaz, kimlik sağlar**.

---

## 3. Mimari Yaklaşım

Bu servis aşağıdaki mimari prensiplere göre tasarlanmıştır:

- Clean Architecture
- Hexagonal Architecture (Ports & Adapters)
- CQRS
- MediatR
- Dependency Inversion Principle

Amaçlanan kazanımlar:
- İş kurallarını altyapıdan ayırmak
- REST / Event / gRPC geçişini kolaylaştırmak
- Test edilebilirlik
- Kurumsal ölçeklenebilirlik

---

## 4. Proje Yapısı

    AuthService
    ├─ AuthService.Api
    ├─ AuthService.Application
    ├─ AuthService.Domain
    └─ AuthService.Infrastructure

---

## 5. Katmanların Detaylı Sorumlulukları

---

### 5.1 Domain Katmanı

#### Amacı
- Saf iş kavramlarını barındırmak
- Framework ve altyapı bağımlılıklarından tamamen izole olmak

#### İçerdikleri
- DomainException
- (Gelecekte) Token, Client gibi domain kavramları

#### İçermedikleri
- MediatR
- JWT
- HttpClient
- IConfiguration
- IOptions

#### Bağımlılık Kuralları
- Hiçbir projeye bağımlı değildir
- En alt katmandır
- Kimseyi tanımaz

Domain **her zaman saf kalır**.

---

### 5.2 Application Katmanı (Business / Use-Case)

#### Amacı
- Sistemin **ne yaptığını** tanımlamak
- Use-case’leri yönetmek
- İş akışlarını koordine etmek

#### Sorumlulukları
- Login use-case’ini yönetmek
- Client Credentials token akışını yönetmek
- İş kararlarını almak

#### Application Katmanında BULUNANLAR
- MediatR Commands & Handlers
- Use-case akışları
- Abstraction Interface’ler (Ports)

Kullanılan interface’ler:
- IJwtTokenService
- IUserAuthGateway
- IClientCredentialValidator

Bu interface’ler şu prensibe dayanır:

“Bu yeteneğe ihtiyacım var ama nasıl çalıştığını bilmek istemiyorum.”

#### Application Katmanında BULUNMAYANLAR
- JWT implementasyonu
- REST çağrısı
- HttpClient
- IConfiguration
- IOptions

Application config OKUMAZ.  
Application sadece SORAR.

---

### 5.3 Infrastructure Katmanı (Teknik Detaylar)

#### Amacı
- Application katmanının ihtiyaç duyduğu teknik detayları sağlamak

#### Sorumlulukları
- JWT üretmek (RS256)
- Private key okumak
- HttpClient ile UserService çağırmak
- appsettings.json okumak
- Client bilgilerini doğrulamak

#### Infrastructure Katmanında BULUNANLAR
- JwtTokenService
- JwtOptions
- RestUserAuthGateway
- AppSettingsClientCredentialValidator

#### Infrastructure Katmanında BULUNMAYANLAR
- İş kuralı
- Use-case logic
- Controller

Infrastructure sadece **NASIL** sorusunu cevaplar.

---

### 5.4 Api Katmanı (Presentation)

#### Amacı
- Sistemin HTTP giriş noktası olmak

#### Sorumlulukları
- HTTP request almak
- MediatR aracılığıyla Application katmanına iletmek
- HTTP response döndürmek

#### Yapmadıkları
- JWT üretmek
- İş kuralı yazmak
- User doğrulamak

Api sadece **giriş kapısıdır**.

---

## 6. JWT Stratejisi

### İmza Algoritması
RS256 – RSA + SHA256

- AuthService private key ile imzalar
- Diğer servisler public key ile doğrular
- Secret paylaşımı yoktur
- Kurumsal standarttır

---

## 7. Token Türleri

### User Token
- Client → API
- Kullanıcıyı temsil eder
- sub = userId

### Service Token (Client Credentials)
- Service → Service
- Servisi temsil eder
- sub = service-name

---

## 8. Login Akışı

    Client
     → AuthService.Api (/auth/login)
       → MediatR
         → LoginCommandHandler
           → IUserAuthGateway
             → RestUserAuthGateway
               → UserService (/internal/auth/validate)
           → IJwtTokenService
             → JwtTokenService
     ← JWT

Kazanımlar:
- Application REST bilmez
- Application JWT bilmez
- Infrastructure teknik detayları izole eder

---

## 9. Service-to-Service Token Akışı

    WorkService
     → AuthService (/auth/token)
       → ClientCredentialsTokenCommandHandler
         → IClientCredentialValidator
         → IJwtTokenService
     ← Service JWT

---

## 10. Neden Bu Mimari?

Bu mimari sayesinde:
- Test edilebilirlik artar
- Değişiklikler izole edilir
- Teknik borç birikmez
- Gerçek microservice standartları uygulanır

Bugün karmaşık,
yarın hayat kurtarır.

---

## 11. Katmanların Özet Görevleri

| Katman        | Sorumluluk                |
|---------------|----------------------------|
| Domain        | Saf iş kavramları          |
| Application   | Use-case & akış            |
| Infrastructure| Teknik detay               |
| Api           | Giriş noktası              |

---

## 12. Sonraki Adımlar

- UserService: internal auth validation
- WorkService: JWT validation & authorization
- Event-based communication (opsiyonel)

Bu README mimari bir sözleşmedir.
