from modules.parser import parse
from modules.logger import logger
from modules.config import DAY

extra = {"day": DAY, "part": 1}


def find_start_pos(lines: list[list[str]]):
    for y, line in enumerate(lines):
        if '^' in line:
            for x, c in enumerate(line):
                if c == '^':
                    return y, x
    return 0, 0


def scan_column(y: int, x: int, lines: list[list[str]]):
    column = sum(list(map(labmda item: item[x], lines)), '')
    if "#" in column:
        return True, x - 1, y


if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding="utf-8") as f:
        lines = list(map(lambda line: list(line.strip("\n")), f.readlines()))
    
    y, x = find_start_pos(lines)
    visited_tiles = set()
    
    if parsed.part == 2:
        loops = set()

    run = True
    while run:
        tile = lines[y][x]
        lines[y][x] = 'X'

        logger.info("old tile: %c, new tile: %c, x: %d, y: %d", tile, lines[y][x], x, y, extra=extra)

        if tile == '^':
            if y - 1 < 0:
                run = False
                continue

            new_tile = lines[y - 1][x]
            if new_tile == '#':
                lines[y][x] = ">"
                visited_tiles.add((y, x))
                # run = False
            elif new_tile == '.' or new_tile == 'X':
                lines[y - 1][x] = '^'
                visited_tiles.add((y, x))
                y = y - 1
        elif tile == '>':
            if x + 1 >= len(lines[y]):
                run = False
                continue

            new_tile = lines[y][x + 1]
            if new_tile == '#':
                lines[y][x] = 'v'
                visited_tiles.add((y, x))
            elif new_tile == '.' or new_tile == 'X':
                lines[y][x + 1] = '>'
                visited_tiles.add((y, x))
                x = x + 1
        elif tile == 'v':
            if y + 1 >= len(lines):
                run = False
                continue

            new_tile = lines[y + 1][x]
            if new_tile == '#':
                lines[y][x] = '<'
                visited_tiles.add((y, x))
            elif new_tile == '.' or new_tile == 'X':
                lines[y + 1][x] = 'v'
                visited_tiles.add((y, x))
                y = y + 1
        elif tile == '<':
            if x - 1 < 0:
                run = False
                continue

            new_tile = lines[y][x - 1]
            if new_tile == '#':
                lines[y][x] = '^'
                visited_tiles.add((y, x))
            elif new_tile == '.' or new_tile == 'X':
                lines[y][x - 1] = '<'
                visited_tiles.add((y, x))
                x = x - 1
            
    print(visited_tiles)
    print(len(visited_tiles))