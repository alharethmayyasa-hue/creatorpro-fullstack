import { useState, useEffect } from "react";

const navLinks = [
  { id: "home", icon: "fas fa-home", label: "الرئيسية" },
  { id: "features", icon: "fas fa-star", label: "الميزات" },
  { id: "testimonials", icon: "fas fa-users", label: "آراء المستخدمين" },
  { id: "pricing", icon: "fas fa-credit-card", label: "الاشتراك" },
];

export default function Header({ currentPage, navigate }) {
  const [scrolled, setScrolled] = useState(false);
  const [hidden, setHidden] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const [lastY, setLastY] = useState(0);

  useEffect(() => {
    const onScroll = () => {
      const y = window.scrollY;
      setScrolled(y > 50);
      setHidden(y > lastY && y > 100 && currentPage === "home");
      setLastY(y);
    };
    window.addEventListener("scroll", onScroll);
    return () => window.removeEventListener("scroll", onScroll);
  }, [lastY, currentPage]);

  return (
    <>
      <header style={{
        position: "fixed", top: 0, left: 0, right: 0, height: 80,
        background: scrolled ? "rgba(255,255,255,0.98)" : "rgba(255,255,255,0.95)",
        backdropFilter: "blur(20px)", borderBottom: "1px solid rgba(108,99,255,0.15)",
        zIndex: 1000, display: "flex", alignItems: "center",
        justifyContent: "space-between", padding: "0 5%",
        transform: hidden ? "translateY(-100%)" : "translateY(0)",
        transition: "all 0.3s ease",
        boxShadow: scrolled ? "0 5px 30px rgba(108,99,255,0.1)" : "none",
      }}>
        <a onClick={() => navigate("home")} style={{ cursor: "pointer", fontSize: "1.8rem", fontWeight: 800, color: "#1A1A2E", display: "flex", alignItems: "center", gap: 12, textDecoration: "none" }}>
          <i className="fas fa-bolt" style={{ fontSize: "2.2rem", background: "linear-gradient(135deg,#6C63FF,#00B8D4)", WebkitBackgroundClip: "text", WebkitTextFillColor: "transparent" }} />
          CreatorPro
        </a>

        {/* Desktop Nav */}
        <nav style={{ display: "flex", gap: 8, listStyle: "none" }}>
          {navLinks.map(link => (
            <button key={link.id} onClick={() => navigate(link.id)} style={{
              display: "flex", alignItems: "center", gap: 8, padding: "12px 20px",
              color: currentPage === link.id ? "#6C63FF" : "#4A4A68",
              background: currentPage === link.id ? "rgba(108,99,255,0.15)" : "transparent",
              border: "none", borderRadius: 10, cursor: "pointer",
              fontWeight: 600, fontSize: "1rem", fontFamily: "'Cairo',sans-serif",
              transition: "all 0.3s ease",
            }}>
              <i className={link.icon} />
              {link.label}
            </button>
          ))}
        </nav>

        <button onClick={() => navigate("login")} style={{
          padding: "12px 25px", borderRadius: 10, fontSize: "0.95rem", fontWeight: 700,
          cursor: "pointer", border: "none", display: "flex", alignItems: "center", gap: 8,
          background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
          boxShadow: "0 4px 15px rgba(108,99,255,0.3)", fontFamily: "'Cairo',sans-serif",
        }}>
          <i className="fas fa-sign-in-alt" /> تسجيل الدخول
        </button>

        {/* Mobile Toggle */}
        <button onClick={() => setMobileOpen(!mobileOpen)} style={{
          display: "none", flexDirection: "column", gap: 5, background: "none",
          border: "none", cursor: "pointer", padding: 10,
        }} className="mobile-toggle">
          <span /><span /><span />
        </button>
      </header>

      {/* Mobile Nav */}
      {mobileOpen && (
        <div style={{
          position: "fixed", top: 80, left: 0, right: 0, background: "rgba(255,255,255,0.98)",
          backdropFilter: "blur(20px)", borderBottom: "1px solid rgba(108,99,255,0.15)",
          padding: 20, zIndex: 999,
        }}>
          {navLinks.map(link => (
            <button key={link.id} onClick={() => { navigate(link.id); setMobileOpen(false); }} style={{
              display: "flex", alignItems: "center", gap: 12, padding: "15px 20px",
              width: "100%", color: "#4A4A68", background: "transparent",
              border: "none", borderRadius: 10, cursor: "pointer",
              fontWeight: 600, fontSize: "1rem", fontFamily: "'Cairo',sans-serif",
              marginBottom: 8, textAlign: "right",
            }}>
              <i className={link.icon} /> {link.label}
            </button>
          ))}
          <button onClick={() => { navigate("login"); setMobileOpen(false); }} style={{
            width: "100%", padding: "15px 20px", borderRadius: 10, border: "none",
            background: "linear-gradient(135deg,#6C63FF,#00B8D4)", color: "white",
            fontWeight: 700, cursor: "pointer", fontFamily: "'Cairo',sans-serif",
            marginTop: 10, fontSize: "1rem",
          }}>
            <i className="fas fa-sign-in-alt" /> تسجيل الدخول
          </button>
        </div>
      )}
    </>
  );
}
