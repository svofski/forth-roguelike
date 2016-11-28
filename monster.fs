: (dump-monster) ( thing -- )
    ?dup if 
        [CHAR] ( emit 
            dup }t-class@ dup emit [CHAR] : emit
                              mcls>name type  
        [CHAR] ) emit BL emit
        dup }t-flags@ dup hex . decimal
            TF-AIMED and dup s:" aimed @:unaimed" type
            ( aimed ) if 
                dup }t-tgt-x@ 0 .r [CHAR] , emit
                dup }t-tgt-y@ 0 .r
            then
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

