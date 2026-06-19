# Wolfenstein 3D - Map for DOSBox
## About
Realtime map for Wolfenstein 3D and Tristania 3D.  
The doors don't show up for some reason. Everything else works.  
Tested with DOSBox Staging 0.82 and 0.83-RC1.
## Run
DOSBox Staging (version 0.75.1 and up) [logs the base address](https://www.dosbox-staging.org/releases/release-notes/0.75.1/#log-base-address-of-emulated-memory) of the emulated memory. 
Copy that address to `wolf3d_map.ini` before running the program.
## Build
The project requires .NET Framework 3.5 to build, and Visual Studio 2008 Express or newer or JetBrains Rider.