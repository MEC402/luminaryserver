# Overview
The purpose of the Touch Proxy is to forward touch events captures by the touch panels
to the correct pc.

## Background
Each wall is driven by 1 pc and is made up of n amount of panels. All the panels send their touch events
to a single pc. The difference between these touch events are what port they are sent to. The issue is all panels
regardless of what wall they are apart of send the events to the same pc, thus events on the right wall are sent to the
pc driving the left wall. We simply need to proxy those events back to the correct wall.

# Config Overview
The config.json file's purpose is to act as a grouping of each wall and it's respective panels.
Receiver address and receiver port signify what ip:port the pc powering the wall runs on. The
ports array signifies what ports that should be associated with the wall.