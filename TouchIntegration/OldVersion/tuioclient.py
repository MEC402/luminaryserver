from pythontuio import TuioClient
from pythontuio import Cursor
from pythontuio import TuioListener
from threading import Thread, Timer
from pynput.mouse import Controller, Button
import time
 
class MyListener(TuioListener):
 
    def __init__(self,apanel):
        self.timer = Timer(2.0,self.timeout)
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
        xscale = 1400
        yscale = 1024
        yoffset = 1024
        dragtime = 10  # this is a wild guess in nanoseconds

        #calculate the mouse position
        #we assume that x,y are values in the range of 0 to 1
        px = x*xscale + (self.panel-1) * xscale
        py = y*yscale + yoffset
        #self.mouse.position = (px,py)

        if self.started == 0:
            self.timer.start()
            self.started = 1
            #self.mouse.press(Button.left) # put this in when ready

        #handle drag event -delay the timeout with every press
        currenttime = time.time_ns()
        if ((currenttime - self.lasttime) < dragtime):
           self.timer.cancel()
           self.timer = Timer(2,self.timeout)
           self.timer.start()


        self.lasttime = currenttime
       
 
    def timeout(self):
        #self.mouse.release(Button.left)
        self.timer = Timer(2, self.timeout)
        self.started = 0
        self.lasttime = 0
        for val in range(25):
            print('timeout')
 
 
     
 
#panel numbers range from 1 to 17 
# we will need a client for every individual panel
#client = TuioClient(("10.31.11.138",3003))
client = TuioClient(("localhost",3007))
t = Thread(target=client.start)
listener = MyListener(7)
client.add_listener(listener)
 
t.start()

