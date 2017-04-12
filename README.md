# LogDiver
Reads SS13 logs (currently only client side) and outputs them in a more
readable format. Main feature is reading the logfile as it is being written
to by Byond, using a file monitor.

## Current features
- Create/remove and drag, dock or float any number of configurable windows
(I call them targets) to display log lines that match a string.contains filter.
- Read log files, both closed and open (still being written to) and parse them.
- Adds timestamps to lines read by the live logger.

## Planned features
- Regex filters
- Let targets contain multiple filters
- Save targets configurations to disk. Load on startup.
- Support loading of server-side log files and parse them accordingly.
