import socket
from logger import create_logger
from pynput.mouse import Controller, Button

class WallClient():
  def __init__(self,ip,port) -> None:
    self.logger = create_logger(__name__)
    self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    self.sock.bind((ip,port))
    self.mouse = Controller()
    self.prev_pos = (None, None)
    self.running = False
  
  def start(self) -> None:
    self.logger.debug('Starting Wall Client')
    self.running = True
    while self.running:
      (data, addr) = self.sock.recvfrom(1024)
      (action, msg) = data.decode('utf-8').split(":")
      self.logger.debug(f"[Message]:{action} {msg} from {addr}")
      match action:
        case 'exit': self.running = False
        case 'click': self.click(msg)
        case 'move': self.move(msg)
  
  def click(self, msg) -> None:
    (x, y, pressed) = msg.split(" ")
    (s_x, s_y) = self.scale_coordinates(int(x),int(y))
    if pressed == 'True':
      self.mouse.press(Button.left)
    else:
      self.mouse.position = (s_x,s_y)
      self.mouse.release(Button.left)

  def move(self, msg) -> None:
    (x, y) = msg.split(" ")
    (s_x, s_y) = self.scale_coordinates(int(x),int(y))
    self.mouse.position = (s_x,s_y) 
        

  def scale_coordinates(self,x,y):
    # Do math here

    return (x,y)

if __name__ == "__main__":
  hostname = socket.gethostname()
  listener_ip = socket.gethostbyname(hostname)
  listener_port = 7200
  
  client = WallClient(listener_ip, listener_port)
  client.start()