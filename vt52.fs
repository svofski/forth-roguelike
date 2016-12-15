: vtxy ( x y -- )
    27 emit [CHAR] Y emit 32 + emit 32 + emit ;

: clreol
    27 emit [CHAR] K emit ;

hex
: page 0c emit 1f emit 1b emit 45 emit 1b emit 4a emit ;
decimal

( hax for koi-7 microdos on vector-06c )
\ hex 1b emit 5b emit decimal
