import math

# part 1
with open('input.txt', 'r') as file:
    lines = file.readlines()
    total = 0
    for l in lines:
        total += math.floor(int(l) / 3 - 2)
    print(total)
    
# part 2
with open('input.txt', 'r') as file:
    lines = file.readlines()
    total = 0
    for l in lines:
        fuel = math.floor(int(l) / 3 - 2)
        while fuel > 0 :
            total += fuel
            fuel = math.floor(fuel / 3 - 2)    
    print(total)