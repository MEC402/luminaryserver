from math import floor
from pythontuio import TuioClient
from pythontuio import Cursor
from pythontuio import TuioListener
from threading import Thread, Timer
from pynput.mouse import Controller, Button
import time
 
class MyListener(TuioListener):
 
    def __init__(self,apanel):
        self.timer = Timer(.125,self.timeout)
        self.mouse = Controller()
        self.started = 0
        self.lasttime = 0
        self.panel=apanel
 
 
    def add_tuio_cursor(self, cursor: Cursor):
        print("detect a new Cursor")
    (...)
 
    def update_tuio_cursor(self, cursor: Cursor):
        msg = cursor.get_message().params        
        if 'set' in msg:
            self.handle_touch(params=msg)
 
    def handle_touch(self, params):
        (_, _, x, y, _, _, _) = params

        # xscale factor - these are all guesses
        xscale = 680.5
        yscale = 1234
        yoffset = -344
        dragtime = 1  # this is a wild guess in nanoseconds

        #calculate the mouse position
        #we assume that x,y are values in the range of 0 to 1
        px = floor(x*xscale + (self.panel-1) * xscale)
        py = y*yscale + yoffset
        self.mouse.position = (px,py)

        if self.started == 0:
            self.timer.start()
            self.started = 1
            self.mouse.press(Button.left) # put this in when ready

        #handle drag event -delay the timeout with every press
        currenttime = time.time_ns()
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
    #panel numbers range from 1 to 17 
    # we will need a client for every individual panel

    # If pulling be sure to modify tuioclient/listener to disable print statements
    for i in range(1,7):
        client = TuioClient(("10.31.11.138",3000+i))
        #client = TuioClient(("localhost",3003))
        t = Thread(target=client.start)
        listener = MyListener(i)
        client.add_listener(listener)        
        t.start()

