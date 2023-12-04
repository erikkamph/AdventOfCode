import json


with open('games.json', 'r') as f:
    data = json.load(f)
    cards = [1 for _ in data]
    for index, game in enumerate(data):
        winning_numbers = game['NumbersWinningCount']
        for i in range(winning_numbers):
            cards[index + i + 1] += cards[index]
    print(f"Total number of cards won per card: {cards}")
    print(f"Total number of cards won: {sum(cards)}")