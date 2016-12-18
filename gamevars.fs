0 0 point rogue-xy          \ rogue xy location
create rogue-room 0 ,       \ the room rogue is in
0 0 point last-door         \ last disappeared through this door

: roguexy@ rogue-xy p-xy@ ;

: can-@-go? ( x y -- true|false )
    dcellyx@ 
        dup [ B-MONSTER ] literal and (feckoff)
        [ B-FLOOR B-PASSAGE B-DOOR B-THING or or or ] literal
        and ;

: can-M-go? ( x y -- true|false )
    2dup roguexy@ d= if 2drop false exit then
    can-@-go? ;
