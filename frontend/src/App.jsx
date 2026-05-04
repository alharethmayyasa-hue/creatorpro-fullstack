import { useState } from "react";
import { AuthProvider } from "./context/AuthContext";
import Header from "./components/Header";
import Footer from "./components/Footer";
import Toast from "./components/Toast";
import HomePage from "./pages/HomePage";
import FeaturesPage from "./pages/FeaturesPage";
import TestimonialsPage from "./pages/TestimonialsPage";
import PricingPage from "./pages/PricingPage";
import LoginPage from "./pages/LoginPage";
import { useToast } from "./hooks/useToast";

export default function App() {
  const [currentPage, setCurrentPage] = useState("home");
  const { toast, showToast } = useToast();

  const navigate = (page) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const pages = { home: HomePage, features: FeaturesPage, testimonials: TestimonialsPage, pricing: PricingPage };
  const PageComponent = pages[currentPage];

  return (
    <AuthProvider>
      <div dir="rtl" style={{ fontFamily: "'Cairo', sans-serif" }}>
        {currentPage !== "login" && (
          <Header currentPage={currentPage} navigate={navigate} />
        )}

        {currentPage === "login" ? (
          <LoginPage navigate={navigate} showToast={showToast} />
        ) : (
          <main style={{ paddingTop: 80 }}>
            <PageComponent navigate={navigate} showToast={showToast} />
          </main>
        )}

        {currentPage !== "login" && <Footer navigate={navigate} />}
        <Toast toast={toast} />
      </div>
    </AuthProvider>
  );
}
