[lglcd]
This project transforms the entire Logitech LCD SDK (lglcd.lib), which is only
provided as a static C library, into a dynamic link library, by exporting all
functions in the outputted native lglcd.dll.

[LgLcd.NET]
This C# project makes use of the lglcd.dll, importing all of its functionality
(whilst ignoring deprecated functionality) and exposes it in an abstract manner
to faciliate development of Logitech LCD applets from within .NET languages.
