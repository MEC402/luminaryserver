from ipaddress import ip_address
import socket
import sys
from logger import create_logger
from pynput.mouse import Controller, Button
from threading import Timer
from time import time_ns
from math import floor

class WallClient():
  def __init__(self,ip,port) -> None:
    self.logger = create_logger(__name__)
    self.timer = Timer(.125,self.timeout)
    self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    self.sock.bind((ip,port))
    self.mouse = Controller()
    self.running = False
    self.started = 0
    self.lasttime = 0
  
  def start(self) -> None:
    self.logger.debug('Starting Wall Client')
    self.running = True
    while self.running:
      (data, addr) = self.sock.recvfrom(1024)
      msg = data.decode('utf-8')
      #self.logger.debug(f"[Message]:{msg} from {addr}")
      self.handle(msg)

  def handle(self, msg) -> None:
    (x, y, panel) = msg.split(" ")
    # xscale factor - these are all guesses
    xscale = 680
    yscale = 1234
    yoffset = -344
    dragtime = 1  # this is a wild guess in nanoseconds

    #calculate the mouse position
    #we assume that x,y are values in the range of 0 to 1
    px = float(x)*xscale + (int(panel)-1) * xscale
    py = float(y)*yscale + yoffset
    self.mouse.position = (px,py)

    if self.started == 0:
      self.timer.start()
      self.started = 1
      self.mouse.press(Button.left) # put this in when ready

    #handle drag event -delay the timeout with every press
    currenttime = time_ns()
    if ((currenttime - self.lasttime) < dragtime):
      self.timer.cancel()
      self.timer = Timer(.125,self.timeout)
      self.timer.start()
    self.lasttime = currenttime   

  def timeout(self):
    self.mouse.release(Button.left)
    self.timer = Timer(.125, self.timeout)
    self.started = 0
    self.lasttime = 0
        
if __name__ == "__main__":
  args = sys.argv
  ip_address = "10.31.11.138"
  if len(args) > 1:
    if args[1] == 'm':
      ip_address = "10.31.11.193"
    elif args[1] == 'r':
      ip_address = "10.31.11.139" 
  listener_port = 3000
  client = WallClient(ip_address, listener_port)
  client.start()