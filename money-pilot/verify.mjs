import { chromium } from "playwright";

const errors = [];
// Set CHROME_PATH to a Chromium binary, or leave unset to use Playwright's bundled one.
const browser = await chromium.launch(
  process.env.CHROME_PATH ? { executablePath: process.env.CHROME_PATH } : {}
);
const page = await browser.newPage({ viewport: { width: 1360, height: 900 } });
page.on("console", (m) => { if (m.type() === "error") errors.push(m.text()); });
page.on("pageerror", (e) => errors.push("PAGEERROR: " + e.message));

await page.goto("http://localhost:4173/", { waitUntil: "networkidle" });
await page.waitForTimeout(600);

const check = async (label) => {
  const navCount = await page.locator(".nav-item").count();
  return navCount;
};

// Dashboard should render.
const heroValue = await page.locator(".hero-value").first().textContent();
console.log("Hero net worth:", heroValue);
const navCount = await page.locator(".nav-item").count();
console.log("Nav items:", navCount);

// Screenshot dashboard.
await page.screenshot({ path: "shot-dashboard.png" });

// Navigate every page and screenshot.
const pages = ["Accounts", "Transactions", "Budget", "Cash Flow", "Investments", "Recurring", "Goals"];
for (const name of pages) {
  await page.locator(".nav-item", { hasText: name }).click();
  await page.waitForTimeout(350);
  const title = await page.locator(".page-title").textContent();
  console.log(`Navigated -> ${name}: title=${title}`);
  await page.screenshot({ path: `shot-${name.replace(/\s/g, "").toLowerCase()}.png` });
}

// Interaction: on Transactions, search + toggle review + change category + delete.
await page.locator(".nav-item", { hasText: "Transactions" }).click();
await page.waitForTimeout(300);
await page.fill(".search-input", "coffee");
await page.waitForTimeout(300);
const filtered = await page.locator(".tx-row").count();
console.log("Rows after search 'coffee':", filtered);
await page.fill(".search-input", "");
await page.waitForTimeout(200);

// Toggle a review button.
await page.locator(".review-btn").first().click();
await page.waitForTimeout(200);

// Open Add modal and add a transaction.
await page.locator(".primary-btn", { hasText: "Add" }).click();
await page.waitForTimeout(300);
await page.fill('.modal input[placeholder="e.g. Whole Foods"]', "Test Merchant");
await page.fill('.modal input[placeholder="0.00"]', "42.50");
await page.locator(".modal .primary-btn").click();
await page.waitForTimeout(400);
const toast = await page.locator(".toast").count();
console.log("Toast after add:", toast > 0 ? "shown" : "none");

// Budget stepper.
await page.locator(".nav-item", { hasText: "Budget" }).click();
await page.waitForTimeout(300);
const before = await page.locator(".budget-amt").first().textContent();
await page.locator(".step-btn", { hasText: "＋" }).first().click();
await page.waitForTimeout(200);
const after = await page.locator(".budget-amt").first().textContent();
console.log("Budget change:", before, "->", after);

// Goals contribute.
await page.locator(".nav-item", { hasText: "Goals" }).click();
await page.waitForTimeout(300);
await page.locator(".mini-btn", { hasText: "$100" }).first().click();
await page.waitForTimeout(300);

// Theme toggle.
await page.locator(".icon-btn").nth(1).click();
await page.waitForTimeout(300);
const themeClass = await page.locator(".app").getAttribute("class");
console.log("Theme class after toggle:", themeClass);
await page.screenshot({ path: "shot-light.png" });

console.log("\nConsole errors:", errors.length);
errors.forEach((e) => console.log("  -", e));
await browser.close();
process.exit(errors.length > 0 ? 1 : 0);
