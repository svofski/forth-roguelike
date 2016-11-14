include base.fs

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
