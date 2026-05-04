export default function Toast({ toast }) {
  if (!toast.show) return null;
  const icons = { success: "fas fa-check-circle", error: "fas fa-exclamation-circle", info: "fas fa-info-circle" };
  const colors = { success: "#00C851", error: "#ff4444", info: "#6C63FF" };

  return (
    <div style={{
      position: "fixed", bottom: 30, left: 30, background: "white",
      border: "1px solid rgba(108,99,255,0.15)",
      borderRight: `4px solid ${colors[toast.type]}`,
      borderRadius: 16, padding: "20px 28px",
      display: "flex", alignItems: "center", gap: 15,
      boxShadow: "0 20px 60px rgba(108,99,255,0.15)",
      zIndex: 2000, maxWidth: 400, animation: "slideIn 0.4s ease",
      fontFamily: "'Cairo',sans-serif",
    }}>
      <i className={icons[toast.type]} style={{ fontSize: "1.4rem", color: colors[toast.type] }} />
      <span style={{ color: "#1A1A2E", fontWeight: 600 }}>{toast.message}</span>
    </div>
  );
}
