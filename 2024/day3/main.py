from modules.parser import parse
import re

if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    pattern = r'mul\(\d+,\d+\)'
    prog = re.compile(pattern)
    matches = sum(list(map(lambda line: prog.findall(line), lines)), [])

    pattern = r'mul\((\d+),(\d+)\)'
    prog = re.compile(pattern)

    mul = list(map(lambda item: int(prog.match(item).group(1)) * int(prog.match(item).group(2)), matches))
    total = sum(mul)
    print(total)

    pattern = r"(mul|don't|do)\((\d+,\d+)?\)"
    prog = re.compile(pattern)
    matches = sum(list(map(lambda line: prog.findall(line), lines)), [])
    
    mul = []
    skip = False
    for match in matches:
        instruction = match[0]
        if instruction == "don't":
            skip = True
        elif instruction == 'do':
            skip = False
        elif instruction == 'mul' and not skip:
            numbers = match[1].split(",")
            mul.append(int(numbers[0]) * int(numbers[1]))
    total = sum(mul)
    print(total)