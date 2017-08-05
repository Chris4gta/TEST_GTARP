let lastData;
let browser;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "openPayDayUI") {
        const resolution = API.getScreenResolution();
        const width = Math.round(resolution.Width < 690 ? resolution.Width : 690);
        const height = Math.round(resolution.Height < 390 ? resolution.Height : 390);

        browser = API.createCefBrowser(width, height, true);
        API.waitUntilCefBrowserInit(browser);

        const x = (resolution.Width - width) / 2;
        const y = (resolution.Height - height) / 2;

        API.setCefBrowserPosition(browser, x, y);
        API.setCefBrowserHeadless(browser, false);
        API.loadPageCefBrowser(browser, 'UI/PayDay-Calculation.html', false);
        API.showCursor(true);
        API.waitUntilCefBrowserLoaded(browser);

        lastData = JSON.parse(args[0]);
    }
});

function loadComplete() {
    for (name in lastData.Income) {
        if (lastData.Income.hasOwnProperty(name)) {
            browser.call("addIncome", name, lastData.Income[name]);
        }
    }

    for (name in lastData.Outgoings) {
        if (lastData.Outgoings.hasOwnProperty(name)) {
            browser.call("addOutgoing", name, lastData.Outgoings[name]);
        }
    }
}

function closeWindow() {
    API.showCursor(false);
    API.destroyCefBrowser(browser);
}