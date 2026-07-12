/** FIREside -> Google Sheets sync bridge.
 * 1. In your Google Sheet: Extensions -> Apps Script. Paste this file, save.
 * 2. Deploy -> New deployment -> Web app. Execute as: Me. Access: Anyone.
 * 3. Copy the Web app URL (ends in /exec) into FIREside's Sheet panel.
 * Optional: set SECRET to a passphrase and enter the same one in FIREside.
 */
const SECRET = '';

function doPost(e){
  try{
    const d = JSON.parse(e.postData.contents);
    if(SECRET && d.secret !== SECRET) return json_({ok:false, error:'bad secret'});
    const ss = SpreadsheetApp.getActiveSpreadsheet();

    // raw app state, kept hidden, so FIREside can Pull it back
    const raw = sheet_(ss, '_fireside_raw');
    raw.getRange(1,1).setValue(JSON.stringify(d.state));
    try{ raw.hideSheet(); }catch(err){}

    // Dashboard tab
    const s = d.summary;
    const dash = sheet_(ss, 'Dashboard');
    dash.clear();
    const kv = [
      ['FIREside dashboard', 'updated ' + d.ts],
      ['Monthly take-home income', s.income],
      ['Monthly spending', s.spend],
      ['Invested assets', s.assets],
      ['Savings rate %', s.savingsRatePct],
      ['FIRE number', s.fireNumber],
      ['Years to FI (optimized)', s.yearsToFI],
      ['Years to FI (current plan)', s.baseYearsToFI],
      ['Spending trimmed $/mo', s.spendTrimmedMo],
      ['Extra invested $/mo', s.extraInvestedMo],
      ['Steps done', s.stepsDone + ' / ' + s.stepsTotal],
      ['Potential still unchecked $/mo', s.potentialMo],
    ];
    dash.getRange(1,1,kv.length,2).setValues(kv);
    dash.getRange(1,1,1,2).setFontWeight('bold');
    dash.autoResizeColumns(1,2);

    // Checklist tab
    const cl = sheet_(ss, 'Checklist');
    cl.clear();
    cl.getRange(1,1,1,6).setValues([['Area','Step','Done','Est $/mo','Type','Quick win']]).setFontWeight('bold');
    if(d.rows && d.rows.length) cl.getRange(2,1,d.rows.length,6).setValues(d.rows);
    cl.setFrozenRows(1);
    cl.autoResizeColumns(1,6);

    // History tab: one row per push, a progress log over time
    const h = sheet_(ss, 'History');
    if(h.getLastRow() === 0) h.appendRow(['Timestamp','Steps done','Found $/mo','Years to FI','FIRE number']);
    h.appendRow([d.ts, s.stepsDone, s.spendTrimmedMo + s.extraInvestedMo, s.yearsToFI, s.fireNumber]);

    return json_({ok:true});
  }catch(err){
    return json_({ok:false, error:String(err)});
  }
}

function doGet(e){
  if(SECRET && (!e.parameter || e.parameter.secret !== SECRET)) return json_({ok:false, error:'bad secret'});
  const raw = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('_fireside_raw');
  const txt = raw ? String(raw.getRange(1,1).getValue() || '') : '';
  return ContentService.createTextOutput(txt ? '{"ok":true,"state":' + txt + '}' : '{"ok":false,"error":"no data pushed yet"}')
    .setMimeType(ContentService.MimeType.JSON);
}

function sheet_(ss, name){ return ss.getSheetByName(name) || ss.insertSheet(name); }
function json_(o){ return ContentService.createTextOutput(JSON.stringify(o)).setMimeType(ContentService.MimeType.JSON); }
