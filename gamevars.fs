0 0 point rogue-xy          \ rogue xy location
create rogue-room 0 ,       \ the room rogue is in
0 0 point last-door         \ last disappeared through this door

\ player stats
2variable   2stat-$         0 0 , ,     \ gold (LONG!)
create      stat-str        16 ,        \ strength
create      stat-strmax     16 ,        \ max strength
create      stat-exp        1 ,         \ experience level
2variable   2stat-exp-pts   0 0 , ,     \ experience points (LONG!)
create      stat-arm        0 ,         \ armor class
create      stat-hp         12 ,        \ hit points
create      stat-hpmax      12 ,        \ max hit points
create      stat-movs       1250 ,      \ moves
create      stat-faint      0 , 

2variable stat-dmg          \ string describing damage done
2variable stat-hunger       \ hungry string

: roguexy@ rogue-xy p-xy@ ;

: can-@-go? ( x y -- true|false )
    dcellyx@ 
        dup [ B-MONSTER ] literal and (feckoff)
        [ B-FLOOR B-PASSAGE B-DOOR B-THING or or or ] literal
        and ;

: can-M-go? ( x y -- true|false )
    2dup roguexy@ d= if 2drop false exit then
    can-@-go? ;

: @-init
    \ todo inventory init
    ;    

: @-count-move
    stat-movs dup @ 1- dup rot ! ( -- stat-movs@ )
    dup HUNGRY = if s" Hungry"   stat-hunger 2! 
        else
    dup WEAK   = if s" Weak"     stat-hunger 2! 
        else
    dup FAINT <= if 
        s" Fainting" stat-hunger 2! 
        1 stat-faint !
        else
        STARVE = if s" Starved"  stat-hunger 2! 
        then then then then 
    ;
