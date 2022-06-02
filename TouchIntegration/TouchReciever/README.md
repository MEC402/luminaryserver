# Overview
The purpose of the Touch Receiver is to receive the proxied data
from the Touch Proxy and perform mouse operations based on it.

## Background
The proxied data consists of packets in the form of "panelnumber x y". 
These packest need to be parsed and then have the x/y values scaled so that
they correctly match the dimensions of the host pc. One thing to note, is that
the x/y values will range from 0.0 - 1.0, thus they need to be denormalized
before scaling them. To denormalize, the equations we will use are:

x * ((PanelNumber - PanelOffset) / PanelCount) 
y * (Foil offset foil height)

These then need to be multiplied by 65535. See [stackoverflow](https://stackoverflow.com/questions/21965353/how-to-calculate-coordinates-to-move-the-mouse-cursor-programmatically):
x * 65535 
y * 65535 

These values can then be send to the input stream via windows api in order to cause clicks to happen.
Since we only have these values to go off of, we must determine wether to click and release, click and drag,
and right click based off the time inbetween these events.

# Config Overview
The config.json file consists of what port the proxy is going to send data to.
It must match the ReceiverPort defined in the TounchProxy config.json file for the
specific wall. PanelCount signifies the amount of panels that make up the wall.
Each wall has a different amount thus we use this value to divide the horizontal resolution
into "panel" sized chunks. Finally PanelOffset defines what to subract from the incoming panel
number so that the first panel in each wall starts at 1. We can do it this way because
going from the the left, to middle, to right wall, the panels numbers go from 1-x in order.
Thus if the middle wall is made up of panels 6-11, we need to subtract 5.
