import { chromium } from "playwright";

// Set CHROME_PATH to a Chromium binary, or leave unset to use Playwright's bundled one.
const browser = await chromium.launch(
  process.env.CHROME_PATH ? { executablePath: process.env.CHROME_PATH } : {}
);
const page = await browser.newPage({ viewport: { width: 1360, height: 900 } });
const errors = [];
page.on("console", (m) => { if (m.type() === "error") errors.push(m.text()); });
page.on("pageerror", (e) => errors.push("PAGEERROR: " + e.message));

const base = process.env.BASE_URL || "http://localhost:4173/";
await page.goto(base, { waitUntil: "networkidle" });
await page.waitForTimeout(500);

console.log("Hero net worth:", await page.locator(".hero-value").first().textContent());
console.log("Nav items:", await page.locator(".nav-item").count());
await page.screenshot({ path: "shot-dashboard.png" });

// Walk every page.
for (const name of ["Accounts", "Transactions", "Budget", "Cash Flow", "Investments", "Recurring", "Goals"]) {
  await page.locator(".nav-item", { hasText: name }).click();
  await page.waitForTimeout(300);
  console.log(`-> ${name}: ${await page.locator(".page-title").textContent()}`);
  await page.screenshot({ path: `shot-${name.replace(/\s/g, "").toLowerCase()}.png` });
}

// Sidebar collapse (the reported bug) — assert the width actually shrinks.
await page.locator(".nav-item", { hasText: "Dashboard" }).click();
await page.waitForTimeout(200);
const wOpen = await page.locator(".sidebar").evaluate((el) => el.getBoundingClientRect().width);
await page.locator(".icon-btn").first().click(); // ☰ toggle
await page.waitForTimeout(400);
const wClosed = await page.locator(".sidebar").evaluate((el) => el.getBoundingClientRect().width);
const collapsed = await page.locator(".sidebar").evaluate((el) => el.classList.contains("collapsed"));
console.log(`Sidebar width ${Math.round(wOpen)} -> ${Math.round(wClosed)} (collapsed class: ${collapsed})`);
if (!(wClosed < wOpen - 100 && collapsed)) throw new Error("Sidebar did not collapse");
await page.locator(".icon-btn").first().click(); // expand again
await page.waitForTimeout(300);

// Search filter.
await page.locator(".nav-item", { hasText: "Transactions" }).click();
await page.waitForTimeout(300);
await page.fill(".search-input", "coffee");
await page.waitForTimeout(300);
console.log("Rows after search 'coffee':", await page.locator(".tx-row").count());
await page.fill(".search-input", "");

// Add transaction.
await page.locator(".primary-btn", { hasText: "Add" }).click();
await page.waitForTimeout(300);
await page.fill('.modal input[placeholder="e.g. Whole Foods"]', "Test Merchant");
await page.fill('.modal input[placeholder="0.00"]', "42.50");
await page.locator(".modal .primary-btn").click();
await page.waitForTimeout(400);
console.log("Toast after add:", (await page.locator(".toast").count()) > 0 ? "shown" : "none");

// Budget stepper.
await page.locator(".nav-item", { hasText: "Budget" }).click();
await page.waitForTimeout(300);
const before = await page.locator(".budget-amt").first().textContent();
await page.locator(".step-btn", { hasText: "＋" }).first().click();
await page.waitForTimeout(200);
console.log("Budget change:", before, "->", await page.locator(".budget-amt").first().textContent());

// Goal contribution.
await page.locator(".nav-item", { hasText: "Goals" }).click();
await page.waitForTimeout(300);
await page.locator(".mini-btn", { hasText: "$100" }).first().click();
await page.waitForTimeout(300);

// Theme toggle.
await page.locator(".icon-btn").nth(1).click();
await page.waitForTimeout(300);
console.log("Theme class after toggle:", await page.locator(".app").getAttribute("class"));
await page.screenshot({ path: "shot-light.png" });

console.log("\nConsole errors:", errors.length);
errors.forEach((e) => console.log("  -", e));
await browser.close();
process.exit(errors.length > 0 ? 1 : 0);
