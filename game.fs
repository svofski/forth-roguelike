: game.fs ;                 \ for easy forgetting

create dlevel 0 ,           \ current dungeon level
create quit-game 0 ,        \ termination flag
2variable debugmsg

0 0 debugmsg 2!

1 constant RS-REPEAT-RUN
2 constant RS-REPEAT-COUNT

create repeat-state 0 ,     \ 0, RS-REPEAT-RUN, RS-REPEAT-COUNT
create repeat-count 0 ,     \ number prefix before command
create repeat-command 0 ,   \ valid for repeat and run
7 value rogue-speed@        \ baseline speed 7
0 value turn-time@          \ turn time 

1 constant PF-BLIND 
create player-flags 0 ,

1 constant GF-ESTOCADA
0 value game-flags

: pf-blind?
    player-flags @ [ PF-BLIND ] literal and ;

\ xt are ( ptr -- )
: apply-adjacent-a ( xt x y -- )
    p-y dcellyx 
    (dungeon) +    \ get top-left pointer
       2dup swap execute 
    1+ 2dup swap execute
    1+ 2dup swap execute
    [ COLS 2 - ] literal + 
       2dup swap execute
    1+ 2dup swap execute
    1+ 2dup swap execute
    [ COLS 2 - ] literal + 
       2dup swap execute
    1+ 2dup swap execute
    1+      swap execute ;

: should-stop-because? ( c -- true|false )
    [ B-FLOOR B-PASSAGE B-HWALL B-VWALL or or or ] literal
    and not ;

: diag-nogo? ( x y -- true|false )
    dcellyx@ is-door? ;

: advance-time
    \ this probably has to do with running vs monsters thing,
    \ currently unused
    turn-time@ rogue-speed@ + to turn-time@ 
    ;

: update-@-room 
    roguexy@ dcellyx@ is-door? if
        roguexy@ last-door p-xy!
    then
    roguexy@ xy-find-room rogue-room ! ;
   
: try-move-@ ( x y -- true|false )
    2dup can-@-go? 
    if 
        rogue-xy p-xy! true
        update-@-room
        advance-time
        @-count-move
    else 
        game-flags GF-ESTOCADA or to game-flags
        2drop 
        false 
    then ;

: try-move-@-diag ( x y -- true|false )
    roguexy@ diag-nogo? if 2drop false exit then \ now on + ?
    2dup diag-nogo? not                          \ target not +
    if 
        try-move-@
    else
        2drop false
    then ;

( these ops are invoked a lot, especially when running )
( they should be as quick as possible )
: lightup-any-a ( ptr -- ) 
    dup c@ 
    dup should-stop-because? if
        repeat-state off
    then
    [c-make-visible] swap c! ;

: lightup-pass-only-a ( ptr -- )
    \ 1 %debugcount +!
    dup c@ 
    dup [ B-PASSAGE B-DOOR or ] literal and if
        [c-make-visible] swap c! exit
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
    start-room @ rogue-room ! 
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

\ walk commands, return true if success false otherwise 
: walk-h ['] p-h xt-walk ;
: walk-l ['] p-l xt-walk ;
: walk-k ['] p-k xt-walk ;
: walk-j ['] p-j xt-walk ;
: walk-y ['] p-y xt-walk-diag ;
: walk-u ['] p-u xt-walk-diag ;
: walk-b ['] p-b xt-walk-diag ;
: walk-n ['] p-n xt-walk-diag ;
: rest ['] p-. xt-walk ;

: walk->
    roguexy@ char@xy is-exit?
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

\ print single-cell number without trailing space
: .# ( n -- )
    0 <# #s #> type ;

\ print doublel-cell number without trailing space
: d.# ( d -- )
    <# #s #> type ; 

: stats-> ( -- )
    0 24 vtxy ." Level: " dlevel @ . 
    ." Gold: " 2stat-$ 2@ d.# space
    ." Hp: " stat-hp @ .# [CHAR] ( emit stat-hpmax @ .# [CHAR] ) emit
    space
    ." Str: " stat-str @ .# [CHAR] ( emit stat-strmax @ .# [CHAR] ) emit
    space
    ." Arm: " stat-arm @ . 
    ." Exp: " stat-exp @ .# [CHAR] / emit 2stat-exp-pts 2@ d.
    stat-hunger 2@ type
    space
    \ debug print
    debug-on
    ." stack:" depth . ." R:" repeat-count @ . 
    ." dcnt:" %debugcount @ .
    ." rn:" rogue-room @ .
    ." m:" stat-movs @ .
    debugmsg 2@ type 
    debug-off
    clreol 
    0 0 debugmsg 2! ;

: cmd-debug-magic
    ['] mons-aim-rnd mons-foreach 
    false ;
: cmd-quit  quit-game on false ;
: cmd-refresh page invalidate-all false ;
: cmd-show-entire-map ( -- )
    0 0 COLS ROWS dfill-visible
    invalidate-all 
    false ;
: cmd-dprint dprint false ;
: cmd-esc repeat-count off false ;

\ create command dispatch table: 
\ char dispatch char dispatch ..
create (cmds)
    CHAR h c, ' walk-h ,
    CHAR l c, ' walk-l ,
    CHAR j c, ' walk-j ,
    CHAR k c, ' walk-k ,
    CHAR y c, ' walk-y ,
    CHAR u c, ' walk-u ,
    CHAR b c, ' walk-b ,
    CHAR n c, ' walk-n ,
    CHAR . c, ' rest ,
    ( vector-06c arrow keys )
    8 c, ' walk-h ,
    24 c, ' walk-l ,
    26 c, ' walk-j ,
    25 c, ' walk-k ,

    CHAR > c, ' walk-> ,

    CHAR q c, ' cmd-quit ,
    12 c,       ' cmd-refresh , 
    CHAR \ c, ' cmd-show-entire-map ,
    CHAR ] c, ' cmd-debug-magic ,
    CHAR / c, ' cmd-dprint ,
    27 c,       ' cmd-esc ,
    0 c,

: (cmd-find) ( char -- xt )
    >R
    (cmds) 
    begin
        dup c@ dup
    while
        ( cmds cmd-key )
        R@ = if
            1+ @
            R> drop
            exit
        then
        [ 1 1 cells + ] literal + 
    repeat 
    drop R> drop 0 ;

( true if ok, false if couldn't go )
: (dispcmd) ( char -- true|false )
    (cmd-find) ?dup if
        execute
    else
        drop
        false
    then ;

: dispatch-command ( -- true|false )
    repeat-command @ 
    (dispcmd)
    dup not if
        game-flags GF-ESTOCADA and if
            s" NUTSKICK" debugmsg 2!
            game-flags GF-ESTOCADA invert and to game-flags
        then
    then 
    monsters-turn ;

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
    @-init
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
