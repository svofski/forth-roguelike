80 constant COLS
24 constant ROWS
COLS ROWS * constant dngsize

: COLS* 4 lshift dup 2 lshift + ;

32 constant C-NOTHING
32 constant C-NOTHING  
46 constant C-FLOOR 
35 constant C-PASSAGE
43 constant C-DOOR 
62 constant C-EXIT

0 0 COLS 1- ROWS 1- rect update-rect

\ create (dungeon) dngsize allot 
here value (dungeon) ( -- adr )

: allot-dungeon ( -- )
    here to (dungeon) dngsize allot ;

: c-visible?
    128 and ;

: c-make-visible
    128 or ;

: c-char
    127 and ;

: c-char@
    c@ 127 and ;

: dcell ( i -- a )
  (dungeon) + ;

: dcell! ( val idx -- )
  dcell c! ;

: dcell@ ( idx -- val )
  dcell c@ ;

: dcell-make-visible
    dup dcell@ c-make-visible swap dcell! ;

: dcell-visible?
    dcell c@ c-visible? ;

: dcellyx  ( x y -- ofs )
    COLS* + ;

: dcellyx! ( value x y -- )
    COLS* + dcell! ;

: dcellyx@ ( x y -- val )
    COLS* + (dungeon) + c@ ;

: dcellyx-make-visible ( x y -- )
    COLS* + dup dcell@ c-make-visible swap dcell! ;

: dclear ( val -- ) 
    (dungeon) dngsize rot fill ;

: make-all-visible ( -- )
    dngsize 0 do i dcell-make-visible loop ;

: c-skip?
    dup c-visible? not if drop true exit then 
    [ C-NOTHING ] literal = ;

: dfillln ( val width ptr -- )
    swap 0 do
        2dup c! 1+
    loop 2drop ;

: dfillcol ( val height ptr -- )
    swap 0 do
        2dup c! [ COLS ] literal +
    loop 2drop ;

: dfillrect ( val r1 -- )
    dup rect-topleft dcellyx (dungeon) + swap ( v & r1 -- )
    dup rect-height swap rect-width ( v & h w -- )
    -rot ( v w & h -- )
    0 do
        3dup dfillln ( v w & )
        [ COLS ] literal +
    loop 
    3drop ;

: dfill-visible ( x1 y1 x2 y2 -- )
    1+ rot do
        2dup 1+ swap do 
        i j dcellyx-make-visible
        loop 
    loop 
    2drop ;

: dlineh ( char x1 y1 x2 -- )
    rot swap                    ( char y1 x1 x2 -- )
    2dup > if swap then

    over - 1-                   ( char y1 x1 w -- )
    dup 0> if
        swap rot                ( char w x1 y1 -- )
        COLS* + (dungeon) + 1+ dfillln
    else
        2drop 2drop
    then ;

: dlinev ( char x1 y1 y2 -- )
    2dup > if swap then
    1 pick - 1-                 ( char x1 y1 h -- )
    dup 0> if
        2 roll 1- 2 roll 1+ COLS* +
        (dungeon) + 1+          ( char w ptr )
        dfillcol
    else
        2drop 2drop
    then ;

: invalidate ( x1 y1 x2 y2 -- )
    update-rect rect-add ;

: invalidate-all ( -- )
    0 0 COLS 1- ROWS 1- invalidate ;

: validate-all ( -- )
    COLS ROWS 0 0 update-rect rect! ;

: nothing-to-update?
    [ COLS ROWS ] literal literal 0 0 update-rect rect-eq? ;

: updaterect-row-increment ( -- increment ) 
    [ COLS ] literal 
    update-rect }rx2@ update-rect }rx1@ - 1+ - ;

: dupdate-invalid ( -- )
    nothing-to-update? if exit then 
    updaterect-row-increment
    0 (dungeon)                 ( inc nspaces dungeon -- ) 
    update-rect }rx1@ update-rect }ry1@ dcellyx + \ start adr
    update-rect }ry2@ 1+ update-rect }ry1@ do
        nip 0 swap              ( nspaces = 0 )
        update-rect }rx1@ i vtxy
        update-rect }rx2@ 1+ update-rect }rx1@ do
            dup c@ c-skip? if   ( inc nspaces &dng -- )
                swap 1+ swap
            else
                over if
                    i j vtxy
                    nip 0 swap  ( nspaces = 0 )
                then
                dup c-char@ emit 
            then
            1+                  ( inc nspaces &++dng -- )
        loop
        2 pick +
    loop 3drop
    validate-all ;

