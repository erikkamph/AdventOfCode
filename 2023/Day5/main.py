contents = []
with open('input.txt', 'r') as f:
    contents = f.readlines()
maps = [i.strip().replace(" map:", "") for i in contents[2:]]
lowest_per_serie = []

def calculate(m, n):
    origin = []
    current_row = ""
    start = int(m)
    rlen = int(n)

    print(f"Start {start}, len {rlen}")
    for seed in range(start, start + rlen):
        num_has_changed = True
        paths = [seed]
        n = seed

        for line in maps:
            if '-to-' in line:
                current_row = line
            if current_row != '' and '-to-' not in line and line != '':
                dest, src, r = line.split()
                ndest = range(int(dest), int(dest) + int(r))
                nsrc = range(int(src), int(src) + int(r))

                if not(num_has_changed):
                    try:
                        ridx = nsrc.index(seed)
                        n = ndest[ridx]
                        num_has_changed = True
                    except:
                        n = seed
            if line == '':
                paths.append(n)
                num_has_changed = False
        
        paths.sort()
        origin.append(paths[0])
    origin.sort()
    return origin[0]

numbers = contents[0].replace("seeds: ", "").strip().split()

from multiprocessing import cpu_count
from concurrent import futures
print(cpu_count() * 2 + 1)

with futures.ThreadPoolExecutor((cpu_count() * 2) + 1) as executor:
    items = [executor.submit(calculate, numbers[i], numbers[i + 1]) for i in range(0, len(numbers), 2)]
    res = [i.result() for i in futures.as_completed(items)]

res.sort()
print(res[-1])