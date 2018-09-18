# BobCSharp
A C# library which can quickly and easily run bellringing-related queries.



## Quickstart
First things first, import the library to your code: 
```C#
using Bob;
```

### Changes:
Let's create a new change:
```C#
Change change = new Change ("13524");
```

Suppose you want to know the parity (odd/evenness) of the change
```C#
Parity p = change.parity; // >>> Parity.Odd
```

. . . or you want to know how many times this change can be applied before coming round (this is called the order of a change)
```C#
int o = change.order; // >>> 4
```

Suppose you just wanted rounds on 5:
```C#
Change rounds = Change.Rounds (Stage.Doubles);
```

Suppose you wanted to transpose a change by another change:
```C#
Change plain = new Change ("15738264"); // Cambridge Major's lead end
Change bob = new Change ("13578264"); // Cambridge Major's lead end after a bob

Change plain_then_bob = plain.Transpose (bob); // ==> 17864523
Change bob_then_plain = bob.Transpose (plain); // ==> 18654327
```

. . . or even simpler
```C#
Change plain_then_bob = plain * bob; // ==> 17864523
Change bob_then_plain = bob * plain; // ==> 18654327
```
