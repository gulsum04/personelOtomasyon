# 🎓 Akademik Başvuru Otomasyon Sistemi

Bu proje, Kocaeli Üniversitesi'nin akademik personel alım süreçlerini dijital ortama taşımayı amaçlayan **.NET Core MVC tabanlı web tabanlı bir otomasyon sistemidir**. Sistem; başvuruların alınması, kadro kriterlerinin tanımlanması, jüri değerlendirmelerinin yapılması ve süreç yönetiminin sağlanması gibi işlevleri yerine getirir.

## 🔰 Proje Amacı

Manuel olarak yürütülen akademik başvuru süreçleri;

- Evrak yoğunluğu,
- Zaman kaybı,
- İzlenebilirlik eksikliği gibi zorluklar doğurmaktadır.

Bu sistem, **şeffaf, hızlı ve hatasız** bir başvuru süreci sağlamak amacıyla geliştirilmiştir.

## 👥 Kullanıcı Rolleri

Sistem 4 ana kullanıcı rolü içerir:

- **Aday**: İlanlara başvuru yapar, belgeleri yükler.
- **Admin**: Yeni ilanlar oluşturur ve yönetir.
- **Yönetici**: Kadro kriterlerini belirler, jüri ataması yapar, sonuçları bildirir.
- **Jüri Üyesi**: Aday belgelerini değerlendirir ve raporlar.

## 🏗️ Kullanılan Teknolojiler

| Teknoloji | Açıklama |
|----------|----------|
| ASP.NET Core MVC | Backend uygulama çatısı |
| Entity Framework Core | ORM - Veritabanı yönetimi |
| SQL Server | Veritabanı sistemi |
| Razor Pages | View yönetimi |
| Bootstrap | Arayüz tasarımı |
| ASP.NET Identity | Kimlik doğrulama ve yetkilendirme |
| SMTP | E-posta bildirim sistemi |

## 🔧 Özellikler
🎯 Role dayalı giriş sistemi

📢 Dinamik ilan yönetimi

📎 Kadro kriterleri tanımlama (Tablo 5'e uygun)

📝 Jüri değerlendirme raporları

📂 Belge yükleme ve durum takibi

📧 SMTP destekli e-posta bildirimi

📄 PDF çıktı (Tablo 5 formatında - gelişme aşamasında)

🔐 Kimlik doğrulama için KPS entegrasyonu (taslak aşamasında)


## 🔄 İşleyiş Akışı
Admin, ilan oluşturur.

Yönetici, ilana kriter tanımlar ve yayına alır.

Aday, ilana başvurur ve belgeleri yükler.

Yönetici, başvuruya jüri atar.

Jüri, belge değerlendirir ve rapor yükler.

Yönetici, nihai sonucu belirler.


## 👨‍💻 Geliştiriciler

| Ad Soyad       | Mail                        | Üniversite |
|----------------|-----------------------------|------------|
| Elif İrem Uğuz | 231307087@kocaeli.edu.tr    | KOÜ        |
| Ayşenur Akcan  | 221307033@kocaeli.edu.tr    | KOÜ        |
| Gülsüm Demir   | 221307024@kocaeli.edu.tr    | KOÜ        |


## 

Yeni Kullanıcı Kaydı Oluşturma Sayfası  
![Image](https://github.com/user-attachments/assets/30f6e357-3d69-4d66-9b0f-baad625ac122)  
#

Panellere Giriş Sayfası  
![Image](https://github.com/user-attachments/assets/94813bee-7386-45e8-bbba-6fae25594e28)  
#

Kullanıcı Hesap Girişi Sayfası  
![Image](https://github.com/user-attachments/assets/1ef4c014-111b-48dc-a34e-c256827b9600)  
#

Aday Paneli Yayındaki İlanları Görüntüleme Sayfası  
![Image](https://github.com/user-attachments/assets/db573779-afce-4865-8b5b-cd55b54737c1)  
#

Aday İçan Başvuracağı İlan Detayları Sayfası  
![Image](https://github.com/user-attachments/assets/d45f84d6-02ef-4fdd-b28c-b4b5a4e80d22)  
#

Admin Panelinin Kullanıcı Tür Ataması Yaptığı Sayfa  
![Image](https://github.com/user-attachments/assets/81a4bc4f-fa98-4254-8350-472c1534f742)  
#

Yönetici Panelinde Mevcut İlanlara Kriter Belirleme-Güncelleme Ve İlana Alma Sayfası  
![Image](https://github.com/user-attachments/assets/9fdc6690-1fdb-49a2-94f5-78dd3dab5739)  
#

Yönetici Panelinde Başvuruların Yönetici Tarafından Nihai Karar Verme Sayfası  
![Image](https://github.com/user-attachments/assets/83ab0628-eb98-4b73-b6d7-4c6afae77fcd)  
#

Admin Panelinde Yeni İlan Oluşturma Sayfası  
![Image](https://github.com/user-attachments/assets/b164a2da-7512-4806-a2c2-4733d52ec499)  
#

Kullanıcılara Gelen Kullunacı Tür Ataması Ve Yeni İlan Bildirim Sayfası  
![Image](https://github.com/user-attachments/assets/60aeaa3c-befe-4dca-bf9b-e173d4fb555b)  
#

Yönetici Paneli İlanlara Kriter Belirleme Sayfası  
![Image](https://github.com/user-attachments/assets/7c87268c-cbf4-4e23-9f27-06dda29b80a2)  

##


# 🎓 Academic Recruitment Automation System

This project is a web-based automation system developed using **.NET Core** to digitize the academic personnel recruitment processes in universities. The system manages application intake, requirement definition, jury evaluations, and process tracking efficiently.

## 🔰 Project Objective

Manual academic recruitment processes are often:

- Document-heavy,
- Time-consuming,
- Prone to administrative errors.

This system aims to provide a **transparent, fast, and error-free** application process by transferring all operations to a digital platform.

## 👥 User Roles

The system defines 4 main roles:

- **Candidate**: Applies to job listings and uploads required documents.
- **Admin**: Creates and manages job postings.
- **Manager**: Defines application criteria, assigns juries, and announces results.
- **Jury Member**: Evaluates candidates and uploads assessment reports.

## 🏗️ Technologies Used

| Technology | Purpose |
|------------|---------|
| ASP.NET Core MVC | Backend application framework |
| Entity Framework Core | ORM - Database management |
| SQL Server | Relational database |
| Razor Pages | View handling |
| Bootstrap | UI design |
| ASP.NET Identity | Authentication & authorization |
| SMTP | Email notification system |


## 🔧 Features
🔐 Role-based authentication and routing

📋 Dynamic job posting and listing

📎 Customizable recruitment criteria (Tablo 5 compatible)

📝 Jury evaluation reporting

📂 Secure document upload and tracking

📧 Email notifications via SMTP

📄 PDF report generation (Tablo 5 format - under development)

🇹🇷 KPS (Turkish ID Verification) integration (planned)


## 🔄 Workflow
Admin creates new academic job postings.

Manager sets criteria and publishes the listing.

Candidate applies and uploads required documents.

Manager assigns jury members to each application.

Jury evaluates and submits reports.

Manager announces the final decision.


| Name           | Email                        | University |
|----------------|------------------------------|------------|
| Elif İrem Uğuz | 231307087@kocaeli.edu.tr     | KOÜ        |
| Ayşenur Akcan  | 221307033@kocaeli.edu.tr     | KOÜ        |
| Gülsüm Demir   | 221307024@kocaeli.edu.tr     | KOÜ        |




