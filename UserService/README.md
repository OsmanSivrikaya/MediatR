# DAL / Business Kafasından Clean Architecture’a Geçiş Rehberi

Bu doküman, klasik DAL / Business / Entity yaklaşımından
Clean Architecture + CQRS yapısına geçerken yaşanan kafa karışıklığını,
özellikle **interface (abstract) konusu** üzerinden netleştirmek için hazırlanmıştır.

Amaç:
- “Neyi nereye koyacağım?” sorusunu bitirmek
- Eski alışkanlıkları yeni mimariye birebir çevirmek

---

## 1. Eski Kafa ≈ Yeni Kafa Eşleştirmesi

Eski Katman | Yeni Karşılığı | Ne Koyarsın
--- | --- | ---
Entity | Domain | Entity + iş kuralları
Business | Application | Use-case / iş akışı
DAL | Infrastructure | DB, HTTP, JWT, dış dünya
API | API | Controller

Bu tablo referans tablodur.
Her karar önce buraya bakılarak verilir.

---

## 2. Kafayı Karıştıran Asıl Soru

“Eskiden interface’ler DAL’da olurdu.
Şimdi neden IJwtTokenService Application’da,
ama implementasyonu Infrastructure’da?”

Bu soru doğrudur ve mimarinin kırılma noktasıdır.

---

## 3. Eski DAL / Business Gerçeği

### Klasik yapı

    API
     └─ Business
         └─ DAL
             ├─ ITokenService
             └─ JwtTokenService

### Görünürde
- Interface var
- Loose coupling var gibi durur

### Gerçekte
- Business, DAL’ı tanır
- Business, altyapıya bağımlıdır
- DAL sözleşmeyi belirler

Bu yapı **gizli şekilde sıkı bağlıdır**.

---

## 4. Yeni Mimaride Temel Değişim

Yeni mimaride ana kural şudur:

**İhtiyacı olan, sözleşmeyi tanımlar.**

Artık sözleşmeyi altyapı (DAL) değil,
işi yapan katman tanımlar.

---

## 5. JWT Örneği ile Netleştirme

Soru:
JWT token üretmeye kim ihtiyaç duyar?

Cevap:
- Login akışını yöneten katman
- Client Credentials akışını yöneten katman

Yani:
**Application**

---

## 6. Doğru Yapı (Yeni Mimari)

### Application – İHTİYAÇ TANIMI

Application katmanı şunu söyler:

“Bana token üreten bir şey lazım.
Nasıl ürettiğin umurumda değil.”

Bu yüzden interface burada bulunur.

    Application
     └─ IJwtTokenService

---

### Infrastructure – NASIL YAPILIR

Infrastructure katmanı şunu bilir:
- JWT standardı
- RSA key
- Framework ve teknik detaylar

Bu yüzden implementasyon burada bulunur.

    Infrastructure
     └─ JwtTokenService

---

## 7. Kritik Kural (EZBER)

Interface, **ihtiyacı olan katmanda** olur.
Implementasyon, **nasıl yapan katmanda** olur.

Soru | Cevap
--- | ---
Token’a kim ihtiyaç duyuyor? | Application
Token nasıl üretilir? | Infrastructure
Token iş kuralı mı? | Hayır

---

## 8. Eski DAL Kafasına Birebir Çeviri

### Eskiden

    DAL
     ├─ ITokenService
     └─ JwtTokenService

    Business
     └─ new JwtTokenService()

---

### Şimdi

    Application
     └─ IJwtTokenService   (NE lazım)

    Infrastructure
     └─ JwtTokenService    (NASIL yapılır)

Fark:
- Application, Infrastructure’ı TANIMAZ
- Sadece interface’i bilir

---

## 9. Neden Interface Infrastructure’da Olmamalı?

Çünkü:
- Infrastructure bir detaydır
- Detay sözleşme belirleyemez
- Detay, ihtiyaca göre şekillenir

Ezber cümle:

**Detay sözleşme belirlemez.
Sözleşmeyi ihtiyacı olan belirler.**

---

## 10. Domain Bu İşin Neresinde?

JWT:
- Teknik bir güvenlik standardıdır
- İş kuralı değildir

Bu yüzden:
- Domain → İş kuralları
- Application → İş akışı + ihtiyaç
- Infrastructure → Teknik detay

Domain bu örnekte bilinçli olarak küçüktür.

---

## 11. Tek Cümlelik Özet

Domain = KURAL  
Application = SÜREÇ  
Infrastructure = NASIL  
API = KAPI

Bu dört kelimeyi bildiğin sürece
“neyi nereye koyacağım” sorusu biter.

---

## 12. SharedKernel Nedir ve Neden Kullanılır?

SharedKernel, birden fazla servisin **ortak kabul ettiği**,  
**çok küçük**, **çok stabil** ve **değişimi nadir** olan yapı taşlarını barındıran katmandır.

Amaç:
- Servisler arasında **aynı kavramın aynı anlamda** kullanılmasını sağlamak
- Kopya kodu önlemek
- Ortak dili (ubiquitous language) korumak

> SharedKernel **kolaylık katmanı değil**,  
> **zorunluluk katmanıdır**.

---

## 13. SharedKernel Ne DEĞİLDİR?

SharedKernel:

- ❌ Ortak helper deposu değildir
- ❌ Ortak DTO deposu değildir
- ❌ Ortak entity deposu değildir
- ❌ Ortak business logic deposu değildir

Aksi halde:
- Servisler birbirine kilitlenir
- Versiyonlama cehennemi oluşur
- Microservice mimarisi monolite geri döner

---

## 14. SharedKernel’e Ne Konur? (DOĞRU LİSTE)

SharedKernel’e sadece **domain seviyesinde**,  
**framework’ten bağımsız**,  
**her serviste aynı anlamı taşıyan** yapı taşları konur.

### Konulabilecekler

- DomainException
- Result / Result<T>
- Error (Code + Message)
- ValueObject base class
- DomainEvent base class
- StronglyTypedId base class

Bunlar:
- Davranıştan çok **anlam** taşır
- Değişimi nadirdir
- Servis bağımsızdır

---

## 15. SharedKernel’e Ne KONMAZ? (ÇOK ÖNEMLİ)

Aşağıdakiler **ASLA** SharedKernel’e konmamalıdır:

- Entity’ler
- Aggregate’ler
- Repository interface’leri
- Application service’leri
- JWT / Security kodları
- HTTP / API ile ilgili tipler
- Helper sınıflar (StringHelper, DateTimeHelper vb.)

Ezber cümle:

**Ne kadar “işime yarar” görünüyorsa,  
o kadar SharedKernel için yanlıştır.**

---

## 16. DomainException SharedKernel’e Taşınır mı?

Evet.  
`DomainException` SharedKernel için **ideal bir adaydır**.

Sebebi:
- Tüm servislerde aynı anlamı taşır
- İş kuralı ihlalini temsil eder
- Framework bağımlılığı yoktur
- Çok nadir değişir

### Önerilen yapı

    SharedKernel
     └─ DomainException

Her servis isterse kendi exception’ını türetebilir:

    AuthService.Domain
     └─ AuthDomainException : DomainException

Bu yaklaşım **kurumsal projelerde standarttır**.

---

## 17. Result / Result<T> SharedKernel’de Olur mu?

Evet, **doğru tasarlanırsa**.

### Doğru Result

- HTTP bilmez
- StatusCode bilmez
- Framework bilmez
- Sadece başarı / başarısızlık durumunu temsil eder

Bu tür bir Result:
- Application katmanında use-case sonucu için idealdir
- API katmanında HTTP’ye çevrilir

### Yanlış Result

- StatusCode içeriyorsa
- HTTP kavramları taşıyorsa
- UI mesajları barındırıyorsa

Bu tür Result **SharedKernel’e konmaz**.

---

## 18. SharedKernel ile Servislerin İlişkisi

Bağımlılık yönü şu şekildedir:

    Service.Domain
      └─ SharedKernel

    Service.Application
      └─ Service.Domain

    Service.Infrastructure
      └─ Service.Application

SharedKernel:
- Hiçbir servisi TANIMAZ
- Hiçbir servise BAĞLI değildir

---

## 19. Önerilen Genel Proje Yapısı

    src
    ├─ BuildingBlocks
    │   └─ SharedKernel
    │       ├─ DomainException.cs
    │       ├─ Result.cs
    │       ├─ ResultOfT.cs
    │       ├─ Error.cs
    │       └─ ValueObject.cs
    │
    ├─ AuthService
    ├─ UserService
    └─ WorkService

“Shared”, “Common”, “Utils” gibi isimler yerine  
**SharedKernel** veya **BuildingBlocks** tercih edilir.

---

## 20. SharedKernel Kullanımı İçin Karar Ağacı

Bir şey SharedKernel’e girecek mi?

Şu soruları sırayla sor:

1. Bu kavram tüm servislerde **aynı anlamı mı taşıyor?**
    - Hayır → SharedKernel’e girmez
    - Evet → devam

2. Bu kavram bir **altyapı detayı mı?**
    - Evet → SharedKernel’e girmez
    - Hayır → devam

3. Bu kavram değiştiğinde **tüm servislerin değişmesi kabul edilebilir mi?**
    - Hayır → SharedKernel’e girmez
    - Evet → SharedKernel adayı

---

## 21. SharedKernel İçin Altın Kural

**SharedKernel küçük, sıkıcı ve değişmez olmalıdır.**  
**Ne kadar faydalı görünüyorsa, o kadar tehlikelidir.**

---

## 22. Nihai Özet

Domain = KURAL  
Application = SÜREÇ  
Infrastructure = NASIL  
API = KAPI  
SharedKernel = ORTAK DİL

Bu kavramlar net olduğu sürece:
- Mimari bozulmaz
- Servisler bağımsız kalır
- Sistem büyüdükçe kontrol kaybolmaz

