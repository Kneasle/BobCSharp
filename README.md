# BobC# (In development)
A C# library which can quickly and easily run bellringing-related queries.



## Quickstart
First things first, import the library to your code: 
```C#
using Bob;
```


### Changes and Place Notations
Let's create a new change:
```C#
Change change = new Change ("13524");

int third_bell = change [2]; // ==> 4 (because this treble is bell #0, bell #4 is the 5)
Parity parity = change.parity; // ==> Parity.Odd
int order = change.order; // ==> 4
```

. . . or some place notation:
```C#
PlaceNotation notation = new PlaceNotation ("145", Stage.Doubles);
```

Implicit places are automatically filled in:
```C#
PlaceNotation lazy_notation = new PlaceNotation ("4", Stage.Doubles); ==> 145
```

Stages work up to twenty-two:
```C#
Change rounds = Change.Rounds (Stage.Doubles); // ==> 12345
Change big_rounds = Change.Rounds (Stage.Nonuples); // ==> 1234567890ETABCDFGH
Change bigger_rounds = Change.Rounds (Stage.TwentyTwo); // ==> 1234567890ETABCDFGHIJK
```

Suppose you wanted to transpose a change by another change, or by some place notation:
```C#
Change plain = new Change ("15738264"); // Cambridge Major's lead end
Change bob = new Change ("13578264"); // Cambridge Major's lead end after a bob

Change plain_then_bob = plain * bob; // ==> 17864523
Change bob_then_plain = bob * plain; // ==> 18654327

PlaceNotation notation = new PlaceNotation ("14", Stage.Major);
Change transposed_change = rounds * notation; // ==> 17532846
```




### Methods
Let's create a new method from its place notation
```C#
Method plain_bob_major = new Method ("Plain", Catagory.Bob, Stage.Triples, "x18x18x18x18,12");
```

Standard bobs and singles and plain calls are created automagically
```C#
List<Call> calls = plain_bob_major.calls; /* ==> 
[
	Call {
		name: "Bob",
		place_notations: [PlaceNotation ("14")]
	},
	Call {
		name: "Single",
		place_notations: [PlaceNotation ("1234")]
	},
	Call () {
		name: "Plain",
		place_notations: [null]
	}
]
*/
```

Some common methods have their own static properties:
```C#
Method grandsire_triples = Method.grandsire_triples;
Method cambridge_major = Method.cambridge_major;
```

**(TODO)** You can get any method from the CCCBR's method library (also comes with the standard calls):
```C#
Method stedman_triples = MethodLibrary.GetMethodByName ("Stedman Triples");
```

You only need a name, stage and any place notation to make a method (BobC# can classify methods)
```C#
Method cambridge_surprise_minor = new Method ("Cambridge", "x3x4x2x3x4x5,2", Stage.Minor);
```





### Touches
Any piece of ringing is a **Touch** object:
```C#
Touch touch = new Touch ();
```

Let's suppose we wanted to look at a plain course of Plain Bob Doubles ('cos we we've all been there at some point):
```C#
Touch plain_course = Method.plain_bob_doubles.plain_course;

Change change_no_3 = plain_course [2]; // ==> 24153

int length = plain_course.length; // ==> 40
bool is_true = plain_course.is_true; // ==> true
bool is_extent = plain_course.is_extent; // ==> false
```

. . . or a basic 120 of plain bob doubles:
```C#
Touch touch = Method.plain_bob_doubles.TouchFromCallList ("MMMB");

int length = plain_course.length; // ==> 120
bool is_true = plain_course.is_true; // ==> true
bool is_extent = plain_course.is_extent; // ==> true
```
