from collections import deque
from modules.parser import parse
from modules.logger import logger
from modules.config import DAY

def part1(grid):
    rows, cols = len(grid), len(grid[0])
    paths = []

    # Find all starting points (cells with value 0)
    starts = [(r, c) for r in range(rows) for c in range(cols) if grid[r][c] == 0]

    # Helper to check if a cell is within bounds and valid for the next step
    def is_valid(x, y, current_value):
        return 0 <= x < rows and 0 <= y < cols and grid[x][y] == current_value + 1

    # BFS for each starting point
    for start in starts:
        queue = deque([(start, [start])])  # (current cell, path so far)
        visited = set()  # To keep track of visited cells

        while queue:
            (x, y), path = queue.popleft()
            current_value = grid[x][y]

            # If we've reached a cell with value 9, save the path
            if current_value == 9:
                paths.append(path)
                continue

            # Explore neighbors
            for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:  # Up, Down, Left, Right
                nx, ny = x + dx, y + dy
                if is_valid(nx, ny, current_value) and (nx, ny) not in visited:
                    visited.add((nx, ny))
                    queue.append(((nx, ny), path + [(nx, ny)]))

    return paths



def part2(grid):
    rows, cols = len(grid), len(grid[0])
    paths = []

    # Find all starting points (cells with value 0)
    starts = [(r, c) for r in range(rows) for c in range(cols) if grid[r][c] == 0]

    # Helper to check if a cell is within bounds and valid for the next step
    def is_valid(x, y, current_value):
        return 0 <= x < rows and 0 <= y < cols and grid[x][y] == current_value + 1

    # BFS for each starting point
    for start in starts:
        queue = deque([(start, [start])])  # (current cell, path so far)
        while queue:
            (x, y), path = queue.popleft()
            current_value = grid[x][y]

            # If we've reached a cell with value 9, save the path
            if current_value == 9:
                paths.append(path)
                continue

            # Explore neighbors
            for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:  # Up, Down, Left, Right
                nx, ny = x + dx, y + dy
                if is_valid(nx, ny, current_value):
                    queue.append(((nx, ny), path + [(nx, ny)]))

    return paths


def find_start_point(t: tuple[int, list[int]]):
    """
    find_start_point(list[str])

    Iterates the current list of integers,
    and yields the position as an integer
    if current positions integer is 0

    Params:
        row (list[int])
    """
    y, row = t
    for i, c in enumerate(row):
        if c == 0:
            yield y, i

if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as f:
        lines = list(map(lambda row: [int(i) for i in row.strip("\n")], f.readlines()))

    paths = part2(lines) if parsed.part == 2 else part1(lines)
    logger.info("returned with sum %d", len(paths), extra={"day": DAY, "part": parsed.part})