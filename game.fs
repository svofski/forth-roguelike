require vt.fs
require level.fs

create dlevel 0 ,
: ++level 
    1 dlevel +!
    page
    level
    dprint ;

: can-@-go? 
    dcellyx@ 
             dup C-FLOOR = if drop true exit then
             dup C-PASSAGE = if drop true exit then
             dup C-DOOR = if drop true exit then
             dup C-EXIT = if drop true exit then
             drop false ; 

: try-move-@
    2dup can-@-go? 
    if rogue-xy p-xy! 
        true
    else 2drop 
        false 
    then ;

: walk-h
    rogue-xy p-xy@ swap 1- swap try-move-@ ;

: walk-l
    rogue-xy p-xy@ swap 1+ swap try-move-@ ;

: walk-k
    rogue-xy p-xy@ 1- try-move-@ ;

: walk-j
    rogue-xy p-xy@ 1+ try-move-@ ;

: walk-y
    rogue-xy p-xy@ 1- swap 1- swap try-move-@ ;

: walk-u
    rogue-xy p-xy@ 1- swap 1+ swap try-move-@ ;

: walk-b
    rogue-xy p-xy@ 1+ swap 1- swap try-move-@ ;

: walk-n
    rogue-xy p-xy@ 1+ swap 1+ swap try-move-@ ;

: walk->
    rogue-xy p-xy@ dcellyx@ C-EXIT =
    if
        ++level
        true
    else 
        false
    then ;


: @->
    rogue-xy p-xy@ vtxy '@' emit ;

: .->
    rogue-xy p-xy@ 2dup vtxy dcellyx@ emit ;

: stats->
    0 24 vtxy ." Dlvl: " dlevel @ . ;

: 0play
    0 dlevel !
    ++level
    begin
        @-> stats->
        key 
        .-> 
        dup 'h' = if walk-h drop then
        dup 'l' = if walk-l drop then
        dup 'j' = if walk-j drop then
        dup 'k' = if walk-k drop then
        dup 'y' = if walk-y drop then
        dup 'u' = if walk-u drop then
        dup 'b' = if walk-b drop then
        dup 'n' = if walk-n drop then
        dup '>' = if walk-> drop then
        drop
    again ;

: play
    time&date + + + + + $ffff and lfsr !
    0play ;
