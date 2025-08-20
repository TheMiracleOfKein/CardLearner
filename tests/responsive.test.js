const { spawn } = require('child_process');
const puppeteer = require('puppeteer');

(async () => {
  const server = spawn('dotnet', ['run', '--project', 'CardLearner/CardLearner', '--urls', 'http://localhost:5000'], { stdio: 'inherit' });
  // wait for server to start
  await new Promise(resolve => setTimeout(resolve, 5000));

  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto('http://localhost:5000/quiz');

  await page.setViewport({ width: 1024, height: 800 });
  const columnsDesktop = await page.evaluate(() => getComputedStyle(document.querySelector('.options-grid')).gridTemplateColumns.split(' ').length);

  await page.setViewport({ width: 500, height: 800 });
  const columnsMobile = await page.evaluate(() => getComputedStyle(document.querySelector('.options-grid')).gridTemplateColumns.split(' ').length);

  console.log('columnsDesktop', columnsDesktop, 'columnsMobile', columnsMobile);

  await browser.close();
  server.kill();

  if (columnsDesktop <= columnsMobile) {
    console.error('Responsive grid test failed');
    process.exit(1);
  }
  console.log('Responsive grid test passed');
})();
