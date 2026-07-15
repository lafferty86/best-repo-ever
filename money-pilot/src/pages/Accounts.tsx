import { useStore, useDispatch } from "../store";
import type { Account, AccountKind } from "../types";
import { accountKindLabel } from "../types";
import * as F from "../format";
import { Card, CardHead, TrendChip, netWorth, assetsTotal, liabilitiesTotal } from "../components/shared";

function AccountRow({ a, onOpen }: { a: Account; onOpen: (id: number) => void; }) {
  return (
    <button className="acct-row" onClick={() => onOpen(a.id)}>
      <div className="acct-mark" style={{ background: a.color + "22", color: a.color }}>{F.initials(a.institution)}</div>
      <div className="acct-main">
        <span className="acct-name">{a.name}</span>
        <span className="acct-sub">{a.institution} · {accountKindLabel[a.kind]} ••{a.mask}</span>
      </div>
      <div className="acct-right">
        <span className={a.balance >= 0 ? "acct-bal" : "acct-bal neg"}>{F.currency(a.balance)}</span>
        <TrendChip value={a.change} />
      </div>
    </button>
  );
}

export function Accounts() {
  const { accounts } = useStore();
  const dispatch = useDispatch();
  const nw = netWorth(accounts);
  const assets = assetsTotal(accounts);
  const liabilities = liabilitiesTotal(accounts);

  const openAccount = (id: number) => {
    dispatch({ t: "setAccountFilter", value: id });
    dispatch({ t: "navigate", page: "transactions" });
  };

  const group = (title: string, kinds: AccountKind[]) => {
    const list = accounts.filter((a) => kinds.includes(a.kind));
    if (list.length === 0) return null;
    const sum = list.reduce((s, a) => s + a.balance, 0);
    return (
      <Card>
        <CardHead title={title} right={<span className="muted-sm">{F.currency(sum)}</span>} />
        <div className="acct-list">{list.map((a) => <AccountRow key={a.id} a={a} onOpen={openAccount} />)}</div>
      </Card>
    );
  };

  const solvency = assets <= 0 ? 0 : ((assets - liabilities) / assets) * 100;

  return (
    <div className="page">
      <div className="networth-band">
        <div className="nw-cell"><span className="nw-label">Net worth</span><span className="nw-value">{F.currency(nw)}</span></div>
        <div className="nw-cell"><span className="nw-label">Assets</span><span className="nw-value pos">{F.currency(assets)}</span></div>
        <div className="nw-cell"><span className="nw-label">Liabilities</span><span className="nw-value neg">{F.currency(liabilities)}</span></div>
        <div className="nw-bar"><div className="nw-bar-fill" style={{ width: `${solvency}%` }} /></div>
      </div>
      <div className="grid-2">
        <div className="stack">
          {group("Cash", ["checking", "savings", "cash"])}
          {group("Investments", ["investment"])}
        </div>
        <div className="stack">
          {group("Credit cards", ["credit"])}
          {group("Loans", ["loan"])}
        </div>
      </div>
    </div>
  );
}
