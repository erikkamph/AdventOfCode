from collections import deque
from modules.parser import parse
from modules.logger import logger
from modules.config import DAY

extra = {
    "day": DAY
}


def valid_start(x: int, y: int, grid: list[list[int]]):
    return valid(x, y, grid) and grid[x][y] == 0


def valid(x: int, y: int, grid: list[list[int]]):
    return 0 <= x < len(grid) and 0 <= y < len(grid[x])


def valid_step(x: int, y: int, grid: list[list[int]], value: int):
    return valid(x, y, grid) and grid[x][y] == value + 1


def traverse(start: tuple[int, int], grid: list[list[int]], part: int):
    x, y = start
    queue = deque([(start, [start])]) 
    visited = set()  # Part 1 -> Keep track of visited cells

    paths = []

    while queue:
        (x, y), path = queue.popleft()
        value = grid[x][y]

        if value == 9:
            paths.append(path)
            continue

        steps = [(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)]
        for npos in filter(lambda item: valid_step(item[0], item[1], grid, value), steps):
            if npos not in visited:
                if part == 1:
                    visited.add(npos)
                queue.append((npos, path + [npos]))

    return paths


if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as f:
        grid = list(map(lambda row: list(map(int, row.strip("\n"))), f.readlines()))
    
    extra['part'] = parsed.part
    if parsed.log:
        logger.info("Todays Grid:", extra=extra)
        for row in grid:
            logger.info(''.join(map(str, row)), extra=extra)

    starts = sum([[(x, y) for y, _ in enumerate(row) if valid_start(x, y, grid)] for x, row in enumerate(grid)], [])
    if parsed.log:
        logger.info("Starting positions:", extra=extra)
        logger.info(starts, extra=extra)

    paths = list(map(lambda start: traverse(start, grid, parsed.part), starts))
    number_paths = list(map(lambda path: len(path), paths))
    if parsed.log:
        logger.info("List of paths per start pos: %a", paths, extra=extra)
        logger.info("Number of paths per start pos found: %a", number_paths, extra=extra)

    # There will always be one log row printed for the part you run
    logger.info("Sum of paths found: %d", sum(number_paths), extra=extra)