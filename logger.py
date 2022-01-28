import logging

def create_logger(name) -> logging.Logger:
  """
  Creates a generic logger.
  """
  logger = logging.getLogger(name)
  logger.setLevel(logging.DEBUG)
  ch = logging.StreamHandler()
  ch.setLevel(logging.DEBUG)
  formatter = logging.Formatter('[%(asctime)s] %(message)s')
  ch.setFormatter(formatter)
  logger.addHandler(ch)
  return logger