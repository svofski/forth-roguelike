: game.fs ;                 \ for easy forgetting

create dlevel 0 ,           \ current dungeon level
create quit-game 0 ,        \ termination flag

1 constant RS-REPEAT-RUN
2 constant RS-REPEAT-COUNT

create repeat-state 0 ,     \ 0, RS-REPEAT-RUN, RS-REPEAT-COUNT
create repeat-count 0 ,     \ number prefix before command
create repeat-command 0 ,   \ valid for repeat and run
create current-room 0 ,
0 0 point rogue-xy

: roguexy@ rogue-xy p-xy@ ;

1 constant PF-BLIND 
create player-flags 0 ,

: pf-blind?
    player-flags @ [ PF-BLIND ] literal and ;

: is-door? c-char [ C-DOOR ] literal = ;
: is-pass? c-char [ C-PASSAGE ] literal = ;
: is-floor? c-char [ C-FLOOR ] literal = ;
: is-thing? c-char [ C-FLOOR+THING ] literal = ;
: is-exit? c-char [ C-EXIT ] literal = ;
: is-monster? c-char isupper? ;

\ xt are ( ptr -- )
: apply-adjacent-a ( xt x y -- )
    p-y dcellyx 
    (dungeon) +    \ get top-left pointer
       2dup swap execute ( xt a -- )
    1+ 2dup swap execute
    1+ 2dup swap execute
    [ COLS 2 - ] literal + 2dup swap execute
    1+ 2dup swap execute
    1+ 2dup swap execute
    [ COLS 2 - ] literal + 2dup swap execute
    1+ 2dup swap execute
    1+      swap execute ;

: should-stop-because? ( c -- true|false )
    dup is-door? swap
        is-thing? or ; 

: can-@-go? 
    2dup
    dcellyx@ 
        dup is-floor? if 3drop true exit then
        dup is-pass? if 3drop true exit then
        dup is-door? if 3drop true exit then
        dup is-thing? if 
            drop
            char@xy dup is-monster? if drop false exit then
            drop true exit 
        then
        drop false ; 

: diag-nogo? ( x y -- true|false )
    dcellyx@ is-door? ;
   
: try-move-@ ( x y -- true|false )
    2dup can-@-go? 
    if 
        rogue-xy p-xy! true
    else 2drop 
        false 
    then ;

: try-move-@-diag ( x y -- true|false )
    2dup can-@-go? not if 2drop false exit then \ can go at all
    roguexy@ diag-nogo? if 2drop false exit then \ now on + ?
    2dup diag-nogo? not                         \ target not +
    if 
        rogue-xy p-xy! true
    else 
        2drop false 
    then ;

: lightup-any-a ( ptr -- ) 
    dup c@ 
    repeat-state @ [ RS-REPEAT-RUN ] literal = if
        dup should-stop-because? if
            repeat-state off
        then
    then 
    c-make-visible swap c! ;

: lightup-pass-only-a ( ptr -- )
    dup c@
    dup is-pass? over is-door? or
    if
        c-make-visible swap c! exit
    then 
    drop drop ;

: should-light-room? ( rn -- true|false )
    dup room-lit? swap room-shown? not and ;

: repaint-room ( rn -- )
    dup room-topleft rot room-bottomright invalidate ;

: show-entire-room ( rn -- )
    dup paint-room-visible
    dup room-lit! 
    dup room-shown! 
        repaint-room ;

: show-entire-map ( -- )
    0 0 COLS ROWS dfill-visible
    invalidate-all ;

: light-spot ( -- )
    pf-blind? if 
        roguexy@ dcellyx-make-visible
        exit 
    then

    roguexy@ 2dup dcellyx@ ( x y c -- )
    ( c ) dup is-pass? if
        drop
        ['] lightup-pass-only-a -rot apply-adjacent-a
        exit
    then
    ( c ) dup is-door? if
        ( c) drop
        2dup xy-find-room
        dup should-light-room? if
            show-entire-room
            2drop
            exit
        then
    then
    ( c ) drop
    ['] lightup-any-a -rot apply-adjacent-a ;

: add-lights ( dlvl -- )
    10 1 do 
        i room-exists? if
            dlevel @ dice-for-roomlight if
                i room-lit!
            then
        then
    loop ;

: place-rogue 
    somewhere-in-room rogue-xy p-xy! ;

: ++level 
    1 dlevel +!
    page
    level
    add-lights
    start-room @ current-room ! 
    start-room @ place-rogue
    start-room @ 
    dup should-light-room? if
        show-entire-room
    else 
        drop 
        light-spot
    then ;

: xt-walk
    roguexy@ rot execute try-move-@ light-spot ;

: xt-walk-diag
    roguexy@ rot execute try-move-@-diag light-spot ;

: walk-h ['] p-h xt-walk ;
: walk-l ['] p-l xt-walk ;
: walk-k ['] p-k xt-walk ;
: walk-j ['] p-j xt-walk ;
: walk-y ['] p-y xt-walk-diag ;
: walk-u ['] p-u xt-walk-diag ;
: walk-b ['] p-b xt-walk-diag ;
: walk-n ['] p-n xt-walk-diag ;

: walk->
    roguexy@ char@xy is-exit?
    \ dcellyx@ is-exit?
    if
        ++level
        true
    else 
        false
    then ;

: @-invalidate ( -- )
    roguexy@ 2dup p-y 2swap p-n invalidate ;

: @-> ( -- )
    roguexy@ vtxy [CHAR] @ emit ;

: stats-> ( -- )
    0 24 vtxy ." Dlvl: " dlevel @ . 
    ." depth:" depth . ." R:" repeat-count @ . ;

\ true if ok, false if couldn't go
: dispatch-cmd-in ( char -- true|false )
    dup [CHAR] h = if walk-h exit then
    dup [CHAR] l = if walk-l exit then
    dup [CHAR] j = if walk-j exit then
    dup [CHAR] k = if walk-k exit then
    dup [CHAR] y = if walk-y exit then
    dup [CHAR] u = if walk-u exit then
    dup [CHAR] b = if walk-b exit then
    dup [CHAR] n = if walk-n exit then

    dup [CHAR] > = if walk-> exit then
    dup [CHAR] q = if quit-game on false exit then
    dup 12  = if page invalidate-all false exit then
    dup [CHAR] \ = if show-entire-map false exit then
    dup 27  = if repeat-count off false exit then
    false ; 

: dispatch-command ( -- true|false )
    repeat-command @ dispatch-cmd-in nip ;

: repeat-off
    repeat-state off
    repeat-count off
    repeat-command off ;

: input-repeat ( char -- )
    repeat-command off
    repeat-count @ 1000 < if
        atoi repeat-count @ 10 * + repeat-count !
    else drop then ;

: expect-input ( -- ) 
    key dup isupper? swap tolower swap
    if
        repeat-command !
        RS-REPEAT-RUN repeat-state !
        repeat-count off
    else
        dup isdigit? if
            input-repeat
        else
            repeat-command !
            repeat-state off
            repeat-count @ 0> if
                RS-REPEAT-COUNT repeat-state !
                1 repeat-count +!
            then
        then
    then ;

: cmd-single ( -- )
    dispatch-command 
    drop ;

: cmd-count ( -- )
    repeat-count @ 1- 
    dup not if
        repeat-off
    then 
    repeat-count !
    dispatch-command
    not if
        repeat-off
    then ;

: cmd-run ( -- )
    dispatch-command
    dup not if 
        repeat-off
    then
    drop ;

create RS-DISP ' cmd-single , 
               ' cmd-run ,
               ' cmd-count , 

: 0play
    dlevel off
    quit-game off
    repeat-off
    ++level
    begin
        @-invalidate
        repeat-state @ not if
            dupdate-invalid
            @-> stats->
            expect-input
        then

        RS-DISP repeat-state @ cells + @ execute
    quit-game @ until ;

: time&date 1234 56 78 99 11 666 ;

: play
    time&date + + + + + -1 and lfsr !
    allot-dungeon
    0play ;
