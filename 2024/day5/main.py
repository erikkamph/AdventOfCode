"""
Day 5 of 24 in Advent of Code
"""
from modules.parser import parse
from modules.logger import logger
from modules.config import DAY


def convert_to_int(item: str, sep: str = ","):
    item = item.strip("\n")
    split = item.split(sep)
    return list(map(lambda item: int(item), split))


def validate(line: list[int], orders: list[tuple[int, int]]):
    for x, y in orders:
        if x not in line or y not in line:
            continue

        if line.index(x) > line.index(y):
            return False
    
    return True


if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    # Initialize the two lists
    index_empty_line = lines.index("\n")
    ordering_rules = list(map(lambda item: convert_to_int(item, "|"), lines[:index_empty_line]))
    print_pages = list(map(convert_to_int, lines[index_empty_line + 1:]))

    # Map the numbers of the middle indexes if they are correct according to a validation rule
    values = list(map(lambda line: line[len(line) // 2] if validate(line, ordering_rules) else 0, print_pages))
    
    # Calculate the sum of the pages in the validated list
    sum_mid_indexes = sum(values)
    extra = {"day": DAY, "part": 1}

    logger.warning("Sum of mid indexes in correct order %d",
                   sum_mid_indexes,
                   extra=extra)
