**Python Example**
----------

This example shows how to use Python inside Petri.NET simulator. It is a modified version of illustrated example [here](https://github.com/larics/Petri.Net/wiki/14.Using-Python)

![enter image description here](https://media.giphy.com/media/3oriOgzBg6cMxiZ7jO/source.gif)


**Code Only**
```python
#Python Example
def Step(k):
	if k==0:
		print "P2","\t","P7","\t","P5"
	p7=FindPlace("P7")
	p2=FindPlace("P2")
	p5=FindPlace("P5")
	if p2.Tokens >=10:
		p7.Tokens=1
		print p2.Tokens,"\t",p7.Tokens,"\t",p5.Tokens
	
def Reset():
	print "Simulation Ended..."
```
