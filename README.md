Petri.NET
=========

A Petri Net simulator with a Python (and C#) scripting capability

This software use following additional libraries:

* [MagicLibrary.dll](http://www.codeproject.com/Articles/4193/Magic-Library-Docking-Manager-Designer), use [The Code Project Open License (CPOL)](http://www.codeproject.com/info/cpol10.aspx)
* [CDiese.dll](http://www.codeproject.com/Articles/1916/ActionLists-for-Windows-Forms), use [The Code Project Open License (CPOL)](http://www.codeproject.com/info/cpol10.aspx)
* [IronPython](https://ironpython.codeplex.com/), use [Apache License 2.0 (Apache)](http://www.apache.org/licenses/LICENSE-2.0.html)
* [ICSharpCode.TextEditor](http://www.icsharpcode.net/opensource/sd/), use [LGPL](http://www.gnu.org/copyleft/lesser.html)

To compile version that target NET 2.0 and IronPython 2.6 you'll need to load "Petri .NET Simulator.sln" into Visual Studio 2005 (or compatible C# compiler) and start "Build". 

To compile version that target NET 4.0 and IronPython 2.7 you'll need to load "Petri .NET Simulator-vs2010.sln" into Visual Studio 2010 (or compatible C# compiler) and start "Build".

To compile both versions you'll also need to download [Libs.zip](https://github.com/larics/Petri.Net/releases/download/v0.0/libs.zip) and unpack it under Libs directory. This zip file contains previously described assemblies that we need to compile and run simulator.
