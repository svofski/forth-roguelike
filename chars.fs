\
\ x x x x  x x x x    - B-FLOOR
\              |_______ B-PASSAGE
\            |_________ B-DOOR
\          |___________ B-THING
\       |______________ B-MONSTER
\     |________________ B-VWALL     
\   ___________________ B-HWALL   
\ _____________________ B-MYSTERY

  0 constant NOTHING

  1 constant B-FLOOR
  2 constant B-PASSAGE
  4 constant B-DOOR
  8 constant B-THING
 16 constant B-MONSTER
 32 constant B-VWALL
 64 constant B-HWALL
128 constant B-VISIBLE


char . constant C-FLOOR
char - constant C-HWALL
char | constant C-VWALL

35 constant C-PASSAGE
43 constant C-DOOR 
62 constant C-EXIT
41 constant C-WEAPON
33 constant C-POTION
63 constant C-SCROLL
92 constant C-WAND
93 constant C-ARMOR
37 constant C-FOOD
65 constant C-MONSTER

\ thing flags ( things.fs }t-flags )
1   constant TF-AIMED       \ monster has life goals
2   constant TF-CHASING     \ monster really means it

create static-items 
here C-WEAPON c, C-POTION c, C-SCROLL c,
     C-WAND c, C-ARMOR c, C-FOOD c, 
here swap - constant |static-items|

: rnd-static-thing ( -- ) 
    |static-items| rnd static-items + c@ ;

: csarray ( there nstrings -- )
    create 
        0 do
            dup , dup c@ 1+ + aligned 
        loop
        drop
    does>
        swap cells + @ dup c@ swap 1+ swap ;

here
    ," aquator"
    ," bat"
    ," centaur" 
    ," dragon" 
    ," emu" 
    ," venus fly-trap" 
    ," griffin" 
    ," hobgoblin" 
    ," ice monster" 
    ," jabberwock" 
    ," kestrel" 
    ," leprechaun" 
    ," medusa" 
    ," nymph" 
    ," orc" 
    ," phantom" 
    ," quagga" 
    ," rattlesnake" 
    ," snake" 
    ," troll" 
    ," black unicorn" 
    ," vampire" 
    ," wraith" 
    ," xeroc" 
    ," yeti" 
    ," zombie" 
26 csarray monster-name

: monster-char
    monster-name drop c@ toupper ;

( monster class to name )
: mcls>name ( cls -- str u )
    [CHAR] A - monster-name ;
    
: rnd-monster-class
    26 rnd ;

: c-visible?
    [ B-VISIBLE ] literal and ;

: c-make-visible
    [ B-VISIBLE ] literal or ;

: [c-make-visible]
    128 postpone literal postpone or ; immediate

: is-door? [ B-DOOR ] literal and ;
: is-pass? [ B-PASSAGE ] literal and ;
: is-floor? [ B-FLOOR ] literal and ;

( not applicable to floors )
: is-exit?  [ C-EXIT ] literal = ;

( if stuff, return c, otherwise 0 )
: (?stuff) ( c -- c|0 )
    [ B-THING B-MONSTER or ] literal and ; 
: is-thing? [ B-THING ] literal and ;
: is-monster? [ B-MONSTER ] literal and ;

: bits2print ( bits -- c )
    dup [ B-VISIBLE ] literal and if 
        dup [ B-FLOOR ] literal and if drop C-FLOOR exit then 
        dup [ B-PASSAGE ] literal and if 
                                     drop C-PASSAGE exit then
        dup [ B-HWALL ] literal and if drop C-HWALL exit then
        dup [ B-VWALL ] literal and if drop C-VWALL exit then
        dup [ B-DOOR ] literal  and if drop C-DOOR  exit then
    then
    drop BL ;
