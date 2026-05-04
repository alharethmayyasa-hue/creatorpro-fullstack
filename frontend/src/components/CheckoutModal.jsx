import { useEffect, useMemo, useState } from "react";
import { loadStripe } from "@stripe/stripe-js";
import {
  Elements,
  CardElement,
  useElements,
  useStripe,
} from "@stripe/react-stripe-js";
import {
  discountAPI,
  plansAPI,
  paymentAPI,
  getUserIdFromToken,
} from "../services/api";

// Stripe singleton — lazily loaded once.
const STRIPE_PK = import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY;
const stripePromise = STRIPE_PK ? loadStripe(STRIPE_PK) : Promise.resolve(null);

// ---- Card field styling (مطابق لطابع الموقع) ----
const CARD_OPTIONS = {
  style: {
    base: {
      iconColor: "#6C63FF",
      color: "#1A1A2E",
      fontFamily: "'Cairo', sans-serif",
      fontSize: "16px",
      "::placeholder": { color: "#8A8AA8" },
    },
    invalid: { color: "#E53935", iconColor: "#E53935" },
  },
};

function fmtUSD(n) {
  if (typeof n !== "number" || !Number.isFinite(n)) return "—";
  return `${n.toFixed(2)}$`;
}

// ============================================================
//  CheckoutModal: مودال خارج التصميم الحالي يفتح فقط عند اختيار خطة
//  + تسجيل الدخول. لا يعدل أي صفحة موجودة.
// ============================================================
export default function CheckoutModal({ plan, onClose, showToast }) {
  if (!plan) return null;
  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(26,26,46,0.55)",
        backdropFilter: "blur(6px)",
        zIndex: 2000,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 20,
        fontFamily: "'Cairo', sans-serif",
      }}
      dir="rtl"
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "white",
          borderRadius: 24,
          width: "100%",
          maxWidth: 520,
          maxHeight: "92vh",
          overflowY: "auto",
          boxShadow: "0 30px 80px rgba(108,99,255,0.35)",
          padding: 0,
        }}
      >
        <Elements stripe={stripePromise}>
          <CheckoutForm plan={plan} onClose={onClose} showToast={showToast} />
        </Elements>
      </div>
    </div>
  );
}

function CheckoutForm({ plan, onClose, showToast }) {
  const stripe = useStripe();
  const elements = useElements();

  // ---- خصم ----
  const [discountCode, setDiscountCode] = useState("");
  const [appliedDiscount, setAppliedDiscount] = useState(null); // { code, finalPrice, savedAmount }
  const [validating, setValidating] = useState(false);

  // ---- دفع ----
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  const userId = useMemo(() => getUserIdFromToken(), []);
  const originalPrice = useMemo(() => Number(plan.priceNumber) || 0, [plan]);
  const finalPrice = appliedDiscount ? appliedDiscount.finalPrice : originalPrice;

  // إعادة تعيين الخصم لو غير المستخدم الكود
  useEffect(() => {
    if (appliedDiscount && discountCode.trim() !== appliedDiscount.code) {
      setAppliedDiscount(null);
    }
  }, [discountCode, appliedDiscount]);

  const applyDiscount = async () => {
    const code = discountCode.trim();
    if (!code) {
      showToast("info", "أدخل كود الخصم أولاً");
      return;
    }
    setValidating(true);
    setError("");
    try {
      const res = await discountAPI.validate(code, originalPrice);
      if (res && res.isValid) {
        setAppliedDiscount({
          code: res.code || code,
          finalPrice: Number(res.finalPrice) || originalPrice,
          savedAmount: Number(res.savedAmount) || 0,
        });
        showToast(
          "success",
          `تم تطبيق الخصم! وفرت ${fmtUSD(Number(res.savedAmount) || 0)}`
        );
      } else {
        setAppliedDiscount(null);
        const msg = (res && res.message) || "كود الخصم غير صالح";
        showToast("error", msg);
      }
    } catch (err) {
      setAppliedDiscount(null);
      showToast("error", err.message || "فشل التحقق من كود الخصم");
    } finally {
      setValidating(false);
    }
  };

  const handlePay = async (e) => {
    e.preventDefault();
    if (submitting) return;
    setError("");

    if (!userId) {
      setError("لم يتم العثور على معرف المستخدم. الرجاء تسجيل الدخول مجدداً.");
      return;
    }
    if (!stripe || !elements) {
      setError("Stripe لم يحمل بعد. حاول مرة أخرى.");
      return;
    }
    const card = elements.getElement(CardElement);
    if (!card) {
      setError("بيانات البطاقة غير مكتملة.");
      return;
    }
    if (originalPrice <= 0) {
      // خطة مجانية: نسجل الاشتراك مباشرة بدون Stripe
      setSubmitting(true);
      try {
        await plansAPI.subscribe(plan.backendId);
        showToast("success", "تم تفعيل الخطة المجانية!");
        onClose?.();
      } catch (err) {
        setError(err.message || "فشل تسجيل الاشتراك");
      } finally {
        setSubmitting(false);
      }
      return;
    }

    setSubmitting(true);
    try {
      // 1) أنشئ PaymentIntent من الباك اند
      const intent = await discountAPI.createPaymentIntent({
        userId,
        discountCode: appliedDiscount?.code || "",
        amount: originalPrice, // الباك اند يعيد حساب الخصم بنفسه
      });
      if (!intent?.clientSecret) {
        throw new Error("فشل إنشاء جلسة الدفع");
      }

      // 2) أكد الدفع بالبطاقة (يدير Stripe 3D Secure تلقائياً)
      const { error: stripeError, paymentIntent } =
        await stripe.confirmCardPayment(intent.clientSecret, {
          payment_method: { card },
        });

      if (stripeError) {
        throw new Error(stripeError.message || "فشل الدفع");
      }
      if (!paymentIntent || paymentIntent.status !== "succeeded") {
        throw new Error(
          `حالة الدفع: ${paymentIntent?.status || "غير معروفة"}`
        );
      }

      // 3) سجل الاشتراك في الباك اند بعد نجاح الدفع
      try {
        await plansAPI.subscribe(plan.backendId);
      } catch (subErr) {
        // الدفع نجح. لو فشل تسجيل الاشتراك، نخبر المستخدم بدون فشل المعاملة.
        console.error("Subscribe after payment failed:", subErr);
        showToast(
          "warning",
          "نجح الدفع لكن تعذر تفعيل الاشتراك تلقائياً. تواصل مع الدعم."
        );
      }

      // 4) (اختياري) تأكيد حالة الدفع من الباك اند (يحدث عبر webhook)
      paymentAPI.getStatus(paymentIntent.id).catch(() => {});

      showToast("success", `تم الدفع بنجاح! تم تفعيل خطة ${plan.name}.`);
      onClose?.();
    } catch (err) {
      setError(err.message || "فشل الدفع");
    } finally {
      setSubmitting(false);
    }
  };

  const isFree = originalPrice <= 0;

  return (
    <form onSubmit={handlePay} style={{ padding: "30px 30px 28px" }}>
      {/* ---- رأس المودال ---- */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 18 }}>
        <h2 style={{ fontSize: "1.6rem", fontWeight: 900, color: "#1A1A2E", margin: 0 }}>
          إتمام الاشتراك
        </h2>
        <button
          type="button"
          onClick={onClose}
          aria-label="إغلاق"
          style={{
            border: "none", background: "transparent", cursor: "pointer",
            color: "#4A4A68", fontSize: "1.4rem", padding: 6,
          }}
        >
          <i className="fas fa-times" />
        </button>
      </div>

      {/* ---- ملخص الخطة ---- */}
      <div style={{
        background: "linear-gradient(180deg,rgba(108,99,255,0.06),white)",
        border: "1px solid rgba(108,99,255,0.2)",
        borderRadius: 16, padding: 18, marginBottom: 18,
      }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <div>
            <div style={{ fontWeight: 700, color: "#1A1A2E", fontSize: "1.1rem" }}>{plan.name}</div>
            {plan.desc && (
              <div style={{ color: "#4A4A68", fontSize: "0.9rem", marginTop: 4 }}>{plan.desc}</div>
            )}
          </div>
          <div style={{
            fontSize: "1.6rem", fontWeight: 900,
            background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
            WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent",
          }}>
            {fmtUSD(originalPrice)}
            <span style={{ fontSize: "0.85rem", WebkitTextFillColor: "#8A8AA8", fontWeight: 400 }}>
              {plan.period ? ` / ${plan.period}` : ""}
            </span>
          </div>
        </div>
      </div>

      {/* ---- كود الخصم ---- */}
      {!isFree && (
        <div style={{ marginBottom: 18 }}>
          <label style={{ display: "block", fontWeight: 700, color: "#1A1A2E", marginBottom: 8 }}>
            كود الخصم (اختياري)
          </label>
          <div style={{ display: "flex", gap: 8 }}>
            <input
              type="text"
              value={discountCode}
              onChange={(e) => setDiscountCode(e.target.value)}
              placeholder="أدخل كود الخصم"
              disabled={submitting || validating}
              style={{
                flex: 1, padding: "14px 16px",
                border: `2px solid ${appliedDiscount ? "#00C851" : "rgba(108,99,255,0.2)"}`,
                borderRadius: 12, fontSize: "1rem",
                fontFamily: "'Cairo', sans-serif",
                background: "#F8F9FC", outline: "none",
              }}
            />
            <button
              type="button"
              onClick={applyDiscount}
              disabled={submitting || validating || !discountCode.trim()}
              style={{
                padding: "0 22px", borderRadius: 12, border: "2px solid #6C63FF",
                background: "transparent", color: "#6C63FF", fontWeight: 700,
                cursor: validating ? "wait" : "pointer",
                fontFamily: "'Cairo', sans-serif", fontSize: "0.95rem",
                opacity: !discountCode.trim() ? 0.6 : 1,
              }}
            >
              {validating ? "..." : "تطبيق"}
            </button>
          </div>
          {appliedDiscount && (
            <div style={{ marginTop: 10, color: "#00A040", fontSize: "0.95rem", fontWeight: 600 }}>
              <i className="fas fa-check-circle" />{" "}
              تم تطبيق "{appliedDiscount.code}" — وفرت {fmtUSD(appliedDiscount.savedAmount)}
            </div>
          )}
        </div>
      )}

      {/* ---- حقل البطاقة ---- */}
      {!isFree && (
        <div style={{ marginBottom: 18 }}>
          <label style={{ display: "block", fontWeight: 700, color: "#1A1A2E", marginBottom: 8 }}>
            بيانات البطاقة
          </label>
          <div style={{
            padding: "16px 18px",
            border: "2px solid rgba(108,99,255,0.2)",
            borderRadius: 12,
            background: "#F8F9FC",
          }}>
            <CardElement options={CARD_OPTIONS} />
          </div>
          <div style={{ marginTop: 8, color: "#4A4A68", fontSize: "0.85rem" }}>
            <i className="fas fa-lock" /> الدفع آمن عبر Stripe — بطاقة الاختبار: <b>4242 4242 4242 4242</b>
          </div>
        </div>
      )}

      {/* ---- الإجمالي ---- */}
      <div style={{
        display: "flex", justifyContent: "space-between", alignItems: "center",
        padding: "14px 16px", background: "#F0F2F8", borderRadius: 12, marginBottom: 18,
      }}>
        <div style={{ color: "#4A4A68", fontWeight: 600 }}>الإجمالي</div>
        <div style={{
          fontWeight: 900, fontSize: "1.3rem",
          background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
          WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent",
        }}>
          {fmtUSD(finalPrice)}
        </div>
      </div>

      {error && (
        <div style={{
          color: "#E53935", background: "rgba(229,57,53,0.08)",
          padding: "10px 14px", borderRadius: 10, marginBottom: 14,
          fontSize: "0.95rem",
        }}>
          {error}
        </div>
      )}

      <button
        type="submit"
        disabled={submitting || (!isFree && (!stripe || !elements))}
        style={{
          width: "100%", padding: "16px 32px", borderRadius: 14,
          fontFamily: "'Cairo',sans-serif", fontSize: "1.05rem", fontWeight: 800,
          cursor: submitting ? "wait" : "pointer",
          display: "flex", alignItems: "center", justifyContent: "center", gap: 10,
          background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
          border: "none", boxShadow: "0 6px 20px rgba(108,99,255,0.35)",
          opacity: submitting ? 0.85 : 1,
        }}
      >
        {submitting ? (
          <>
            <i className="fas fa-spinner fa-spin" /> جاري المعالجة...
          </>
        ) : isFree ? (
          <>
            <i className="fas fa-check" /> تفعيل الخطة المجانية
          </>
        ) : (
          <>
            <i className="fas fa-lock" /> ادفع {fmtUSD(finalPrice)} الآن
          </>
        )}
      </button>
    </form>
  );
}
