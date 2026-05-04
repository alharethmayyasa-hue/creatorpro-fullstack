// ============================================================
//  CreatorPro — API Service Layer
//  مرتبط بـ ASP.NET Web API (GraduationProject.API)
//  يحوّل المسارات الخارجية الموحدة إلى المسارات الفعلية في الباك اند
//  ويفك غلاف ApiResponse<T> { isSuccess, data, message, errors }
// ============================================================

const BASE_URL = import.meta.env.VITE_API_URL || "/api";

// ---- مساعد لاستخراج رسالة الخطأ من ردود الباك اند ----
function extractErrorMessage(payload, fallback) {
  if (!payload) return fallback;

  // ApiResponse<object>.Fail
  if (payload.message) return payload.message;

  // {message: "..."} المباشرة (Account/login/register/logout)
  // ModelState validation: { errors: { Field: ["msg"] } }
  if (payload.errors && typeof payload.errors === "object") {
    const firstKey = Object.keys(payload.errors)[0];
    const arr = payload.errors[firstKey];
    if (Array.isArray(arr) && arr.length > 0) return arr[0];
  }

  if (payload.title) return payload.title;
  return fallback;
}

// ---- مساعد طلبات HTTP ----
async function request(endpoint, options = {}) {
  const token = localStorage.getItem("token");

  const headers = {
    "Content-Type": "application/json",
    Accept: "application/json",
    ...(token && { Authorization: `Bearer ${token}` }),
    ...options.headers,
  };

  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (response.status === 204) return null;

  let payload = null;
  const contentType = response.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    payload = await response.json().catch(() => null);
  } else {
    const text = await response.text().catch(() => "");
    payload = text ? { message: text } : null;
  }

  if (!response.ok) {
    throw new Error(
      extractErrorMessage(payload, `HTTP ${response.status}`) || "خطأ في الخادم"
    );
  }

  // فك غلاف ApiResponse<T> الموحد إن وُجد
  if (payload && typeof payload === "object" && "isSuccess" in payload) {
    if (payload.isSuccess === false) {
      throw new Error(extractErrorMessage(payload, "خطأ في الخادم"));
    }
    return payload.data;
  }

  return payload;
}

// ============================================================
//  Auth Endpoints  →  /api/Account/...
// ============================================================
export const authAPI = {
  /**
   * POST /api/Account/login
   * Body: { email, password }
   * Backend response: AuthResponseDto
   *   { isAuthenticated, message, token, refreshToken, expiresOn,
   *     email, userName, roles }
   * Returns adapted shape: { token, refreshToken, user }
   */
  login: async (email, password) => {
    const data = await request("/Account/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });

    if (!data || data.isAuthenticated === false) {
      throw new Error(data?.message || "بيانات الدخول غير صحيحة");
    }

    return {
      token: data.token,
      refreshToken: data.refreshToken,
      expiresOn: data.expiresOn,
      user: {
        email: data.email,
        name: data.userName,
        userName: data.userName,
        roles: data.roles || [],
      },
    };
  },

  /**
   * POST /api/Account/register
   * Body (backend): { fullName, email, password, confirmPassword, userName }
   * نشتق UserName من البريد و ConfirmPassword = Password حتى لا نمسّ التصميم.
   */
  register: async (name, email, password) => {
    const baseUserName = (email || "").split("@")[0] || "user";
    const userName =
      baseUserName.length >= 3 ? baseUserName : baseUserName + "user";

    const data = await request("/Account/register", {
      method: "POST",
      body: JSON.stringify({
        fullName: name,
        email,
        password,
        confirmPassword: password,
        userName,
      }),
    });

    if (!data || data.isAuthenticated === false) {
      throw new Error(data?.message || "حدث خطأ في إنشاء الحساب");
    }

    return {
      token: data.token,
      refreshToken: data.refreshToken,
      expiresOn: data.expiresOn,
      user: {
        email: data.email,
        name: data.userName,
        userName: data.userName,
        roles: data.roles || [],
      },
    };
  },

  /**
   * Google login غير متوفر حالياً في هذا الـ backend.
   */
  googleLogin: () => {
    return Promise.reject(
      new Error("تسجيل الدخول عبر Google غير مفعل حالياً")
    );
  },

  /**
   * Forgot password غير متوفر حالياً في هذا الـ backend.
   */
  forgotPassword: () => {
    return Promise.reject(
      new Error("استعادة كلمة المرور غير مفعلة حالياً")
    );
  },

  /**
   * POST /api/Account/logout
   */
  logout: () => request("/Account/logout", { method: "POST" }),
};

// ============================================================
//  Stats Endpoints (لا توجد حالياً في الـ backend — تبقى كـ stubs)
// ============================================================
export const statsAPI = {
  getPlatformStats: () =>
    Promise.reject(new Error("Stats endpoint غير متوفر")),
  getGrowthData: () =>
    Promise.reject(new Error("Stats endpoint غير متوفر")),
  getPlatformsDistribution: () =>
    Promise.reject(new Error("Stats endpoint غير متوفر")),
};

// ============================================================
//  Reviews Endpoints (لا توجد حالياً في الـ backend — تبقى كـ stubs)
// ============================================================
export const reviewsAPI = {
  getAll: () => Promise.reject(new Error("Reviews endpoint غير متوفر")),
  submit: () => Promise.reject(new Error("Reviews endpoint غير متوفر")),
};

// ============================================================
//  Plans Endpoints  →  /api/Plan
//  Subscribe Endpoint →  /api/Subscription/subscribe
// ============================================================
export const plansAPI = {
  /**
   * GET /api/Plan
   * Response: ApiResponse<List<PlanResponseDto>>
   * PlanResponseDto: { planId, name, description, price,
   *                    creditsAmount, durationDays, isTrial, isActive, features }
   */
  getAll: () => request("/Plan"),

  /**
   * POST /api/Subscription/subscribe   (يتطلب Authorization: Bearer)
   * Body: { planId }
   */
  subscribe: (planId) =>
    request("/Subscription/subscribe", {
      method: "POST",
      body: JSON.stringify({ planId }),
    }),

  /**
   * GET /api/Subscription/active-details   (يتطلب Authorization: Bearer)
   */
  getActive: () => request("/Subscription/active-details"),

  /**
   * GET /api/Subscription/my-status   (يتطلب Authorization: Bearer)
   */
  getMyStatus: () => request("/Subscription/my-status"),

  /**
   * POST /api/Subscription/cancel   (يتطلب Authorization: Bearer)
   */
  cancel: () => request("/Subscription/cancel", { method: "POST" }),
};

// ============================================================
//  Discount + Stripe Payment Endpoints  →  /api/Discount/...
//  /api/Payment/{paymentIntentId}
// ============================================================
export const discountAPI = {
  /**
   * POST /api/Discount/validate
   * Body: { code, originalPrice }
   * Response: DiscountResponseDto
   *   { isValid, code, finalPrice, savedAmount, message }
   */
  validate: (code, originalPrice) =>
    request("/Discount/validate", {
      method: "POST",
      body: JSON.stringify({ code, originalPrice }),
    }),

  /**
   * POST /api/Discount/payment-intent?userId=&discountCode=&amount=
   * Backend uses [FromQuery] for all parameters, so we encode them in URL.
   * Returns: PaymentIntentDto { clientSecret, paymentIntentId }
   */
  createPaymentIntent: ({ userId, discountCode, amount }) => {
    const params = new URLSearchParams();
    params.append("userId", String(userId));
    if (discountCode) params.append("discountCode", discountCode);
    params.append("amount", String(amount));
    return request(`/Discount/payment-intent?${params.toString()}`, {
      method: "POST",
    });
  },
};

export const paymentAPI = {
  /**
   * GET /api/Payment/{paymentIntentId}
   * Returns PaymentStatusDto from the DB (after webhook fires) or from
   * Stripe API as a fallback.
   */
  getStatus: (paymentIntentId) => request(`/Payment/${paymentIntentId}`),
};

// ============================================================
//  مساعد لاستخراج userId من JWT (الـ backend يضيفه كـ claim "userId")
// ============================================================
export function getUserIdFromToken() {
  const token = localStorage.getItem("token");
  if (!token) return null;
  try {
    const payloadBase64 = token.split(".")[1];
    if (!payloadBase64) return null;
    const padded = payloadBase64.replace(/-/g, "+").replace(/_/g, "/");
    const decoded = atob(padded.padEnd(padded.length + ((4 - (padded.length % 4)) % 4), "="));
    const json = JSON.parse(decoded);
    const raw =
      json.userId ||
      json.nameid ||
      json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
      json.sub;
    const id = Number(raw);
    return Number.isFinite(id) && id > 0 ? id : null;
  } catch {
    return null;
  }
}

