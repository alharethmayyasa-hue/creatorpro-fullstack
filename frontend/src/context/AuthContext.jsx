import { createContext, useContext, useState, useEffect } from "react";
import { authAPI } from "../services/api";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const savedUser = localStorage.getItem("user");
    if (token && savedUser) {
      try {
        setUser(JSON.parse(savedUser));
      } catch {
        localStorage.removeItem("user");
      }
    }
    setLoading(false);
  }, []);

  const persistAuth = (data) => {
    if (data?.token) localStorage.setItem("token", data.token);
    if (data?.refreshToken)
      localStorage.setItem("refreshToken", data.refreshToken);
    if (data?.user) {
      localStorage.setItem("user", JSON.stringify(data.user));
      setUser(data.user);
    }
  };

  const login = async (email, password) => {
    const data = await authAPI.login(email, password);
    persistAuth(data);
    return data;
  };

  const register = async (name, email, password) => {
    const data = await authAPI.register(name, email, password);
    persistAuth(data);
    return data;
  };

  const logout = async () => {
    try {
      await authAPI.logout();
    } catch {
      // تجاهل أخطاء logout (قد يكون التوكن منتهياً) ونكمل التنظيف محلياً
    }
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
