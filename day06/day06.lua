local objects = {}

function split(inputstr)
    local t = {}
    for str in string.gmatch(inputstr, "([^)]+)") do table.insert(t, str) end
    return t
end

function getOrCreate(id)
    if objects[id] == nil then
        objects[id] = { name = id, parent = nil }
    end
    return objects[id]
end

for line in io.lines("input.txt") do
    local tokens = split(line)
    local obj = getOrCreate(tokens[1])
    local obj2 = getOrCreate(tokens[2])
    obj2.parent = obj
end

-- Part 1
local total = 0
for k, obj in pairs(objects) do
    local p = obj.parent
    while p ~= nil do
        p = p.parent
        total = total + 1
    end
end
print (total)

-- Part 2
local you = objects["YOU"]
local san = objects["SAN"]
local backtrack = {}
local p = you.parent
while p ~= nil do
    table.insert(backtrack, p.name)
    p = p.parent
end

total = 0
done = false
p = san.parent
while p ~= nil do
    total = total + 1
    for k, v in pairs (backtrack) do
        if v == p.name then
            total = total + k
            done = true
            break;
        end
    end
    if done == true then break end
    p = p.parent
end
print (total - 2) -- First steps don't count.