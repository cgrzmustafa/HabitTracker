# 🎯 HabitTracker - Gamified Habit Management System

![Banner](screenshots/4-confetti-celebration.png)

**HabitTracker**, günlük rutinlerinizi bir oyuna dönüştüren, **ASP.NET Core MVC** ile geliştirilmiş modern bir web uygulamasıdır. Klasik "To-Do" listelerinin sıkıcılığını ortadan kaldırır; kullanıcıları XP (Puan), Seviye ve Rozet sistemleri ile motive eder.

> **Öne Çıkan:** Proje, **Glassmorphism** tasarım dili, **Admin Analitikleri** ve **Gerçek Zamanlı Etkileşimler** (AJAX) üzerine kuruludur.

---

## 🛠️ Teknolojiler ve Mimari

Bu proje, modern web geliştirme standartlarına uygun, ölçeklenebilir bir mimari ile geliştirilmiştir.

| Alan | Teknoloji |
| :--- | :--- |
| **Backend** | .NET 7/8, C#, ASP.NET Core MVC |
| **Veritabanı** | MS SQL Server, Entity Framework Core (Code First) |
| **Frontend** | Razor View Engine, HTML5, CSS3 |
| **Styling** | **Tailwind CSS** (Glassmorphism Effects) |
| **JavaScript** | Vanilla JS, Fetch API (AJAX), **Chart.js**, Canvas Confetti |
| **Güvenlik** | BCrypt Hashing, Role-Based Authorization, CSRF Protection |

---

## 📸 Proje Turu

### 1. Modern ve Güvenli Giriş
Kullanıcılar sisteme güvenli bir şekilde kayıt olup giriş yapabilirler. Glassmorphism tasarımı ilk andan itibaren hissedilir.
![Login Screen](screenshots/1-login-register.png)

### 2. Kullanıcı Deneyimi (UX) Odaklı Tasarım
Kullanıcı sisteme ilk girdiğinde boş bir ekranla karşılaşmaz. Yönlendirici "Empty State" tasarımları ile ne yapması gerektiği anlatılır.
![Empty State](screenshots/2-empty-state.png)

### 3. Kolay Veri Girişi
Yeni alışkanlıklar, kategori ve hatırlatıcı saatleri ile birlikte kolayca sisteme eklenir.
![Add Habit](screenshots/3-add-habit.png)

### 4. 🎉 Kutlama ve Konfeti Efekti (Gamification)
Projenin en can alıcı noktası! Kullanıcı o günkü tüm hedeflerini tamamladığında, sistem bunu görsel bir şölenle (Konfeti Yağmuru) kutlar.
![Confetti](screenshots/4-confetti-celebration.png)

### 5. Profil ve İlerleme Sistemi
Tamamlanan her görev kullanıcıya **XP** kazandırır. Belirli eşiklerde kullanıcı **Level Atlar** ve başarılarına göre **Rozetler** (Örn: İlk Adım Rozeti) kazanır.
![Profile](screenshots/5-profile-gamification.png)

---

## 🛡️ Yönetim Paneli (Admin Dashboard)

Sistem sadece son kullanıcı için değil, yöneticiler için de detaylı analiz araçları sunar.

### 📊 Canlı Analiz ve Akış
Admin, sisteme kayıt olan kullanıcı sayılarını grafiksel olarak (Chart.js) görebilir ve sağ paneldeki **"Canlı Akış (Live Feed)"** üzerinden kimin hangi görevi tamamladığını anlık takip edebilir.
![Admin Dashboard](screenshots/6-admin-dashboard.png)

### ⚡ İçerik Yönetimi
Admin, sekmeli yapı sayesinde (Tab System) kullanıcıları ve eklenen alışkanlıkları denetleyebilir. Uygunsuz içerikleri veya kullanıcıları tek tıkla sistemden silebilir.
![Admin Management](screenshots/7-admin-management.png)

---

## 👨‍💻 Geliştirici Notu

Bu proje, **BMB401 - YAZILIM MÜHENDİSLİĞİ** dersi kapsamında; Clean Code prensiplerine dikkat edilerek, kullanıcı etkileşimini en üst düzeye çıkarmak amacıyla geliştirilmiştir.

---
*2025 © HabitTracker*
