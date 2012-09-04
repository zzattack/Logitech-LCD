lglcd
-----
This project transforms the entire Logitech LCD SDK (lglcd.lib), which is only provided as a static C library, into a dynamic link library, by exporting all functions in the outputted native lglcd.dll.

LgLcd.NET
---------
This C# project makes use of the lglcd.dll, importing all of its functionality (whilst ignoring deprecated functionality) and exposes it in an abstract manner to faciliate development of Logitech LCD applets from within .NET languages.

LgBackLight.NET
---------------
This standalone C# project provides a static class called BackLight, that enables one to set the keyboards RGB backlight, which before was not possible without using either Lua scripts or Logitechs own closed source manual configuration software. Details on how this was done can be found on Franks blog at http://zzattack.wordpress.com/2011/12/10/reverse-engineering-the-rgb-led-in-high-end-logitech-keyboards/.
