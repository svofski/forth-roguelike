: game.fs ;

create dlevel 0 ,           \ current dungeon level
create quit-game 0 ,        \ termination flag
create run-flag 0 ,         \ e.g. shift-hjkl
create repeat-count 0 ,     \ number prefix before command
create repeat-command 0 , 
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

: test-around-@
    >R
    false
    rogue-xy p-xy@ p-y R@ execute or
    rogue-xy p-xy@ p-k R@ execute or
    rogue-xy p-xy@ p-u R@ execute or
    rogue-xy p-xy@ p-h R@ execute or
    rogue-xy p-xy@ p-l R@ execute or
    rogue-xy p-xy@ p-b R@ execute or
    rogue-xy p-xy@ p-j R@ execute or
    rogue-xy p-xy@ p-n R@ execute or
    rogue-xy p-xy@     R@ execute or
    R> drop ;

: apply-adjacent ( xt x y -- )
  3dup p-y rot execute 3dup p-k rot execute 3dup p-u rot execute
  3dup p-h rot execute 3dup     rot execute 3dup p-l rot execute
  3dup p-b rot execute 3dup p-j rot execute      p-n rot execute 
  ;

: of-happenings?
    dcellyx@-c
        dup is-door? swap
        dup is-exit? swap
        drop
    or ;

: should-stop-running?
    ['] of-happenings? test-around-@ ;

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

: lightup-any ( x y -- ) 
    dcellyx-make-visible ;

: lightup-pass-only ( x y -- )
    2dup dcellyx@ 
    dup is-pass? swap is-door? or 
    if
        dcellyx-make-visible exit 
    then
    2drop ;

: should-light-room? ( rn -- true|false )
    dup room-lit? swap room-shown? not and ;

: show-entire-room ( rn -- )
    dup paint-room-visible
    dup room-lit! 
        room-shown! ;

: repaint-room ( rn -- )
    dup room-topleft
    rot room-bottomright
    dprint-small-area ;

: light-spot ( -- )
    pf-blind? if 
        rogue-xy p-xy@ dcellyx-make-visible
        exit 
    then

    rogue-xy p-xy@ 
    2dup dcellyx@ is-pass? if
        ['] lightup-pass-only -rot apply-adjacent
    else
        2dup xy-find-room 
        dup should-light-room? if
            dup show-entire-room
            repaint-room
            2drop
        else
            drop 
            ['] lightup-any -rot apply-adjacent 
        then
    then ;

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
        drop light-spot
    then
    dprint-vt ;

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

: @->
    rogue-xy p-xy@ 
        2dup 
        2dup p-y 2swap p-n dprint-small-area 
        vtxy [CHAR] @ emit ;

: .->
    rogue-xy p-xy@ 2dup vtxy dcellyx@-c emit ;

: stats->
    0 24 vtxy ." Dlvl: " dlevel @ . 
    ." depth:" depth . ." R:" repeat-count @ . ;

\ true if ok, false if couldn't go
: dispatch-cmd ( char -- true|false )
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
    dup 12  = if page dprint-vt false exit then
    dup 27  = if repeat-count off false exit then
    false
    ; 

: 0play
    dlevel off
    quit-game off
    repeat-count off
    ++level
    begin
        @-> stats->
        repeat-command @ ?dup not if
        run-flag @ ?dup not if
            key dup isupper?  swap tolower swap 
            if dup run-flag ! then
        then then
        \ .->  -- no need to
        dup isdigit? if 
            repeat-count @ 3276 < if 
                atoi repeat-count @ 10 * + repeat-count !
            else drop then
        else
            dup repeat-command !

            dispatch-cmd swap drop ( t|f -- ) 

            dup repeat-count @ and if
                    repeat-count @ 1- dup not if
                        repeat-command off
                    then repeat-count ! 
                else 
                    repeat-command off
                then

            dup not if 
                repeat-count off
                repeat-command off
            then
            not should-stop-running? or if
                run-flag off
            then
        then
    quit-game @ until ;

: time&date 1234 56 78 99 11 666 ;

: play
    time&date + + + + + $ffff and lfsr !
    0play ;
