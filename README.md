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

## How to
### On a modern pc (Linux, macOS, Windows, ... )
```
$ gforth gforth-main.fs
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


