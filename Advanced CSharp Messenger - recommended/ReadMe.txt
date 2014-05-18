Advanced CSharp Messenger
Author: Ilya Suzdalnitski
http://wiki.unity3d.com/index.php?title=Advanced_CSharp_Messenger

Usage

Event listener

void OnPropCollected( PropType propType ) {
	if (propType == PropType.Life)
		livesAmount++;
}


Registering an event listener

void Start() {
	Messenger.AddListener< Prop >( "prop collected", OnPropCollected );
}


Unregistering an event listener

	Messenger.RemoveListener< Prop > ( "prop collected", OnPropCollected );
	
	
Broadcasting an event

public void OnTriggerEnter(Collider _collider) 
{
	Messenger.Broadcast< PropType > ( "prop collected", _collider.gameObject.GetComponent<Prop>().propType );
}


Cleaning up the messenger

The messenger cleans up its eventTable automatically when a new level loads. This will ensure that the eventTable of the messenger gets cleaned up and will save us from unexpected MissingReferenceExceptions. In case you want to clean up manager's eventTable manually, there's such an option by calling Messenger.Cleanup();


Permanent messages

If you want a certain message to survive the Cleanup, mark it with Messenger.MarkAsPermanent(string). This may be required if a certain class responds to messages broadcasted from across different levels.

Misc

Log all messages
For debugging purposes, you can set the shouldLogAllMessages flag in Messenger to true. This will log all calls to the Messenger.
Transition from other messengers
To quickly change all calls to messaging system from older CSharpMessenger's to the advanced, do the following steps:
In MonoDevelop go to Search => Replace in files
In Find field enter: Messenger<([^<>]+)>.([A-Za-z0-9_]+)
In Replace field enter: Messenger.$2<$1>
Select scope: Whole solution.
Check the Regex search check box.
Press the Replace button
Code

There're two files required for the messenger to work - Callback.cs and Messenger.cs.