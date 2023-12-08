from multiprocessing import cpu_count
from concurrent import futures
from threading import Semaphore

semaphore = Semaphore()

def x(y):
    print(y)
    return y


with futures.ThreadPoolExecutor(max_workers=cpu_count() * 2 + 1) as executor:
    n = [executor.submit(x, i) for i in range(1000)]
    res = [i for i in futures.as_completed(n)]
    print(res)