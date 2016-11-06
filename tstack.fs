9 constant TSTACK-SIZE
create tstack-sp TSTACK-SIZE ,
create tstack-data TSTACK-SIZE cells allot
does> swap cells + ;

: >tstack ( n -- )
    tstack-sp dup @ 1- dup rot !
    tstack-data ! ;
: tstack> ( -- n )
    tstack-sp dup @ dup 1+ rot !
    tstack-data @ ;
: tsdepth ( -- depth ) TSTACK-SIZE tstack-sp @ - ;
: tsavail? ( -- avail ) tsdepth 0> ;
