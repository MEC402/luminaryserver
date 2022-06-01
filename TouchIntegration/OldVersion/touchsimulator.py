import socket
from logger import create_logger
from pynput.mouse import Listener as MouseListener
from pynput.keyboard import Listener as KeyboardListener


class TouchSimulator:
  def __init__(self, target_ip, target_port) -> None:
    self.logger = create_logger(__name__)
    self.target_ip = target_ip
    self.target_port = target_port
    self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    self.mouse_listener = MouseListener(on_click=self.on_click, on_move=self.on_move)
    self.key_listener = KeyboardListener(on_press=self.on_press)

  def start(self) -> None:
    self.logger.debug('Starting Touch Simulator: Press any key to stop')
    self.mouse_listener.start()
    self.key_listener.start()
    self.mouse_listener.join()
    self.key_listener.join()

  def on_click(self, x, y, button, pressed) -> None:
    msg = f"click:{x} {y} {pressed}"
    self.logger.debug(msg)
    self.sock.sendto(msg.encode('utf-8'), (self.target_ip, self.target_port))

  def on_move(self, x, y):
    msg = f"move:{x} {y}"
    self.logger.debug(msg)
    self.sock.sendto(msg.encode('utf-8'), (self.target_ip, self.target_port))

  def on_press(self, key) -> None:
    self.logger.debug('Stop Requested')
    self.sock.sendto('exit:'.encode('utf-8'), (self.target_ip, self.target_port))
    self.mouse_listener.stop()
    self.key_listener.stop()

if __name__ == "__main__":
  hostname = socket.gethostname()
  target_ip = socket.gethostbyname(hostname)
  target_port = 7200
  client = TouchSimulator(target_ip, target_port)
  client.start()
