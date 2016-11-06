80 constant COLS
24 constant ROWS
COLS ROWS * constant dngsize

32 constant C-NOTHING
'.' constant C-FLOOR
'#' constant C-PASSAGE

create (dungeon) dngsize allot 

: dcell ( i -- a )
  (dungeon) + ;

: dcell! ( val idx -- )
  dcell c! ;

: dcell@ ( idx -- val )
  dcell c@ ;

: dcellyx! ( value x y -- )
  COLS * + dcell! ;

: dcellyx@ ( x y -- val )
  COLS * + dcell@ ;

: dclear ( val -- ) 
  (dungeon) dngsize rot fill ;

: dprint ( -- )
  (dungeon)
  ROWS 0 do 
    COLS 0 do
      dup c@ emit 1+
    loop
    cr
  loop drop ;

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
  loop drop drop drop ;

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

