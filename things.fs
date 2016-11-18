
\ thing:
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
1 soffset }t-x
1 soffset }t-y
1 soffset }t-class

current-offset off
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
: mkthings-yidx
    create 10 allot
    does> swap + ;

: car>data+next dup 8 rshift swap 255 and ;    
: data+next>car swap 8 lshift or ;

mkthings-data (t-data) 
mkthings-list (t-list)
mkthings-yidx (t-yidx)

variable nthings

\ access thing-data by index
: thing-data[] ( n -- thing )
    (t-data) ; 

: dump-thing ( ptr -- )
    ." x:" dup }t-x@ .
    ." y:" dup }t-y@ .
    ." cls:" dup }t-class@ .
    drop ; 

: dump-things-data
    nthings @ 0 do
        i (t-data) dump-thing
    loop ;

: things-clear
    nthings off
    0 (t-list) |THINGSLIST| erase
    0 (t-data) |THINGSDATA| erase 
    0 (t-yidx) 10 255 fill ;

\ pointer to newly create thing
: newtdata ( -- t-data )
    nthings @ (t-data) ;
: newcar ( -- car )
    nthings @ ;

: stash-at-y ( data-idx rn -- )
    dup >R
    (t-yidx) c@ ( dataidx caridx -- )
    data+next>car ( car ) 
    nthings @ (t-list) ! ( store car in new t-list cell )
    nthings @ R> (t-yidx) c! ( update by-room )
    ;     

\ create an empty thing at x, y
: thing-new ( x, y -- thing-data )
    swap newtdata }t-x c!
    dup newtdata }t-y c!
        nthings @ swap stash-at-y
        newtdata ( return value )
        1 nthings +! ;

: with-y-things ( xt y -- )
    swap >R
    (t-yidx) c@ \ car idx
    dup 255 = if dup exit then
    begin
        (t-list) @ 
            car>data+next
            swap (t-data) R@ execute
    dup 255 = until 
    drop R> drop ;

: dump-y-things ( rn -- )
    ['] dump-thing swap with-y-things ;

: get-thing-xy ( x y -- t-data|0 )
    swap >R
    (t-yidx) c@  
    dup 255 = if drop R> drop 0 exit then
    begin
        (t-list) @
            car>data+next
            swap (t-data) dup }t-x@ R@ = if
                swap drop
                R> drop
                exit
            then
            drop 
    dup 255 = until
    drop R> drop ;
    
things-clear
