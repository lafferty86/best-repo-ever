import { useEffect } from "react";
import { useStore, useDispatch } from "../store";

export function Toast() {
  const { toast } = useStore();
  const dispatch = useDispatch();
  const id = toast?.id;

  useEffect(() => {
    if (id == null) return;
    const handle = window.setTimeout(() => dispatch({ t: "clearToast", id }), 2600);
    return () => window.clearTimeout(handle);
  }, [id, dispatch]);

  if (!toast) return null;
  return (
    <div className={`toast ${toast.kind}`}>
      <span>{toast.kind === "warning" ? "⚠" : "✓"}</span>
      <span>{toast.msg}</span>
    </div>
  );
}
