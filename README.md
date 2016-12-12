# forth-rogue
This is an experiment in creating a believable Rogue clone that runs from single source on a CP/M-80 computer 
and a modern PC. It is most likely that it would never go past purely experimental phase. 

When reading about Forth I frequently see comments how the beginners dont "get" it. This project is my platform
to, hopefully, get it. If you are a Forth person, you get it and you see how I don't, please leave me enlightening comments.

## Status
The main development platforms are DXFORTH for CP/M-80 and gforth on PC.

Status:
 * maze generator
 * ability to walk around the dungeon and downstairs
 * rooms can be lit up and dark
 * running and repeat count
 * static things are added to the dungeon, they can't be used but they already have the data structures
 * pointlessly wandering monsters

### About performance and things
I regularly check the speed on the target CP/M machine, which is a 3MHz 8080 computer with a lot of waitstates
and emulated text mode. The raw screen output speed of it is estimated at about 700 cps. When I first saw
the running speed on it I had to reconsider a few things. Now I try to be performance-conscious with the code
that I write. 

  Forth seems to hold some middle ground between compiled and interpreted languages. On a 8080 CPU, a C compiler
  would often have a very hard time using the resources efficiently. It is reasonable to expect Forth performance
  to be comparable with compiled code because of this, on this particular platform. But being comparable with
  a poor C compiler on a difficult machine does not necessarily mean that it is fast.

This made me spend a bit of time building a slightly ugly structure to keep the things on the level. A monster
is also a thing. Because lookaround must happen as quickly as possible: searching for 8 things around the @
every move would be prohibitively slow. So there are a few things to make searching faster:
 * where there is a thing, the floor is marked with C-FLOOR+THING in the dungeon (it's a comma)
 * things are stored in array (t-data)
 * the array is indexed by a linked list made of (data-index,next) tuples, each tuple occupies one cell
 * an array of 24 bytes maps y-coordinate to the head of its corresponding list
 
Considering how small the number of things usually is (3-10 tops, although sometimes there are many monsters, or the @ might wish to dump all his possessions onto the floor), the structure seems more elaborate than it should be. 

## How to
### On a modern pc (Linux, macOS, Windows, ... )
```
$ gforth main.fs
play
```

### On CP/M
Obtain a copy of DX-Forth for CP/M-80 from http://dxforth.netbay.com.au/#dxforth
If you plan to run it on a real machine, you should be knowing what you're
doing then better than I do. 

I run it in a simulation under aliados-0.1.1 http://ninsesabe.arrakis.es/aliados/

My target CP/M-80 machine has a VT-52 compatible terminal. On a modern
PC there are several alternatives:
 * edit main.fs and change vt.fs to vt100.fs - this will switch to vt100 commands
 * xterm -ti vt52 -tn vt52 for GUI terminal
 * Qodem emulates vt52 in modern text terminals like macOS Terminal

```
$ aliados forth.com
include main.fs
play
```

To create an executable:
```
$ aliados forth.com
include main.fs
turnkey play rog.com
```


