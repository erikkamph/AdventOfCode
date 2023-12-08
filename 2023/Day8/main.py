content = []

with open('input.txt') as f:
    content = f.readlines()

from multiprocessing import cpu_count
from concurrent import futures
from threading import Semaphore
from copy import deepcopy

semaphore = Semaphore()
traversed = []
steps_per_worker = []
starting_points = []
mapped = {}

def run(start_key: int, mapping: dict, instructions: str):
    global semaphore, starting_points
    steps = 0
    paths = []
    current = starting_points[start_key]
    truthy = True
    while truthy:
        for s in range(len(instructions)):
            paths.append(current)
            val = mapping[current][instructions[s % len(instructions)]]
            current = val
            steps += 1
            if val.endswith('Z'):
                break
        
        if semaphore.acquire(timeout=1000):
            traversed[start_key] = current
            steps_per_worker[start_key] = steps
            truthy = not all([j == 6 for j in steps_per_worker])
            semaphore.release()
    return steps, current

instructions = content[0].strip()

for line in content[2:]:
    parts = line.strip().split(" = ")
    l, r = parts[1].replace("(", "").replace(")", "").split(", ")
    mapped[parts[0].strip()] = {'L': l.strip(), 'R': r.strip()}

starting_points = [i for i in mapped.keys() if i.endswith('A')]
c = deepcopy(starting_points)
c.reverse()
starting_points.extend(c)

traversed = ['' for _ in range(len(starting_points))]
steps_per_worker = [0 for _ in range(len(starting_points))]

with futures.ThreadPoolExecutor(max_workers=(cpu_count() * 2) + 1) as executor:
    futurelist = [executor.submit(run, n, mapped, instructions) for n in range(len(starting_points))]
    results = [i for i in futures.as_completed(futurelist)]
    res = [i.result() for i in results]
    print(res)