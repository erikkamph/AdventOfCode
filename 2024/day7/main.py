from modules.parser import parse
import itertools
from operator import add, mul


def myconcat(num1: int, num2: int):
    return int(f"{num1}{num2}")


def evaluate_expression(numbers: list[int], operators: list[str], target: int):
    result = numbers[0]

    for i, op in enumerate(operators):
        func = add if op == '+' else mul if op == '*' else myconcat if op == '||' else None
        result = func(result, numbers[i + 1])
    
    return operators, target == result


if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as file:
        lines = file.readlines()
    
    lines = list(map(lambda item: (item.split(":")[0], item.split(":")[1].replace(" ", "", 1).strip()), lines))
    lines = list(map(lambda item: {"answer": int(item[0]), "input": [int(i) for i in item[1].split(" ")], "solutions": [], "possible": [], "ispossible": False}, lines))

    operators = ['*', '+']

    if parsed.part == 2:
        operators.append('||')
    
    for k, (a, i, s, p, o) in enumerate(lines):
        numbers = lines[k][i]
        combinations = itertools.product(operators, repeat=len(numbers) - 1)
        tested = list(map(lambda op_combo: evaluate_expression(lines[k][i], op_combo, lines[k][a]), combinations))
        possible = list(filter(lambda item: not item[1], tested))
        solutions = list(filter(lambda item: item[1], tested))
        lines[k][p] = possible
        lines[k][s] = solutions
        lines[k][o] = len(solutions) > 0
    
    possible = list(filter(lambda item: item['ispossible'], lines))
    sum_possible = sum(list(map(lambda item: item['answer'], possible)))
    if 'output' in parsed:
        with open(parsed.output, 'w+', encoding='utf-8') as f:
            f.writelines([f"Sum of possible: {sum_possible}"])
