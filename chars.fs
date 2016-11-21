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

create static-items 
here C-WEAPON c, C-POTION c, C-SCROLL c,
     C-WAND c, C-ARMOR c, C-FOOD c, 
here swap - constant |static-items|

: rnd-static-thing ( -- ) 
    |static-items| rnd static-items + c@ ;

: 2, , , ;

: mk-monster-names
    create 
        s" aquator" 2,
        s" bat" 2,
        s" centaur" 2,
        s" dragon" 2,
        s" emu" 2,
        s" venus fly-trap" 2,
        s" griffin" 2,
        s" hobgoblin" 2,
        s" ice monster" 2,
        s" jabberwock" 2,
        s" kestrel" 2,
        s" leprechaun" 2,
        s" medusa" 2,
        s" nymph" 2,
        s" orc" 2,
        s" phantom" 2,
        s" quagga" 2,
        s" rattlesnake" 2,
        s" snake" 2,
        s" troll" 2,
        s" black unicorn" 2,
        s" vampire" 2,
        s" wraith" 2,
        s" xeroc" 2,
        s" yeti" 2,
        s" zombie" 2,
    does> swap 2* cells + 2@ ;

mk-monster-names monster-name

: monster-char
    monster-name drop c@ toupper ;
