80 constant COLS
24 constant ROWS

: COLS* 4 lshift dup 2 lshift + ;
: [COLS*] 4 postpone literal postpone lshift postpone dup 
    2 postpone literal postpone lshift postpone + ; immediate

: COLS+ postpone COLS postpone + ; immediate

[DEFINED] not 0= [IF]
: not if false else true then ;
: gforth true ;
[ELSE]
: gforth false ;
[THEN]

: >= < 0= ;
: <= > 0= ;
: 3drop drop drop drop ;
: 3dup 2 pick 2 pick 2 pick ;

hex
: isupper?  [char] A - 1A u< ;
: islower?  [char] a - 1A u< ;
: tolower  dup isupper? 20 and + ;
: toupper 
    [DEFINED] upcase [IF] upcase 
    [ELSE]
        dup isupper? not 20 and -  
    [THEN] ;
: isdigit?  [char] 0 - 0A u< ;
: atoi 30 - ;
decimal 

( parse colon-separated string ) 
( leave left part if true, right part if false )
: truehalf 
    2dup [CHAR] : scan nip - ;
: falsehalf
    [CHAR] : scan 1- swap 1+ swap ;
: s:" ( b -- c-addr u1 )
    [CHAR] " parse postpone SLiteral
    postpone rot postpone if
        postpone truehalf
    postpone else
        postpone falsehalf
    postpone then ; immediate

variable current-offset
: soffset ( n -- ) ( addr -- addr')
    create current-offset dup @ dup , 2 roll + swap !
    does> @ + ;

: soffset@ 
    create current-offset dup @ dup , 2 roll + swap !
    does> @ + c@ ;

: point@ 
    create c, c, 
    does> dup c@ swap 1+ c@ swap ;

: point
    create c, c,
    does> dup 1+ ;
: p-xy@ ( point -- x y )
    c@ swap c@ ;
: p-xy! ( x y py px -- )
    >R rot R> c! c! ; 

: p-y 1- swap 1- swap ;
: p-k 1- ;
: p-u 1- swap 1+ swap ;
: p-h swap 1- swap ;
: p-l swap 1+ swap ;
: p-b swap 1- swap 1+ ;
: p-j 1+ ;
: p-n 1+ swap 1+ swap ;

: rect
    create swap 2 roll 3 roll c, c, c, c, ;

: rect@
    dup c@ swap 1+ dup c@ swap 1+ dup c@ swap 1+ c@ ;

current-offset off
    1 soffset@ }rx1@
    1 soffset@ }ry1@
    1 soffset@ }rx2@
    1 soffset@ }ry2@
current-offset off
    1 soffset }rx1
    1 soffset }ry1
    1 soffset }rx2
    1 soffset }ry2

: rect! ( x1 y1 x2 y2 r1 -- )
    3 + dup             ( x1 y1 x2 y2 r1+3 r1+3 )
    rot swap c! 1- dup  ( x1 y1 x2 r1+3 r1+3 )
    rot swap c! 1- dup  ( x1 y1 r1+3 r1+3 )
    rot swap c! 1-      ( x1 r1+3 )
             c! ;

: rect-add ( x1 y1 x2 y2 r1 -- )
    dup }ry2@            ( x1 y1 x2 y2 r1 r1y2 -- )
    rot max over }ry2 c! ( x1 y1 x2 r1 -- )
    dup }rx2@            ( x1 y1 x2 r1 r1x2 -- )
    rot max over }rx2 c! ( x1 y1 r1 -- )
    dup }ry1@            ( x1 y1 r1 r1y1 -- )
    rot min over }ry1 c! ( x1 r1 -- )
    dup }rx1@            ( x1 r1 r1x1 -- )
    rot min swap }rx1 c! ; 

: rect-eq? ( x1 y1 x2 y2 r1 -- t|f )
    dup }rx1@ 5 roll =  ( y1 x2 y2 r1 = -- )
    over }ry1@ 5 roll = ( x2 y2 r1 = = -- )
    2 pick }rx2@ 5 roll = ( y2 r1 = = = -- )
    3 roll }ry2@ 4 roll =
    and and and ;

: rect-topleft ( r -- x1 y1 )
    dup c@ swap 1+ c@ ;
: rect-topright ( r -- x2 y1 )
    dup 2 + c@ swap 1+ c@ ;
: rect-bottomleft ( r -- x1 y2 )
    dup c@ swap 3 + c@ ;
: rect-bottomright ( r -- x2 y2 )
    dup 2 + c@ swap 3 + c@ ;

( ensure that x1,y1 is left/above of x2,y2 )
: norm4 ( x1 y1 x2 y2 -- x1' y1' x2' y2' )
    rot 2dup > if swap then
    2swap 2dup > if swap then
    ( y1' y2' x1' x2' )
    2swap
    ( x1' x2' y1' y2' -- x1' y1' x2' y2' )
    rot swap ;

: rect-width ( r1 -- h )
    dup }rx2@ swap }rx1@ - ;

: rect-height ( r1 -- h )
    dup }ry2@ swap }ry1@ - ;

: (inrec-x) ( x r1 -- b )
    swap over }rx1@ over ( r1 x r1x1 x )
    > if 2drop false exit then
    swap }rx2@ ( x r1x2 )
    <= ;

: (inrec-y) ( y r1 -- b )
    swap over }ry1@ over ( r1 y r1y1 y )
    > if 2drop false exit then
    swap }ry2@ ( x r1y2 )
    <= ;

: xy-in-r? ( x y r1 -- true|false )
    rot over (inrec-x) if
        (inrec-y)
    else
        2drop false
    then ;

: dump-rect ( r1 -- )
    ." (" dup }rx1@ . dup }ry1@ 0 .r
    [CHAR] - emit
    dup }rx2@ . }ry2@ 0 .r ." )";

( if condition, drop tos and exit from the callee with false )
: ?false/~
    if 
        drop false
        R> drop
    then ;
( if condition, drop tos and exit from the callee with true )
: ?true/~
    if 
        drop true
        R> drop
    then ;

( drop tos and return false )
: (feckoff) ( x -- false )
    postpone if 
        postpone drop 0 postpone literal 
        postpone exit 
    postpone then ; immediate

create %debugcount 0 ,
