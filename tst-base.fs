include base.fs
include lfsr.fs
include chars.fs

24 constant ROWS \ gforth imports ROWS from the shell, override

\ for gforth
: not if false else true then ;

10 5 30 15 rect r1

: test-rect0
    10 5 30 15 r1 rect-eq? . 
    1 2 10 6 r1 rect-add
    1 2 30 15 r1 rect-eq? . 
    3 4 35 20 r1 rect-add
    1 2 35 20 r1 rect-eq? .
    
    4 3 2 1 r1 rect!
    4 3 2 1 r1 rect-eq? .  ;

cr .( test-rect0) cr
test-rect0

include things.fs

: {stack-in}
    depth 0= not if abort" stack in" then ;
: {stack-out}
    depth 0= not if abort" stack out" then ;

: test-things-clear
    {stack-in}
    things-clear
    nthings @ 0= . 
    {stack-out}
    ;
: test-thing-new
    {stack-in}
    5 6 thing-new drop
    nthings @ 1 = . 
    {stack-out}
    ;

: dump-to-stack ( data -- rn x y class )
    >R
    R@ }t-x@ R@ }t-y@ R@ }t-class@
    R> drop ;

: .hold 0 #s 2drop [char] # hold ;

: dump-to-hold ( data -- rn x y class )
    >R
    R@ }t-x@ .hold R@ }t-y@ .hold R@ }t-class@ .hold
    R> drop ;

: dump-to-hold-t
    dump-to-hold true ;

: hold-y-things ( y -- addr u )
    <# ['] dump-to-hold-t swap with-y-things 0 0 #> ;

: test-thing-rooms
    {stack-in}
    1 5 thing-new 
        C-SCROLL   over }t-class c!
        drop

    2 5 thing-new
        C-POTION   over }t-class c!
        drop

    \ 5 dump-room-things
    5 hold-y-things
    \ 2dup cr [char] ! emit type [char] ! emit cr
    s" #63#5#1#33#5#2" compare 0= .
    {stack-out}
    ;

: test-get-thing-xy
    {stack-in}
    12 13 thing-new
        C-SCROLL over }t-class c!
        drop

    67 4 thing-new
        C-POTION over }t-class c!
        drop

    12 13 get-thing-xy }t-class@ C-SCROLL = .
    67 4  get-thing-xy }t-class@ C-POTION = .

    1 1 get-thing-xy 0= .

    {stack-out}
;

: make-test-things
    1 5 thing-new drop
    2 5 thing-new drop
    3 5 thing-new drop
;

: test-unlink
    {stack-in}
    things-clear

    1 5 unlink-xy 

    things-clear

    make-test-things

    5 hold-y-things s" #0#5#1#0#5#2#0#5#3" compare 0= .
    1 5 unlink-xy
    5 hold-y-things s" #0#5#2#0#5#3" compare 0= .
    3 5 unlink-xy
    5 hold-y-things s" #0#5#2" compare 0= .
    2 5 unlink-xy
    5 hold-y-things s" " compare 0= .

    {stack-out}
    ;

0 value tempval
: test-move
    {stack-in}
    things-clear
    make-test-things
    2 5 get-thing-xy dup to tempval 
    2 6 move-thing
    5 hold-y-things s" #0#5#1#0#5#3" compare 0= .
    6 hold-y-things s" #0#6#2" compare 0= .
    
    tempval 2 7 move-thing
    6 hold-y-things s" " compare 0= .
    7 hold-y-things s" #0#7#2" compare 0= .

    tempval 2 0 move-thing
    0 hold-y-things s" #0#0#2" compare 0= .

    24 0 do
        tempval 3 i move-thing
    loop 
    ROWS 1- hold-y-things s" #0#23#3" compare 0= .

    {stack-out} ;

cr .( test-thing-new ) cr
test-thing-new
cr .( test-thing-rooms ) cr
test-thing-rooms
cr .( test-get-xy ) cr
test-get-thing-xy
cr .( test-unlink ) cr
test-unlink
cr .( test-move ) cr
test-move
cr
