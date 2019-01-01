( player's bag of things )

( the things are a simple array
  to collect all things, the entire list must be walked through
  quantity 0= means this slot is vacant
) 


\ AMULET
\ GOLD
\ 
\ except ARMOR
\     quantity:
\         "a "
\         "%d "
\ 
\ 
\ ARMOR
\     UNIDENTIFIED
\         id_table[which_kind].title
\     IDENTIFIED
\         "%s%d" d_enchant >= 0 ? "+":"", d_enchant
\         id_table[which_kind].title [get_armor_class(obj)]
\ 
\ WEAPON
\     UNIDENTIFIED
\         name_of(obj)
\     IDENTIFIED
\         "%s%d,%s%d ", hit_enchant >= 0 ? "+":"", hit_enchant
\                       d_enchant >= 0 ? "+":"", d_enchant
\         name_of(obj)
\ 
\ WAND
\     UNIDENTIFIED
\         id_table[which_kind].title item_name
\     CALLED
\         item_name "called " id_table[which_kind].title 
\     IDENTIFIED
\         item_name id_table[which_kind].real [obj->class] (class is charge?)
\ 
\ RING
\     UNIDENTIFIED
\         id_table[which_kind].title item_name
\     CALLED
\         item_name "called " id_table[which_kind].title 
\     IDENTIFIED
\         more_info = ""
\         if which_kind in [DEXTERITY,ADD_STRENGTH]
\             more_info = "%s%d" obj.class > 0 "+" : "", obj.class
\         more_info item_name id_table[which_kind].real
\ 
\ SCROLL
\     e.g. "a scroll titled 'greyueng mikun oxala'
\ 
\     UNIDENTIFIED
\         item_name " titled: " id_table[which_kind].title
\     CALLED
\         item_name "called " id_table[which_kind].title 
\     IDENTIFIED
\         item_name id_table[which_kind].real
\ 
\ POTION
\     UNIDENTIFIED
\         id_table[which_kind].title item_name
\     CALLED
\         item_name " called " id_table[which_kind].title
\     IDENTIFIED
\         item_name id_table[which_kind].real
\     
\ 
\ FOOD            "some food", "2 rations of food", "a slime mold" 
\   RATION
\   FRUIT
\   named
\ 
\ in_use_flags
\     BEING_WIELDED
\         "in hand"
\     BEING_WORN
\         "being worn"
\     ON_LEFT_HAND
\         "on left hand"
\     ON_RIGHT_HAND
\         "on right hand"




decimal

\ inventory items are references to the prototypes + attrs
( struct inventory-object )
current-offset off
1 soffset }i-quant      \ quantity, 0 = vacant item
1 soffset }i-what-is    \ food, scroll, potion, wand, ring, armor, weapon
1 soffset }i-proto      \ reference index of the original
1 soffset }i-flags      \ item flags 
1 soffset }i-useflags   \ in use: WIELDED, WORN, ON-LEFT-HAND, ON-RIGHT-HAND
\ 7

16 constant |INVENTORY-ITEM| \ size 16: obj[i] offset = i << 4
24 constant INVENTORY-SIZE  \ items, but quantity can be >1
|INVENTORY-ITEM| INVENTORY-SIZE * constant |INVENTORY-DATA|

INVENTORY-SIZE cells constant |INVLIST|

: mkinv-data
    create |INVENTORY-DATA| allot
    does> swap 4 lshift + ;

mkinv-data (i-data)

\ set quantity of item to zero, input: index
: inv-0 ( n -- )
    (i-data) }i-quant 0 swap c! ;

: inv-clear 
    INVENTORY-SIZE 0 do
        i inv-0
    loop ;

\ find a vacant entry
: inv-vacant
    INVENTORY-SIZE 0 do 
        i (i-data) }i-quant c@ 0= if
            i unloop exit 
        then
    loop 
    -1 ;

( proto n what-is -- )
: inv+item
    -rot ( -- what-is proto n )
    1 over (i-data) }i-quant c! 
    rot over (i-data) }i-what-is c!  
    over over (i-data) }i-proto c! 
    0 over (i-data) }i-flags c! 
    0 over (i-data) }i-useflags c! 
    swap drop ;

: inv-t-food ." food" ;

( i-data s1 s2 -- i-data )
: t-quant
    2 pick }i-quant c@
        dup 1 = if
            drop ." a " type bl emit
        else
            . type [char] s emit bl emit
        then ;


( n -- )
: inv-t-scroll 
    (i-data)  
    s" scroll" t-quant
    \ dup }i-quant c@ 
    \    dup 1 = if 
    \        ." a scroll " drop
    \    else 
    \        . ." scrolls "
    \    then
    }i-proto c@ ( proto-ref -- )
    dup scroll-status c@ IDENTIFIED and 0<> if
        ." of " dup scroll-real type
    else 
        dup scroll-status c@ CALLED and 0<> if
            ." called " 
        else
            ." titled: "
        then
        dup scroll-name type
    then
    drop ;

( n -- )
: inv-type-item
    dup 
    (i-data) }i-what-is c@ case
        WI-FOOD of inv-t-food endof
        WI-SCROLL of inv-t-scroll endof
        \ WI-POTION of inv-t-potion endof
        drop
    endcase ;

\ leaves number of item or -1 if a vacant place was not found
\ ( proto what-is -- n )
: inv+new
    inv-vacant dup 0< if drop exit then
    swap
    inv+item
    ;

cr ." ------ temporary test of inventory functions ------" cr
gen-scroll-names
4 WI-SCROLL inv+new
dup cr inv-type-item cr
0 (i-data) }i-quant 3 swap c!
dup cr inv-type-item cr
cr ." -----------------" cr
