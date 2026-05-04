import { useState } from "react";

const FEATURES = [
  { icon: "fas fa-magic", title: "توليد بالذكاء الاصطناعي", short: "توليد سكريبتات ذكية واحترافية في ثوانٍ", full: "أنشئ سكريبتات فيديو ومنشورات احترافية في ثوانٍ باستخدام أحدث تقنيات الذكاء الاصطناعي المتقدمة." },
  { icon: "fas fa-video", title: "تحليل الفيديو", short: "تحليل شامل لأداء فيديوهاتك", full: "ارفع فيديوهاتك واحصل على تحليل شامل واقتراحات دقيقة للتحسين لزيادة التفاعل." },
  { icon: "fas fa-share-alt", title: "نشر تلقائي", short: "نشر على جميع المنصات بضغطة زر", full: "انشر محتواك على يوتيوب، فيسبوك، إنستغرام وتيك توك بضغطة زر واحدة." },
  { icon: "fas fa-comments", title: "إدارة التعليقات", short: "ردود تلقائية ذكية للجمهور", full: "رد تلقائي ذكي على التعليقات مع إمكانية تخصيص الردود لتوفير وقتك." },
  { icon: "fas fa-chart-line", title: "إحصائيات شاملة", short: "لوحة تحكم دقيقة للأداء", full: "تابع أداء محتواك مع تحليلات مفصلة لكل منصة لمعرفة ما ينجح مع جمهورك." },
  { icon: "fas fa-newspaper", title: "أخبار الصناعة", short: "تابع آخر صيحات المحتوى", full: "تابع آخر أخبار ومنصات صناعة المحتوى والذكاء الاصطناعي لتبقى في الصدارة." },
];

function FeatureCard({ feature, onAuthRequired }) {
  const [locked, setLocked] = useState(false);

  return (
    <div
      onClick={() => setLocked(!locked)}
      style={{
        background: "white", border: "1px solid rgba(108,99,255,0.15)", borderRadius: 24,
        padding: 40, cursor: "pointer", display: "flex", flexDirection: "column",
        alignItems: "center", textAlign: "center", minHeight: 300, position: "relative",
        boxShadow: "0 10px 40px rgba(108,99,255,0.1)",
        transition: "all 0.4s cubic-bezier(0.175,0.885,0.32,1.275)",
        overflow: "hidden",
      }}
    >
      <div style={{
        width: 80, height: 80, borderRadius: 20, display: "flex", alignItems: "center",
        justifyContent: "center", fontSize: "2.2rem", marginBottom: 25,
        background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
        boxShadow: "0 10px 20px rgba(108,99,255,0.3)",
      }}>
        <i className={feature.icon} />
      </div>

      <h3 style={{ fontSize: "1.5rem", fontWeight: 700, marginBottom: 15, color: "#1A1A2E" }}>{feature.title}</h3>
      <p style={{ color: "#4A4A68", lineHeight: 1.6, fontWeight: 600 }}>{feature.short}</p>

      {/* Locked Overlay */}
      {locked && (
        <div style={{
          position: "absolute", inset: 0, background: "white",
          display: "flex", flexDirection: "column", alignItems: "center",
          justifyContent: "center", padding: 20, zIndex: 10,
        }}>
          <div style={{
            background: "linear-gradient(135deg,rgba(108,99,255,0.1),rgba(0,184,212,0.1))",
            borderRadius: 16, padding: 25, width: "100%",
            border: "1px solid rgba(108,99,255,0.15)", textAlign: "center",
          }}>
            <div style={{ fontSize: "2.5rem", color: "#6C63FF", marginBottom: 15 }}>🔒</div>
            <h4 style={{ fontSize: "1.1rem", fontWeight: 700, color: "#1A1A2E", marginBottom: 10 }}>ميزة حصرية!</h4>
            <p style={{ color: "#4A4A68", fontSize: "0.9rem", lineHeight: 1.6, marginBottom: 20 }}>هذه الميزة حصرية — لتجربتها سجل دخولك</p>
            <button
              onClick={(e) => { e.stopPropagation(); onAuthRequired(); }}
              style={{
                background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
                padding: "12px 25px", borderRadius: 12, border: "none", cursor: "pointer",
                fontWeight: 600, fontSize: "0.9rem", fontFamily: "'Cairo',sans-serif",
                display: "flex", alignItems: "center", gap: 8, margin: "0 auto",
              }}>
              <i className="fas fa-bolt" /> ابدأ مجاناً
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export default function FeaturesPage({ navigate }) {
  return (
    <>
      {/* Features Grid */}
      <section style={{ padding: "100px 5%" }}>
        <div style={{ textAlign: "center", marginBottom: 70 }}>
          <h2 style={{ fontSize: "2.8rem", fontWeight: 800, marginBottom: 15, color: "#1A1A2E" }}>
            كل ما تحتاجه في <span style={{ background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }}>مكان واحد</span>
          </h2>
          <p style={{ color: "#4A4A68", fontSize: "1.2rem" }}>اضغط على أي ميزة لمعرفة المزيد</p>
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit,minmax(300px,1fr))", gap: 35, maxWidth: 1300, margin: "0 auto" }}>
          {FEATURES.map((f, i) => <FeatureCard key={i} feature={f} onAuthRequired={() => navigate("login")} />)}
        </div>
      </section>

      {/* Dashboard Mockup Section */}
      <section style={{ padding: "100px 5%", background: "#F0F2F8", display: "flex", alignItems: "center", justifyContent: "space-between", gap: "4rem", flexWrap: "wrap" }}>
        {/* Mockup */}
        <div style={{
          flex: 1, minWidth: 340, background: "#1e293b", borderRadius: 20,
          padding: "2rem", color: "white", boxShadow: "0 20px 50px rgba(0,0,0,0.2)",
        }}>
          <div style={{ display: "flex", gap: 8, marginBottom: "2rem" }}>
            {["#ef4444","#f59e0b","#10b981"].map(c => <div key={c} style={{ width: 12, height: 12, borderRadius: "50%", background: c }} />)}
          </div>
          <div style={{ textAlign: "center", marginBottom: "2rem", opacity: 0.8, fontSize: "0.9rem" }}>CreatorPro Dashboard</div>
          {[
            { label: "توليد نص احترافي", width: "100%", color: "#10b981", status: "مكتمل" },
            { label: "إنشاء فيديو قصير", width: "60%", color: "#6366f1", status: "جاري..." },
            { label: "جدولة 5 منشورات", width: "40%", color: "#06b6d4", status: "مجدول" },
          ].map(({ label, width, color, status }) => (
            <div key={label} style={{ marginBottom: "1.5rem" }}>
              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: "0.5rem", fontSize: "0.9rem" }}>
                <span>{label}</span><span style={{ color }}>{status}</span>
              </div>
              <div style={{ height: 8, background: "rgba(255,255,255,0.1)", borderRadius: 4, overflow: "hidden" }}>
                <div style={{ width, height: "100%", background: color, borderRadius: 4 }} />
              </div>
            </div>
          ))}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "1rem", marginTop: "2rem" }}>
            {[["98%","رضا"],["★4.9","تقييم"],["127","محتوى"]].map(([v, l]) => (
              <div key={l} style={{ background: "rgba(255,255,255,0.05)", padding: "1rem", borderRadius: 12, textAlign: "center" }}>
                <div style={{ fontSize: "1.5rem", fontWeight: 700, marginBottom: "0.2rem" }}>{v}</div>
                <div style={{ fontSize: "0.8rem", opacity: 0.7 }}>{l}</div>
              </div>
            ))}
          </div>
          <div style={{ textAlign: "center", marginTop: "1.5rem" }}>
            <span style={{ background: "#10b981", padding: "5px 15px", borderRadius: 20, fontSize: "0.8rem" }}>
              تم النشر بنجاح ✓
            </span>
          </div>
        </div>

        {/* Feature List */}
        <div style={{ flex: 1, minWidth: 300 }}>
          <div style={{ marginBottom: "2rem" }}>
            <h2 style={{ fontSize: "2rem", fontWeight: 800, marginBottom: "0.5rem", color: "#1A1A2E" }}>أدوات ذكية لنتائج مضاعفة</h2>
            <p style={{ color: "#4A4A68" }}>كل ما تحتاجه لصناعة محتوى رقمي ناجح في منصة واحدة.</p>
          </div>
          {[
            { icon: "fa-wand-magic-sparkles", title: "إنشاء ذكي", desc: "اكتب فكرتك وسيتولى الذكاء الاصطناعي إنتاج محتوى احترافي في ثوانٍ معدودة" },
            { icon: "fa-video", title: "إنشاء فيديو", desc: "حول النصوص إلى مقاطع فيديو جذابة مع رسوم متحركة وموسيقى تلقائية" },
            { icon: "fa-chart-pie", title: "تحليلات متقدمة", desc: "احصل على رؤى عميقة حول أداء محتواك واقتراحات للتحسين" },
          ].map(({ icon, title, desc }) => (
            <div key={title} style={{
              background: "white", padding: "1.5rem", borderRadius: 16, marginBottom: "1.5rem",
              display: "flex", alignItems: "center", gap: "1.5rem",
              boxShadow: "0 4px 20px rgba(0,0,0,0.05)", transition: "transform 0.3s ease",
            }}>
              <div style={{
                width: 50, height: 50, background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
                borderRadius: 12, display: "flex", alignItems: "center", justifyContent: "center",
                color: "white", fontSize: "1.2rem", flexShrink: 0,
              }}>
                <i className={`fas ${icon}`} />
              </div>
              <div>
                <h3 style={{ fontSize: "1.1rem", marginBottom: "0.3rem", color: "#1A1A2E", fontWeight: 700 }}>{title}</h3>
                <p style={{ fontSize: "0.9rem", color: "#64748b" }}>{desc}</p>
              </div>
            </div>
          ))}
        </div>
      </section>
    </>
  );
}
