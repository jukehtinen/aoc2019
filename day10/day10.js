const fs = require("fs");

const data = fs.readFileSync("input.txt", "utf8");
const size = data.indexOf("\n") - 1;
const map = data.replace(/(\r\n|\n|\r)/gm, "").split("");

// Part 1
function getVisibleAsteroidCount(pos) {
    const p0 = { x: pos % size, y: Math.floor(pos / size) };
    const uniqueAngles = new Set();
    for (let i = 0; i < map.length; i++) {
        if (i === pos) continue;
        if (map[i] === ".") continue;

        const p1 = { x: i % size, y: Math.floor(i / size) };
        const angle = Math.atan2(p1.y - p0.y, p1.x - p0.x) * 180 / Math.PI;
        uniqueAngles.add(angle);
    }
    return uniqueAngles.size;
}

let best = 0;
let bestIndex = 0;
for (let i = 0; i < map.length; i++) {
    if (map[i] === "#") {
        const count = getVisibleAsteroidCount(i);
        if (count > best) {
            best = count;
            bestIndex = i;
        }
    }
}
console.log(best);

// Part 2
const laserPos = { x: bestIndex % size, y: Math.floor(bestIndex / size) };
const asteroids = [];
for (let i = 0; i < map.length; i++) {
    if (i === bestIndex) continue;
    if (map[i] === ".") continue;

    const p1 = { x: i % size, y: Math.floor(i / size)};
    asteroids.push({
        angle: -Math.atan2(p1.x - laserPos.x, p1.y - laserPos.y) * 180 / Math.PI,
        distance: Math.sqrt(Math.pow(p1.x - laserPos.x, 2.0) + Math.pow(p1.y - laserPos.y, 2.0)),
        index: i
    });
}

asteroids.sort((a, b) => { return a.angle > b.angle ? 1 : a.angle < b.angle ? -1 : a.distance > b.distance ? 1 : a.distance < b.distance ? -1 : 0 });

for (let i = 0; i < 199; i++) {
    const destroy = asteroids[0];
    asteroids.splice(0, 1);
    while (asteroids[0].angle == destroy.angle) {
        asteroids.push(asteroids[0]);
        asteroids.splice(0, 1);
    }
}

const nextToBeDestroyed = { x: asteroids[0].index % size, y: Math.floor(asteroids[0].index / size) };
console.log(nextToBeDestroyed.x * 100 + nextToBeDestroyed.y);