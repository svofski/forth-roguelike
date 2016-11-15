\ who knows if this becomes useful later, not used
\ at the moment

: dprint-vt ( -- )
    0 (dungeon)                 ( nspaces dungeon -- ) 
    ROWS 0 do
        nip 0 swap              ( nspaces = 0 )
        0 i vtxy
        COLS 0 do
            dup c@ c-skip? if   ( nspaces &dng -- )
                swap 1+ swap    
            else
                over if
                    i j vtxy
                    nip 0 swap  ( nspaces = 0 )
                then
                dup c-char@ emit 
            then
            1+                  ( nspaces &++dng -- )
        loop
    loop drop drop ;

: dprint-small-area ( x1 y1 x2 y2 -- )
    1+ rot do
        over i vtxy 
        2dup 1+ swap do 
            i j dcellyx@
            dup c-skip? if
                drop BL emit
            else
                c-char emit 
            then
        loop 
    loop 
    drop drop ;


