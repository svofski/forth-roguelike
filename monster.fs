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
          \ 4 R@ }t-count1 c! \ frustration counter
            R> }t-flags dup c@ TF-AIMED or swap c! ;

: mons-chase-on ( thing -- )
    }t-flags dup c@ [ TF-CHASING ] literal or swap c! ;
: mons-chase-off ( thing -- )
    }t-flags dup c@ [ TF-CHASING invert ] literal and swap c! ;

( make the monster aimless )
: mons-unaim ( thing -- )
     4 over }t-count1 c! \ frustration counter
        }t-flags dup c@ 
        [ TF-AIMED TF-CHASING or invert ] literal and swap c! ;

( true if the monster knows where to go )
: mons-aimed? ( thing -- true|false )
    }t-flags@ [ TF-AIMED ] literal and ;
: mons-chasing? ( thing -- true|false )
    }t-flags@ [ TF-CHASING ] literal and ;

: mons-aim-rnd ( thing -- )
    dup rect-topleft
        xy-find-room
        somewhere-in-room 
        mons-aim ;

( try not to let the @ escape from our claws )
( follow directly if on the same kind of ground )
( otherwise try and aim at the door )
: do-chase ( thing -- )
    dup rect-topleft dcellyx@ 
        dup is-door? if
            drop rogue-xy 
        else 
            rogue-room @ 0=
                swap is-pass? if not then
            if
                last-door 
            else
                rogue-xy
            then 
        then p-xy@
        ( thing x y ) mons-aim ;

( try to aim at monsieur @ )
: mons-sniff ( thing -- )
    dup mons-chasing? if
        do-chase
        exit
    then
    dup rect-topleft xy-find-room
        rogue-room @ = if
            dup mons-chase-on
            roguexy@ mons-aim
        else
            drop
        then ;

: dircmp ( x1 x2 -- -1|0|+1 )
    - dup 0< if drop -1 exit then
          0> if       1 exit then
          0 ;

: mons-movexy ( thing x y -- )
    2dup 4 pick rect-topleft ( t x y x y x' y' )
    2dup dreset-monster ( reset monster marker from floor )
    invalidate1         ( invalidate old loc )
    invalidate1         ( invalidate new loc )
    ( t x y -- ) 
    3dup move-thing     ( remap to the new y )
    2dup dset-monster   ( set monster marker on the floor )
    ( update x,y in thing data )
    ( t x y -- ) 
    2 pick }t-y c!
      swap }t-x c! ;

: (mons-try-moverel) ( thing dx dy -- bool )
    2 pick dup }t-y@    ( t dx dy t y )
    rot +               ( t dx t y' )
    -rot }t-x@ + swap   ( t x' y' )
    2dup can-M-go? if
        mons-movexy true
    else        
        3drop false 
    then ;

: mons-try-moverel
    3dup (mons-try-moverel) if
        3drop exit then
    3dup drop 0 (mons-try-moverel) if
        3drop exit then
    3dup swap drop 0 swap (mons-try-moverel) if
        3drop exit then

    \ only really sometimes, back off
    12 rnd 0= if
        3dup negate (mons-try-moverel) if
            3drop exit then
        3dup swap negate swap (mons-try-moverel) if
            3drop exit then
    then
    3drop ;

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
        2drop 
        mons-unaim 
        exit
    then
    ( try going by dx,dy )
    mons-try-moverel ;

: monsters-turn
    ['] (mons-turn) mons-foreach ;
