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
```

. . . or some place notation:
```C#
PlaceNotation notation = new PlaceNotation ("145", Stage.Doubles);
```

Implicit places are automatically filled in:
```C#
PlaceNotation lazy_notation = new PlaceNotation ("4", Stage.Doubles);

bool are_the_same = (lazy_notation == notation); // ==> true
```

Suppose you want to know the parity (odd/evenness) of the change
```C#
Parity p = change.parity; // ==> Parity.Odd
```

. . . or you want to know how many times this change can be applied before coming round (this is called the order of a change)
```C#
int o = change.order; // ==> 4
```

. . . or you just wanted rounds on 5:
```C#
Change rounds = Change.Rounds (Stage.Doubles); // ==> 12345
```

. . . or even rounds on nineteen:
```C#
Change rounds = Change.Rounds (Stage.Nonuples); // ==> 1234567890ETABCDFGH
```

Stages work up to twenty-two:
```C#
Change rounds = Change.Rounds (Stage.TwentyTwo); // ==> 1234567890ETABCDFGHIJK
```

Suppose you wanted to transpose a change by another change:
```C#
Change plain = new Change ("15738264"); // Cambridge Major's lead end
Change bob = new Change ("13578264"); // Cambridge Major's lead end after a bob
```
```C#
Change plain_then_bob = plain.Transpose (bob); // ==> 17864523
Change bob_then_plain = bob.Transpose (plain); // ==> 18654327
```

. . . or even simpler
```C#
Change plain_then_bob = plain * bob; // ==> 17864523
Change bob_then_plain = bob * plain; // ==> 18654327
```

. . . or you wanted to transpose by some place notation
```C#
Change rounds = Change.Rounds (Stage.Maximus);
PlaceNotation notation = new PlaceNotation ("14ET", Stage.Maximus);

Change transpose_change = rounds * notation; // ==> 1324658709ET
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
