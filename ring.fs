decimal 

11 constant NRINGS
14 constant |ring-materials|

\ array of ring real names
here
    ," of stealth "
    ," of teleportation "
    ," of regeneration "
    ," of slow digestion "
    ," of add strength "
    ," of sustain strength "
    ," of dexterity "
    ," of adornment "
    ," of see invisible "
    ," of maintain armor "
    ," of searching "
NRINGS csarray ring-real

constarray ring-value
    250 ,
    100 ,
    255 ,
    295 ,
    200 ,
    250 ,
    250 ,
    25 ,
    300 ,
    290 ,
    270 ,

char-array ring-status NRINGS allot

\ create buffers for ring names
NRINGS mk-names (ring-name)

here
    ," diamond "
    ," stibotantalite "
    ," lapi-lazuli "
    ," ruby "
    ," emerald "
    ," sapphire "
    ," amethyst "
    ," quartz "
    ," tiger-eye "
    ," opal "
    ," agate "
    ," turquoise "
    ," pearl "
    ," garnet "
|ring-materials| csarray ring-materials

: gen-ring-names
    |ring-materials| mkshuffle
    NRINGS 0 do
        ring-materials i (ring-name) place
    loop 
    |ring-materials| NRINGS do drop loop ;

\ get nth ring called name as string
: ring-name ['] (ring-name) its-name ;

\ n call-ring
: call-ring
    dup 
        (ring-name) call-it 
        ring-status CALLED swap c! ;

: type-ring-info    
    dup ." $: " ring-value .
    dup ."  status: " ring-status c@ .
    dup ."  name: " ring-name type
    dup ."  real: " ring-real type
    drop ;

: dump-rings
    cr
    NRINGS 0 do
        i dup . type-ring-info cr
    loop ;

\ gen-ring-names
\ dump-rings

