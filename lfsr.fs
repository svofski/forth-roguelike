variable lfsr
0xace1 lfsr !

: rand16 
  ( roll dice, -- nextrand )
  lfsr @ dup        ( -- lfsr lfsr )
  1 rshift swap     ( -- new_lfsr old_lfsr )
  1 and             ( -- lfsr lsb )
  0<> if            ( -- lfsr )
    0xb400 xor      ( -- lfsr^0xb400 ) 
  then
  dup lfsr !        ( -- lfsr )
;

: rnd
  ( max -- nextrand 
    get next random value lesser than max )

  65535 swap / rand16 swap / ;

: coinflip rand16 32768 < ;
  
: rnd1+ rnd 1+ ;    

: any-between ( a b -- i ) \ a < i < b
    2dup > if swap then
    over - 1- rnd1+ + ;
