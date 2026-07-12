import type { ReactNode } from "react";
import { useStore, useDispatch, type TxDraft } from "../store";
import { categories } from "../data";

function Field({ label, children }: { label: string; children: ReactNode; }) {
  return (
    <label className="field">
      <span className="field-label">{label}</span>
      {children}
    </label>
  );
}

export function Modal() {
  const { draft, accounts } = useStore();
  const dispatch = useDispatch();
  if (!draft) return null;
  const d = draft;
  const patch = (p: Partial<TxDraft>) => dispatch({ t: "updateDraft", draft: { ...d, ...p } });

  return (
    <div className="modal-overlay" onClick={() => dispatch({ t: "closeModal" })}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-head">
          <h3>Add transaction</h3>
          <button className="icon-btn" onClick={() => dispatch({ t: "closeModal" })}>✕</button>
        </div>
        <div className="seg">
          <button className={!d.isIncome ? "seg-item active" : "seg-item"} onClick={() => patch({ isIncome: false })}>Expense</button>
          <button className={d.isIncome ? "seg-item active" : "seg-item"} onClick={() => patch({ isIncome: true })}>Income</button>
        </div>
        <div className="modal-body">
          <Field label="Merchant">
            <input className="input" placeholder="e.g. Whole Foods" value={d.merchant}
              autoFocus onChange={(e) => patch({ merchant: e.target.value })} />
          </Field>
          <div className="field-row">
            <Field label="Amount">
              <input className="input" placeholder="0.00" type="number" value={d.amount}
                onChange={(e) => patch({ amount: e.target.value })} />
            </Field>
            <Field label="Date">
              <input className="input" type="date" value={d.date}
                onChange={(e) => patch({ date: e.target.value })} />
            </Field>
          </div>
          {!d.isIncome && (
            <Field label="Category">
              <select className="input" value={d.categoryId} onChange={(e) => patch({ categoryId: e.target.value })}>
                {categories.filter((c) => !c.isIncome && c.id !== "transfer").map((c) => (
                  <option key={c.id} value={c.id}>{c.icon}  {c.name}</option>
                ))}
              </select>
            </Field>
          )}
          <Field label="Account">
            <select className="input" value={d.accountId} onChange={(e) => patch({ accountId: parseInt(e.target.value, 10) })}>
              {accounts.map((a) => (
                <option key={a.id} value={a.id}>{a.name} ••{a.mask}</option>
              ))}
            </select>
          </Field>
          <Field label="Note (optional)">
            <input className="input" placeholder="Add a note" value={d.note}
              onChange={(e) => patch({ note: e.target.value })} />
          </Field>
        </div>
        <div className="modal-foot">
          <button className="ghost-btn" onClick={() => dispatch({ t: "closeModal" })}>Cancel</button>
          <button className="primary-btn" onClick={() => dispatch({ t: "submitDraft" })}>Save transaction</button>
        </div>
      </div>
    </div>
  );
}
