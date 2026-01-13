# UserService – User & Identity Data Service

## 1. Servisin Amacı

UserService, microservice mimarisi içerisinde sistemdeki **kullanıcı verilerinin tek sahibi (Single Source of Truth)** olan servistir.

Bu servis:
- Kullanıcı kimlik bilgilerini
- Kullanıcı durumlarını (aktif, pasif, kilitli)
- Kullanıcı rollerini
- Kullanıcıya ait domain verilerini

yönetir ve **başka hiçbir servisin bu verilere doğrudan sahip olmasına izin vermez**.

UserService:
- Token üretmez
- Login yapmaz
- Authorization kararı vermez

UserService **kimlik verisinin sahibidir**,  
**güvenliğin sahibi değildir**.

---

## 2. Temel Kurallar

- UserService token üretmez
- UserService AuthService’e güvenmez
- UserService sadece **client token** kabul eder
- UI tarafından doğrudan çağrılamaz
- Internal servis endpoint’leri vardır

---

## 3. Mimari Yaklaşım

UserService aşağıdaki prensiplere göre tasarlanmıştır:

- Clean Architecture
- Hexagonal Architecture (Ports & Adapters)
- CQRS
- MediatR
- Dependency Inversion Principle

Amaç:
- User domain’ini izole etmek
- Servisler arası bağımlılığı azaltmak
- Test edilebilirliği artırmak
- Kurumsal ölçeklenebilirlik sağlamak

---

## 4. Proje Yapısı

```text
UserService
├─ UserService.Api
├─ UserService.Application
├─ UserService.Domain
└─ UserService.Infrastructure 
```

---

## 5. Katmanların Sorumlulukları

### 5.1 Domain Katmanı

**Amacı**
- Saf kullanıcı iş kurallarını barındırmak

**İçerdikleri**
- User aggregate
- UserRole
- UserStatus
- DomainException

**İçermedikleri**
- MediatR
- JWT
- HttpClient
- IConfiguration
- DTO

Domain hiçbir katmanı tanımaz.

---

### 5.2 Application Katmanı

**Amacı**
- Kullanıcı ile ilgili use-case’leri yönetmek

**Sorumlulukları**
- Kullanıcı doğrulama
- Kullanıcı durum kontrolü
- Rol bilgisini boundary seviyesinde dönmek

**Bulunanlar**
- MediatR Command & Handler
- Use-case akışları
- Abstraction interface’ler

Örnek interface’ler:
- IUserRepository
- IPasswordHasher

Application:
- JWT bilmez
- REST bilmez
- Config okumaz

---

### 5.3 Infrastructure Katmanı

**Amacı**
- Application’ın ihtiyaç duyduğu teknik detayları sağlamak

**Sorumlulukları**
- Repository implementasyonları
- Hash algoritmaları (BCrypt vb.)
- Veritabanı erişimi (ileride EF Core)

Infrastructure:
- İş kuralı yazmaz
- Use-case bilmez

---

### 5.4 Api Katmanı

**Amacı**
- HTTP giriş noktası olmak

**Sorumlulukları**
- Request almak
- MediatR üzerinden Application’a iletmek
- Response döndürmek

Api sadece **taşıyıcıdır**.

---

## 6. AuthService ile İlişki

UserService:
- AuthService’e güvenmez
- Sadece servis token’ı kabul eder
- UI’dan çağrı kabul etmez

User doğrulama **AuthService üzerinden dolaylı yapılır**.

---

## 7. Internal Endpoint

POST /internal/auth/validate

yaml
Kodu kopyala

Bu endpoint:
- Sadece **client token** kabul eder
- UI’dan çağrılamaz
- Kullanıcı doğrulaması yapar
- JWT üretmez

---

## 8. Internal Auth Validation Akışı

```text
AuthService
→ UserService.Api (/internal/auth/validate)
→ MediatR
→ ValidateUserCommandHandler
→ IUserRepository
→ IPasswordHasher
← UserAuthResult
```

---

## 9. Domain → DTO Dönüşüm Kuralı

- Domain enum’ları dış dünyaya açılmaz
- Boundary seviyesinde string’e dönüştürülür

Örnek:

UserRole.Admin → "Admin"

Bu sayede:
- Servisler gevşek bağlı kalır
- Domain değişiklikleri dış dünyayı kırmaz

---

## 10. Güvenlik Prensipleri

- UserService token doğrulamaz
- Client token doğrulaması API seviyesinde yapılır
- UserService sadece doğrulama yapar
- Authorization başka servislerin sorumluluğudur

---

## 11. Katmanların Özet Görevleri

| Katman        | Sorumluluk                    |
|---------------|--------------------------------|
| Domain        | Kullanıcı iş kuralları         |
| Application   | Kullanıcı use-case’leri        |
| Infrastructure| Teknik detaylar                |
| Api           | HTTP giriş noktası             |

---

## 12. Mimari İlke

UserService:
- Kullanıcıyı tanır
- Sistemi tanımaz
- Güvenliği sahiplenmez
- Kimlik bilgisini üretir

Bu doküman **mimari bir sözleşmedir**.