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
    dup room-centre
        dup ROOM_HH rnd - swap
            ROOM_HH rnd +
        rot dup ROOM_HW rnd - swap
                ROOM_HW rnd +   ( rn y1 y2 x1 x2 -- )
    4 roll (rooms)              ( y1 y2 x1 x2 ptr -- )
    swap over }x2 c!            ( y1 y2 x1 ptr -- )
    swap over }x1 c!
    swap over }y2 c!
    swap over }y1 c!
    0 swap }flags c! ;

: room-thru ( rn -- )
    dup room-centre rot (rooms)
        over over }y1 c!
        over over }y2 c!
        swap drop over over }x1 c! 
        dup rot swap }x2 c!
        }flags dup @ RF-THRU RF-EXISTS or or swap c! ;

: room-width ( rn -- width )
    (rooms) dup }x2 c@ swap }x1 c@ - ;

: room-height ( rn -- height )
    (rooms) dup }y2 c@ swap }y1 c@ - ;

\ validate room rn, update }flags, 
\ return true if exists, false otherwise
: room-validate ( rn -- exists )
    dup
    dup room-width 3 > swap
        room-height 2 > and
    dup ( rn flags -- rn flags flags )
    if swap (rooms) }flags RF-EXISTS swap c!  
    else swap drop then ;

: room-exists?  (rooms) }flags@ RF-EXISTS and 0<> ;

: room-thru? (rooms) }flags@ RF-THRU and 0<> ;

: room-nexisteplus!
    (rooms) }flags dup @ RF-EXISTS RF-THRU or invert and swap ! ;

: room-trunk? (rooms) }flags@ RF-TRUNK and 0 <> ;
: room-trunk! (rooms) }flags dup @ RF-TRUNK or swap ! ;
: room-cut!   (rooms) }flags dup @ RF-TRUNK invert and swap ! ;

: room-lit?   (rooms) }flags@ RF-LIT and 0 <> ;
: room-shown? (rooms) }flags@ RF-SHOWN and 0 <> ;
: room-flags+! (rooms) }flags dup @ rot or swap ! ;
: room-lit!   RF-LIT swap room-flags+! ;
: room-shown! RF-SHOWN swap room-flags+! ;

: room-make ( rn -- )
    dup (rooms) }flags @ RF-EXISTS and 0= if
        dup rnd-room
            room-validate
        else drop false then ;

: dump-room ( rn -- )
    ." ("
    (rooms) dup }x1@ . ." , "
            dup }y1@ . ." )-("
            dup }x2@ . ." , "
            dup }y2@ . ." ):"
                }flags@ hex . decimal ;

: dump-rooms
    10 1 do 
        i dump-room
    loop ;

: make-rooms 
    0 (rooms) ROOM-SIZEOF 10 * 0 fill
    4 rnd 3 + >R
    0 0 ( nrooms i -- )
    begin
        dup 1+ 9 mod swap       ( nrooms i+1 i -- )
        1+ room-make            ( nrooms i+1 exists -- )
        if swap 1+ swap then    ( room a success, nrooms 1+ )
    over R@ >= until
    R> drop drop drop ( drop limit, drop counters ) ;

: fill-room ( rn -- )
    C-FLOOR swap (rooms) dup 
            c@ swap 1+ dup
            c@ swap 1+ dup
            c@ swap 1+ 
            c@ 
            dfillx ;

: paint-room-visible ( rn -- )
    (rooms) dup c@ swap 1+
            dup c@ swap 1+
            dup c@ swap 1+
                c@ dfill-visible ;

: +door! ( rn x y -- )
    3dup 24 > if abort" y > 80" then
         80 > if abort" x > 80" then
         9 > if abort" rn > 9" then
    rot (rooms) >R
        R@ }n-doors dup c@ dup 1+ rot c! ( -- x y &room ndoors )
        2* R@ }doors + dup 1+            ( -- x y &door &door+1)
        rot swap c! c! R> drop ;

: door[] ( rn n -- x y ) 
    swap (rooms) }doors swap 2* + dup c@ swap 1+ c@ ;

: room-topleft ( rn -- x1 y1 )
    (rooms) dup c@ swap 1+ c@ ;
: room-topright ( rn -- x2 y1 )
    (rooms) dup 2 + c@ swap 1+ c@ ;
: room-bottomleft ( rn -- x1 y2 )
    (rooms) dup c@ swap 3 + c@ ;
: room-bottomright ( rn -- x2 y2 )
    (rooms) dup 2 + c@ swap 3 + c@ ;

: room-border ( rn -- )
    dup [CHAR] / swap room-topleft     dcellyx!
    dup [CHAR] \ swap room-topright    dcellyx!
    dup [CHAR] \ swap room-bottomleft  dcellyx!
    dup [CHAR] / swap room-bottomright dcellyx!

    dup [CHAR] - swap 
        dup room-topleft rot room-topright drop dlineh
    dup [CHAR] - swap 
        dup room-bottomleft rot room-bottomright drop dlineh
    dup [CHAR] | swap 
        dup room-topleft rot room-bottomleft swap drop dlinev
    dup [CHAR] | swap 
        dup room-topright rot room-bottomright swap drop dlinev

    drop ;

: room-doors ( rn -- )
    dup (rooms) }n-doors@ 
        dup 0> if 
            1- 0 swap do 
                dup C-DOOR swap i 
                    door[] 
                dcellyx!
            -1 +loop
            drop
        else drop drop
        then ;

: render-room ( rn -- )
    >R
    R@ room-thru? if
        C-PASSAGE R@ (rooms) dup c@ swap 1+ c@ dcellyx!
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
    C-PASSAGE 3 roll 3 roll 3 roll dlineh ;

: vpass ( x1 y1 y2 -- )
    C-PASSAGE 3 roll 3 roll 3 roll dlinev ;

: conn-h ( left right -- )
    dup door-h 3dup +door! rot drop rot
    dup door-l 3dup +door! rot drop 2swap

    3 pick 2 pick any-between       ( x1 y1 x2 y2 m -- )
    4 pick 4 pick 2 pick 1+ hpass   \ x1,y1 - mx,y1 
    dup    4 pick 3 pick vpass      \ mx,y1 - mx,y2
    1- swap rot hpass               \ mx,y2 - x2,y2
    drop drop ;

: conn-v ( top bottom -- ) 
    dup door-k 3dup +door! rot drop rot
    dup door-j 3dup +door! rot drop 2swap

    2 pick over any-between         ( x1 y1 x2 y2 m -- )
    4 pick 4 pick 2 pick 1+ vpass   \ x1,y1 - x1,m
    4 pick over 4 pick hpass        \ x1,m - x2,m 
    2 pick over 3 pick vpass        \ x2,m - x2,y2
    1- swap vpass 
    drop drop ;

: connect-2rooms ( r1 r2 -- )
    2dup  ( r1 r2 r1 r2 -- )
    rn-xy ( r1 r2 r1 x2 y2 -- )
    rot
    rn-xy ( r1 r2 x2 y2 x1 y1 -- )
    swap drop rot drop = if  ( y1 == y2 )
        2dup > if swap then 
        conn-h
    else 
        2dup > if swap then
        conn-v
    then ;

: xy-in-room ( x y rn -- true|false )
    3dup ( x y rn x y rn -- )
    room-topleft rot swap   ( x y rn x tlx y tly -- )
    >= -rot >= and          ( x y rn cond1 -- )
    not if 3drop false exit then
    room-bottomright rot swap
    <= -rot <= and ;

: xy-find-room ( x y -- rn )
    0 -rot                  ( rn x y -- )
    10 1 do
        2dup i xy-in-room if
            rot drop i -rot leave
        then
    loop 
    2drop ;

: x-rnd-in-room
    dup (rooms) }x1@ swap (rooms) }x2@ any-between ;

: y-rnd-in-room
    dup (rooms) }y1@ swap (rooms) }y2@ any-between ;

: somewhere-in-room ( rn -- x y )
    dup x-rnd-in-room swap y-rnd-in-room ;

