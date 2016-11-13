: not if false else true then ;

include base.fs
include lfsr.fs
include tstack.fs
include vt100.fs
include dung.fs
include rooms.fs
include level.fs
include game.fs

: d-rooms 10 1 do cr i . i dump-room loop ;

: main
  \ level cr dprint bye ;
  play 
  bye
  ;
