80 constant COLS
24 constant ROWS
COLS ROWS * constant dngsize

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
    COLS * + ;

: dcellyx! ( value x y -- )
    COLS * + dcell! ;

: dcellyx@ ( x y -- val )
    COLS * + dcell@ ;

: dcellyx@-c ( x y -- val )
    dcellyx@ c-char ;

: dcellyx-make-visible
    COLS * + dup dcell@ c-make-visible swap dcell! ;

: dclear ( val -- ) 
  (dungeon) dngsize rot fill ;

: make-all-visible
    dngsize 0 do i dcell-make-visible loop ;

: dprint ( -- )
  (dungeon)
  ROWS 0 do 
    COLS 0 do
      dup c@ c-char emit 1+
    loop
    \ cr
  loop drop ;

: c-skip?
    dup c-visible? not if drop true exit then 
    C-NOTHING = ;

: dprint-vt ( -- )
    0 (dungeon)                 ( nspaces dungeon -- ) 
    ROWS 0 do
        nip 0 swap              ( nspaces = 0 )
        0 i vtxy
        COLS 0 do
            dup c@ c-skip? if   ( nspaces &dng -- )
                swap 1+ swap    
            else
                over if
                    i j vtxy
                    nip 0 swap  ( nspaces = 0 )
                then
                dup c-char@ emit 
            then
            1+                  ( nspaces &++dng -- )
        loop
    loop drop drop ;

: dprint-small-area ( x1 y1 x2 y2 -- )
    1+ rot do
        over i vtxy 
        2dup 1+ swap do 
            i j dcellyx@
            dup c-skip? if
                drop BL emit
            else
                c-char emit 
            then
        loop 
    loop 
    drop drop ;

: dfillln ( val width ptr )
    swap 0 do
        2dup c! 1+
    loop 2drop ;

: dfillcol ( val height ptr )
    swap 0 do
        2dup c! COLS +
    loop 2drop ;

: 3dup 2 pick 2 pick 2 pick ;

: dfillx ( val x1 y1 x2 y2 -- )
  2 pick - 1+           ( val x1 y1 x2 height )
  1 roll 3 pick - 1+    ( val x1 y1 height width )
  2 roll COLS *
  3 roll +    ( val height width dptr )
  (dungeon) +
  rot 0 do
    3dup
    dfillln
    COLS +
  loop 
  drop drop drop ;

: dfill ( val y1 x1 y2 x2 -- )
  2 roll 
  do
    2dup
    4 pick rot rot swap
    do 
      dup j i dcellyx!
    loop
    drop
  loop ;

: dfill-visible ( x1 y1 x2 y2 -- )
    1+ rot do
        2dup 1+ swap do 
        i j dcellyx-make-visible
        loop 
    loop 
    drop drop ;

: dlineh ( char x1 y1 x2 -- )
    rot swap                    ( char y1 x1 x2 -- )
    2dup > if swap then

    over - 1-                   ( char y1 x1 w -- )
    dup 0> if
        swap rot                ( char w x1 y1 -- )
        COLS * + (dungeon) + 1+ dfillln
    else
        drop drop drop drop
    then ;

: dlinev ( char x1 y1 y2 -- )
    2dup > if swap then
    1 pick - 1-                 ( char x1 y1 h -- )
    dup 0> if
        2 roll 1- 2 roll 1+ COLS * +
        (dungeon) + 1+          ( char w ptr )
        dfillcol
    else
        drop drop drop drop
    then ;

: invalidate ( x1 y1 x2 y2 -- )
    update-rect rect-add ;

: invalidate-all ( -- )
    0 0 COLS 1- ROWS 1- invalidate ;

: validate-all ( -- )
    COLS ROWS 0 0 update-rect rect! ;

: dupdate-invalid ( -- )
    COLS ROWS 0 0 update-rect rect-eq? if exit then
    \ ." U!" update-rect dump-rect 
    COLS update-rect }rx2@ update-rect }rx1@ - 1+ -
    0 (dungeon)                 ( nspaces dungeon -- ) 
    update-rect }rx1@ update-rect }ry1@ dcellyx +
    update-rect }ry2@ 1+ update-rect }ry1@ do
        nip 0 swap              ( nspaces = 0 )
        update-rect }rx1@ i vtxy
        update-rect }rx2@ 1+ update-rect }rx1@ do
            dup c@ c-skip? if   ( nspaces &dng -- )
                swap 1+ swap    
            else
                over if
                    i j vtxy
                    nip 0 swap  ( nspaces = 0 )
                then
                dup c-char@ emit 
            then
            1+                  ( nspaces &++dng -- )
        loop
        2 pick +
    loop drop drop drop 
    validate-all ;

