
const mapMarginLeft = API.getScreenResolution().Width / 64;
const mapMarginBottom = API.getScreenResolution().Height / 60;
const mapWidth = API.getScreenResolution().Width / 7.11;
const mapHeight = API.getScreenResolution().Height / 5.71;
const resX = mapMarginLeft + mapWidth + mapMarginLeft;
const resY = API.getScreenResolution().Height - mapHeight - mapMarginBottom;

let endRpBarY = 0;
let lastMoneyString = "";
let money = 0;
let level = 0;
let rp = 0;
let rpNextLevel = 0;
let rpPreviousLevel = 0;
let rpPercentage = 0;

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName === "RefreshMoneyUI") {
        money = args[0];
        lastMoneyString = money.toFixed(2).replace(/\./g, ",").replace(/./g,
            function(c, i, a) {
                return i && c !== "," && ((a.length - i) % 3 === 0) ? '.' + c : c;
            });
        lastMoneyString += " €";
    }
    if (eventName === "RefreshRpAndLevel") {
        level = args[0];
        rp = args[1];
        rpNextLevel = args[2];
        rpPreviousLevel = args[3];

        rpPercentage = (rp - rpPreviousLevel) / (rpNextLevel - rpPreviousLevel);
        endRpBarY = API.getScreenResolution().Width * rpPercentage;
    }
    if (eventName === "NewLevel") {
        API.showShard("Du hast Level " + args[0] + " erreicht!", 2000);
    }
});

API.onUpdate.connect(function() {
    const date = new Date();
    const hours = date.getHours() < 10 ? " " + date.getHours() : date.getHours();
    const minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();

    API.drawText(hours + ":" + minutes, resX + 70, resY + 133, 0.5, 255, 255, 255, 200, 0, 2, false, true, 0);
    API.drawText(lastMoneyString, resX - 10, resY + 165, 0.5, 50, 255, 50, 200, 0, 0, false, true, 0);
    API.drawRectangle(0, 0, endRpBarY, 5, 0, 0, 220, 150);
});