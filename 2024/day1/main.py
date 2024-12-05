from modules.parser import parse

def find_lowest(arr: list[int]):
    lowest = arr[0]
    for item in arr[1:]:
        if item < lowest:
            lowest = item
    return lowest

def count_occurrences(arr: list[int], number: int):
    count = 0
    for item in arr:
        if item == number:
            count += 1
    return count

if __name__ == "__main__":
    test = parse()
    with open(test.run, 'r') as f:
        lines = [i.strip('\n').split("   ") for i in f.readlines()]

    print(lines)
    left = [int(x) for x, _ in lines]
    right = [int(y) for _, y in lines]

    used_left = []
    used_right = []
    distances = []
    for i in range(len(left)):
        lowest_left = find_lowest(left)
        if not lowest_left:
            break

        used_left.append(left.pop(left.index(lowest_left)))

        lowest_right = find_lowest(right)
        if not lowest_right:
            break

        used_right.append(right.pop(right.index(lowest_right)))

        distance = abs(lowest_left - lowest_right)
        distances.append(distance)
    
    print(distances)
    print(sum(distances))

    similarity_scores = []
    for number in used_left:
        count = count_occurrences(used_right, number)
        similarity_score = number * count
        similarity_scores.append(similarity_score)

    print(similarity_scores)
    print(sum(similarity_scores))