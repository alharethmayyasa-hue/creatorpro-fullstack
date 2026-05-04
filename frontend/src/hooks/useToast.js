import { useState, useCallback } from "react";

export function useToast() {
  const [toast, setToast] = useState({ show: false, type: "", message: "" });

  const showToast = useCallback((type, message) => {
    setToast({ show: true, type, message });
    setTimeout(() => setToast({ show: false, type: "", message: "" }), 3000);
  }, []);

  return { toast, showToast };
}
