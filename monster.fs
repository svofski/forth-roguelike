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
    dup dup 1+ swap
        p-xy@
        xy-find-room
        somewhere-in-room 
        mons-aim ;

: dircmp ( x1 x2 -- -1|0|+1 )
    - dup 0< if drop -1 exit then
          0> if       1 exit then
          0 ;

: can-M-go? ( x y -- true|false )
    dcellyx@
        dup is-floor? if drop true exit then
        dup is-pass? if drop true exit then
        dup is-door? if drop true exit then
        drop false ;



: mons-movexy ( thing x y -- )
    2dup 4 pick dup ( t x y x y t t )
    }t-x@ swap }t-y@ ( t x y x y x' y' )
    ( mark old location as floor )
    2dup dwipe-floor    
    ( invalidate rect x,y-x',y' )
    invalidate          
    ( move thing in the things map, attach to the new y ) 
    ( t x y ) 3dup move-thing
    ( mark new location as thing )
    2dup ddirty-floor
    ( update x,y in thing data )
    2 pick }t-y c!   ( t x )
    swap }t-x c! ;

: mons-try-moverel ( thing dx dy -- bool )
    2 pick dup ( t dx dy t t )
    }t-x@ swap }t-y@ ( t dx dy tx ty )
    d+ 2dup can-M-go? if
        mons-movexy
    else
        3drop
    then
;

: (mons-turn)
    \ 0 25 vtxy ." GRRR I MOVE" dump-thing 
    dup mons-aimed? not if drop exit then
    dup dup }t-tgt-x@ swap }t-x@ dircmp
    over dup }t-tgt-y@ swap }t-y@ dircmp
    ( thing dx dy )
    ( try going by dx,dy )
    mons-try-moverel 
    ;

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
