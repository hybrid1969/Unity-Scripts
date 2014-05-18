// Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
//
// Inspired by and based on Rod Hyde's Messenger:
// http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger
//
// This is a C# messenger (notification center). It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other. The major improvement from Hyde's implementation is that
// there is more extensive error detection, preventing silent bugs.
//
// Usage example:
// Messenger<float>.AddListener("myEvent", MyEventHandler);
// ...
// Messenger<float>.Broadcast("myEvent", 1.0f);
 
 
using System;
using System.Collections.Generic;
 
public enum MessengerMode {
	DONT_REQUIRE_LISTENER,
	REQUIRE_LISTENER,
}
 
 
static internal class MessengerInternal {
	static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
	static public readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;
 
	static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded) {
		if (!eventTable.ContainsKey(eventType)) {
			eventTable.Add(eventType, null);
		}
 
		Delegate d = eventTable[eventType];
		if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}
 
	static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
		if (eventTable.ContainsKey(eventType)) {
			Delegate d = eventTable[eventType];
 
			if (d == null) {
				throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
			} else if (d.GetType() != listenerBeingRemoved.GetType()) {
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
		} else {
			throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
		}
	}
 
	static public void OnListenerRemoved(string eventType) {
		if (eventTable[eventType] == null) {
			eventTable.Remove(eventType);
		}
	}
 
	static public void OnBroadcasting(string eventType, MessengerMode mode) {
		if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType)) {
			throw new MessengerInternal.BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
		}
	}
 
	static public BroadcastException CreateBroadcastSignatureException(string eventType) {
		return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
	}
 
	public class BroadcastException : Exception {
		public BroadcastException(string msg)
			: base(msg) {
		}
	}
 
	public class ListenerException : Exception {
		public ListenerException(string msg)
			: base(msg) {
		}
	}
}
 
 
// No parameters
static public class Messenger {
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);	
		eventTable[eventType] = (Callback)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType) {
		Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
	}
 
	static public void Broadcast(string eventType, MessengerMode mode) {
		MessengerInternal.OnBroadcasting(eventType, mode);
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback callback = d as Callback;
			if (callback != null) {
				callback();
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}
 
// One parameter
static public class Messenger<T> {
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback<T> handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback<T> handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType, T arg1) {
		Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
	}
 
	static public void Broadcast(string eventType, T arg1, MessengerMode mode) {
		MessengerInternal.OnBroadcasting(eventType, mode);
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback<T> callback = d as Callback<T>;
			if (callback != null) {
				callback(arg1);
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}
 
 
// Two parameters
static public class Messenger<T, U> {
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback<T, U> handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback<T, U> handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType, T arg1, U arg2) {
		Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
	}
 
	static public void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode) {
		MessengerInternal.OnBroadcasting(eventType, mode);
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback<T, U> callback = d as Callback<T, U>;
			if (callback != null) {
				callback(arg1, arg2);
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}
 
 
// Three parameters
static public class Messenger<T, U, V> {
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback<T, U, V> handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback<T, U, V> handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType, T arg1, U arg2, V arg3) {
		Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
	}
 
	static public void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode) {
		MessengerInternal.OnBroadcasting(eventType, mode);
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback<T, U, V> callback = d as Callback<T, U, V>;
			if (callback != null) {
				callback(arg1, arg2, arg3);
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}
MessengerUnitTest.cs
// MessengerUnitTest.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
// 
// Some functionality testing of the classes in Messenger.cs.
// A lot of attention is paid to proper exception throwing from the Messenger.
 
using System;
 
class MessengerUnitTest {
 
	private readonly string eventType1 = "__testEvent1";
	private readonly string eventType2 = "__testEvent2";
 
	bool wasCalled = false;
 
	public void RunTest() {
		RunAddTests();
		RunBroadcastTests();
		RunRemoveTests();
		Console.Out.WriteLine("All Messenger tests passed.");
	}
 
 
	private void RunAddTests() {
		Messenger.AddListener(eventType1, TestCallback);
 
		try {
			// This should fail because we're adding a new event listener for same event type but a different delegate signature
			Messenger<float>.AddListener(eventType1, TestCallbackFloat);
			throw new Exception("Unit test failure - expected a ListenerException");
		} catch (MessengerInternal.ListenerException e) {
			// All good
		}
 
		Messenger<float>.AddListener(eventType2, TestCallbackFloat);
	}
 
 
	private void RunBroadcastTests() {
		wasCalled = false;
		Messenger.Broadcast(eventType1);
		if (!wasCalled) { throw new Exception("Unit test failure - event handler appears to have not been called."); }
		wasCalled = false;
		Messenger<float>.Broadcast(eventType2, 1.0f);
		if (!wasCalled) { throw new Exception("Unit test failure - event handler appears to have not been called."); }
 
		// No listener should exist for this event, but we don't require a listener so it should pass
		Messenger<float>.Broadcast(eventType2 + "_", 1.0f, MessengerMode.DONT_REQUIRE_LISTENER);
 
		try {
			// Broadcasting for an event there exists listeners for, but using wrong signature
			Messenger<float>.Broadcast(eventType1, 1.0f, MessengerMode.DONT_REQUIRE_LISTENER);
			throw new Exception("Unit test failure - expected a BroadcastException");
		}
		catch (MessengerInternal.BroadcastException e) {
			// All good
		}
 
		try {
			// Same thing, but now we (implicitly) require at least one listener
			Messenger<float>.Broadcast(eventType2 + "_", 1.0f);
			throw new Exception("Unit test failure - expected a BroadcastException");
		} catch (MessengerInternal.BroadcastException e) {
			// All good
		}
 
		try {
			// Wrong generic type for this broadcast, and we implicitly require a listener
			Messenger<double>.Broadcast(eventType2, 1.0);
			throw new Exception("Unit test failure - expected a BroadcastException");
		} catch (MessengerInternal.BroadcastException e) {
			// All good
		}
 
	}
 
 
	private void RunRemoveTests() {
 
		try {
			// Removal with wrong signature should fail
			Messenger<float>.RemoveListener(eventType1, TestCallbackFloat);
			throw new Exception("Unit test failure - expected a ListenerException");
		}
		catch (MessengerInternal.ListenerException e) {
			// All good
		}
 
		Messenger.RemoveListener(eventType1, TestCallback);
 
		try {
			// Repeated removal should fail
			Messenger.RemoveListener(eventType1, TestCallback);
			throw new Exception("Unit test failure - expected a ListenerException");
		}
		catch (MessengerInternal.ListenerException e) {
			// All good
		}
 
 
 
		Messenger<float>.RemoveListener(eventType2, TestCallbackFloat);
 
		try {
			// Repeated removal should fail
			Messenger<float>.RemoveListener(eventType2, TestCallbackFloat);
			throw new Exception("Unit test failure - expected a ListenerException");
		}
		catch (MessengerInternal.ListenerException e) {
			// All good
		}
	}
 
 
	void TestCallback() {
		wasCalled = true;
		Console.Out.WriteLine("TestCallback() was called.");
	}
 
	void TestCallbackFloat(float f) {
		wasCalled = true;
		Console.Out.WriteLine("TestCallbackFloat(float) was called.");
 
		if (f != 1.0f) {
			throw new Exception("Unit test failure - wrong value on float argument");
		}
	}
 
 
 
}