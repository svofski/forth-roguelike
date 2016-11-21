
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

decimal

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

ROWS constant |YIDX|

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
    create |YIDX| allot
    does> swap + ;

: car>data+next dup 8 rshift swap 255 and ;    
: data+next>car swap 8 lshift or ;

mkthings-data (t-data) 
mkthings-list (t-list)
mkthings-yidx (t-yidx)

: t-data>index
    0 (t-data) - 3 rshift ;

variable nthings

: dump-thing ( ptr -- )
    ?dup if 
        ." x:" dup }t-x@ .
        ." y:" dup }t-y@ .
        ." cls:" dup }t-class@ .
        drop
    else
        ." (nil)"
    then ; 

: dump-things-data
    nthings @ 0 do
        i (t-data) dump-thing
    loop ;

: things-clear
    nthings off
    0 (t-list) |THINGSLIST| erase
    0 (t-data) |THINGSDATA| erase 
    0 (t-yidx) |YIDX| 255 fill ;

\ pointer to newly create thing
: newtdata ( -- t-data )
    nthings @ (t-data) ;
: newcar ( -- car )
    nthings @ ;

0 value freecell-tlist

: link-thing ( t-data -- )
    dup >R
    dup }t-y@ (t-yidx) c@ 
    swap t-data>index swap data+next>car
    freecell-tlist (t-list) ! ( store car in free t-list cell )
    freecell-tlist R> }t-y@ (t-yidx) c! ;

\ create an empty thing at x, y
: thing-new ( x, y -- thing-data )
    nthings @ to freecell-tlist 
    newtdata }t-y c!
    newtdata }t-x c!
    newtdata link-thing
    newtdata ( return value )
    1 nthings +! ;

: car-next+(data) ( n -- next data )
    (t-list) @ car>data+next swap (t-data) ;

( xt: data -- true|false, true continues )
: with-y-things ( xt y -- )
    swap >R
    (t-yidx) c@ \ car idx
    dup 255 = if drop R> drop exit then
    begin
        car-next+(data) R@ execute
        not over 255 = or ( next res next=255 )
    until 
    drop R> drop ;

: dump-thing-t ( data -- true )
    dump-thing true ;

: dump-y-things ( rn -- )
    ['] dump-thing-t swap with-y-things ;

0 value sought-thing
: compare-x 
    dup }t-x@ 3 pick = if
        to sought-thing false
    else
        drop true 
    then ;

: get-thing-xy ( x y -- t-data/0 )
    0 to sought-thing
    ['] compare-x swap with-y-things 
    ( x ) drop 
    sought-thing ;

0 value prev-car

: replace-cdr ( car new-cdr -- )
    swap (t-list) dup @ 
        car>data+next ( -- new-cdr t-list data old-cdr )
    drop rot data+next>car 
    swap ! ;

: unlink-tail ( data cur -- )
    begin
        dup
        car-next+(data) 
        ( data cur next data' )
        3 pick = if 
            ( x cur next ) over to freecell-tlist
            ( x cur next ) prev-car swap replace-cdr 
            2drop
            exit
        then
        ( data cur next )
        swap to prev-car
        dup
    255 = until 
    drop drop ;

: unlink-thing ( data -- )
    dup }t-y@ (t-yidx)
    dup c@ dup 255 = if 3drop exit then

    rot >R
    ( yidx car )
    dup to prev-car  

    car-next+(data) ( yidx next data' )
    R@ = if
        ( yidx next ) over c@ to freecell-tlist
        ( yidx next ) swap c! 
    else
        nip R@ swap unlink-tail 
    then 
    R> drop ;

: unlink-xy ( x y -- )
    get-thing-xy 
    ?dup if 
        unlink-thing
    then ;

: move-thing ( data x y -- )
    rot dup unlink-thing
    dup }t-y 2 roll swap c!
    dup }t-x 2 roll swap c!
    link-thing ;

: char@xy ( x y -- char )
    get-thing-xy ?dup if
        }t-class@
        exit
    then
    C-FLOOR ;

things-clear
