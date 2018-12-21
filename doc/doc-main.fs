: not if false else true then ;
: off false swap ! ;
: on true swap ! ;
: -rot rot rot ;

s" [undefined]" pad c! pad char+ pad c@ move  
pad find nip 0= 
[if] 
   : [undefined]  ( "name" -- flag ) 
      bl word find nip 0= 
   ; immediate 
[then] 

[undefined] [defined] 
[if] : [defined] postpone [undefined] 0= ; immediate [then] 

[undefined] defer [if] : defer create ( "name" -- ) 
    ['] abort ,  does> @ execute ; [then] 

: place
  over >r rot over 1+ r> move c! ;

: S,
  here over char+ allot place align ;

: ,"
  34 parse S, ; 

[undefined] is [if] 
: is                              ( xt "name" -- ) 
    '                               ( xt xt2) 
    state @ if 
        postpone literal  postpone >body  postpone ! 
    else 
        >body ! 
    then ; immediate 
[then] 

[undefined] defer [if] : defer create ( "name" -- ) 
    ['] abort ,  does> @ execute ; [then] 

: rdrop ;

: scan
  >r
  BEGIN  dup
  WHILE  over c@ i <>
  WHILE  1 /string
  REPEAT
  THEN
  rdrop ;

s" base.fs"      included 
s" lfsr.fs"      included 
s" tstack.fs"    included 
s" vt52.fs"      included 
s" chars.fs"     included 
s" things.fs"    included 
s" dung.fs"      included 
s" rooms.fs"     included 
s" level.fs"     included 
s" gamevars.fs"  included 
s" monster.fs"   included 
s" game.fs"      included 

: d-rooms 10 1 do cr i . i dump-room loop ;

: main
  \ level cr dprint bye ;
  play 
  bye
  ;
