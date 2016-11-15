: not if false else true then ;
: off false swap ! ;
: on true swap ! ;
: -rot rot rot ;

s" base.fs" included
s" lfsr.fs" included
s" tstack.fs" included
s" vt100.fs" included
s" dung.fs" included
s" rooms.fs" included
s" level.fs" included
s" game.fs" included

: d-rooms 10 1 do cr i . i dump-room loop ;

: main
  \ level cr dprint bye ;
  play 
  bye
  ;
