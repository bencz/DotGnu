/* This is an example of prototype based object instantiation in jscript
   Also not the member access and dynamic members - and the nice meoows
*/

function say(line)
{
	print("<"+this.name+"> "+ line); 
}

function actor(name) 
{
	this.name = name;
	this.say = say;
} 

var romeo = new actor("romeo");
var juliet = new actor("juliet");

juliet.say("Romeo!");
romeo.say("My dear ?");
juliet.say("At what o'clock to-morrow shall I send to thee?");
romeo.say("At the hour of nine.");
juliet.say("I will not fail: 'tis twenty years till then.");
