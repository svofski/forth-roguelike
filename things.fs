\ thing:
\   room number
\   x
\   y
\   class : 
\       % food 
\       $ gold 
\       ] armour 
\       ) weapon 
\       ! potion 
\       ? scroll 
\       > exit
\       A-Za-z monstar
\   attrib :
\       $80: always shown
\       $40: reserved
\       class-specific $3f
\           $ quantity
\           % satiation
\   flags
\   hp
\   extra1
current-offset off
1 soffset }t-room
1 soffset }t-x
1 soffset }t-y
1 soffset }t-class

8 constant THING-SIZEOF
20 constant THINGS-MAX 
THING-SIZEOF THINGS-MAX * constant THINGSDATA-SIZEOF
THINGS-MAX cells constant THINGS-SIZEOF 
: mkthings-data
    create THINGS-SIZEOF allot
    does> swap 3 lshift + ;

mkthings-data (things-data) 

: mkthings
    create THINGS-SIZEOF allot 
    does> swap cells + ;

mkthings (things)
variable nthings

: things-clear
    nthings off
    0 (things) THINGS-SIZEOF erase
    0 (things-data) THINGSDATA-SIZEOF erase ;

: find-index ( rn -- index )
    THINGS-MAX 0 do
        i (things) ?dup if
            ( rn ) dup }t-room @ < if
                drop i leave
            then
        else
            drop i leave
        then
    loop ;

: thing-new ( rn -- )
    \ dup >R
    nthings @ (things-data) dup
    }t-room rot swap c! ( -- thing )

    \ find index to insert new thing
    \ R@ find-index

    nthings @ (things) .s ! 
    1 nthings +! ;

: dump-thing ( ptr -- )
    }t-room @ ." R:" . ;

: dump-things-data
    THINGS-MAX 0 do
        i (things-data) dup }t-room c@ if
             dump-thing 
        else
            drop
            leave
        then
    loop ;

: dump-things
    THINGS-MAX 0 do
        i (things) @ ?dup if
            dump-thing
        else
            leave
        then
    loop ;
