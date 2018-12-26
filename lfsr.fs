variable lfsr
hex
0ace1 lfsr !

: rand16 
  ( roll dice, -- nextrand )
  lfsr @ dup        ( -- lfsr lfsr )
  1 rshift swap     ( -- new_lfsr old_lfsr )
  1 and             ( -- lfsr lsb )
  0<> if            ( -- lfsr )
    0b400 xor      ( -- lfsr^0xb400 ) 
  then
  dup lfsr !        ( -- lfsr )

  \ Try to sign extend for 32/64-bit forths
  \ for signed coinflip. Unsigned coinflip is portable
  \ regardless of word size.
  \ dup 8000 and if
  \   -1 0ffff xor or
  \ then
;

decimal

: rnd
  ( max -- nextrand 
    get next random value lesser than max )

  32767 swap / rand16 32767 and swap / ;

\ this requires rand16 to be in range -32768..32767
\ bit-testing version is compatible with 32/64-bit systems
\ : coinflip rand16 0< ;
hex
: coinflip rand16 8000 and ;
decimal
  
: rnd1+ rnd 1+ ;    

: any-between ( a b -- i ) \ a < i < b
    2dup > if swap then
    over - 1- rnd1+ + ;

: dice-for-roomlight ( dlvl -- t|f )
    12 min 12 swap - 13 rnd > ;

\ halp! i don't know how to implement this without a scratch var
: mkshuffle ( n -- shuffled sequence of 0..n-1 )
    dup to scratch
    0 do i loop
    scratch 2 * 0 do
        -1 scratch any-between roll
    loop ;


