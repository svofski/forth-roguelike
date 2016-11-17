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

1   constant TC-MONSTER
2   constant TC-SCROLL
3   constant TC-POTION
4   constant TC-FOOD
5   constant TC-GOLD
6   constant TC-ARMOUR
7   constant TC-WEAPON


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

8 constant |THING|
20 constant THINGS-MAX 
|THING| THINGS-MAX * constant |THINGSDATA|

\ linked list using indices:
\   msb = things-data idx   msb = next or 0
THINGS-MAX cells constant |THINGSLIST| 

\ 10 cells ( 0 for roomless, 1 - 9 ) for entry points
\ of thing lists

\ actual things data, unlinked
: mkthings-data
    create |THINGSDATA| allot
    does> swap 3 lshift + ;
: mkthings-list
    create |THINGSLIST| allot 
    does> swap cells + ;
: mkthings-byroom
    create 10 allot
    does> swap + ;

: car>data+next dup 8 rshift swap 255 and ;    
: data+next>car swap 8 lshift or ;

mkthings-data (t-data) 
mkthings-list (t-list)
mkthings-byroom (t-byroom)

variable nthings

\ access thing-data by index
: thing-data[] ( n -- thing )
    (t-data) ; 

: dump-thing ( ptr -- )
    ." R:" dup }t-room@ .
    ." x:" dup }t-x@ .
    ." y:" dup }t-y@ .
    ." cls:" dup }t-class@ .
    drop ; 

: dump-things-data
    THINGS-MAX 0 do
        i (t-data) dup }t-room c@ if
            dump-thing 
        else
            drop
            leave
        then
    loop ;

: things-clear
    nthings off
    0 (t-list) |THINGSLIST| erase
    0 (t-data) |THINGSDATA| erase 
    0 (t-byroom) 10 255 fill ;

\ pointer to newly create thing
: newtdata ( -- t-data )
    nthings @ (t-data) ;
: newcar ( -- car )
    nthings @ ;

: to-room ( data-idx rn -- )
    dup >R
    (t-byroom) c@ ( dataidx caridx -- )
    data+next>car ( car ) 
    nthings @ (t-list) ! ( store car in new t-list cell )
    nthings @ R> (t-byroom) c! ( update by-room )
    ;     

\ create an empty thing in room rn
: thing-new ( rn -- thing-data )
    dup newtdata }t-room c!
        nthings @ swap to-room 
        newtdata ( return value )
        1 nthings +! ;

: with-room-things ( xt rn -- )
    swap >R
    (t-byroom) c@ \ car idx
    dup 255 = if dup exit then
    begin
        (t-list) @ 
            car>data+next
            swap (t-data) R@ execute
    dup 255 = until 
    drop R> drop ;

: dump-room-things ( rn -- )
    ['] dump-thing swap with-room-things ;

things-clear
