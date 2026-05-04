export default function Footer({ navigate }) {
  return (
    <footer style={{ background: "#0f172a", color: "white", padding: "80px 5% 40px", fontFamily: "'Cairo',sans-serif" }}>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit,minmax(200px,1fr))", gap: "3rem", marginBottom: "4rem" }}>
        <div>
          <div style={{ fontSize: "1.8rem", fontWeight: 800, marginBottom: "1rem", display: "flex", alignItems: "center", gap: 10 }}>
            CreatorPro <i className="fas fa-bolt" style={{ color: "#00B8D4" }} />
          </div>
          <p style={{ color: "#94a3b8", lineHeight: 1.8 }}>المنصة الأولى عربياً المعتمدة على الذكاء الاصطناعي لمساعدة صناع المحتوى.</p>
          <div style={{ display: "flex", gap: "1rem", marginTop: "1.5rem" }}>
            {["linkedin-in","youtube","instagram","twitter"].map(icon => (
              <a key={icon} href="#" style={{ width: 40, height: 40, background: "rgba(255,255,255,0.1)", borderRadius: "50%", display: "flex", alignItems: "center", justifyContent: "center", color: "white", textDecoration: "none" }}>
                <i className={`fab fa-${icon}`} />
              </a>
            ))}
          </div>
        </div>

        <div>
          <h3 style={{ marginBottom: "1.5rem" }}>روابط سريعة</h3>
          <ul style={{ listStyle: "none" }}>
            {[["home","الرئيسية"],["features","الميزات"],["pricing","الأسعار"],["testimonials","آراء العملاء"]].map(([page,label]) => (
              <li key={page} style={{ marginBottom: "0.8rem" }}>
                <a onClick={() => navigate(page)} href="#" style={{ color: "#94a3b8", textDecoration: "none" }}>{label}</a>
              </li>
            ))}
          </ul>
        </div>

        <div>
          <h3 style={{ marginBottom: "1.5rem" }}>الموارد</h3>
          <ul style={{ listStyle: "none" }}>
            {["مركز المساعدة","المدونة","شروط الخدمة","سياسة الخصوصية"].map(item => (
              <li key={item} style={{ marginBottom: "0.8rem" }}>
                <a href="#" style={{ color: "#94a3b8", textDecoration: "none" }}>{item}</a>
              </li>
            ))}
          </ul>
        </div>

        <div>
          <h3 style={{ marginBottom: "1.5rem" }}>تواصل معنا</h3>
          {[["far fa-envelope","hello@creatorpro.com"],["fas fa-phone","+966 50 123 4567"],["fas fa-location-dot","الرياض، المملكة العربية السعودية"]].map(([icon,text]) => (
            <div key={text} style={{ display: "flex", alignItems: "flex-start", gap: 10, color: "#94a3b8", marginBottom: "1rem" }}>
              <i className={icon} style={{ marginTop: 3 }} />{text}
            </div>
          ))}
        </div>
      </div>

      <div style={{ borderTop: "1px solid rgba(255,255,255,0.1)", paddingTop: "2rem", display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "1rem", fontSize: "0.9rem", color: "#64748b" }}>
        <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
          <div style={{ width: 8, height: 8, background: "#10b981", borderRadius: "50%", boxShadow: "0 0 10px #10b981" }} />
          النظام يعمل بكفاءة 100%
        </div>
        <span>© 2026 CreatorPro. جميع الحقوق محفوظة.</span>
      </div>
    </footer>
  );
}
