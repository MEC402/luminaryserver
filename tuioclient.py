from pythontuio import TuioClient
from pythontuio import Cursor
from pythontuio import TuioListener
from threading import Thread, Timer
from pynput.mouse import Controller, Button
 
class MyListener(TuioListener):
 
    def __init__(self):
        self.timer = Timer(2.0,self.timeout)
        self.mouse = Controller()
        self.started = 0
 
 
    def add_tuio_cursor(self, cursor: Cursor):
        print("detect a new Cursor")
    (...)
 
    def update_tuio_cursor(self, cursor: Cursor):
        msg = cursor.get_message().params        
        if 'set' in msg:
            self.handle_touch(params=msg)
 
    def handle_touch(self, params):
        (_, _, x, y, _, _, _) = params
 
        #self.mouse.position = (x,y)
        #self.mouse.press(Button.left)
        if self.started == 0:
            self.timer.start()
            self.started = 1
       
 
    def timeout(self):
        #self.mouse.release(Button.left)
        self.timer = Timer(2, self.timeout)
        self.started = 0
        for val in range(25):
            print('timeout')
 
 
     
 
 
client = TuioClient(("10.31.11.138",3003))
t = Thread(target=client.start)
listener = MyListener()
client.add_listener(listener)
 
t.start()

