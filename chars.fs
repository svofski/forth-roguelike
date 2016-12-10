32 constant C-NOTHING
46 constant C-FLOOR 
44 constant C-FLOOR+THING       \ , says to look into things
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
    128 and ;

: c-make-visible
    128 or ;

: c-char
    127 and ;

: c-char@
    c@ 127 and ;

: is-door? c-char [ C-DOOR ] literal = ;
: is-pass? c-char [ C-PASSAGE ] literal = ;
: is-floor? c-char [ C-FLOOR ] literal = ;
: is-thing? c-char [ C-FLOOR+THING ] literal = ;
: is-exit? c-char [ C-EXIT ] literal = ;
: is-monster? c-char isupper? ;


