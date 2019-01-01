decimal 

1 constant CALLED
2 constant IDENTIFIED

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


\ }i-what-is field values
1 constant WI-FOOD
2 constant WI-SCROLL
3 constant WI-POTION
4 constant WI-WAND
5 constant WI-RING
6 constant WI-ARMOR
7 constant WI-WEAPON
