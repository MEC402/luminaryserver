# luminaryserver
Collection of Scripts that support remote control of the Luminary Environment

This Repository contains a collection of simple python scripts that work together to provide a collection of remote services for the Boise State University Luminary space.

List of key components:
- lumenweb.py - a lightweight web service that provides key remote functionality to devlopers and users.
  - getserver: returns the hostname and port for the room server.
  - registerwall: register the named wall client that handles specific requests.
  - registerserver: register the primary server for remote Luminary control.
- wallclient.py - a lightweight python script that runs on a Luminary wall client and provides wall specific features.
  - handle remote commands to launch programs on the machine.
  - inject mouse events onto the given machine.
  - handle the proxy catching and inject from local machine. (SQUID).
- lumenpage.html - web page for interacting with the remote control web server.
- lumenserver.py - a sockets based communication server that manages a variety of interactions amongst clients.
  - responds to requests to launch apps on specific wall machines.
  - listen to touches on the wall from the wall server and forward to required client service.
- install.sh - copy files to necessary machines and targets.

Additional components:
-  touchsimulator.py - a lightweight python script that reads mouse and keyboard input
  - reads mouse click position
  - sends click data to wallclient.py
- logger.py
  - creates a generic logger for use by other components
