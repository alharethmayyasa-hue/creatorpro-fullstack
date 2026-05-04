import { useEffect, useRef, useState } from "react";
import { statsAPI } from "../services/api";

// ---- Static fallback data (يُستبدل بـ API) ----
const FALLBACK_STATS = [
  { icon: "fas fa-users", target: 52000, label: "مستخدم نشط", change: "+15% هذا الشهر", suffix: "" },
  { icon: "fas fa-file-video", target: 1200000, label: "محتوى مولد", change: "+28% هذا الشهر", suffix: "" },
  { icon: "fas fa-heart", target: 98, label: "نسبة الرضا", change: "+3% هذا الشهر", suffix: "%" },
  { icon: "fas fa-globe", target: 45, label: "دولة حول العالم", change: "+8 دول جديدة", suffix: "" },
];

const FALLBACK_BARS = [
  { label: "يناير", height: 40 }, { label: "فبراير", height: 55 },
  { label: "مارس", height: 65 }, { label: "أبريل", height: 75 },
  { label: "مايو", height: 85 }, { label: "يونيو", height: 100 },
];

const PIE_DATA = [
  { label: "YouTube", percent: "45%", color: "#6C63FF" },
  { label: "Instagram", percent: "30%", color: "#00B8D4" },
  { label: "Twitter", percent: "15%", color: "#00C851" },
  { label: "TikTok", percent: "10%", color: "#FF8800" },
];

function formatNumber(n) {
  if (n >= 1000000) return "+" + (n / 1000000).toFixed(1) + "M";
  if (n >= 1000) return "+" + Math.round(n / 1000) + "K";
  return String(n);
}

function StatCard({ icon, target, label, change, suffix }) {
  const [count, setCount] = useState(0);
  useEffect(() => {
    let start = 0;
    const step = target / 120;
    const timer = setInterval(() => {
      start += step;
      if (start >= target) { setCount(target); clearInterval(timer); }
      else setCount(Math.floor(start));
    }, 16);
    return () => clearInterval(timer);
  }, [target]);

  return (
    <div style={{
      background: "white", border: "1px solid rgba(108,99,255,0.15)", borderRadius: 20,
      padding: "35px 25px", textAlign: "center",
      boxShadow: "0 10px 40px rgba(108,99,255,0.1)",
      transition: "all 0.4s ease",
    }}>
      <div style={{
        width: 70, height: 70, borderRadius: 18, display: "flex", alignItems: "center",
        justifyContent: "center", fontSize: "2rem", margin: "0 auto 20px",
        background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
        boxShadow: "0 10px 25px rgba(108,99,255,0.3)",
      }}>
        <i className={icon} />
      </div>
      <div style={{
        fontSize: "3rem", fontWeight: 900,
        background: "linear-gradient(135deg,#6C63FF,#00B8D4)",
        WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent", marginBottom: 10,
      }}>
        {formatNumber(count)}{suffix}
      </div>
      <div style={{ color: "#4A4A68", fontSize: "1rem", fontWeight: 600, marginBottom: 10 }}>{label}</div>
      <span style={{
        display: "inline-block", padding: "5px 15px", borderRadius: 20, fontSize: "0.85rem",
        fontWeight: 700, background: "rgba(0,200,81,0.15)", color: "#00C851",
      }}>
        <i className="fas fa-arrow-up" /> {change}
      </span>
    </div>
  );
}

function BarChart({ bars }) {
  const [animated, setAnimated] = useState(false);
  useEffect(() => {
    const t = setTimeout(() => setAnimated(true), 300);
    return () => clearTimeout(t);
  }, []);

  return (
    <div style={{ display: "flex", alignItems: "flex-end", justifyContent: "space-between", height: 260, padding: "30px 20px 20px", gap: 20 }}>
      {bars.map((bar, i) => (
        <div key={i} style={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 12, flex: 1 }}>
          <div style={{ width: "100%", height: "100%", display: "flex", alignItems: "flex-end", justifyContent: "center" }}>
            <div style={{
              width: "70%", borderRadius: "10px 10px 6px 6px",
              background: i < 3 ? "linear-gradient(180deg,#8b83ff,#6C63FF)" : "linear-gradient(180deg,#00d4ff,#00B8D4)",
              height: animated ? `${bar.height}%` : "0%",
              transition: `height 0.8s cubic-bezier(0.34,1.56,0.64,1) ${i * 0.15}s`,
              boxShadow: "0 4px 15px rgba(108,99,255,0.25)",
              opacity: animated ? 1 : 0,
              minHeight: 4,
            }} />
          </div>
          <div style={{
            fontSize: "0.85rem", fontWeight: 700, color: "#4A4A68",
            padding: "6px 10px", background: "#F0F2F8", borderRadius: 8,
          }}>{bar.label}</div>
        </div>
      ))}
    </div>
  );
}

export default function HomePage({ navigate }) {
  const [stats, setStats] = useState(FALLBACK_STATS);
  const [bars, setBars] = useState(FALLBACK_BARS);
  const snowRef = useRef(null);

  // ---- Snow ----
  useEffect(() => {
    const container = snowRef.current;
    if (!container) return;
    for (let i = 0; i < 25; i++) {
      const flake = document.createElement("div");
      const size = Math.random() * 8 + 4;
      Object.assign(flake.style, {
        position: "absolute", bottom: "-20px",
        background: "linear-gradient(135deg,rgba(108,99,255,0.3),rgba(0,184,212,0.3))",
        borderRadius: "50%", opacity: "0.4",
        width: `${size}px`, height: `${size}px`,
        left: `${Math.random() * 100}%`,
        animation: `snowfall ${Math.random() * 10 + 15}s linear ${Math.random() * 10}s infinite`,
      });
      container.appendChild(flake);
    }
  }, []);

  // ---- Fetch from API (uncomment when backend is ready) ----
  // useEffect(() => {
  //   statsAPI.getPlatformStats().then(data => {
  //     setStats(data); // map data to FALLBACK_STATS shape
  //   }).catch(console.error);
  //   statsAPI.getGrowthData().then(data => setBars(data)).catch(console.error);
  // }, []);

  const card = { background: "white", border: "2px solid rgba(108,99,255,0.15)", borderRadius: 20, padding: 30, boxShadow: "0 10px 40px rgba(108,99,255,0.1)" };

  return (
    <>
      <style>{`
        @keyframes snowfall {
          0% { transform: translateY(0) rotate(0deg); opacity: 0; }
          10% { opacity: 0.4; }
          90% { opacity: 0.4; }
          100% { transform: translateY(-100vh) rotate(720deg) translateX(50px); opacity: 0; }
        }
      `}</style>

      {/* Snow container */}
      <div ref={snowRef} style={{ position: "fixed", top: 0, left: 0, width: "100%", height: "100%", pointerEvents: "none", zIndex: 0, overflow: "hidden" }} />

      {/* Hero */}
      <section style={{
        position: "relative", height: "85vh", minHeight: 600, overflow: "hidden",
        background: "linear-gradient(135deg,#1a1a2e,#16213e)", display: "flex", alignItems: "center", justifyContent: "center",
      }}>
        <div style={{
          position: "absolute", inset: 0,
          background: "linear-gradient(to bottom, rgba(26,26,46,0.4) 0%, rgba(26,26,46,0.5) 50%, #F8F9FC 100%)",
          zIndex: 2,
        }} />
        <div style={{ position: "relative", zIndex: 3, textAlign: "center", padding: "0 20px", maxWidth: 900 }}>
          <h1 style={{ fontSize: "clamp(2rem,5vw,3.5rem)", fontWeight: 900, color: "white", lineHeight: 1.2, marginBottom: 20, textShadow: "0 4px 30px rgba(0,0,0,0.3)" }}>
            اصنع محتوى{" "}
            <span style={{ background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }}>
              احترافي
            </span>
            <br />في ثوانٍ معدودة
          </h1>
          <p style={{ fontSize: "1.2rem", color: "rgba(255,255,255,0.9)", maxWidth: 700, margin: "0 auto 30px", lineHeight: 1.8 }}>
            منصة متكاملة لصناع المحتوى تستخدم الذكاء الاصطناعي لتوليد سكريبتات، تحليل فيديوهات، ونشر تلقائي على جميع منصات التواصل
          </p>
          <button onClick={() => navigate("pricing")} style={{
            padding: "18px 45px", borderRadius: 14, border: "none",
            background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
            fontSize: "1.1rem", fontWeight: 700, cursor: "pointer",
            boxShadow: "0 8px 25px rgba(108,99,255,0.4)",
            fontFamily: "'Cairo',sans-serif", display: "inline-flex", alignItems: "center", gap: 10,
          }}>
            <i className="fas fa-rocket" /> ابدأ الآن مجاناً
          </button>
        </div>
      </section>

      {/* Stats */}
      <section style={{ padding: "80px 5%", background: "white" }}>
        <div style={{ textAlign: "center", marginBottom: 60 }}>
          <h2 style={{ fontSize: "2.8rem", fontWeight: 800, marginBottom: 15, color: "#1A1A2E" }}>
            إحصائيات <span style={{ background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }}>المنصة</span>
          </h2>
          <p style={{ color: "#4A4A68", fontSize: "1.2rem" }}>نمو مستمر وثقة متزايدة من صناع المحتوى</p>
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit,minmax(230px,1fr))", gap: 30, maxWidth: 1200, margin: "0 auto 60px" }}>
          {stats.map((s, i) => <StatCard key={i} {...s} />)}
        </div>

        {/* Charts */}
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit,minmax(450px,1fr))", gap: 30, maxWidth: 1200, margin: "0 auto" }}>
          {/* Bar Chart */}
          <div style={card}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20, paddingBottom: 15, borderBottom: "1px solid rgba(108,99,255,0.15)" }}>
              <h3 style={{ fontWeight: 700, color: "#1A1A2E", display: "flex", alignItems: "center", gap: 10 }}>
                <i className="fas fa-chart-bar" style={{ color: "#6C63FF" }} /> النمو الشهري
              </h3>
              <div style={{ display: "flex", gap: 15 }}>
                {[["#6C63FF","المستخدمين"],["#00B8D4","المحتوى"]].map(([c,l]) => (
                  <div key={l} style={{ display: "flex", alignItems: "center", gap: 8, fontSize: "0.85rem", color: "#4A4A68" }}>
                    <div style={{ width: 12, height: 12, borderRadius: "50%", background: c }} />{l}
                  </div>
                ))}
              </div>
            </div>
            <BarChart bars={bars} />
          </div>

          {/* Pie */}
          <div style={card}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20, paddingBottom: 15, borderBottom: "1px solid rgba(108,99,255,0.15)" }}>
              <h3 style={{ fontWeight: 700, color: "#1A1A2E", display: "flex", alignItems: "center", gap: 10 }}>
                <i className="fas fa-chart-pie" style={{ color: "#6C63FF" }} /> توزيع المنصات
              </h3>
            </div>
            <div style={{ display: "flex", alignItems: "center", gap: 30, padding: "10px 0" }}>
              <div style={{
                width: 180, height: 180, borderRadius: "50%", flexShrink: 0,
                background: "conic-gradient(#6C63FF 0deg 162deg,#00B8D4 162deg 270deg,#00C851 270deg 324deg,#FF8800 324deg 360deg)",
                position: "relative", boxShadow: "0 10px 30px rgba(108,99,255,0.2)",
              }}>
                <div style={{ position: "absolute", top: "50%", left: "50%", transform: "translate(-50%,-50%)", width: 110, height: 110, background: "white", borderRadius: "50%", boxShadow: "0 4px 15px rgba(0,0,0,0.1)" }} />
              </div>
              <div style={{ flex: 1 }}>
                {PIE_DATA.map(({ label, percent, color }) => (
                  <div key={label} style={{
                    display: "flex", alignItems: "center", gap: 12, marginBottom: 12,
                    padding: "12px 15px", background: "#F0F2F8", borderRadius: 12,
                    cursor: "pointer", transition: "all 0.3s ease",
                  }}>
                    <div style={{ width: 16, height: 16, borderRadius: 4, background: color, flexShrink: 0 }} />
                    <div style={{ flex: 1 }}>
                      <div style={{ fontWeight: 700, color: "#1A1A2E", fontSize: "0.9rem" }}>{label}</div>
                      <div style={{ fontSize: "0.8rem", color: "#4A4A68" }}>{percent} من المحتوى</div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
}
