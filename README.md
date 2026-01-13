# MediatR Microservice Architecture

Bu repository, **kurumsal ölçekte**, **gerçek dünya senaryolarına uygun**,  
**Clean Architecture + CQRS + MediatR** temelli bir **microservice mimarisi** örneğidir.

Amaç:
- Servis sorumluluklarını net ayırmak
- İş kurallarını altyapıdan izole etmek
- Servisler arası güvenli iletişim sağlamak
- Uzun vadede sürdürülebilir ve genişletilebilir bir yapı kurmak

---

## 1. Genel Mimari Yaklaşım

Sistem aşağıdaki prensiplere göre tasarlanmıştır:

- Clean Architecture
- Hexagonal Architecture (Ports & Adapters)
- CQRS
- MediatR
- Dependency Inversion Principle
- Single Responsibility Principle
- JWT (RS256 – Asimetrik İmzalama)
- Service-to-Service Authentication (Client Credentials)

Bu yaklaşım sayesinde:
- Test edilebilirlik artar
- Teknik borç minimize edilir
- Servisler bağımsız gelişir
- Production ortamlarında güvenli iletişim sağlanır

---

## 2. Sistem Bileşenleri

Sistem **3 ana microservice**’ten oluşur:

1. **AuthService** – Kimlik doğrulama & token üretimi
2. **UserService** – Kullanıcı domain’i
3. **WorkService** – İş / görev domain’i

Her servis:
- Kendi verisine sahiptir
- Kendi domain kurallarını yönetir
- Diğer servislerin verisine doğrudan erişmez

---

## 3. AuthService – Authentication & Token Management

### 3.1 Amacı

AuthService, sistemin **tek Identity Provider** servisidir.

Tüm kullanıcı ve servis kimlikleri bu servis üzerinden doğrulanır.

---

### 3.2 Sorumlulukları

AuthService şunları yapar:

- Kullanıcı girişini doğrular
- JWT üretir (RS256 – Private/Public Key)
- Service-to-service token üretir (Client Credentials)
- Token içeriklerini standartlaştırır

AuthService şunları yapmaz:

- User CRUD
- Profil yönetimi
- Domain business logic

AuthService **iş yapmaz, kimlik sağlar**.

---

### 3.3 Katman Yapısı

AuthService aşağıdaki katmanlardan oluşur:

```text
AuthService  
├─ AuthService.Api  
├─ AuthService.Application  
├─ AuthService.Domain  
└─ AuthService.Infrastructure
```

---

### 3.4 Token Türleri

**User Token**
- Kullanıcıyı temsil eder
- sub = userId
- Client → API iletişiminde kullanılır

**Service Token (Client Credentials)**
- Servisi temsil eder
- sub = service-name
- Service → Service iletişiminde kullanılır

---

## 4. UserService – User Domain Service

### 4.1 Amacı

UserService, sistemdeki **tüm kullanıcıların tek doğruluk kaynağıdır**.

Kullanıcıya ait:
- Kimlik bilgileri
- Roller
- Durum bilgileri

yalnızca bu serviste bulunur.

---

### 4.2 Sorumlulukları

UserService şunları yapar:

- Kullanıcı domain kurallarını yönetir
- Kullanıcı doğrulama işlemlerini gerçekleştirir
- Diğer servislere internal auth endpoint sağlar

UserService şunları yapmaz:

- Token üretmez
- Authentication standardı belirlemez

---

### 4.3 Katman Yapısı

UserService aşağıdaki katmanlardan oluşur:

```text
UserService  
├─ UserService.Api  
├─ UserService.Application  
├─ UserService.Domain  
└─ UserService.Infrastructure
```

---

### 4.4 Veri Erişimi

UserService şu an örnek repository ile çalışmaktadır ancak:

- EF Core
- Generic Repository
- Unit of Work

yapıları kolayca entegre edilecek şekilde tasarlanmıştır.

Domain katmanı EF Core bilmez.  
Application katmanı DbContext bilmez.

---

## 5. WorkService – Business / Task Domain

### 5.1 Amacı

WorkService, sistemin **iş yapan** servisidir.

Örnek sorumluluklar:
- Task oluşturma
- Task assign etme
- İş süreçlerini yönetme

---

### 5.2 Servisler Arası İletişim

Örnek senaryo:

1. Kullanıcı login olur → User Token alır
2. WorkService’e istek atar
3. WorkService:
    - JWT’yi doğrular
    - Gerekirse UserService’ten kullanıcı listesini alır
    - İş logic’i uygular

WorkService **UserService verisini sahiplenmez**, sadece tüketir.

---

### 5.3 Katman Yapısı

WorkService aşağıdaki katmanlardan oluşur:

```text
WorkService  
├─ WorkService.Api  
├─ WorkService.Application  
├─ WorkService.Domain  
└─ WorkService.Infrastructure
```

---

## 6. Servisler Arası Güvenlik

- AuthService token üretir
- Diğer servisler public key ile doğrular
- Secret paylaşımı yoktur
- Servisler birbirini doğrudan tanımaz

Bu yapı:
- Zero Trust prensibine uygundur
- Kurumsal güvenlik standartlarını karşılar

---

## 7. Genişletilebilirlik

Bu mimari şu genişletmelere hazırdır:

- EF Core + PostgreSQL / SQL Server
- Generic Repository & Unit of Work
- Refresh Token mekanizması
- Event-driven communication (RabbitMQ / Kafka)
- gRPC entegrasyonu
- API Gateway (YARP)

---

## 8. Katmanların Özet Sorumlulukları

| Katman        | Sorumluluk                    |
|---------------|--------------------------------|
| Domain        | Saf iş kuralları               |
| Application   | Use-case & iş akışı            |
| Infrastructure| Teknik detaylar                |
| Api           | HTTP giriş noktası             |

---

## 9. Sonuç

Bu repository:
- Öğrenme amaçlı basit bir demo değildir
- Gerçek hayatta kullanılan mimari prensipleri uygular
- Kurumsal projelere doğrudan adapte edilebilir

Bugün karmaşık gibi görünür,  
yarın sistemi ayakta tutar.

---

**Bu README mimari bir sözleşmedir.**
