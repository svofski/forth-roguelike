include base.fs

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
    4 3 2 1 r1 rect-eq? .

    ;

cr .( test-rect0) cr
test-rect0
cr

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
    1 thing-new drop
    nthings @ 1 = . 
    {stack-out}
    ;

: test-thing-rooms
    {stack-in}
    5 thing-new 
        1           over }t-x c!
        2           over }t-y c!
        TC-SCROLL   over }t-class c!
        drop

    5 thing-new
        3           over }t-x c!
        4           over }t-y c!
        TC-POTION   over }t-class c!
        drop

    cr .s cr

    5 dump-room-things

    cr .s cr

    {stack-out}
    ;
cr .( test-things-clear ) cr
test-things-clear
cr .( test-thing-new ) cr
test-thing-new
cr .( test-thing-new ) cr
test-thing-rooms
cr
