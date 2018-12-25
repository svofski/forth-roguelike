decimal 

1 constant CALLED

36 constant |CALLED-NAME|

: call-it ( buf -- )
    ." call it: " 
    \ execute
    dup 1+
        |CALLED-NAME| 1- accept ( -- buf len )
        swap c! ;

: its-name ( n getter -- )
    execute dup c@ swap 1+ swap ;

\ array of scroll random or called names
: mk-names ( nitems -- )
    create |CALLED-NAME| * allot
    does> swap |CALLED-NAME| * + ;

: mk-values ( nitems -- )
    create cells allot
    does> swap cells + ;


