: >= 2dup = rot rot > or ;
: <= 2dup = rot rot < or ;
: 3drop drop drop drop ;

hex
: isupper?
    dup 41 >= swap 5A <= and ;
: tolower
    dup isupper? if 20 + then ;
: isdigit?
    dup 30 >= swap 39 <= and ;
: atoi
    30 - ;
decimal 

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

: dump-rect ( r1 -- )
    dup }rx1@ . dup }ry1@ . dup }rx2@ . }ry2@ . ;
    
