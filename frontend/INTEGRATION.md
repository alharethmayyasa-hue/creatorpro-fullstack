# ربط CreatorPro (الفرونت إند) بـ GraduationProject.API (الباك اند)

تم ربط صفحة الهبوط بالباك اند **بدون أي تعديل في التصميم أو واجهة الـ UI**،
وأي تعديل أُجري كان فقط في طبقة استدعاء الـ API.

---

## الملفات التي تغيّرت

| الملف | السبب |
|------|------|
| `vite.config.js` | تحديث وجهة الـ proxy إلى `http://localhost:5128` (المنفذ الفعلي للباك اند). |
| `.env.local` و `.env.example` | جعل `VITE_API_URL=/api` ليتم استخدام الـ proxy في وضع التطوير (تجنّب CORS بدون لمس الباك اند). |
| `src/services/api.js` | تحويل المسارات إلى المسارات الفعلية في الباك اند (`/Account/login`, `/Account/register`, `/Account/logout`, `/Plan`, `/Subscription/subscribe`)، وفك غلاف `ApiResponse<T>`، واستخراج رسائل الخطأ. |
| `src/context/AuthContext.jsx` | حفظ التوكن وبيانات المستخدم بالشكل الذي يرجعه الباك اند (`AuthResponseDto`)، وتنظيف آمن عند الخروج. |
| `src/pages/PricingPage.jsx` | جلب الخطط من `GET /api/Plan` وعرضها بنفس بطاقات التصميم الحالية، واستدعاء `POST /api/Subscription/subscribe` عند اختيار المستخدم لخطة (مع توجيه غير المسجلين لصفحة الدخول). |

> ملف `LoginPage.jsx` لم يُلمَس. التحقق الإضافي الذي يحتاجه الباك اند
> (`UserName` و `ConfirmPassword`) يُولَّد تلقائياً داخل `services/api.js`
> للحفاظ على نفس التصميم.

---

## كيف تُشغِّل كل شيء معاً

### 1. شغّل الباك اند

```bash
cd graduation-project-main-develop/GraduationProject.API
dotnet run
```

سيستمع الباك اند على `http://localhost:5128` (راجع `Properties/launchSettings.json`).

> تأكد من أن قاعدة البيانات `GraduationProjectDb` متاحة وأن الـ Migrations
> طُبّقت (إن لزم الأمر: `dotnet ef database update` من داخل مشروع
> `GraduationProject.Infrastructure`).

### 2. شغّل الفرونت اند

```bash
cd creatorpro
npm install
npm run dev
```

افتح المتصفح على `http://localhost:3000`.

أي طلب يبدأ بـ `/api/...` سيمرّ تلقائياً عبر Vite proxy إلى الباك اند،
لذا **لا يحتاج الباك اند إلى تفعيل CORS** في وضع التطوير.

---

## ربط الـ Endpoints

| في الواجهة (api.js) | في الباك اند | الحاجة لتوكن؟ |
|---|---|---|
| `authAPI.login(email, password)` | `POST /api/Account/login` | لا |
| `authAPI.register(name, email, password)` | `POST /api/Account/register` | لا |
| `authAPI.logout()` | `POST /api/Account/logout` | نعم |
| `plansAPI.getAll()` | `GET /api/Plan` | لا |
| `plansAPI.subscribe(planId)` | `POST /api/Subscription/subscribe` | نعم |
| `plansAPI.getActive()` | `GET /api/Subscription/active-details` | نعم |
| `plansAPI.getMyStatus()` | `GET /api/Subscription/my-status` | نعم |
| `plansAPI.cancel()` | `POST /api/Subscription/cancel` | نعم |

التوكن يُحفظ في `localStorage` تحت المفتاح `token` ويُرسَل تلقائياً
في رأس `Authorization: Bearer ...` عبر الدالة `request()` في
`src/services/api.js`.

---

## تنبيهات

- الـ endpoints التالية غير موجودة في الباك اند الحالي (تركتُها كـ stubs ترجع خطأ مفهوماً عند الاستدعاء فقط، بدون أي تعديل بصري على الصفحات):
  - `POST /auth/google`
  - `POST /auth/forgot-password`
  - `GET /stats/*`
  - `GET /reviews`
- `forgot-password` في `LoginPage.jsx` كان مجرد رابط HTML بدون استدعاء API،
  فلم نمسّه.
- إذا أردت الانتقال إلى الإنتاج (بدون proxy): غيّر `VITE_API_URL` في
  `.env.local` إلى الرابط الكامل للباك اند (مثلاً `https://api.example.com/api`)
  وأضف CORS للباك اند.
