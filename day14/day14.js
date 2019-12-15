const fs = require("fs");

const data = fs.readFileSync("input.txt", "utf8");
const recipies = {}
data.split("\n").forEach(i => {
    const toks = i.split(" => ");
    const reqs = toks[0].split(", ").map(x => {
        return { type: x.split(" ")[1], req: parseInt(x.split(" ")[0]) }
    });
    const result = { type: toks[1].split(" ")[1], prod: parseInt(toks[1].split(" ")[0]) };
    recipies[result.type] = { reqs: reqs, result: result };
})

const storage = { "ORE": 0 };
let oreNeed = 0;

function create(chem, need) {
    // No need to create ore, just add it
    if (chem === "ORE") {
        storage["ORE"] = storage["ORE"] + need;
        oreNeed += need;
        return;
    }

    const recipe = recipies[chem];
    let loops = Math.ceil(need / recipe.result.prod);

    // Create and consume requirements
    recipe.reqs.forEach(req => {
        if (storage[req.type] !== undefined) {
            if (storage[req.type] >= req.req * loops) {
                // Already enough in storage, consume
                storage[req.type] -= req.req * loops;
            } else {
                // Not enough in storage, consume all, create rest
                let n = (req.req * loops) - storage[req.type];
                storage[req.type] = 0;
                create(req.type, n);
                storage[req.type] -= n;
            }
        } else {
            // Not in storage, create and consume
            create(req.type, req.req * loops);
            storage[req.type] -= (req.req * loops);
        }
    });

    // Add stuff to storage
    storage[chem] = recipe.result.prod * loops;
}

// Part 1
create("FUEL", 1);
console.log(oreNeed);

// Part 2
let L = 0;
let R = 1000000000000;
let m = 0;
while (L <= R) {
    m = Math.floor((L + R) / 2);
    oreNeed = 0;
    create("FUEL", m);
    if (oreNeed < 1000000000000)
        L = m + 1;
    else if (oreNeed > 1000000000000)
        R = m - 1;
    else
        break;
}
console.log(m - 1); // -1 for one extra was created.