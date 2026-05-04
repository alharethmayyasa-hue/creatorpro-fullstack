import { useState, useEffect } from "react";
import { reviewsAPI } from "../services/api";

const FALLBACK_REVIEWS = [
  { id: 1, name: "أحمد محمد", role: "صانع محتوى يوتيوب • 150K مشترك", rating: 5, text: "منصة غيرت طريقة عملي تماماً! أصبحت أنتج محتوى 3 أضعاف في نفس الوقت. الذكاء الاصطناعي يفهم تماماً ما أريده.", date: "يناير 2024", platform: "YouTube", platformIcon: "fab fa-youtube", initials: "أ", color: "#6C63FF" },
  { id: 2, name: "سارة علي", role: "مسوقة رقمية • وكالة إعلانية", rating: 4, text: "النشر التلقائي وفر علي ساعات من العمل كل أسبوع. أستطيع جدولة محتوى لأسابيع كاملة في جلسة واحدة.", date: "فبراير 2024", platform: "Instagram", platformIcon: "fab fa-instagram", initials: "س", color: "#E6683C" },
  { id: 3, name: "خالد عمر", role: "مؤثر على إنستغرام • 500K متابع", rating: 4, text: "تحليل الفيديو ساعدني في تحسين محتواي بشكل كبير. فيديوهاتي تحصل على تفاعل أعلى بنسبة 200% بعد تطبيق الاقتراحات.", date: "مارس 2024", platform: "TikTok", platformIcon: "fab fa-tiktok", initials: "خ", color: "#00C851" },
  { id: 4, name: "نورة السعيد", role: "مدونة تقنية • 80K متابع", rating: 5, text: "إدارة التعليقات أصبحت أسهل بكثير. الردود التلقائية توفر لي الوقت وأستطيع التركيز على إنشاء محتوى جديد.", date: "أبريل 2024", platform: "Twitter", platformIcon: "fab fa-twitter", initials: "ن", color: "#1DA1F2" },
];

function Stars({ count }) {
  return (
    <div style={{ display: "flex", gap: 6 }}>
      {[1,2,3,4,5].map(i => (
        <i key={i} className="fas fa-star" style={{ color: i <= count ? "#FFB800" : "#e2e8f0", fontSize: "1.2rem" }} />
      ))}
    </div>
  );
}

export default function TestimonialsPage({ navigate, showToast }) {
  const [reviews, setReviews] = useState(FALLBACK_REVIEWS);
  const [current, setCurrent] = useState(0);
  const [form, setForm] = useState({ name: "", email: "", rating: 0, text: "" });
  const [submitting, setSubmitting] = useState(false);
  const [hoverRating, setHoverRating] = useState(0);

  // ---- Fetch reviews from API (uncomment when ready) ----
  // useEffect(() => {
  //   reviewsAPI.getAll().then(setReviews).catch(console.error);
  // }, []);

  const visibleCount = window.innerWidth >= 1200 ? 3 : window.innerWidth >= 768 ? 2 : 1;
  const maxIndex = Math.max(0, reviews.length - visibleCount);

  const move = (dir) => setCurrent(c => Math.max(0, Math.min(maxIndex, c + dir)));

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.rating) { showToast("error", "الرجاء اختيار تقييم"); return; }
    setSubmitting(true);
    try {
      // ---- Uncomment when backend is ready ----
      // await reviewsAPI.submit(form);
      showToast("success", "شكراً لك! تم إرسال رأيك بنجاح");
      setForm({ name: "", email: "", rating: 0, text: "" });
    } catch (err) {
      showToast("error", err.message || "حدث خطأ، حاول مرة أخرى");
    } finally {
      setSubmitting(false);
    }
  };

  const inputStyle = {
    width: "100%", padding: "16px 20px", border: "1px solid rgba(108,99,255,0.2)",
    borderRadius: 14, fontSize: "1rem", fontFamily: "'Cairo',sans-serif",
    color: "#1A1A2E", background: "#F8F9FC", outline: "none",
  };

  const offset = current * (100 / visibleCount);

  return (
    <section style={{ padding: "100px 5%" }}>
      <div style={{ textAlign: "center", marginBottom: 70 }}>
        <h2 style={{ fontSize: "2.8rem", fontWeight: 800, marginBottom: 15, color: "#1A1A2E" }}>
          ماذا يقول <span style={{ background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }}>مستخدمونا؟</span>
        </h2>
        <p style={{ color: "#4A4A68", fontSize: "1.2rem" }}>قيم تجربتك وشاركنا رأيك</p>
      </div>

      {/* Slider */}
      <div style={{ maxWidth: 1200, margin: "0 auto", overflow: "hidden" }}>
        <div style={{
          display: "flex", gap: 30, transition: "transform 0.5s cubic-bezier(0.4,0,0.2,1)",
          transform: `translateX(${offset}%)`,
        }}>
          {reviews.map(r => (
            <div key={r.id} style={{
              minWidth: `calc(${100 / visibleCount}% - ${(visibleCount - 1) * 30 / visibleCount}px)`,
              background: "white", border: "1px solid rgba(108,99,255,0.15)", borderRadius: 24,
              padding: 40, boxShadow: "0 10px 40px rgba(108,99,255,0.1)",
              transition: "all 0.4s ease", position: "relative",
            }}>
              <div style={{ position: "absolute", top: 30, right: 40, fontSize: "5rem", color: "#6C63FF", opacity: 0.1, fontFamily: "Georgia,serif", lineHeight: 1 }}>"</div>
              <div style={{ display: "flex", alignItems: "center", gap: 20, marginBottom: 20 }}>
                <div style={{ width: 70, height: 70, borderRadius: "50%", background: r.color, display: "flex", alignItems: "center", justifyContent: "center", fontSize: "1.5rem", fontWeight: 700, color: "white", flexShrink: 0 }}>
                  {r.initials}
                </div>
                <div>
                  <h4 style={{ fontSize: "1.1rem", color: "#1A1A2E", marginBottom: 5, fontWeight: 700 }}>{r.name}</h4>
                  <p style={{ fontSize: "0.85rem", color: "#8A8AA8" }}>{r.role}</p>
                </div>
              </div>
              <Stars count={r.rating} />
              <p style={{ color: "#4A4A68", lineHeight: 2, fontStyle: "italic", margin: "20px 0", position: "relative", zIndex: 1 }}>"{r.text}"</p>
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", paddingTop: 20, borderTop: "1px solid rgba(108,99,255,0.1)" }}>
                <span style={{ fontSize: "0.85rem", color: "#8A8AA8" }}>
                  <i className="far fa-calendar" style={{ marginLeft: 6 }} />{r.date}
                </span>
                <span style={{ fontSize: "0.85rem", color: "#6C63FF", display: "flex", alignItems: "center", gap: 6 }}>
                  <i className={r.platformIcon} style={{ fontSize: "1.1rem" }} />{r.platform}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Nav */}
      <div style={{ display: "flex", justifyContent: "center", alignItems: "center", gap: 20, marginTop: 40 }}>
        <button onClick={() => move(-1)} style={{ width: 50, height: 50, borderRadius: "50%", background: "white", border: "2px solid rgba(108,99,255,0.2)", cursor: "pointer", color: "#4A4A68", fontSize: "1.2rem", display: "flex", alignItems: "center", justifyContent: "center", transition: "all 0.3s" }}>
          <i className="fas fa-chevron-right" />
        </button>
        <div style={{ display: "flex", gap: 10 }}>
          {reviews.map((_, i) => (
            <div key={i} onClick={() => setCurrent(Math.min(i, maxIndex))} style={{ width: 12, height: 12, borderRadius: "50%", background: i === current ? "#6C63FF" : "rgba(108,99,255,0.2)", cursor: "pointer", transition: "all 0.3s", transform: i === current ? "scale(1.2)" : "scale(1)" }} />
          ))}
        </div>
        <button onClick={() => move(1)} style={{ width: 50, height: 50, borderRadius: "50%", background: "white", border: "2px solid rgba(108,99,255,0.2)", cursor: "pointer", color: "#4A4A68", fontSize: "1.2rem", display: "flex", alignItems: "center", justifyContent: "center", transition: "all 0.3s" }}>
          <i className="fas fa-chevron-left" />
        </button>
      </div>

      {/* Review Form */}
      <div style={{
        maxWidth: 800, margin: "80px auto 0",
        background: "linear-gradient(135deg,rgba(108,99,255,0.05),white)",
        border: "2px solid rgba(108,99,255,0.15)", borderRadius: 30, padding: "60px 40px",
        boxShadow: "0 20px 60px rgba(108,99,255,0.1)", textAlign: "center",
      }}>
        <h3 style={{ fontSize: "2rem", fontWeight: 800, color: "#1A1A2E", marginBottom: 15 }}>
          <i className="fas fa-pen-fancy" style={{ marginLeft: 10, color: "#6C63FF" }} /> شاركنا تجربتك
        </h3>
        <p style={{ color: "#4A4A68", marginBottom: 40 }}>انضم إلى مئات المستخدمين الراضين وشاركنا رأيك</p>

        <form onSubmit={handleSubmit} style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20, textAlign: "right" }}>
          <div>
            <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.9rem" }}>الاسم الكامل</label>
            <input value={form.name} onChange={e => setForm({...form, name: e.target.value})} placeholder="اسمك الكريم" required style={inputStyle} />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.9rem" }}>البريد الإلكتروني</label>
            <input type="email" value={form.email} onChange={e => setForm({...form, email: e.target.value})} placeholder="example@email.com" required style={inputStyle} />
          </div>

          {/* Star Rating */}
          <div style={{ gridColumn: "span 2", textAlign: "center" }}>
            <label style={{ display: "block", marginBottom: 15, color: "#4A4A68", fontWeight: 700, fontSize: "0.9rem" }}>قيّم تجربتك</label>
            <div style={{ display: "flex", gap: 12, justifyContent: "center" }}>
              {[1,2,3,4,5].map(i => (
                <i key={i} className="fas fa-star"
                  onMouseEnter={() => setHoverRating(i)}
                  onMouseLeave={() => setHoverRating(0)}
                  onClick={() => setForm({...form, rating: i})}
                  style={{ fontSize: "2.2rem", cursor: "pointer", color: i <= (hoverRating || form.rating) ? "#FFB800" : "#e2e8f0", transition: "all 0.3s", transform: i <= (hoverRating || form.rating) ? "scale(1.1)" : "scale(1)" }}
                />
              ))}
            </div>
          </div>

          <div style={{ gridColumn: "span 2" }}>
            <label style={{ display: "block", marginBottom: 10, color: "#4A4A68", fontWeight: 700, fontSize: "0.9rem" }}>رأيك</label>
            <textarea value={form.text} onChange={e => setForm({...form, text: e.target.value})} placeholder="اكتب رأيك عن المنصة..." required rows={5} style={{ ...inputStyle, resize: "vertical" }} />
          </div>

          <div style={{ gridColumn: "span 2" }}>
            <button type="submit" disabled={submitting} style={{
              width: "100%", padding: "18px 32px", borderRadius: 14, border: "none",
              background: submitting ? "#ccc" : "linear-gradient(135deg,#6C63FF,#00B8D4)",
              color: "white", fontSize: "1rem", fontWeight: 700, cursor: submitting ? "not-allowed" : "pointer",
              fontFamily: "'Cairo',sans-serif", display: "flex", alignItems: "center", justifyContent: "center", gap: 10,
            }}>
              {submitting ? <><i className="fas fa-spinner fa-spin" /> جاري الإرسال...</> : <><i className="fas fa-paper-plane" /> إرسال الرأي</>}
            </button>
          </div>
        </form>
      </div>
    </section>
  );
}
