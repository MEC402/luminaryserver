import time
from pythontuio import TuioServer
from pythontuio import Cursor

server = TuioServer()
cursor = Cursor(123) # sets session_id to 123

cursor.velocity             = (0.2,0.1)
cursor.motion_acceleration  = 0.1 

server.cursors.append(cursor)
i = 0
while i < 10:
    i+=1
    cursor.position = (0.5+0.01*i,0.5)

    server.send_bundle()
    time.sleep(0.1)