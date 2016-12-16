0 constant NOWAY

: 20array 
    create 20 cells allot
    does> swap + ;
20array (con-edges)
variable n-edge 

variable start-room 
variable exit-room
variable farthest

: init-tree ( -- )
    10 1 do
        i room-cut! loop ;

: init-edges ( -- )
    n-edge off
    20 0 do 0 i (con-edges) ! loop ;

: edge+! ( edge -- )
    4 lshift or n-edge dup -rot @ (con-edges) c! 1 swap +! ;

: edge@ ( en -- edge )
    (con-edges) c@ dup 15 and swap 4 rshift ;

\ remove last edge
: edge-! ( -- ) 
    n-edge dup @ if -1 swap +! then ;

: dump-sets
    cr
    10 1 do
        i room-trunk? if [CHAR] T emit 
        else i room-thru? if [CHAR] + emit
        else i room-exists? if [CHAR] 0 emit
        else [CHAR] _ emit then then then
        1 spaces
        i 1- 3 mod 2 = if cr then
    loop 

    n-edge @ if
    n-edge @ 0 do
        i edge@ [CHAR] ( emit . . [CHAR] ) emit
    loop cr then ;

\ return true if the room is nonexistent or was visited before
: cannot-go? ( i -- b )
    dup room-exists? invert swap room-trunk? or ;

\ Find where we can go starting from given room number.
\ Possible directions are defined as a table of hex numbers.
\ The number of results is variable 0..4, 
\ they are room numbers 1..9.
( rn -- directions )
: make-could-go
    create , , , , , , , , , ,
    does> swap cells + @ 
        begin
            dup 15 and dup cannot-go? if drop else swap then
            4 rshift dup 0= until drop ;            

hex 68 579 48 359 2468 157 26 135 24 0 decimal
make-could-go where? 

\ pick start room and make it trunk
: pick-start ( -- i ) 
    0 ( put a dummy to drop )
    100 0 do
        9 rnd1+ 
        dup room-exists? over room-thru? not and
        if nip leave then
        drop
    loop ;

\ pick a random room to go to
: go-to ( from -- to )
    depth >r where? depth r@ - 1+ 
        dup 0> if rnd pick else NOWAY then
        depth r> - 0 do nip loop ;

\ prune the edge that connects a dead-end thru room
: prune-last ( rn -- )
    n-edge @ if 
        dup                         ( rn rn -- )
        n-edge @ 1- edge@           ( rn rn a b -- )
        drop = if 
            -1 n-edge +!
            room-nexisteplus!
        else
            drop
        then 
    then ;

: farthest?! ( rn -- )
    tsdepth farthest @ > if
        tsdepth farthest !
        exit-room !
        exit 
    then
    drop ;

: push&go ( cur next -- )
    dup rot dup 
    >tstack edge+!
    dup room-trunk!
    dup room-thru? invert if
        dup farthest?!
    then ;

: back&track ( cur next -- )
    drop dup                ( cur next -- cur cur )
    room-thru? if dup prune-last then drop
    tstack> ;    ( -- last-position )

\ pick a random room and walk the maze, create edges as we go
: build-tree
    0 farthest !
    pick-start 
    dup 0= if drop exit then        \ could not pick
    dup room-trunk! dup start-room ! dup exit-room !
    begin
        ( rn -- )
        dup go-to dup if 
            push&go
        else
            back&track ?dup not if exit then
        then
    again ;

\ return true if the rooms are all connected
: tree-complete?
    10 1 do
        i room-exists? if
            i room-trunk? invert if
                R> R> 2drop ( unloop )
                false exit
            then
        then
    loop
    true ;

: render-passages
    n-edge @ if
        n-edge @ 0 do
            i edge@ connect-2rooms
        loop then ;

: add-some-thru
    10 1 do i room-exists? invert if 
        coinflip if i room-thru then 
    then loop ;

: linked-rooms
    C-NOTHING dclear
    make-rooms
    begin 
        init-tree 
        init-edges 
        build-tree 
        tree-complete? if exit then 
        add-some-thru
    again ;

: place-thing ( rn cls -- )
    swap somewhere-in-room ( cls x y )
    3dup thing-new }t-class c! 
    rot drop C-FLOOR+THING -rot dcellyx! ;

: place-monster ( rn monster -- )
    swap somewhere-in-room ( cls x y )
    3dup rot C-MONSTER + -rot 
    thing-new }t-class c!
    rot drop C-FLOOR+MONSTER -rot dcellyx! ;

: add-junk ( rn -- )
    dup rnd-static-thing place-thing 
    coinflip if 
        rnd-monster-class place-monster
    else
        drop
    then ;

: foreach-room
    10 1 do
        i room-exists? if 
            i room-thru? not if
                dup i swap execute 
        then then
    loop drop ;

: place-things ( -- )
    things-clear
    ['] add-junk foreach-room ;

: level
    linked-rooms render-passages render-rooms 
    place-things
    exit-room @ C-EXIT place-thing ;
