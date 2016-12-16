Blinking LED with NodeMCU and PETRI.NET SIMULATOR
----------

This demo shows how to use petri.NET simulator and **C# firmata** client class to interact with **ESP8266 boards** or **Arduino**.

For ESP8266 boards, you may replace StandardFirmata.ino shipped with Arduino compiler with the code below.

**StandardFirmataESP.ino** (*ESP8266 boards*)
**StandardFirmata.ino** (*Arduino boards*)

After uploading StandardFirmata.ino or the code above for ESP8266 , you can use Testing LED file. Make sure you adjust:

 1. Blinking Pin
```C#
int D4=2;
```
 2. COM port and BaudRate
```C#
ObjArduinoUno=new ArduinoUno("COM190",57600);
```
![Blinking LED with Petri.NET simulator](https://media.giphy.com/media/3o6Zt3wBEhSQJaznQA/source.gif)

