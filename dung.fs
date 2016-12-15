COLS ROWS * constant dngsize

0 0 COLS 1- ROWS 1- rect update-rect

0 value |update-points|
200 constant |update-points-max|
\ create update-points 2 |update-points-max| * allot
here value update-points ( -- adr )

: update-point[] ( n -- pointx pointy )
    2* update-points + dup 1+ ; 

\ (dungeon) is initialized in runtime by allot-dungeon
\ static version is simply:
\   create (dungeon) dngsize allot 
\ but we want to reduce the binary image size, so:
here value (dungeon) ( -- adr )

: allot-dungeon ( -- )
    here to (dungeon) dngsize allot 
    here to update-points 2 |update-points-max| * allot
    ;

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
    [COLS*] + ;

: dcellyx! ( value x y -- )
    [COLS*] + dcell! ;

: dcellyx@ ( x y -- val )
    [COLS*] + (dungeon) + c@ ;

( paint a cell at x,y visible )
: dcellyx-make-visible ( x y -- )
    [COLS*] + dup dcell@ c-make-visible swap dcell! ;

( paint a cell @ptr visible )
: visible! ( char-ptr -- )
    dup c@ c-make-visible swap c! ;

: dclear ( val -- ) 
    (dungeon) dngsize rot fill ;

: make-all-visible ( -- )
    dngsize 0 do i dcell-make-visible loop ;

( set thing or monster presence flag )
: (dset) ( x y bit -- )
    -rot ( bit x y -- )
    dcellyx dcell dup c@  ( bit adr c )
        dup c-char (?stuff) if
            ( bit adr c ) rot or 
        else
            ( bit adr c ) c-visible? rot (C-MARKER) or or 
        then
        swap c! ;

: (dreset) ( x y bit -- )
    -rot
    dcellyx dcell dup c@ ( bit adr c )
        dup c-char (?stuff) if
            ( bit adr c )
            rot invert and ( adr c' )
            dup [ (C-THING) (C-MONSTER) or ] literal and
            0= if
                c-visible? C-FLOOR or 
            then
            swap c! 
        else
            2drop  ( there was nothing, leave )
        then ;

: dset-thing ( x y -- )
    (C-THING) (dset) ;
: dreset-thing ( x y -- )
    (C-THING) (dreset) ;
: dmonst-set ( x y -- )
    (C-MONSTER) (dset) ;
: dmonst-reset ( x y -- )
    (C-MONSTER) (dreset) ;

: c-skip?
    dup c-visible? not ?true/~ 
    [ C-NOTHING ] literal = ;

\ apply xt ( ptr -- ) to width elements at ptr
: apply-span ( xt width ptr -- )
    swap 0 do
        ( xt ptr -- )
        2dup swap execute 
        1+
    loop 
    2drop ;

\ fill width elements at ptr with val
: dfillln ( val width ptr -- )
    -rot swap fill ;

: dfillcol ( val height ptr -- )
    swap 0 do
        2dup c! COLS+
    loop 2drop ;

: dfillrect ( val r1 -- )
    dup rect-topleft dcellyx (dungeon) + swap ( v & r1 -- )
    dup rect-height 1+ swap rect-width 1+ ( v & h w -- )
    -rot ( v w & h -- )
    0 do
        3dup dfillln ( v w & )
        COLS+
    loop 
    3drop ;

: dfill-visible ( x1 y1 x2 y2 -- )
    ( vertical bounds to r )
    1+ rot 2>R 
    ( calc start pointer y1 * COLS + x1 )
    1+ over - swap      ( w x1 -- )
    2R@ nip COLS* + dcell ( w ptr -- )
    2R> do
        2dup ( w ptr w ptr -- )
        ['] visible! -rot apply-span
        COLS+
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
    over - 1-                   ( char x1 y1 h -- )
    dup 0> if
        rot 1- rot 1+ COLS* +
        (dungeon) + 1+          ( char w ptr )
        dfillcol
    else
        2drop 2drop
    then ;

: invalidate ( x1 y1 x2 y2 -- )
    norm4 update-rect rect-add ;

( invalidate a single location )
: invalidate1 ( x1 y1 -- )
    |update-points| update-point[] p-xy!
    |update-points| 1+ to |update-points| 
    |update-points| |update-points-max| = if
        abort" invalidate1 overflow"
    then ;

: invalidate-all ( -- )
    0 0 COLS 1- ROWS 1- invalidate 
    0 to |update-points| ;

: validate-all ( -- )
    COLS ROWS 0 0 update-rect rect! 
    0 to |update-points| ;

: (ntu-rect?)
    [ COLS ROWS ] literal literal 0 0 update-rect rect-eq? ;

: (0=up-pts)
    |update-points| 0= ;

: nothing-to-update?
    (ntu-rect?) (0=up-pts) and ;

: dupd-pt  ( x y -- )
    2dup dcellyx@ dup c-visible? if
        (?stuff) if
            2dup char@xy
        else
            C-FLOOR
        then
        -rot vtxy emit 
    else
        drop 2drop
    then ;

: dupdate-points ( -- )
    (0=up-pts) if exit then 
    |update-points| 0 do
        i update-point[] p-xy@  ( -- x y )
        2dup update-rect xy-in-r? not if
            dupd-pt
        else
            2drop
        then
    loop ;

: updaterect-row-increment ( -- increment ) 
    COLS update-rect }rx2@ update-rect }rx1@ - 1+ - ;

: dupdate-invalid ( -- )
    nothing-to-update? if exit then 
    dupdate-points
    updaterect-row-increment
    0 (dungeon)                 ( inc nspaces dungeon -- ) 
    update-rect rect-topleft dcellyx + ( + start adr )
    update-rect }ry2@ 1+ update-rect }ry1@ do
        nip 0 swap              ( nspaces = 0 )
        update-rect }rx1@ i vtxy
        update-rect }rx2@ 1+ update-rect }rx1@ do
            dup c@              ( inc nsp dng c )
            dup (?stuff) if 
                \ switch the thing trait with the actual thing
                128 and i j char@xy or
            then
            ( inc nsp dng c )
            dup c-skip? if   ( inc nspaces &dng -- )
                drop
                swap 1+ swap
            else
                ( inc nsp dng c )
                rot if
                    i j vtxy
                then
                0 -rot  ( nspaces = 0 )
                c-char emit 
            then
            1+                  ( inc nspaces &++dng -- )
        loop
        2 pick +
    loop 3drop
    validate-all ;

: dprint ( -- )
  (dungeon)
  ROWS 0 do 
    COLS 0 do
      dup c@ c-char emit 1+
    loop
    cr
  loop drop ;
