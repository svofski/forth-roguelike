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

1 constant PF-BLIND 
create player-flags 0 ,

: pf-blind?
    player-flags @ PF-BLIND and ;

: is-door? c-char C-DOOR = ;
: is-exit? c-char C-EXIT = ;
: is-pass? c-char C-PASSAGE = ;

: p-y 1- swap 1- swap ;
: p-k 1- ;
: p-u 1- swap 1+ swap ;
: p-h swap 1- swap ;
: p-l swap 1+ swap ;
: p-b swap 1- swap 1+ ;
: p-j 1+ ;
: p-n 1+ swap 1+ swap ;

: apply-adjacent ( xt x y -- )
    rot >R
    2dup p-y R@ execute 2dup p-k R@ execute 2dup p-u R@ execute
    2dup p-h R@ execute 2dup     R@ execute 2dup p-l R@ execute
    2dup p-b R@ execute 2dup p-j R@ execute      p-n R@ execute 
    R> drop ;

: of-happenings?
    dcellyx@-c
        dup is-door? swap
        dup is-exit? swap
        drop
    or ;

 : can-@-go? 
    dcellyx@-c
             dup C-FLOOR = if drop true exit then
             dup C-PASSAGE = if drop true exit then
             dup is-door? if drop true exit then
             dup is-exit? if drop true exit then
             drop false ; 

: diag-nogo? ( x y -- true|false )
    dcellyx@-c is-door? ;
   
: try-move-@ ( x y -- true|false )
    2dup can-@-go? 
    if rogue-xy p-xy! 
        true
    else 2drop 
        false 
    then ;

: try-move-@-diag ( x y -- true|false )
    rogue-xy p-xy@ diag-nogo? if 2drop false exit then

    2dup can-@-go? -rot 2dup diag-nogo? not 3 roll and 
    if rogue-xy p-xy! 
        true
    else 2drop 
        false 
    then ;

: stop-if-running
    repeat-state @ RS-REPEAT-RUN = if
        2dup of-happenings? if
            repeat-state off
        then
    then ;

: lightup-any ( x y -- ) 
    stop-if-running
    dcellyx-make-visible ;

: lightup-pass-only ( x y -- )
    2dup dcellyx@ 
    dup is-pass? swap is-door? or 
    if
        stop-if-running
        dcellyx-make-visible exit 
    then
    2drop ;

: should-light-room? ( rn -- true|false )
    dup room-lit? swap room-shown? not and ;

: repaint-room ( rn -- )
    dup room-topleft rot room-bottomright invalidate ;

: show-entire-room ( rn -- )
    dup paint-room-visible
    dup room-lit! 
    dup room-shown! 
        repaint-room ;

: light-spot ( -- )
    pf-blind? if 
        rogue-xy p-xy@ dcellyx-make-visible
        exit 
    then

    rogue-xy p-xy@ 
    2dup dcellyx@ is-pass? if
        ['] lightup-pass-only -rot apply-adjacent
        exit
    then
    2dup dcellyx@ is-door? if
        2dup xy-find-room
        dup should-light-room? if
            show-entire-room
            2drop
            exit
        then
        drop
    then
    ['] lightup-any -rot apply-adjacent ;

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
    rogue-xy p-xy@ rot execute try-move-@ light-spot ;

: xt-walk-diag
    rogue-xy p-xy@ rot execute try-move-@-diag light-spot ;

: walk-h ['] p-h xt-walk ;
: walk-l ['] p-l xt-walk ;
: walk-k ['] p-k xt-walk ;
: walk-j ['] p-j xt-walk ;
: walk-y ['] p-y xt-walk-diag ;
: walk-u ['] p-u xt-walk-diag ;
: walk-b ['] p-b xt-walk-diag ;
: walk-n ['] p-n xt-walk-diag ;

: walk->
    rogue-xy p-xy@ dcellyx@-c C-EXIT =
    if
        ++level
        true
    else 
        false
    then ;

: @-invalidate ( -- )
    rogue-xy p-xy@ 2dup p-y 2swap p-n invalidate ;

: @-> ( -- )
    rogue-xy p-xy@ 
        vtxy [CHAR] @ emit ;

: stats->
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
    time&date + + + + + $ffff and lfsr !
    allot-dungeon
    0play ;
