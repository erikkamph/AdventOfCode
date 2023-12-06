content = []

with open('example.txt', 'r') as f:
    content = f.readlines()

time = [int(i) for i in content[0].replace("Time: ", "").strip().split()]
distance = [int(i) for i in content[1].replace("Distance: ", "").strip().split()]

time = [int(content[0].replace("Time:", "").replace(" ", "").strip())]
distance = [int(content[1].replace("Distance:", "").replace(" ", "").strip())]

speed = 0
num = 1

for t, h in zip(time, distance):
    possible_distances = [i for i in range(t)]
    for idx, i in enumerate(possible_distances):
        remaining_time = t - i
        length = i * remaining_time
        possible_distances[idx] = length
    valid_distances = [i for i in possible_distances if i > h]
    num *= len(valid_distances)

print(num)