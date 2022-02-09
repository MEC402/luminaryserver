from pythontuio import TuioClient
from pythontuio import Cursor
from pythontuio import TuioListener
from threading import Thread

class MyListener(TuioListener):
    def add_tuio_cursor(self, cursor: Cursor):
        print("detect a new Cursor")
    (...)

    def update_tuio_cursor(self, cursor: Cursor):
      print(cursor.get_message().params)


client = TuioClient(("localhost",3333))
t = Thread(target=client.start)
listener = MyListener()
client.add_listener(listener)

t.start()