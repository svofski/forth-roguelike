1   constant RF-EXISTS    ( room exists and valid )
2   constant RF-CON       ( room is in connected set )
4   constant RF-THRU      ( room is a thru for xing )
8   constant RF-TRUNK     ( room is in the spanning tree )
16  constant RF-LIT       ( room is instantly visible )
32  constant RF-SHOWN     ( room is already shown )

14 constant ROOM-SIZEOF

current-offset off
    1 soffset }x1
    1 soffset }y1
    1 soffset }x2
    1 soffset }y2
    1 soffset }flags
    1 soffset }n-doors
    8 soffset }doors
current-offset off
    1 soffset@ }x1@
    1 soffset@ }y1@
    1 soffset@ }x2@
    1 soffset@ }y2@
    1 soffset@ }flags@
    1 soffset@ }n-doors@
    8 soffset@ }doors@

: struct-array ( n len -- ) ( i -- addr)
    create  dup ,  * allot 
    does>   dup @ rot * + ; 

: room-array 
    create ROOM-SIZEOF * allot
    does>  swap ROOM-SIZEOF * + ;

10 room-array (rooms)

COLS 6 / constant ROOM_HW
ROWS 6 / constant ROOM_HH

: room-centre ( rn -- x y )
    1- dup
        3 mod 2 * 1 + ROOM_HW * swap
        3 / 2 * 1 + ROOM_HH * ;

: rnd-room ( rn -- )
    dup (rooms) swap
        room-centre ( -- &r cx cy )
        over ROOM_HW rnd -  ( -- &r cx cy x1 )
        over ROOM_HH rnd -  ( -- &r cx cy x1 y1 )
        2swap
        swap ROOM_HW rnd +
        swap ROOM_HH rnd +  ( -- &r x1 y1 x2 y2 )

    4 pick rect!    
    0 swap }flags c! ;
        
: room-thru ( rn -- )
    dup (rooms) swap room-centre 2dup 4 pick rect!
        }flags 
        dup @ [ RF-THRU RF-EXISTS or ] literal or swap c! ;

: room-width ( rn -- width )
    (rooms) rect-width ;

: room-height ( rn -- height )
    (rooms) rect-height ;

\ validate room rn, update }flags, 
\ return true if exists, false otherwise
: room-validate ( rn -- exists )
    dup
    dup room-width 3 > swap
        room-height 2 > and
    dup ( rn flags -- rn flags flags )
    if swap (rooms) }flags RF-EXISTS swap c!  
    else nip then ;

: room-exists?  (rooms) }flags@ RF-EXISTS and ( 0<> ) ;

: room-thru? (rooms) }flags@ RF-THRU and ( 0<> ) ;

: room-nexisteplus!
    (rooms) }flags dup @ 
        [ RF-EXISTS RF-THRU or invert ] literal and swap ! ;

: room-trunk? (rooms) }flags@ RF-TRUNK and ( 0<> ) ;
: room-trunk! (rooms) }flags dup @ RF-TRUNK or swap ! ;
: room-cut!   (rooms) }flags dup @ RF-TRUNK invert and swap ! ;

: room-lit?   (rooms) }flags@ RF-LIT and ( 0<> ) ;
: room-shown? (rooms) }flags@ RF-SHOWN and ( 0<> ) ;
: room-flags+! (rooms) }flags dup @ rot or swap ! ;
: room-lit!   RF-LIT swap room-flags+! ;
: room-shown! RF-SHOWN swap room-flags+! ;

: room-make ( rn -- )
    dup room-exists? not if
        dup rnd-room
            room-validate
        else drop false then ;

: dump-room ( rn -- )
    (rooms) dup dump-rect
    [CHAR] : emit }flags@ hex . decimal ;

: dump-rooms
    10 1 do 
        i dump-room
    loop ;

: make-rooms 
    0 (rooms) [ ROOM-SIZEOF 10 * ] literal 0 fill
    4 rnd 3 + >R
    0 0 ( nrooms i -- )
    begin
        dup 1+ 9 mod swap       ( nrooms i+1 i -- )
        1+ room-make            ( nrooms i+1 exists -- )
        if swap 1+ swap then    ( room a success, nrooms 1+ )
    over R@ >= until
    R> 3drop ( drop limit, drop counters ) ;

: fill-room ( rn -- )
    B-FLOOR swap (rooms) dfillrect ;

: paint-room-visible ( rn -- )
    (rooms) rect@ dfill-visible ;

: +door! ( rn x y -- )
    \ 3dup 24 > if abort" y > 80" then
    \    80 > if abort" x > 80" then
    \    9 > if abort" rn > 9" then
    rot (rooms) >R
        R@ }n-doors dup c@ dup 1+ rot c! ( -- x y &room ndoors )
        2* R@ }doors + dup 1+            ( -- x y &door &door+1)
        rot swap c! c! R> drop ;

: door[] ( rn n -- x y ) 
    swap (rooms) }doors swap 2* + dup c@ swap 1+ c@ ;

: room-topleft ( rn -- x1 y1 )
    (rooms) rect-topleft ;
: room-topright ( rn -- x2 y1 )
    (rooms) rect-topright ; 
: room-bottomleft ( rn -- x1 y2 )
    (rooms) rect-bottomleft ; 
: room-bottomright ( rn -- x2 y2 )
    (rooms) rect-bottomright ; 

: room-border ( rn -- )
    B-HWALL swap        ( '-' rn )
    2dup room-topleft     dcellyx!
    2dup room-topright    dcellyx!
    2dup room-bottomleft  dcellyx!
    2dup room-bottomright dcellyx!
    2dup dup room-topleft rot room-topright drop dlineh
    2dup dup room-bottomleft rot room-bottomright drop dlineh
    nip B-VWALL swap    ( '|' rn )
    2dup dup room-topleft rot room-bottomleft nip dlinev
         dup room-topright rot room-bottomright nip dlinev ;

: room-doors ( rn -- )
    dup (rooms) }n-doors@ 
        dup 0> if 
            1- 0 swap do 
                dup B-DOOR swap i 
                    door[] 
                dcellyx!
            -1 +loop
            drop
        else 2drop
        then ;

: render-room ( rn -- )
    >R
    R@ room-thru? if
        B-PASSAGE R@ (rooms) dup c@ swap 1+ c@ dcellyx!
    else 
        R@ room-exists? if
            R@ fill-room 
            R@ room-border
            R@ room-doors
        then
    then 
    R> drop ;

: render-rooms ( -- )
    10 1 do i render-room loop ;

\ room number to x y position in 3x3 grid
\ room number rn n in range 1..9 --> x y in range 0..2
: rn-xy ( rn -- x y )
    1- dup 3 / swap 3 mod swap ;

: door-y ( rn -- y )
    dup room-height 
        dup 0> if 2 - rnd 1 + swap (rooms) }y1@ + 
               else drop (rooms) }y1@ then ;

: door-x ( rn -- x)
    dup room-width 
        dup 0> if 2 - rnd 1 + swap (rooms) }x1@ + 
               else drop (rooms) }x1@ then ;

: door-l ( rn -- x y )
    dup door-y swap (rooms) }x2@ swap ;

: door-h ( rn -- x y )
    dup door-y swap (rooms) }x1@ swap ;

: door-j ( rn -- x y )
    dup door-x swap (rooms) }y2@ ;

: door-k ( rn -- x y )
    dup door-x swap (rooms) }y1@ ;

: hpass ( x1 y1 x2 -- )
    B-PASSAGE 3 roll 3 roll 3 roll dlineh ;

: vpass ( x1 y1 y2 -- )
    B-PASSAGE 3 roll 3 roll 3 roll dlinev ;

: conn-h ( left right -- )
    dup door-h 3dup +door! rot drop rot
    dup door-l 3dup +door! rot drop 2swap

    3 pick 2 pick any-between       ( x1 y1 x2 y2 m -- )
    4 pick 4 pick 2 pick 1+ hpass   \ x1,y1 - mx,y1 
    dup    4 pick 3 pick vpass      \ mx,y1 - mx,y2
    1- swap rot hpass               \ mx,y2 - x2,y2
    2drop ;

: conn-v ( top bottom -- ) 
    dup door-k 3dup +door! rot drop rot
    dup door-j 3dup +door! rot drop 2swap

    2 pick over any-between         ( x1 y1 x2 y2 m -- )
    4 pick 4 pick 2 pick 1+ vpass   \ x1,y1 - x1,m
    4 pick over 4 pick hpass        \ x1,m - x2,m 
    2 pick over 3 pick vpass        \ x2,m - x2,y2
    1- swap vpass 
    2drop ;

: connect-2rooms ( r1 r2 -- )
    2dup  ( r1 r2 r1 r2 -- )
    rn-xy ( r1 r2 r1 x2 y2 -- )
    rot
    rn-xy ( r1 r2 x2 y2 x1 y1 -- )
    nip rot drop = if  ( y1 == y2 )
        2dup > if swap then 
        conn-h
    else 
        2dup > if swap then
        conn-v
    then ;

: xy-find-room ( x y -- rn )
    2dup
    [ ROOM_HH 2* ] literal / swap [ ROOM_HW 2* ] literal / 
    swap 3 * + 1+  ( x y rn -- )
    dup room-thru? if 3drop 0 exit then ( thru rooms not cool )
    dup >r (rooms) xy-in-r? 
        r> swap not if drop 0 then ;

: x-rnd-in-room ( rn -- x )
    dup (rooms) }x1@ swap (rooms) }x2@ any-between ;

: y-rnd-in-room ( rn -- y )
    dup (rooms) }y1@ swap (rooms) }y2@ any-between ;

: somewhere-in-room ( rn -- x y )
    dup x-rnd-in-room swap y-rnd-in-room ;

