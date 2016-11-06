require rooms.fs
require tstack.fs

: point@ 
    create c, c, 
    does> dup c@ swap 1+ c@ swap ;

: point
    create c, c,
    does> dup 1+ ;

0 constant NOWAY

create con-sets 10 cells allot
does> swap cells + ;

variable trunk-set \ see con-sets

create (con-edges) 20 allot
does> swap + ;
variable #edge 

: init-con-sets 
    10 1 do 
        i room-exists? if i else 0 then i con-sets ! loop ;

: init-edges
    #edge off
    20 0 do 0 i (con-edges) ! loop ;

: edge+! 
    4 lshift or #edge dup rot rot @ (con-edges) c! 1 swap +! ;

: edge@
    (con-edges) c@ dup 15 and swap 4 rshift ;

: edge-!
    #edge dup @ if -1 swap +! then ;

: dump-sets
    cr ." trunk=" trunk-set @ . cr
    10 1 do
        i con-sets @ .
        i 1- 3 mod 2 = if cr then
    loop 

    #edge @ if
    #edge @ 0 do
        i edge@ '(' emit . . ')' emit
    loop cr then ;

: room-trunk? con-sets @ trunk-set @ = ;

: room-trunk! con-sets trunk-set @ swap ! ;

\ return true if the room is nonexistent or has been visited before
: cannot-go? ( i -- b )
    dup room-exists? invert swap room-trunk? or ;

\ Find where we can go starting from given room number.
\ Possible directions are defined as a table of hex numbers.
\ The number of results is variable: 0..4.
( rn -- directions )
hex
: make-could-go
    create hex 0 , 24 , 135 , 26 , 157 , 2468 , 359 , 48 , 579 , 68 , decimal
    does> swap cells + @ 
        begin
            dup f and dup cannot-go? if drop else swap then
            4 rshift dup 0= until drop
;            
decimal

make-could-go where?

\ pick start room and make it trunk
: pick-start ( -- i ) 
    0 ( put a dummy to drop )
    begin drop 9 rnd1+ 
        dup room-exists? over room-thru? invert and
    until
    dup trunk-set ! ;

\ pick a random room to go to
: go-to ( from -- to )
    depth >r where? depth r@ - 1+ 
        dup 0> if rnd pick else NOWAY then
        depth r> - 0 do swap drop loop ;

\ prune the edge that connects a dead-end thru room
: prune-last ( rn -- )
    #edge @ if 
        dup                     ( rn rn -- )
        #edge @ 1- edge@        ( rn rn a b -- )
        drop = if 
            -1 #edge +!
            room-nexisteplus!
        else
            drop
        then then ;

: traverse
    pick-start
    begin
        ( rn -- )
        dup go-to dup if 
            ( cur next -- )  
            dup rot dup 
            >tstack edge+! 
            dup room-trunk!
            \ push & keep exploring ( next -- )
        else
            drop dup                ( cur next -- cur cur )
            room-thru? if dup prune-last then drop
            \ back track to where we can branch or doneski
            tsavail? if tstack> else exit then
        then
    again ;

: conn?
    true
    10 1 do
        drop
        i con-sets @ dup 0= swap trunk-set @ = or invert 
            if false leave then
        true
    loop ;

: render-passages
    #edge @ if
        #edge @ 0 do
            i edge@ connect-2rooms
        loop then ;

: linked-rooms
    C-NOTHING dclear
    make-rooms
    begin 
        init-con-sets init-edges
        traverse conn? if exit then   \ all connected, done
        10 1 do i con-sets @ 0= if 
            coinflip if i room-thru then
        then loop
    again ;

: level
    linked-rooms render-passages render-rooms ;
