from modules.parser import parse
import re

if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    pattern = r'mul\(\d+,\d+\)'
    prog = re.compile(pattern)

    matches = []
    for line in lines:
        matches = matches + prog.findall(line)

    print(matches)
    pattern = r'mul\((\d+),(\d+)\)'
    prog = re.compile(pattern)

    mul = []
    for match in matches:
        answer = prog.match(match)
        first = int(answer.group(1))
        second = int(answer.group(2))
        mul.append(first * second)
    
    total = sum(mul)
    print(total)

    pattern = r"(mul|don't|do)\((\d+,\d+)?\)"
    prog = re.compile(pattern)
    matches = []
    for line in lines:
        matches = matches + prog.findall(line)
    
    print(matches)
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