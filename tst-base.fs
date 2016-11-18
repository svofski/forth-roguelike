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

: test-thing-rooms
    {stack-in}
    1 5 thing-new 
        TC-SCROLL   over }t-class c!
        drop

    2 5 thing-new
        TC-POTION   over }t-class c!
        drop

    \ 5 dump-room-things
    <#
    ['] dump-to-hold 5 with-y-things
    0 0 #> 
    \ 2dup cr [char] ! emit type [char] ! emit cr
    s" #2#5#1#3#5#2" compare 0= .
    cr
    {stack-out}
    ;

: test-get-thing-xy
    {stack-in}
    12 13 thing-new
        TC-SCROLL over }t-class c!
        drop

    67 4 thing-new
        TC-POTION over }t-class c!
        drop

    12 13 get-thing-xy }t-class@ TC-SCROLL = .
    67 4  get-thing-xy }t-class@ TC-POTION = .

    1 1 get-thing-xy 0= .

    {stack-out}
;

test-thing-new
cr .( test-thing-rooms ) cr
test-thing-rooms
cr .( test-get-xy ) cr
test-get-thing-xy

