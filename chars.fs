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
