import { useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function LoginPage({ navigate, showToast }) {
  const { login, register } = useAuth();
  const [tab, setTab] = useState("login"); // "login" | "signup"
  const [loading, setLoading] = useState(false);

  const [loginForm, setLoginForm] = useState({ email: "", password: "" });
  const [signupForm, setSignupForm] = useState({ name: "", email: "", password: "" });

  // إذا كان المستخدم اختار خطة قبل الدخول → نعيده لصفحة الأسعار لإكمال الدفع.
  const nextPageAfterAuth = () =>
    localStorage.getItem("pendingPlanId") ? "pricing" : "home";

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await login(loginForm.email, loginForm.password);
      showToast("success", "مرحباً بك! جاري التوجيه...");
      setTimeout(() => navigate(nextPageAfterAuth()), 1500);
    } catch (err) {
      showToast("error", err.message || "بيانات الدخول غير صحيحة");
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await register(signupForm.name, signupForm.email, signupForm.password);
      showToast("success", "تم إنشاء حسابك! جاري التوجيه...");
      setTimeout(() => navigate(nextPageAfterAuth()), 1500);
    } catch (err) {
      showToast("error", err.message || "حدث خطأ في إنشاء الحساب");
    } finally {
      setLoading(false);
    }
  };

  const inputStyle = (focused) => ({
    width: "100%", padding: "16px 50px 16px 20px",
    border: `2px solid ${focused ? "#6C63FF" : "rgba(108,99,255,0.2)"}`,
    borderRadius: 14, fontSize: "1rem", fontFamily: "'Cairo',sans-serif",
    color: "#1A1A2E", background: "#F8F9FC", outline: "none", transition: "all 0.3s",
  });

  const [focusedField, setFocusedField] = useState(null);

  return (
    <div style={{ display: "flex", minHeight: "100vh", fontFamily: "'Cairo',sans-serif" }} dir="rtl">
      {/* Left Side - Branding */}
      <div style={{
        flex: 1, position: "relative", overflow: "hidden",
        display: "flex", alignItems: "center", justifyContent: "center",
        background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
      }} className="login-left-side">
        <div style={{ position: "relative", zIndex: 2, color: "white", textAlign: "center", padding: 40 }}>
          <div style={{ fontSize: "4rem", marginBottom: 20 }}>⚡</div>
          <h1 style={{ fontSize: "3rem", fontWeight: 900, marginBottom: 20 }}>CreatorPro</h1>
          <p style={{ fontSize: "1.2rem", opacity: 0.9, lineHeight: 1.8, maxWidth: 400 }}>
            المنصة الأولى عربياً لصناع المحتوى بالذكاء الاصطناعي
          </p>
          <div style={{ display: "flex", justifyContent: "center", gap: 30, marginTop: 40 }}>
            {[["52K+","مستخدم"],["98%","رضا"],["1.2M+","محتوى"]].map(([num,lbl]) => (
              <div key={lbl} style={{ textAlign: "center" }}>
                <div style={{ fontSize: "2rem", fontWeight: 900 }}>{num}</div>
                <div style={{ opacity: 0.8, fontSize: "0.9rem" }}>{lbl}</div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Right Side - Form */}
      <div style={{
        flex: 1, display: "flex", flexDirection: "column", justifyContent: "center",
        padding: "60px 80px", background: "white", position: "relative", overflowY: "auto",
      }}>
        <button onClick={() => navigate("home")} style={{
          position: "absolute", top: 30, right: 40,
          display: "flex", alignItems: "center", gap: 8, color: "#4A4A68",
          background: "none", border: "none", cursor: "pointer",
          fontWeight: 600, fontSize: "0.95rem", fontFamily: "'Cairo',sans-serif",
          padding: "10px 20px", borderRadius: 12, transition: "all 0.3s",
        }}>
          <i className="fas fa-arrow-right" /> العودة
        </button>

        <div style={{ textAlign: "center", marginBottom: 40 }}>
          <h1 style={{ fontSize: "2.2rem", fontWeight: 900, color: "#1A1A2E", marginBottom: 12 }}>
            {tab === "login" ? "مرحباً بعودتك! 👋" : "أهلاً بك! ✨"}
          </h1>
          <p style={{ color: "#4A4A68", fontSize: "1.05rem" }}>
            {tab === "login" ? "سجل دخولك للوصول إلى مساحة عملك" : "أنشئ حسابك وابدأ الإبداع الآن"}
          </p>
        </div>

        {/* Tab Switcher */}
        <div style={{
          display: "flex", background: "#F0F2F8", borderRadius: 16, padding: 6,
          marginBottom: 40, position: "relative", border: "1px solid rgba(108,99,255,0.15)",
        }}>
          <div style={{
            position: "absolute", top: 6,
            right: tab === "login" ? 6 : "50%",
            height: "calc(100% - 12px)",
            width: "calc(50% - 6px)",
            background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
            borderRadius: 12, transition: "all 0.3s ease",
            boxShadow: "0 4px 15px rgba(108,99,255,0.3)",
          }} />
          <button onClick={() => setTab("login")} style={{
            flex: 1, padding: "14px 28px", border: "none", background: "transparent",
            color: tab === "login" ? "white" : "#4A4A68",
            fontSize: "1rem", fontWeight: 700, cursor: "pointer",
            borderRadius: 12, position: "relative", zIndex: 1,
            fontFamily: "'Cairo',sans-serif", transition: "color 0.3s",
          }}>تسجيل الدخول</button>
          <button onClick={() => setTab("signup")} style={{
            flex: 1, padding: "14px 28px", border: "none", background: "transparent",
            color: tab === "signup" ? "white" : "#4A4A68",
            fontSize: "1rem", fontWeight: 700, cursor: "pointer",
            borderRadius: 12, position: "relative", zIndex: 1,
            fontFamily: "'Cairo',sans-serif", transition: "color 0.3s",
          }}>حساب جديد</button>
        </div>

        {/* Login Form */}
        {tab === "login" && (
          <form onSubmit={handleLogin}>
            <div style={{ marginBottom: 24 }}>
              <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.95rem" }}>البريد الإلكتروني</label>
              <div style={{ position: "relative" }}>
                <input
                  type="email" required placeholder="name@example.com"
                  value={loginForm.email}
                  onChange={e => setLoginForm({...loginForm, email: e.target.value})}
                  onFocus={() => setFocusedField("l-email")}
                  onBlur={() => setFocusedField(null)}
                  style={inputStyle(focusedField === "l-email")}
                />
                <i className="fas fa-envelope" style={{ position: "absolute", right: 18, top: "50%", transform: "translateY(-50%)", color: focusedField === "l-email" ? "#6C63FF" : "#8A8AA8" }} />
              </div>
            </div>
            <div style={{ marginBottom: 10 }}>
              <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.95rem" }}>كلمة المرور</label>
              <div style={{ position: "relative" }}>
                <input
                  type="password" required placeholder="••••••••"
                  value={loginForm.password}
                  onChange={e => setLoginForm({...loginForm, password: e.target.value})}
                  onFocus={() => setFocusedField("l-pass")}
                  onBlur={() => setFocusedField(null)}
                  style={inputStyle(focusedField === "l-pass")}
                />
                <i className="fas fa-lock" style={{ position: "absolute", right: 18, top: "50%", transform: "translateY(-50%)", color: focusedField === "l-pass" ? "#6C63FF" : "#8A8AA8" }} />
              </div>
            </div>
            <a href="#" style={{ display: "block", textAlign: "left", color: "#6C63FF", textDecoration: "none", fontSize: "0.9rem", fontWeight: 600, marginBottom: 20 }}>نسيت كلمة المرور؟</a>

            <button type="submit" disabled={loading} style={{
              width: "100%", padding: 18, background: loading ? "#ccc" : "linear-gradient(135deg,#6C63FF,#00B8D4)",
              color: "white", border: "none", borderRadius: 14, fontSize: "1.1rem", fontWeight: 800,
              cursor: loading ? "not-allowed" : "pointer", fontFamily: "'Cairo',sans-serif",
              display: "flex", alignItems: "center", justifyContent: "center", gap: 10,
              boxShadow: "0 4px 15px rgba(108,99,255,0.3)", transition: "all 0.3s",
            }}>
              {loading ? <><i className="fas fa-spinner fa-spin" /> جاري التحقق...</> : <><i className="fas fa-sign-in-alt" /> تسجيل الدخول</>}
            </button>
          </form>
        )}

        {/* Signup Form */}
        {tab === "signup" && (
          <form onSubmit={handleRegister}>
            {[
              { key: "name", label: "الاسم الكامل", type: "text", icon: "fas fa-user", placeholder: "أدخل اسمك" },
              { key: "email", label: "البريد الإلكتروني", type: "email", icon: "fas fa-envelope", placeholder: "name@example.com" },
              { key: "password", label: "كلمة المرور", type: "password", icon: "fas fa-lock", placeholder: "••••••••" },
            ].map(({ key, label, type, icon, placeholder }) => (
              <div key={key} style={{ marginBottom: 24 }}>
                <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.95rem" }}>{label}</label>
                <div style={{ position: "relative" }}>
                  <input
                    type={type} required placeholder={placeholder}
                    value={signupForm[key]}
                    onChange={e => setSignupForm({...signupForm, [key]: e.target.value})}
                    minLength={key === "password" ? 6 : undefined}
                    onFocus={() => setFocusedField(`s-${key}`)}
                    onBlur={() => setFocusedField(null)}
                    style={inputStyle(focusedField === `s-${key}`)}
                  />
                  <i className={icon} style={{ position: "absolute", right: 18, top: "50%", transform: "translateY(-50%)", color: focusedField === `s-${key}` ? "#6C63FF" : "#8A8AA8" }} />
                </div>
              </div>
            ))}

            <button type="submit" disabled={loading} style={{
              width: "100%", padding: 18, background: loading ? "#ccc" : "linear-gradient(135deg,#6C63FF,#00B8D4)",
              color: "white", border: "none", borderRadius: 14, fontSize: "1.1rem", fontWeight: 800,
              cursor: loading ? "not-allowed" : "pointer", fontFamily: "'Cairo',sans-serif",
              display: "flex", alignItems: "center", justifyContent: "center", gap: 10,
              boxShadow: "0 4px 15px rgba(108,99,255,0.3)", transition: "all 0.3s",
            }}>
              {loading ? <><i className="fas fa-spinner fa-spin" /> جاري الإنشاء...</> : <><i className="fas fa-user-plus" /> إنشاء حساب</>}
            </button>
          </form>
        )}

        {/* Divider */}
        <div style={{ display: "flex", alignItems: "center", margin: "30px 0", color: "#8A8AA8", fontSize: "0.9rem" }}>
          <div style={{ flex: 1, height: 1, background: "rgba(108,99,255,0.15)" }} />
          <span style={{ padding: "0 20px" }}>أو</span>
          <div style={{ flex: 1, height: 1, background: "rgba(108,99,255,0.15)" }} />
        </div>

        {/* Google Button */}
        <button type="button" onClick={() => showToast("info", "جاري الاتصال بـ Google...")} style={{
          width: "100%", maxWidth: 400, margin: "0 auto", display: "flex",
          alignItems: "center", justifyContent: "center", gap: 12, padding: 16,
          background: "white", border: "2px solid #e5e7eb", borderRadius: 14,
          fontSize: "1rem", fontWeight: 700, color: "#374151", cursor: "pointer",
          fontFamily: "'Cairo',sans-serif", transition: "all 0.3s",
        }}>
          <svg width="24" height="24" viewBox="0 0 24 24">
            <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
            <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
            <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
            <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
          </svg>
          المتابعة باستخدام Google
        </button>

        <div style={{ textAlign: "center", marginTop: 40, color: "#8A8AA8", fontSize: "0.9rem" }}>
          <p>بالمتابعة، أنت توافق على <a href="#" style={{ color: "#6C63FF", textDecoration: "none", fontWeight: 600 }}>شروط الخدمة</a> و <a href="#" style={{ color: "#6C63FF", textDecoration: "none", fontWeight: 600 }}>سياسة الخصوصية</a></p>
          <p style={{ marginTop: 10 }}>© 2026 CreatorPro. جميع الحقوق محفوظة</p>
        </div>
      </div>

      <style>{`
        @media (max-width: 968px) { .login-left-side { display: none !important; } }
      `}</style>
    </div>
  );
}
