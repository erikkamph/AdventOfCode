from argparse import ArgumentParser, RawTextHelpFormatter
from .config import DAY
import sys

parser = ArgumentParser(prog="AdventOfCode",
                        description=f"""
The current day you are trying to run is day {DAY}.
Do you want to run the example or the input?

PS.
The code doesn't come with any input other than 
the example, so the concept is simple. Bring your
own input as everyones is different. If the code
URL: https://adventofcode.com/2024/day/{DAY}/input

If you get gold stars, it means that it's working
for your input as well. Otherwise, don't complain
and fix the code yourself.
""",
                        epilog=f"Advent of code (https://adventofcode.com/)",
                        formatter_class=RawTextHelpFormatter)
parser.add_argument("-e", "--example",
                    dest='file',
                    help="Run with input data from example.txt (if -r is used as well, it'll override this option)",
                    action='store_const',
                    const='example.txt')
parser.add_argument("-r", "--run",
                    dest='file',
                    help="Run with input data from input.txt",
                    action='store_const',
                    const='input.txt')

def parse():
    test = parser.parse_args(sys.argv[1:])
    return test