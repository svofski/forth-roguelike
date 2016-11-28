include base.fs
include lfsr.fs
include tstack.fs

gforth [IF]
    include vt100.fs
[ELSE]
    include vt52.fs
[THEN]

include chars.fs
include things.fs
include monster.fs
include dung.fs
include rooms.fs
include level.fs
include game.fs

