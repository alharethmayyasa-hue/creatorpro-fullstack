# CreatorPro — React Frontend

## هيكل المشروع

```
creatorpro/
├── src/
│   ├── services/
│   │   └── api.js          ← كل طلبات الـ API هنا ✅
│   ├── context/
│   │   └── AuthContext.jsx ← إدارة حالة المصادقة ✅
│   ├── hooks/
│   │   └── useToast.js     ← hook للإشعارات
│   ├── components/
│   │   ├── Header.jsx
│   │   ├── Footer.jsx
│   │   └── Toast.jsx
│   ├── pages/
│   │   ├── HomePage.jsx
│   │   ├── FeaturesPage.jsx
│   │   ├── TestimonialsPage.jsx
│   │   ├── PricingPage.jsx
│   │   └── LoginPage.jsx   ← Login + Signup
│   ├── App.jsx
│   └── main.jsx
├── .env.example            ← انسخه إلى .env.local
├── vite.config.js          ← proxy للـ backend
└── package.json
```

---

## تشغيل المشروع

```bash
npm install
cp .env.example .env.local
# عدّل VITE_API_URL في .env.local
npm run dev
```

---

## الربط مع ASP.NET Web API

### 1. تعيين URL الـ Backend

في `.env.local`:
```
VITE_API_URL=https://localhost:7000/api
```

### 2. Endpoints المطلوبة في الـ Backend

#### Auth Controller — `/api/auth`
| Method | Endpoint | Body | Response |
|--------|----------|------|----------|
| POST | `/login` | `{email, password}` | `{token, user}` |
| POST | `/register` | `{name, email, password}` | `{token, user}` |
| POST | `/google` | `{idToken}` | `{token, user}` |
| POST | `/forgot-password` | `{email}` | `{message}` |
| POST | `/logout` | — | 204 |

#### Stats Controller — `/api/stats`
| Method | Endpoint | Response |
|--------|----------|----------|
| GET | `/platform` | `{users, content, satisfaction, countries}` |
| GET | `/growth` | `[{month, value}]` |
| GET | `/platforms-distribution` | `[{platform, percentage}]` |

#### Reviews Controller — `/api/reviews`
| Method | Endpoint | Body | Response |
|--------|----------|------|----------|
| GET | `/` | — | `[{id, name, role, rating, text, date, platform}]` |
| POST | `/` | `{name, email, rating, text}` | `{id, message}` |

#### Plans Controller — `/api/plans`
| Method | Endpoint | Body | Response |
|--------|----------|------|----------|
| GET | `/` | — | `[{id, name, price, features}]` |
| POST | `/subscribe` | `{planId}` | `{subscriptionId}` |

### 3. تفعيل الـ API في الكود

في كل صفحة، ابحث عن التعليقات المحاطة بـ `// ---- Uncomment when backend is ready ----`
وأزل التعليق عن الكود للتفعيل.

**مثال في `HomePage.jsx`:**
```jsx
useEffect(() => {
  statsAPI.getPlatformStats().then(data => setStats(data)).catch(console.error);
  statsAPI.getGrowthData().then(data => setBars(data)).catch(console.error);
}, []);
```

### 4. CORS في ASP.NET

```csharp
// Program.cs
builder.Services.AddCors(options => {
    options.AddPolicy("ReactApp", policy => {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
app.UseCors("ReactApp");
```

### 5. JWT Authentication في ASP.NET

الـ token يُرسل تلقائياً في كل request عبر header:
```
Authorization: Bearer <token>
```

```csharp
// في كل Controller محمي
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase { ... }
```

---

## Build للـ Production

```bash
npm run build
# الملفات تُنتج في مجلد dist/
```
