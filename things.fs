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

current-offset off
1 soffset@ }t-room@
1 soffset@ }t-x@
1 soffset@ }t-y@
1 soffset@ }t-class@

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

\ Return pointer to thing e.g. 0 thing[] }t-room@
: thing[] ( n -- thing )
    (things) @ ; 

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
    
: things-clear
    nthings off
    0 (things) THINGS-SIZEOF erase
    0 (things-data) THINGSDATA-SIZEOF erase ;

: find-inspoint ( rn -- index )
    THINGS-MAX 0 do
        i (things) @ ?dup if
            ( rn thing )
            }t-room c@ over > if
                drop i leave
            then
        else
            drop i leave
        then
    loop ;

: gap-at ( index -- )
    dup nthings @ >= if drop exit then 
    1+ nthings @ do
        i 1- (things) @ i (things) !
    -1 +loop 
    ;

: squeeze-at ( thing index -- )
    dup gap-at
        (things) ! ;

: squeeze-in ( thing rn -- )
    find-inspoint  
    squeeze-at ;

: thing-new ( rn -- )
    dup >R
    nthings @ (things-data) dup
    }t-room rot swap c! ( -- thing )
    R> squeeze-in 
    1 nthings +! ;

\ meh, not gonna work
: thing-del ( idx -- )
    thing[] }t-room 0 swap c! ;

: search ( x -- i )
    >R
    0 nthings @ 1- ( initial a b )
    begin
        2dup + 2/  ( a b -- a b m )
        dup thing[] }t-room@ ( a b m r[m] )
        dup R@ = if
            drop -rot 2drop
            R> drop exit
        then
        R@ > if             \ x < r[m]
            nip  ( a b m -- a m )
            2dup = if 2drop -1 R> drop exit then
        else
            1+ swap rot drop
            2dup > if 2drop -1 R> drop exit then
        then
    again ;

