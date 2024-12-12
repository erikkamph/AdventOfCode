import os
import pathlib

CURRENT_FOLDER = pathlib.Path(__file__).parent.parent
DAY = os.path.basename(CURRENT_FOLDER).removeprefix("day")
DAY_FULL = f"day{DAY}"

TIME_FMT = "%Y-%m-%d %H:%M:%S"
FMT = f'[%(levelname)s %(asctime)s %(filename)s:%(lineno)d -> Day {DAY} Part %(part)s]: %(message)s'