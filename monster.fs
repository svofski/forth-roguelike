: time>s,t ( t-time@ -- speed time )
    dup 15 and swap 4 rshift ;
: s,t>time ( speed time -- time )
    15 and 4 lshift swap 15 and or ;

: (dump-monster) ( thing -- )
    ?dup if 
        [CHAR] ( emit 
            dup }t-class@ dup emit [CHAR] : emit
                              mcls>name type  
        [CHAR] ) emit BL emit
        dup }t-flags@ dup hex . decimal
            TF-AIMED and dup s:" aimed @:unaimed " type
            ( aimed ) if 
                dup }t-tgt-x@ 0 .r [CHAR] , emit
                dup }t-tgt-y@ .
            then
        dup }t-time@ time>s,t swap ." Speed:" . ." Time:" 0 .r
        ." ; "
        drop
    then ;
' (dump-monster) is dump-monster

( apply xt to every monster on level )
: mons-foreach ( xt -- )
    nthings @ 0 do
        i (t-data) dup }t-class c@ isupper? if
            over execute 
        else
            drop
        then
    loop drop ;

( aim the monster at x y )
: mons-aim ( thing x y -- )
    rot dup >R }t-tgt-y c!
            R@ }t-tgt-x c! 
            R> }t-flags dup c@ TF-AIMED or swap c! ;

( make the monster aimless )
: mons-unaim ( thing -- )
    }t-flags dup c@ [ TF-AIMED invert ] literal and swap c! ;

( true if the monster knows where to go )
: mons-aimed? ( thing -- true|false )
    }t-flags@ TF-AIMED and 0<> ;

: mons-aim-rnd ( thing -- )
    dup 
        rect-topleft
        xy-find-room
        somewhere-in-room 
        mons-aim ;

( try to aim at monsieur @ )
: mons-sniff ( thing -- )
    dup
        rect-topleft xy-find-room
        rogue-room @ = if
            roguexy@ mons-aim
        else
            drop
        then ;

: dircmp ( x1 x2 -- -1|0|+1 )
    - dup 0< if drop -1 exit then
          0> if       1 exit then
          0 ;

: can-M-go? ( x y -- true|false )
    2dup roguexy@ d= if 2drop false exit then
    dcellyx@
        dup is-floor? if drop true exit then
        dup is-pass? if drop true exit then
        dup is-door? if drop true exit then
        dup is-thing? if drop true exit then
        dup is-monster? if drop false exit then
        drop false ;

: mons-movexy ( thing x y -- )
    2dup 4 pick rect-topleft ( t x y x y x' y' )
    2dup dmonst-reset   ( reset monster marker from floor )
    invalidate1         ( invalidate old loc )
    invalidate1         ( invalidate new loc )
    ( t x y -- ) 
    3dup move-thing     ( remap to the new y )
    2dup dmonst-set     ( set monster marker on the floor )
    ( update x,y in thing data )
    ( t x y -- ) 
    2 pick }t-y c!
      swap }t-x c! ;

: mons-try-moverel ( thing dx dy -- bool )
    2 pick dup }t-y@    ( t dx dy t y )
    rot +               ( t dx t y' )
    -rot }t-x@ + swap   ( t x' y' )
    2dup can-M-go? if
        mons-movexy
    else
        2drop mons-unaim
    then ;

: (mons-keep-busy) ( thing -- )
    dup mons-sniff
    dup mons-aimed? not if
        mons-aim-rnd
    else
        drop
    then ;

: (mons-turn)
    dup (mons-keep-busy)
    dup mons-aimed? not if 
        drop exit 
    then
    dup dup }t-tgt-x@ swap }t-x@ dircmp
    over dup }t-tgt-y@ swap }t-y@ dircmp
    ( thing dx dy )
    2dup or 0= if
        2drop mons-unaim 
        exit
    then
    ( try going by dx,dy )
    mons-try-moverel ;

( advance monster time counter and make a move if it's time )
: mons-turn ( thing -- )
    dup 
        }t-time dup c@ time>s,t over + 
        ( &thing &t-time speed accu ) 
        dup >R  ( store accu in R )
        7 and   ( keep fractional part )
        s,t>time swap c!
        R> 8 and if 
            (mons-turn)
        else
            drop
        then ;

: monsters-turn
    ['] (mons-turn) mons-foreach ;
