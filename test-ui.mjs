import { chromium } from 'playwright';

(async () => {
  console.log('Launching browser...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1920, height: 1080 }
  });
  const page = await context.newPage();

  try {
    console.log('Navigating to http://localhost:5174...');
    await page.goto('http://localhost:5174', { waitUntil: 'networkidle', timeout: 10000 });

    console.log('Taking screenshot...');
    await page.screenshot({ path: 'frontend-login.png', fullPage: true });

    console.log('✅ Screenshot saved: frontend-login.png');
    console.log('Page title:', await page.title());
    console.log('Current URL:', page.url());

    // Check for console errors
    const errors = [];
    page.on('console', msg => {
      if (msg.type() === 'error') {
        errors.push(msg.text());
      }
    });

    // Wait a bit to collect any errors
    await page.waitForTimeout(2000);

    if (errors.length > 0) {
      console.log('❌ Console errors detected:');
      errors.forEach(err => console.log('  -', err));
    } else {
      console.log('✅ No console errors detected');
    }

  } catch (error) {
    console.error('❌ Error:', error.message);
    await page.screenshot({ path: 'frontend-error.png' });
    console.log('Error screenshot saved: frontend-error.png');
  } finally {
    await browser.close();
  }
})();
