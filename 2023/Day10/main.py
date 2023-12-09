contents = []

with open('example.txt', 'r') as f:
    contents = [i.strip() for i in f.readlines()]

start_point = 0
for i in contents:
    for c in i:
        if c == 'S':
            start_point = (contents.index(i), i.index(c))

distances = [[0 if start_point == (y, x) else 1 for x, _ in enumerate(line)] for y, line in enumerate(contents)]
print(f"Before: {distances}")

chars = ('|', '-', 'L', 'J', '7', 'F')

current = start_point
distance_to_start = 0
while current != start_point:
    positions = [(current[0] - 1, current[1]), (current[0] - 1, current[1] + 1), (current[0], current[1] + 1), (current[0] + 1, current[1] + 1), (current[0] + 1, current[1]), (current[0] + 1, current[1] - 1), (current[0], current[1] - 1), (current[0] - 1, current[1] - 1)]
    for (y, x) in positions:
        if 0 <= y < len(contents) and 0 <= x < len(contents[y]):
            char = contents[y][x]
            if char in chars:
                if char in ('L', 'J', '7', 'F'):
                    distance_to_start += 1
                    distances[y][x] = distance_to_start
                    current = (y, x)
                elif char == '|' and current[1] + 1 != x and current[1] - 1 != x:
                    distance_to_start += 1
                    distances[y][x] = distance_to_start
                    current = (y, x)
                elif char == '-' and current[0] - 1 != y and current[0] + 1 != y:
                    distance_to_start += 1
                    distances[y][x] = distance_to_start
                    current = (y, x)
                else: 
                    distance_to_start += 1
                    distances[y][x] = distance_to_start
                    current = (y, x)

print(f"After: {distances}")