import logging
from .config import FMT, DAY_FULL, TIME_FMT

# Uses the format specified to log to stdout
logging.basicConfig(format=FMT, level=logging.INFO, datefmt=TIME_FMT)

# Returns a logger for the current day
logger = logging.getLogger(DAY_FULL)


class FileHandler(logging.Handler):
    def __init__(self, filename):
        super().__init__()
        self.fname = filename
    
    def emit(self, record):
        log_entry = self.format(record)
        binary = log_entry.encode('utf-8') + b'\n'
        with open(self.fname, 'ab+') as f:
            f.write(binary)