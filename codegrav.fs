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

\ xt are ( x y -- )
: apply-adjacent ( xt x y -- )
    rot >R
    2dup p-y R@ execute 2dup p-k R@ execute 2dup p-u R@ execute
    2dup p-h R@ execute 2dup     R@ execute 2dup p-l R@ execute
    2dup p-b R@ execute 2dup p-j R@ execute      p-n R@ execute 
    R> drop ;


