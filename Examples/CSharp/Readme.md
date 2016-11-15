**C# Example**
----------

This example shows how to use C# inside Petri.NET simulator. It is a modified version of illustrated example [here](https://github.com/larics/Petri.NET/wiki/15.Using-CSharp)

![enter image description here](https://media.giphy.com/media/l0HlEtFnevmEDaa2Y/source.gif)


**Code Only**
```C#
//C#

//Note : You must use NameID for
//each place you intend to access
//from code

//Create place objects
Place p6=new Place();
Place p5=new Place();
Place p2=new Place();
//Main loop
public void Step(int k)
{
	
	if(k==0)
	{
		//Executed only once
		p6=FindPlace("P6"); // Assign object to existing place
		p5=FindPlace("P5");
		p2=FindPlace("P2");
		Print("P1"+"\t"+"P2"+"\t"+"P6"+"\n");
	}
	
	//Update Output window every 2 steps
	if(k%2==0)
	{
		p6.Tokens=1; //Add one token to place "P1"
		Print(p6.Tokens+"\t"+p2.Tokens+"\t"+p5.Tokens+"\n");
	}
}

//Reset routine
public void Reset()
{
	Print("Simulation Ended...!");
}
```
