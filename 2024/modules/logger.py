import logging
from .config import FMT, DAY_FULL

# Uses the format specified to log to stdout
logging.basicConfig(format=FMT, level=logging.INFO)

# Returns a logger for the current day
logger = logging.getLogger(DAY_FULL)