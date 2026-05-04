# CreatorPro — Fullstack (Frontend + Backend Integrated)

ربط صفحة الهبوط React (CreatorPro) مع الباك اند ASP.NET Core 8
(GraduationProject.API) — بدون أي تعديل على التصميم. المستودع يضم
المشروعين معاً في مجلدين منفصلين:

```
creatorpro-fullstack/
├── backend/        ← ASP.NET Core 8 Web API (GraduationProject)
└── frontend/       ← React 18 + Vite (CreatorPro)
```

---

## ✨ ما الذي يعمل بعد الربط

| الوظيفة | المسار في الباك اند | الحالة |
|---|---|---|
| تسجيل الدخول | `POST /api/Account/login` | ✅ |
| إنشاء حساب | `POST /api/Account/register` | ✅ |
| تسجيل الخروج | `POST /api/Account/logout` | ✅ |
| جلب الخطط من قاعدة البيانات | `GET /api/Plan` | ✅ |
| الاشتراك في خطة | `POST /api/Subscription/subscribe` | ✅ (يتم بعد نجاح الدفع) |
| التحقق من كود الخصم | `POST /api/Discount/validate` | ✅ |
| إنشاء PaymentIntent (Stripe) | `POST /api/Discount/payment-intent` | ✅ |
| تأكيد الدفع (3D Secure) | `Stripe Elements` على الفرونت | ✅ |
| Webhook ما بعد الدفع | `POST /api/StripeWebhook` | ✅ (يضيف الكريديت + يسجل المدفوعات) |

---

## 🚀 التشغيل السريع

### 1) الباك اند

> يحتاج SQL Server (LocalDB أو SQLEXPRESS) و .NET 8 SDK.

أولاً انسخ ملف الإعدادات المحلي وضع المفاتيح الحقيقية فيه (هذا الملف
مستثنى من git):

```bash
cd backend/GraduationProject.API
cp appsettings.Development.json.example appsettings.Development.json
# ثم افتح appsettings.Development.json وضع مفاتيح Stripe و Jwt الحقيقية
```

ثم شغّل الباك اند:

```bash
cd backend/GraduationProject.API
dotnet restore
dotnet ef database update --project ../GraduationProject.Infrastructure
dotnet run
```

سيستمع الباك اند على `http://localhost:5128`. صفحة Swagger متاحة على
`http://localhost:5128/swagger`.

> القيم في
> [`backend/GraduationProject.API/appsettings.json`](backend/GraduationProject.API/appsettings.json)
> مجرد placeholders بسبب GitHub Push Protection. القيم الفعلية في
> `appsettings.Development.json` المستثناة من git.

### 2) الفرونت اند

```bash
cd frontend
cp .env.example .env.local        # اختياري — موجود مسبقاً
npm install
npm run dev
```

افتح `http://localhost:3000`. أي طلب يبدأ بـ `/api/...` سيمر تلقائياً عبر
Vite proxy إلى `http://localhost:5128` (راجع [`frontend/vite.config.js`](frontend/vite.config.js))
لذا **لا حاجة لتفعيل CORS** على الباك اند في وضع التطوير.

---

## 💳 تدفق الدفع كاملاً (Stripe + كود خصم)

1. المستخدم يفتح صفحة الأسعار → يجلب الفرونت الخطط من `GET /api/Plan`.
2. يضغط "اختر الخطة":
   - إن لم يكن مسجل دخول → نخزن `pendingPlanId` في `localStorage`
     ونوجهه لصفحة الدخول.
   - بعد نجاح الدخول/التسجيل → يعود تلقائياً لصفحة الأسعار وتفتح
     مودال الدفع للخطة المختارة.
3. **مودال الدفع** ([`frontend/src/components/CheckoutModal.jsx`](frontend/src/components/CheckoutModal.jsx))
   يعرض:
   - ملخص الخطة + السعر.
   - حقل اختياري لكود الخصم → يضغط "تطبيق" يستدعي
     `POST /api/Discount/validate` ويعرض المبلغ الموفّر فوراً.
   - **Stripe CardElement** (بطاقة + 3D Secure تلقائياً).
4. عند الضغط على "ادفع":
   - الفرونت يستدعي `POST /api/Discount/payment-intent?userId&discountCode&amount`
     فيعيد الباك اند `clientSecret`.
   - الفرونت يستدعي `stripe.confirmCardPayment(clientSecret, ...)` —
     Stripe يتولى 3D Secure والمصادقة.
   - عند `succeeded` يستدعي الفرونت `POST /api/Subscription/subscribe`
     لتسجيل الاشتراك في قاعدة البيانات.
5. الـ Webhook في الباك اند (`POST /api/StripeWebhook`) يستقبل
   `payment_intent.succeeded` ويضيف الكريديت + يسجل عملية الدفع
   + يسجل استخدام كود الخصم.

> بطاقة الاختبار من Stripe: `4242 4242 4242 4242` — أي تاريخ مستقبل / أي CVC.

---

## 🔧 ما الذي تغير لربط المشروعين

### الباك اند (تعديل واحد فقط — تهيئة Stripe الناقصة)

- [`backend/GraduationProject.API/Program.cs`](backend/GraduationProject.API/Program.cs):
  أضيف سطر واحد قبل `AddIdentity(...)`:
  ```csharp
  Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
  ```
  بدون هذا السطر كان `new PaymentIntentService()` يفشل لأن
  `Stripe.net` SDK لا يعرف المفتاح. لم تتغير أي endpoint أو خدمة أو
  أي منطق آخر.

### الفرونت اند (إضافات فقط — لا تغيير في التصميم)

| الملف | التغيير |
|---|---|
| `frontend/package.json` | أضيف `@stripe/stripe-js` و `@stripe/react-stripe-js`. |
| `frontend/.env.local` و `.env.example` | أضيف `VITE_STRIPE_PUBLISHABLE_KEY`. |
| `frontend/src/services/api.js` | أضيف `discountAPI` و `paymentAPI` و `getUserIdFromToken()`. |
| `frontend/src/components/CheckoutModal.jsx` | **جديد** — مودال الدفع (Stripe Elements + كود الخصم). |
| `frontend/src/pages/PricingPage.jsx` | بعد اختيار الخطة وتسجيل الدخول، يفتح المودال بدلاً من استدعاء `subscribe` مباشرة (التصميم نفسه). |
| `frontend/src/pages/LoginPage.jsx` | بعد نجاح الدخول/التسجيل، يعود لصفحة الأسعار إن كان هناك `pendingPlanId`. |

ملف [`frontend/INTEGRATION.md`](frontend/INTEGRATION.md) يحوي تفاصيل
ربط Auth + Plans (تكاملٌ سابق لم يلمس في هذه المرحلة).

---

## 🧪 التحقق

- `npm run build` داخل `frontend/` ينتج build نظيف بدون أخطاء.
- `dotnet build` لـ `backend/GraduationProject.API.sln` ينجح بدون أخطاء (تحذيرات فقط).
