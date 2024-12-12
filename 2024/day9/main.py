from modules.parser import parse


if __name__ == "__main__":
    parsed = parse()
    with open(parsed.file, 'r', encoding='utf-8') as f:
        line = f.readlines()[0]

    visualized = []
    fno = 0

    for i in range(0, len(line), 2):
        c = [f'{fno}'] * int(line[i])
        s = ['.'] * (0 if i + 1 >= len(line) else int(line[i + 1]))
        visualized = visualized + c + s
        fno += 1
    
    print(''.join(visualized))

    spaces = len(list(filter(lambda c: c == '.', visualized)))
    right = visualized[len(visualized) - spaces:]
    left = 0

    for i, v in enumerate(reversed(visualized[len(visualized) - spaces:])):
        while visualized[left] != '.':
            left += 1

        if v != '.' and visualized[left] == '.':
            visualized[len(visualized) - 1 - i] = '.'
            visualized[left] = v
    
    print(''.join(visualized))
    print(sum(list(map(lambda item: 0 if item[1] == '.' else int(item[1]) * item[0], enumerate(visualized)))))
