decimal 

10 constant NWANDS
30 constant |wand-materials|
14 constant MAX-METAL

\ array of wand real names
here
    ," of teleport away "
    ," of slow monster "
    ," of confuse monster "
    ," of invisibility "
    ," of polymorph "
    ," of haste monster "
    ," of sleep "
    ," of magic missile "
    ," of cancellation "
    ," of do nothing "
NWANDS csarray wand-real

constarray wand-value
    25 ,
    50 ,
    45 ,
    8 ,
    55 ,
    2 ,
    25 ,
    20 ,
    20 ,
    0 ,

char-array wand-status NWANDS allot

\ create buffers for wand materials
NWANDS mk-names (wand-name)

here
    ," steel "
    ," bronze "
    ," gold "
    ," silver "
    ," copper "
    ," nickel "
    ," cobalt "
    ," tin "
    ," iron "
    ," magnesium "
    ," chrome "
    ," carbon "
    ," platinum "
    ," silicon "
    ," titanium "
    \ wood 
    ," teak "
    ," oak "
    ," cherry "
    ," birch "
    ," pine "
    ," cedar "
    ," redwood "
    ," balsa "
    ," ivory "
    ," walnut "
    ," maple "
    ," mahogany "
    ," elm "
    ," palm "
    ," wooden "
|wand-materials| csarray wand-materials

: gen-wand-names
    |wand-materials| mkshuffle
    NWANDS 0 do
        dup wand-materials
            i (wand-name) place
        MAX-METAL > 128 and i wand-status c!
    loop 
    |wand-materials| NWANDS do drop loop 
    ;

: (wood?)
    128 and 0<> ; 

: is-wood?
    wand-status (wood?) ;

\ get nth wand called name as string
: wand-name ['] (wand-name) its-name ;

\ n call-wand
: call-wand
    dup 
        (wand-name) call-it 
        wand-status CALLED swap c! ;

: type-wand-info    
    dup ." $: " wand-value .
    dup ."  status: " wand-status 
        c@ dup .
        (wood?) if ." wooden " else ." metal " then
    dup ."  name: " wand-name type
    dup ."  real: " wand-real type
    drop ;

: dump-wands
    cr
    NWANDS 0 do
        i dup . type-wand-info cr
    loop ;

\ gen-wand-names
\ dump-wands

