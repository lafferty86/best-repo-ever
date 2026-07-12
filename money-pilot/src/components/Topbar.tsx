import { useStore, useDispatch } from "../store";
import { pageTitle, pageSubtitle } from "./shared";

export function Topbar() {
  const { page, theme } = useStore();
  const dispatch = useDispatch();
  return (
    <header className="topbar">
      <div className="topbar-left">
        <button className="icon-btn" title="Toggle sidebar" onClick={() => dispatch({ t: "toggleSidebar" })}>☰</button>
        <div>
          <h1 className="page-title">{pageTitle[page]}</h1>
          <p className="page-sub">{pageSubtitle[page]}</p>
        </div>
      </div>
      <div className="topbar-right">
        <button className="icon-btn" title="Toggle theme" onClick={() => dispatch({ t: "toggleTheme" })}>
          {theme === "dark" ? "☀" : "🌙"}
        </button>
        <button className="primary-btn" onClick={() => dispatch({ t: "openModal" })}>
          <span>＋</span><span>Add</span>
        </button>
        <div className="avatar">AL</div>
      </div>
    </header>
  );
}
