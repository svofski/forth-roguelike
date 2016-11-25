: vtxy ( x y -- )
    27 emit [CHAR] [ emit 
    1+ 0 .r [CHAR] ; emit 1+ 0 .r [CHAR] H emit ;

: clreol
    27 emit [CHAR] [ emit
    [CHAR] K emit ;
