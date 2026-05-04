import { useEffect, useState } from "react";
import { plansAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import CheckoutModal from "../components/CheckoutModal";

const PENDING_PLAN_KEY = "pendingPlanId";

// ---- خطط ثابتة كـ fallback في حال فشل الاتصال بالباك اند ----
const FALLBACK_PLANS = [
  {
    id: "free", name: "مجاني", price: "0$", period: "شهرياً", desc: "مثالي للبداية والتجربة",
    features: [
      { text: "5 سكريبتات شهرياً", included: true },
      { text: "منصة واحدة", included: true },
      { text: "دعم أساسي", included: true },
      { text: "تحليل الفيديو", included: false },
      { text: "نشر تلقائي", included: false },
    ],
    featured: false, btnStyle: "outline",
  },
  {
    id: "pro", name: "محترف", price: "29$", period: "شهرياً", desc: "لصناع المحتوى الجادين",
    features: [
      { text: "سكريبتات غير محدودة", included: true },
      { text: "جميع المنصات", included: true },
      { text: "دعم مميز", included: true },
      { text: "تحليل الفيديو", included: true },
      { text: "نشر تلقائي", included: true },
    ],
    featured: true, btnStyle: "primary", badge: "الأكثر شعبية",
  },
  {
    id: "enterprise", name: "شركات", price: "99$", period: "شهرياً", desc: "للشركات والفرق الكبيرة",
    features: [
      { text: "كل ميزات المحترف", included: true },
      { text: "10 حسابات فريق", included: true },
      { text: "دعم 24/7", included: true },
      { text: "API كامل", included: true },
      { text: "تقارير مخصصة", included: true },
    ],
    featured: false, btnStyle: "outline",
  },
];

// تحويل ميزات الباك اند (نص واحد) إلى مصفوفة عناصر مع علامة included.
function parseFeatures(features) {
  if (!features) return [];
  if (Array.isArray(features)) {
    return features.map((f) => ({ text: String(f), included: true }));
  }
  return String(features)
    .split(/\r?\n|,|;|\|/)
    .map((s) => s.trim())
    .filter(Boolean)
    .map((text) => ({ text, included: true }));
}

// تحويل خطة من الباك اند (PlanResponseDto) إلى نفس شكل بطاقة الـ UI.
function mapBackendPlan(p, index, total) {
  const isFeatured = total >= 3 ? index === 1 : false;
  const priceNumber =
    typeof p.price === "number" ? p.price : Number(p.price) || 0;
  const periodLabel =
    p.durationDays && p.durationDays > 0
      ? p.durationDays === 30
        ? "شهرياً"
        : `${p.durationDays} يوم`
      : "شهرياً";

  const features = parseFeatures(p.features);
  if (p.creditsAmount && features.length === 0) {
    features.push({ text: `${p.creditsAmount} كريديت`, included: true });
  }

  return {
    id: p.planId,
    backendId: p.planId,
    name: p.name,
    price: `${priceNumber}$`,
    priceNumber,
    period: periodLabel,
    desc: p.description || "",
    features,
    featured: isFeatured,
    btnStyle: isFeatured ? "primary" : "outline",
    badge: isFeatured ? "الأكثر شعبية" : undefined,
  };
}

export default function PricingPage({ navigate, showToast }) {
  const { user } = useAuth();
  const [plans, setPlans] = useState(FALLBACK_PLANS);
  const [selected, setSelected] = useState(null);
  const [checkoutPlan, setCheckoutPlan] = useState(null);

  // ---- جلب الخطط من الباك اند ----
  useEffect(() => {
    let cancelled = false;
    plansAPI
      .getAll()
      .then((data) => {
        if (cancelled) return;
        if (Array.isArray(data) && data.length > 0) {
          const active = data.filter((p) => p.isActive !== false);
          const list = (active.length > 0 ? active : data).map((p, i, arr) =>
            mapBackendPlan(p, i, arr.length)
          );
          setPlans(list);
        }
      })
      .catch((err) => {
        // نبقى على الخطط الثابتة في حال فشل الاتصال
        console.error("Failed to fetch plans:", err);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  // ---- بعد تسجيل الدخول: لو كان المستخدم اختار خطة قبل الدخول، افتح الدفع ----
  useEffect(() => {
    if (!user) return;
    if (plans.length === 0) return;
    const pendingId = localStorage.getItem(PENDING_PLAN_KEY);
    if (!pendingId) return;
    const target = plans.find(
      (p) => String(p.backendId ?? p.id) === String(pendingId)
    );
    if (target && typeof target.backendId === "number") {
      localStorage.removeItem(PENDING_PLAN_KEY);
      setSelected(target.id);
      setCheckoutPlan(target);
    }
  }, [user, plans]);

  const handleSelect = (plan) => {
    setSelected(plan.id);

    // إذا لم يسجل المستخدم الدخول → احفظ الخطة المختارة ووجهه لصفحة الدخول
    if (!user) {
      if (typeof plan.backendId === "number") {
        localStorage.setItem(PENDING_PLAN_KEY, String(plan.backendId));
      }
      showToast("info", `تم اختيار خطة ${plan.name}. سجل الدخول للمتابعة!`);
      setTimeout(() => navigate("login"), 1200);
      return;
    }

    // الخطط الثابتة (fallback) ليس لها معرف رقمي حقيقي في الباك اند
    if (typeof plan.backendId !== "number") {
      showToast(
        "info",
        "لا يمكن إكمال الاشتراك حالياً، الخطط لم تُحمّل من الخادم."
      );
      return;
    }

    setCheckoutPlan(plan);
  };

  const closeCheckout = () => {
    setCheckoutPlan(null);
    setSelected(null);
  };

  return (
    <section style={{ padding: "100px 5%" }}>
      <div style={{ textAlign: "center", marginBottom: 70 }}>
        <h2 style={{ fontSize: "2.8rem", fontWeight: 800, marginBottom: 15, color: "#1A1A2E" }}>
          خطط <span style={{ background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }}>تناسب الجميع</span>
        </h2>
        <p style={{ color: "#4A4A68", fontSize: "1.2rem" }}>اختر الخطة المناسبة لك وسجل الدخول للبدء</p>
      </div>

      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit,minmax(300px,1fr))", gap: 35, maxWidth: 1100, margin: "0 auto" }}>
        {plans.map(plan => (
          <div
            key={plan.id}
            onClick={() => handleSelect(plan)}
            style={{
              background: plan.featured ? "linear-gradient(180deg,rgba(108,99,255,0.05),white)" : "white",
              border: `${selected === plan.id || plan.featured ? 2 : 1}px solid ${selected === plan.id ? "#6C63FF" : plan.featured ? "rgba(108,99,255,0.5)" : "rgba(108,99,255,0.15)"}`,
              borderRadius: 24, padding: "45px 40px", textAlign: "center",
              cursor: "pointer", position: "relative", overflow: "hidden",
              boxShadow: selected === plan.id ? "0 0 60px rgba(108,99,255,0.35)" : plan.featured ? "0 0 50px rgba(108,99,255,0.2)" : "0 10px 40px rgba(108,99,255,0.1)",
              transform: selected === plan.id ? "scale(1.02)" : "scale(1)",
              transition: "all 0.4s ease",
            }}
          >
            {plan.badge && (
              <div style={{
                position: "absolute", top: -16, left: "50%", transform: "translateX(-50%)",
                background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
                padding: "8px 28px", borderRadius: 25, fontWeight: 700, fontSize: "0.9rem",
              }}>
                {plan.badge}
              </div>
            )}

            <h3 style={{ fontSize: "1.6rem", fontWeight: 700, marginBottom: 15, color: "#1A1A2E" }}>{plan.name}</h3>

            <div style={{
              fontSize: "3.5rem", fontWeight: 900, marginBottom: 15,
              background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
              WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent",
            }}>
              {plan.price}<span style={{ fontSize: "1.1rem", WebkitTextFillColor: "#8A8AA8", fontWeight: 400 }}>/{plan.period}</span>
            </div>

            <p style={{ color: "#4A4A68", marginBottom: 30 }}>{plan.desc}</p>

            <ul style={{ listStyle: "none", textAlign: "right", marginBottom: 35 }}>
              {plan.features.map((f, i) => (
                <li key={i} style={{
                  padding: "14px 0", borderBottom: "1px solid rgba(108,99,255,0.1)",
                  color: f.included ? "#1A1A2E" : "#8A8AA8", display: "flex",
                  alignItems: "center", gap: 12, opacity: f.included ? 1 : 0.6,
                }}>
                  <i className={f.included ? "fas fa-check" : "fas fa-times"}
                    style={{ color: f.included ? "#00C851" : "#8A8AA8" }} />
                  {f.text}
                </li>
              ))}
            </ul>

            <button style={{
              width: "100%", padding: "16px 32px", borderRadius: 14, fontFamily: "'Cairo',sans-serif",
              fontSize: "1rem", fontWeight: 700, cursor: "pointer",
              display: "flex", alignItems: "center", justifyContent: "center", gap: 10,
              background: plan.btnStyle === "primary" ? "linear-gradient(135deg,#6C63FF,#00B8D4)" : "transparent",
              color: plan.btnStyle === "primary" ? "white" : "#6C63FF",
              border: plan.btnStyle === "primary" ? "none" : "2px solid #6C63FF",
              boxShadow: plan.btnStyle === "primary" ? "0 4px 15px rgba(108,99,255,0.3)" : "none",
            }}>
              <i className={plan.btnStyle === "primary" ? "fas fa-rocket" : "fas fa-arrow-left"} />
              اختر الخطة
            </button>
          </div>
        ))}
      </div>

      {checkoutPlan && (
        <CheckoutModal
          plan={checkoutPlan}
          onClose={closeCheckout}
          showToast={showToast}
        />
      )}
    </section>
  );
}
