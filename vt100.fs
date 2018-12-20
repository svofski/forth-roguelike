: vtxy ( x y -- )
    27 emit [CHAR] [ emit 
    1+ 0 .r [CHAR] ; emit 1+ 0 .r [CHAR] H emit ;

: clreol
    27 emit [CHAR] [ emit
    [CHAR] K emit ;

: debug-on 
    27 emit [CHAR] [ emit [CHAR] 7 emit [CHAR] m emit ;
: debug-off
    27 emit [CHAR] [ emit [CHAR] 0 emit [CHAR] m emit ;
