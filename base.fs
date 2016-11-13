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


