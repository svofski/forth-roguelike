9 constant TSTACK-SIZE
create tstack-sp TSTACK-SIZE ,

: mktsdata 
    create TSTACK-SIZE cells allot
    does> swap cells + ;

mktsdata tstack-data

: >tstack ( n -- )
    tstack-sp dup @ 1- dup rot !
    tstack-data ! ;
: tstack> ( -- n )
    tstack-sp dup @ dup 1+ rot !
    tstack-data @ ;
: tsdepth ( -- depth ) TSTACK-SIZE tstack-sp @ - ;
: tsavail? ( -- avail ) tsdepth 0> ;
